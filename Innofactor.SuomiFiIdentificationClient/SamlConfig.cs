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
    /// <summary>
    /// Primary IdpCertificate
    /// </summary>
    public string Saml2IdpCertificate { get; set; }
    /// <summary>
    /// Secondary IdpCertificate
    /// </summary>
    public string Saml2SecondaryIdpCertificate { get; set; }

    /// <summary>
    /// Service provider SAML certificate (both public and private key, a .pfx file).
    /// Used when signing an authentication request and validating the response SAML
    /// </summary>
    public string Saml2Certificate { get; set; }
    /// <summary>
    /// Service provider SAML certificate (both public and private key, a .pfx file).
    /// Used when validating the response SAML
    /// </summary>
    public string Saml2SecondaryCertificate { get; set; }

    public StoreLocation Saml2CertificateStoreLocation { get; set; }

  }

}
