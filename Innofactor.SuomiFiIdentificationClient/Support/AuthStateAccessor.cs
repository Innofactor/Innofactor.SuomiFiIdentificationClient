using Microsoft.IdentityModel.Tokens.Saml2;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  /// <summary>
  /// Stores Suomi.fi authentication state in an encrypted/protected cookie.
  /// </summary>
  public class AuthStateAccessor : EncryptedCookieAccessor {

    private const string cookieName = "SamlIdentRequest";

    public AuthStateAccessor(IEncryptedCookieStorage encryptedCookieStorage)
      : base(encryptedCookieStorage, cookieName, "SuomiFiIdent") {
    }

    public Saml2Id Id {
      get {
        var val = Value;
        return !string.IsNullOrEmpty(val) ? new Saml2Id(val) : new Saml2Id();
      }
      set => Value = value.Value;
    }

  }
}
