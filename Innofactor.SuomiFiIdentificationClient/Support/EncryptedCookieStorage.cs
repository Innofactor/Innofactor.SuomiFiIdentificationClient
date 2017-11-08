using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  public interface IEncryptedCookieStorage {
    void Delete(string name);
    string Get(string name, string reason);
    void Set(string name, string reason, string value);
  }

  public class EncryptedCookieStorage : IEncryptedCookieStorage {

    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly HttpRequest request;
    private readonly HttpResponse response;

    public EncryptedCookieStorage(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider) {
      this.request = httpContextAccessor.HttpContext.Request;
      this.response = httpContextAccessor.HttpContext.Response;
      this.dataProtectionProvider = dataProtectionProvider;
    }

    public void Delete(string name) {
      this.response.Cookies.Delete(name);
    }

    public string Get(string name, string reason) {

      var cookie = request.Cookies[name];

      if (string.IsNullOrEmpty(cookie))
        return null;

      var dataProtector = dataProtectionProvider.CreateProtector(reason);
      var decrypted = dataProtector.Unprotect(cookie);

      return decrypted;

    }

    public void Set(string name, string reason, string value) {
      var dataProtector = dataProtectionProvider.CreateProtector(reason);
      var encrypted = dataProtector.Protect(value);
      response.Cookies.Append(name, encrypted);
    }

  }

}
