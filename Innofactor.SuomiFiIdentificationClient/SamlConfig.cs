using System.Security.Cryptography.X509Certificates;

namespace Innofactor.SuomiFiIdentificationClient {

  public class SamlConfig {

    public string Saml2EntityId { get; set; }

    /// <summary>
    /// Suomi.fi SAML2 single sign on URL.
    /// </summary>
    public string Saml2SSOUrl { get; set; }

    /// <summary>
    /// Suomi.fi SAML2 single sign on logout URL.
    /// </summary>
    public string Saml2SLOUrl { get; set; }

    public string Saml2IdpEntityId { get; set; }
    public string Saml2IdpCertificate { get; set; }

    /// <summary>
    /// Service provider SAML certificate (both public and private key, a .pfx file).
    /// </summary>
    public string Saml2Certificate { get; set; }

    public StoreLocation Saml2CertificateStoreLocation { get; set; }

  }

}
