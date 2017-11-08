using Innofactor.SuomiFiIdentificationClient.Saml;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient {

  public class SuomiFiIdentificationClient<TAction> {

    private readonly AuthStateAccessor authStateAccessor;
    private readonly SamlConfig config;

    public SuomiFiIdentificationClient(SamlConfig config, AuthStateAccessor authStateAccessor) {
      this.config = config;
      this.authStateAccessor = authStateAccessor;
    }

    public string Authenticate(TAction samlAction, string entityId, string returnUrl, string language) {

      var authRequest = new Saml2AuthRequest();
      var authRequestXml = authRequest.ToXml(config.Saml2EntityId, config.Saml2SSOUrl, returnUrl, language);
      authStateAccessor.Id = authRequest.Id;

      // RelayState contains action + entity ID (if any)
      var binding = new Saml2HttpRedirect(new RelayState<TAction>(samlAction, entityId, language).ToString(), config);
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
