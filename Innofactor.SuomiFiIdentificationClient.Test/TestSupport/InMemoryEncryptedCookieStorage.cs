using System.Collections.Generic;
using Innofactor.SuomiFiIdentificationClient.Support;

namespace Innofactor.SuomiFiIdentificationClient.Test.TestSupport {

  public class InMemoryEncryptedCookieStorage : IEncryptedCookieStorage {

    private readonly Dictionary<string, string> cookies = new Dictionary<string, string>();

    public void Delete(string name) {
      cookies.Remove(name);
    }

    public string Get(string name, string reason) {
      return cookies.TryGetValue(name, out var val) ? val : null;
    }

    public void Set(string name, string reason, string value) {
      cookies.Add(name, value);
    }

  }
}
