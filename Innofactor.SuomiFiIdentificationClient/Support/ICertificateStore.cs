using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Innofactor.SuomiFiIdentificationClient.Exceptions;
using Microsoft.Extensions.Logging;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  public interface ICertificateStore {
    X509Certificate2 LoadCertificate(string certificatePath);
  }

  public class CertificateStore : ICertificateStore {

    private readonly SamlConfig config;
    private static readonly ILogger<CertificateStore> log = new LoggerFactory().CreateLogger<CertificateStore>();

    public CertificateStore(SamlConfig config) {
      this.config = config;
    }

    private X509Certificate2 LoadFromStore(string certName) {

      if (string.IsNullOrEmpty(certName))
        return null;

      var storeLocation = config.Saml2CertificateStoreLocation;
      var store = new X509Store(StoreName.My, storeLocation);
      store.Open(OpenFlags.ReadOnly);

      var cert = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == certName);

      if (cert != null) {
        log.LogDebug("Loaded certificate from store with name {0}", certName);
      }

      return cert;

    }

    public X509Certificate2 LoadCertificate(string certPath) {

      var cert = LoadFromStore(certPath);

      if (cert != null)
        return cert;

      throw new ConfigurationErrorsException("SAML2 certificate not found");

    }
  }

}
