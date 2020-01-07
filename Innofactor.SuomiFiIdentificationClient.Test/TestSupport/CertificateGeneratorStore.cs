using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient.Test.TestSupport {

  public class CertificateGeneratorStore : ICertificateStore {
    public X509Certificate2 LoadCertificate(string certificatePath) {
      return BuildSelfSignedServerCertificate();
    }

    private X509Certificate2 BuildSelfSignedServerCertificate() {
      var distinguishedName = new X500DistinguishedName("CN=test");

      using (RSA rsa = RSA.Create(2048)) {
        var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
        certificate.FriendlyName = "test";

        return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "WeNeedASaf3rPassword"), "WeNeedASaf3rPassword", X509KeyStorageFlags.MachineKeySet);
      }
    }
  }
}