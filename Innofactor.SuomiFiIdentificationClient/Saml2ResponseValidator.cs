using System.IdentityModel.Metadata;
using Innofactor.SuomiFiIdentificationClient.Saml;
using Innofactor.SuomiFiIdentificationClient.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Innofactor.SuomiFiIdentificationClient {

  public interface ISaml2ResponseValidator {
    /// <summary>
    /// Validates SAML response.
    /// </summary>
    /// <param name="samlResponse">SAML response string.</param>
    /// <param name="validateConditions">Whether to validate timestamp and ID. This should always be true in production, but can be set to false for testing.</param>
    /// <returns>Authentication response. Cannot be null.</returns>
    Saml2AuthResponse Validate(string samlResponse, bool validateConditions);
  }

  public class Saml2ResponseValidator : ISaml2ResponseValidator {

    private readonly AuthStateAccessor authStateAccessor;
    private readonly SamlConfig config;
    private readonly ICertificateStore certificateStore;
    private readonly ILogger<Saml2ResponseValidator> log;

    public Saml2ResponseValidator(AuthStateAccessor authStateAccessor, IOptions<SamlConfig> config, ILogger<Saml2ResponseValidator> log, ICertificateStore certificateStore) {
      this.authStateAccessor = authStateAccessor;
      this.config = config.Value;
      this.log = log;
      this.certificateStore = certificateStore;
    }

    public bool IsValid(string samlResponse, bool validateConditions) {
      return Validate(samlResponse, validateConditions).Success;
    }

    public Saml2AuthResponse Validate(string samlResponse, bool validateConditions) {

      var authId = authStateAccessor.Id;

      if (string.IsNullOrEmpty(authId?.Value)) {
        log.LogInformation("SAML auth state was not initialized");
        return new Saml2AuthResponse(false);
      }

      var idpCertificate = certificateStore.LoadCertificate(config.Saml2IdpCertificate);
      var serviceCertificate = certificateStore.LoadCertificate(config.Saml2Certificate);
      var issuer = new EntityId(config.Saml2IdpEntityId);
      var saml2Response = Saml2AuthResponse.Create(samlResponse, authId, issuer, idpCertificate, serviceCertificate, validateConditions);

      return saml2Response;

    }

  }

}
