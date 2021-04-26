# [Suomi.fi e-identification](https://esuomi.fi/suomi-fi-services/suomi-fi-e-identification/?lang=en) client for .NET

![Publish NuGet](https://github.com/Innofactor/Innofactor.SuomiFiIdentificationClient/workflows/Publish%20NuGet/badge.svg)

Depends on Sustainsys.Saml2.AspNetCore2.

**Note:**
The client was created for a specific use case and is provided "as is". Pull requests and suggestions for generalizing the usage are welcome.

* Targets .NET Standard 2.1, see other release branches for 2.0 support
* Only HTTP Redirect binding is supported. 
* Supports new AES-GCM encryption algorithm

## Usage example 

First make sure SamlConfig is configured, for example in appsettings.json (replace ENTITYID and CERTIFICATE_NAME as necessary):

```json
  "Saml": {
    "Saml2EntityId": "ENTITYID",
    "Saml2SSOUrl": "https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SSO",
    "Saml2SLOUrl": "https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SLO",
    "Saml2IdpEntityId": "https://testi.apro.tunnistus.fi/idp1",
    "Saml2IdpCertificate": "apro-test.cer",
    "Saml2Certificate": "CERTIFICATE_NAME",
    "Saml2CertificateStoreLocation": "CurrentUser"
  },
```

Add your certificate to certificate manager, for example Current user -> Personal -> Certificates. 
Make sure the private key is exportable. When using the standard certificate store, 
CERTIFICATE_NAME above must match certificate friendly name. The certificate store loading can be customized by
replacing it with your own implementation of the ICertificateStore interface.

In Startup.cs:

```csharp

    public void ConfigureServices(IServiceCollection services) {

      // ...

      services.Configure<SamlConfig>(Configuration.GetSection("Saml"));
      services.AddOptions();
      services.AddScoped<ICertificateStore>(x => new CertificateStore(x.GetService<IOptions<SamlConfig>>().Value));
      services.AddSuomiFiIdentificationClient();
    }

```

In your controller (for example SuomiFiIdentificationController):
```csharp

    [AllowAnonymous]
    [HttpGet("authenticate")]
    public ActionResult AuthenticateWithSaml(Saml2Action samlAction, string language = "") {

      var returnUrl = "http://example.com/ACSPost";
      var redirectUrl = client.Authenticate(returnUrl, language, new RelayState(Saml2Action.Register, string.Empty, language));

      return new RedirectResult(redirectUrl);

    }

    [HttpPost("ACSPost")]
    public async Task<ActionResult> ACSPost(string samlResponse, string relayState = "") {

      var errorUrl = "/#/login?error=true";
      var saml2Response = validator.Validate(samlResponse, true);

      if (!saml2Response.Success) {
        return new RedirectResult(errorUrl);
      }

      var parsedState = RelayState.Parse(relayState);

      // Log in user, store session claims etc.

    }

    [HttpGet("logout")]
    public async Task<ActionResult> Logout() {
      // Fetch stored session claims to end session properly
      var sessionNameIdentifier = "";
      var sessionIndex = "";

      await HttpContext.SignOutAsync();
      var redirectUrl = client.Logout(sessionNameIdentifier, sessionIndex);

      return new RedirectResult(redirectUrl);

    }

    [HttpGet("SLORedirect")]
    public ActionResult SLORedirect(string samlResponse) {

      authStateAccessor.Delete();
      
      return new RedirectResult("/");

    }

```
