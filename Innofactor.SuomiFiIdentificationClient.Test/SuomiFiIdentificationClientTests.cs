using System;
using Innofactor.SuomiFiIdentificationClient.Support;
using Innofactor.SuomiFiIdentificationClient.Test.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Innofactor.SuomiFiIdentificationClient.Test {

  /// <summary>
  /// Tests for <see cref="SuomiFiIdentificationClient"/>.
  /// </summary>
  [TestClass]
  public class SuomiFiIdentificationClientTests {

    private readonly SuomiFiIdentificationClient client;

    public SuomiFiIdentificationClientTests() {
      var cookieStorage = new InMemoryEncryptedCookieStorage();
      var samlConfig = new SamlConfig {
        Saml2SSOUrl = "https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SSO",
        Saml2SLOUrl = "https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SLO",
        Saml2IdpEntityId = "https://testi.apro.tunnistus.fi/idp1",
        Saml2EntityId = "https://localhost/SAML/2SP",
        Saml2Certificate = "localhost.pfx",
        Saml2IdpCertificate = "apro-test.cer",
      };
      var store = new EmbeddedCertificateStore();
      client = new SuomiFiIdentificationClient(samlConfig, new AuthStateAccessor(cookieStorage), new RsaShaCrypto(samlConfig, store));
    }

    [TestMethod]
    public void Authenticate() {

      var result = client.Authenticate("https://localhost:39390", "fi", new RelayState<string>("register", null, "fi"));

      Assert.IsNotNull(result, "result");
      Assert.IsTrue(result.StartsWith("https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SSO?SAMLRequest"), "Start of request URL");

    }

    [TestMethod]
    public void Logout() {

      var result = client.Logout();

      Assert.IsNotNull(result, "result");
      Assert.IsTrue(result.StartsWith("https://testi.apro.tunnistus.fi/idp/profile/SAML2/Redirect/SLO?SAMLRequest"), "Start of request URL");

    }

  }
}
