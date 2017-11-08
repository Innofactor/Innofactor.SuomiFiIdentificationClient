using System.Security.Cryptography.X509Certificates;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  public interface ICertificateStore {
    X509Certificate2 LoadCertificate(string certificatePath);
  }

}
