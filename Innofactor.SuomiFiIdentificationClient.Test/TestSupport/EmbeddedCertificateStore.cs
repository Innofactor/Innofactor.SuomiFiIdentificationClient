using System.Security.Cryptography.X509Certificates;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient.Test.TestSupport {

  public class EmbeddedCertificateStore : ICertificateStore {
    public X509Certificate2 LoadCertificate(string certificatePath) {
      return TestDataReader.ReadCertificate(certificatePath);
    }
  }
}
