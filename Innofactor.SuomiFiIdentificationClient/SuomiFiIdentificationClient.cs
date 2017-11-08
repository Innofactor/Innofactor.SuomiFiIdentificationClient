using Innofactor.SuomiFiIdentificationClient.Saml;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient {

  public class SuomiFiIdentificationClient {

    private readonly AuthStateAccessor authStateAccessor;
    private readonly SamlConfig config;

    public SuomiFiIdentificationClient(SamlConfig config, AuthStateAccessor authStateAccessor) {
      this.config = config;
      this.authStateAccessor = authStateAccessor;
    }

    /// <summary>
    /// Starts Suomi.fi identification request.
    /// </summary>
    /// <param name="returnUrl">Return URL after identification request, for example https://localhost:39390/SAML/ACSPOST </param>
    /// <param name="language">Language code, for example "fi". Optional, defaults to "fi".</param>
    /// <param name="relayState">Relay state. Optional.</param>
    public string Authenticate(string returnUrl, string language, IRelayState relayState) {

      var authRequest = new Saml2AuthRequest();
      var authRequestXml = authRequest.ToXml(config.Saml2EntityId, config.Saml2SSOUrl, returnUrl, language);
      authStateAccessor.Id = authRequest.Id;

      var binding = new Saml2HttpRedirect(relayState?.ToString(), config);
      binding.Run(authRequestXml);

      var redirectUrl = config.Saml2SSOUrl + "?" + binding.RedirectUrl;
      return redirectUrl;

    }

    public string Logout() {

      var logoutRequest = new Saml2LogoutRequest();
      var logoutRequestXml = logoutRequest.ToXml(config.Saml2EntityId, config.Saml2SLOUrl);
      authStateAccessor.Id = logoutRequest.Id;

      var binding = new Saml2HttpRedirect(string.Empty, config);
      binding.Run(logoutRequestXml);

      var redirectUrl = config.Saml2SLOUrl + "?" + binding.RedirectUrl;
      return redirectUrl;

    }

  }

}
