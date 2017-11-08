using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Innofactor.SuomiFiIdentificationClient.Support;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Saml2P;
using Microsoft.Extensions.Logging;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  [Serializable]
  public class Saml2AuthResponse {

    private static readonly ILogger<Saml2AuthResponse> log = new LoggerFactory().CreateLogger<Saml2AuthResponse>();
    private static readonly string RsaSha256Namespace = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

    private static string DecodeBase64(string base64) {
      var data = Convert.FromBase64String(base64);
      var decoded = Encoding.UTF8.GetString(data);
      return decoded;
    }

    // For testing only, when the tokens need to be re-used.
    class DummyTokenReplayCache : TokenReplayCache {
      public override void AddOrUpdate(string key, SecurityToken securityToken, DateTime expirationTime) {

      }

      public override bool Contains(string key) {
        return false;
      }

      public override SecurityToken Get(string key) {
        return null;
      }

      public override void Remove(string key) {

      }
    }

    public static Saml2AuthResponse Create(string samlResponse, 
      Saml2Id responseToId,
      EntityId issuer,
      X509Certificate2 idpCert,
      X509Certificate2 serviceCertificate,
      bool validateConditions = true) {

      if (CryptoConfig.CreateFromName(RsaSha256Namespace) == null)
        Options.GlobalEnableSha256XmlSignatures();

      var decoded = DecodeBase64(samlResponse);
      var xmlDoc = new XmlDocument();
      xmlDoc.PreserveWhitespace = true;
      xmlDoc.LoadXml(decoded);

      var response = new Saml2Response(xmlDoc.DocumentElement, responseToId);

      if (response.Status != Saml2StatusCode.Success) {
        log.LogWarning("SAML authentication error: " + response.Status + " (" + response.StatusMessage + ")");
        return new Saml2AuthResponse(false) { Status = response.Status };
      }

      var spOptions = new SPOptions();
      spOptions.SystemIdentityModelIdentityConfiguration.AudienceRestriction = new AudienceRestriction(AudienceUriMode.Never);
      spOptions.ServiceCertificates.Add(serviceCertificate);
      if (!validateConditions) {
        spOptions.Saml2PSecurityTokenHandler.Configuration.MaxClockSkew = TimeSpan.MaxValue;
        spOptions.Saml2PSecurityTokenHandler.Configuration.Caches.TokenReplayCache = new DummyTokenReplayCache();
      }
      var options = new Options(spOptions);
      var idp = new IdentityProvider(issuer, spOptions);
      idp.SigningKeys.AddConfiguredKey(idpCert);
      options.IdentityProviders.Add(idp);


      var identities = response.GetClaims(options)?.ToArray();

      if (identities == null || identities.Length == 0)
        return new Saml2AuthResponse(false);

      var identity = identities.First();
      var firstName = identity.FindFirstValue(AttributeNames.GivenName);
      var lastName = identity.FindFirstValue(AttributeNames.Sn);
      var ssn = identity.FindFirstValue(AttributeNames.NationalIdentificationNumber);

      return new Saml2AuthResponse(true) { FirstName = firstName, LastName = lastName, SSN = ssn, RelayState = response.RelayState };

    }

    public Saml2AuthResponse() { }

    public Saml2AuthResponse(bool success) {
      Success = success;
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string RelayState { get; set; }

    public string SSN { get; set; }

    public Saml2StatusCode Status { get; set; }

    public bool Success { get; set; }

  }

}
