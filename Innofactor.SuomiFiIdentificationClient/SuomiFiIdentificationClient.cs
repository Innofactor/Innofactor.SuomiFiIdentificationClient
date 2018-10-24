using Innofactor.SuomiFiIdentificationClient.Saml;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient {

  public class SuomiFiIdentificationClient {

    private readonly AuthStateAccessor authStateAccessor;
    private readonly SamlConfig config;
    private readonly RsaShaCrypto crypto;

    public SuomiFiIdentificationClient(SamlConfig config, AuthStateAccessor authStateAccessor, RsaShaCrypto crypto) {
      this.config = config;
      this.authStateAccessor = authStateAccessor;
      this.crypto = crypto;
    }

    /// <summary>
    /// Starts Suomi.fi identification request.
    /// </summary>
    /// <param name="returnUrl">Return URL after identification request, for example https://localhost:39390/SAML/ACSPOST </param>
    /// <param name="language">Language code, for example "fi". Optional, defaults to "fi".</param>
    /// <param name="relayState">Relay state. Optional.</param>
    /// <returns>Redirect URL</returns>
    public string Authenticate(string returnUrl, string language, IRelayState relayState) {

      var authRequest = new Saml2AuthRequest();
      var authRequestXml = authRequest.ToXml(config.Saml2EntityId, config.Saml2SSOUrl, returnUrl, language);
      authStateAccessor.Id = authRequest.Id;

      var binding = new Saml2HttpRedirect(relayState?.ToString(), crypto);
      binding.Run(authRequestXml);

      var redirectUrl = config.Saml2SSOUrl + "?" + binding.RedirectUrl;
      return redirectUrl;

    }

    /// <summary>
    /// Starts Suomi.fi logout request.
    /// </summary>
    /// <returns>Redirect URL</returns>
    public string Logout(string sessionId, string sessionIndex) {

      var logoutRequest = new Saml2LogoutRequest();
      var logoutRequestXml = logoutRequest.ToXml(config.Saml2EntityId, config.Saml2IdpEntityId, config.Saml2SLOUrl, sessionId, sessionIndex);
      authStateAccessor.Id = logoutRequest.Id;

      var binding = new Saml2HttpRedirect(string.Empty, crypto);
      binding.Run(logoutRequestXml);

      var redirectUrl = config.Saml2SLOUrl + "?" + binding.RedirectUrl;
      return redirectUrl;

    }

  }

}
