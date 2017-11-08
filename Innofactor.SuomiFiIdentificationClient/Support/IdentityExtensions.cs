using System;
using System.Security.Claims;

namespace Innofactor.SuomiFiIdentificationClient.Support {

  public static class IdentityExtensions {

    public static string FindFirstValue(this ClaimsIdentity identity, string claimType) {
      if (identity == null) {
        throw new ArgumentNullException("identity");
      }
      var claim = identity.FindFirst(claimType);
      return claim != null ? claim.Value : null;
    }

  }

}
