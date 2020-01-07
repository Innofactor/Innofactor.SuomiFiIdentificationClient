using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  /// <summary>
  /// Sign data with RSA+SHA256.
  /// </summary>
  public class RsaShaCrypto {

    private readonly ICertificateStore certificateStore;
    private readonly SamlConfig config;

    public RsaShaCrypto(SamlConfig config, ICertificateStore certificateStore) {
      this.config = config;
      this.certificateStore = certificateStore;
    }

    private X509Certificate2 LoadCertificate() {
      var certPath = config.Saml2Certificate;
      return certificateStore.LoadCertificate(certPath);
    }

    /// <summary>
    /// Sign data using RSA+SHA256 as required by SAML2 signature.
    /// </summary>
    /// <param name="bytes">Bytes to be signed.</param>
    /// <returns>Signature, hashed with SHA256 and encrypted with our private RSA key</returns>
    public byte[] SignData(byte[] bytes) {
      
      var cert = LoadCertificate();

      using (var rsa = cert.GetRSAPrivateKey()) {
        return rsa.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
      }

    }

  }

}
