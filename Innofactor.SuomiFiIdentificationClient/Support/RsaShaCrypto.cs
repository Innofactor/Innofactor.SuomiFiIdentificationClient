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
      var sha256 = CryptoConfig.CreateFromName("SHA256");

      var rsa = (RSACryptoServiceProvider)cert.PrivateKey;

      /* 
       * Cryptographic provider used by the rsa variable may or may not be the provider that supports SHA256
       * (it needs to be Microsoft Enhanced RSA and AES Cryptographic Provider).
       * Need to construct a new crypto provider and copy the key to make sure.
       * Without this the signing may crash with "Invalid algorithm specified".
       * For this the key needs to be exportable.
       * http://hintdesk.com/c-how-to-fix-invalid-algorithm-specified-when-signing-with-sha256/ might also work.
       * 
       * Adapted from http://stackoverflow.com/a/27637121.
       */
      var enhCsp = new RSACryptoServiceProvider().CspKeyContainerInfo;
      var cspparams = new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, rsa.CspKeyContainerInfo.KeyContainerName);

      // http://stackoverflow.com/a/1324320
      cspparams.Flags = CspProviderFlags.UseMachineKeyStore;

      var privKey = new RSACryptoServiceProvider(rsa.KeySize, cspparams);
      var xmlString = rsa.ToXmlString(true); // Key needs to be exportable, otherwise "Key not valid for use in specified state"
      privKey.FromXmlString(xmlString);

      var signature = privKey.SignData(bytes, sha256);
      return signature;

    }

  }

}
