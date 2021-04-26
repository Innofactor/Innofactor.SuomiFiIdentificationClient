using Innofactor.SuomiFiIdentificationClient.Decryption;
using Innofactor.SuomiFiIdentificationClient.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Innofactor.SuomiFiIdentificationClient {
  public static class StartupExtension {

    /// <summary>
    /// Dependency injection for suomi fi client and it's prerequisite resources.
    /// Expects some prerequisites to be registered independently: <see cref="ICertificateStore"/> and IOptions for <see cref="SamlConfig"/>
    /// </summary>
    /// <param name="services">Service collection for di</param>
    public static void AddSuomiFiIdentificationClient(this IServiceCollection services) {
      CryptoConfig.AddAlgorithm(typeof(AesGcmAlgorithm), AesGcmAlgorithm.AesGcm128Identifier);
      services.AddHttpContextAccessor();
      services.AddScoped<IEncryptedCookieStorage, EncryptedCookieStorage>();
      services.AddScoped<AuthStateAccessor>();
      services.AddScoped<ISaml2ResponseValidator, Saml2ResponseValidator>();
      services.AddScoped(x => new RsaShaCrypto(x.GetService<IOptions<SamlConfig>>().Value, x.GetService<ICertificateStore>()));
      services.AddScoped(x => new SuomiFiIdentificationClient(x.GetService<IOptions<SamlConfig>>().Value, x.GetService<AuthStateAccessor>(), x.GetService<RsaShaCrypto>()));
    }
  }
}
