namespace Innofactor.SuomiFiIdentificationClient.Support {

  public class EncryptedCookieAccessor {

    private readonly string cookieName;
    private readonly IEncryptedCookieStorage storage;
    private readonly string reason;

    public EncryptedCookieAccessor(IEncryptedCookieStorage storage, string cookieName, string reason) {
      this.storage = storage;
      this.cookieName = cookieName;
      this.reason = reason;
    }

    public string Value {
      get => storage.Get(cookieName, reason);
      set => storage.Set(cookieName, reason, value);
    }

    public void Delete() {
      storage.Delete(cookieName);
    }

  }

}
