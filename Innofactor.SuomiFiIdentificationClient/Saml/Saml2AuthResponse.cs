using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Innofactor.SuomiFiIdentificationClient.Support;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  [Serializable]
  public class Saml2AuthResponse {

    private static readonly ILogger<Saml2AuthResponse> log = new LoggerFactory().CreateLogger<Saml2AuthResponse>();

    private static string DecodeBase64(string base64) {
      var data = Convert.FromBase64String(base64);
      var decoded = Encoding.UTF8.GetString(data);
      return decoded;
    }

    public static Saml2AuthResponse Create(string samlResponse,
      Saml2Id responseToId,
      EntityId issuer,
      X509Certificate2 idpCert,
      X509Certificate2 serviceCertificate,
      EntityId serviceId) {

      var decoded = DecodeBase64(samlResponse);
      var xmlDoc = new XmlDocument();
      xmlDoc.PreserveWhitespace = true;
      xmlDoc.LoadXml(decoded);

      var response = new Saml2Response(xmlDoc.DocumentElement, responseToId);

      if (response.Status != Saml2StatusCode.Success) {
        log.LogWarning("SAML authentication error: " + response.Status + " (" + response.StatusMessage + ")");
        return new Saml2AuthResponse(false) {Status = response.Status};
      }

      var spOptions = new SPOptions();
      spOptions.EntityId = serviceId;
      spOptions.ServiceCertificates.Add(serviceCertificate);
      var options = new Options(spOptions);
      var idp = new IdentityProvider(issuer, spOptions);
      idp.SigningKeys.AddConfiguredKey(idpCert);
      options.IdentityProviders.Add(idp);


      var identities = response.GetClaims(options)?.ToArray();

      if (identities == null || identities.Length == 0)
        return new Saml2AuthResponse(false);

      var identity = identities.First();
      var firstName = identity.FindFirstValue(AttributeNames.GivenName) ?? identity.FindFirstValue(AttributeNames.EidasCurrentGivenName);
      var lastName = identity.FindFirstValue(AttributeNames.Sn) ?? identity.FindFirstValue(AttributeNames.EidasCurrentFamilyName);
      var ssn = identity.FindFirstValue(AttributeNames.NationalIdentificationNumber);
      var foreignPersonIdentifier = identity.FindFirstValue(AttributeNames.ForeignPersonIdentifier);
      var nameId = identity.FindFirstValue(AttributeNames.NameIdentifier);
      var sessionId = identity.FindFirstValue(AttributeNames.SessionIndex);
      var eidasPersonIdentifier = identity.FindFirstValue(AttributeNames.EidasPersonIdentifier);
      var eidasDateOfBirth = identity.FindFirstValue(AttributeNames.EidasDateOfBirth);

      return new Saml2AuthResponse(true) {
        FirstName = firstName, LastName = lastName, SSN = ssn, RelayState = response.RelayState,
        NameIdentifier = nameId, SessionIndex = sessionId, ForeignPersonIdentifier = foreignPersonIdentifier,
        EidasPersonIdentifier = eidasPersonIdentifier, EidasDateOfBirth = eidasDateOfBirth
      };

    }

    public Saml2AuthResponse() { }

    public Saml2AuthResponse(bool success) {
      Success = success;
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string RelayState { get; set; }

    public string SSN { get; set; }
    public string ForeignPersonIdentifier { get; set; }
    /// <summary>
    /// Name / Session identifier used for Suomi.Fi logout request
    /// </summary>
    public string NameIdentifier { get; set; }

    /// <summary>
    /// Eidas dateOfBirth value in
    /// YYYY-MM-DD -format
    /// </summary>
    public string EidasDateOfBirth { get; set; }

    public string EidasPersonIdentifier { get; set; }

    /// <summary>
    /// Session identifier for Suomi.Fi logout request
    /// </summary>
    public string SessionIndex { get; set; }
    public Saml2StatusCode Status { get; set; }

    public bool Success { get; set; }
  }
}
