using System;
using System.Linq;

namespace Innofactor.SuomiFiIdentificationClient {

  public class RelayState<T> {

    public static RelayState<T> Parse(string relayState) {

      var parts = relayState.Split(',');

      if (parts.Length < 1)
        return null;

      return new RelayState<T>((T)Enum.Parse(typeof(T), parts[0]), parts.ElementAtOrDefault(1), parts.ElementAtOrDefault(2));

    }

    public RelayState(T action, string entityId, string language) {
      Action = action;
      EntityId = entityId;
      Language = language;
    }

    public T Action { get; }
    public string EntityId { get; }
    public string Language { get; }

    public override string ToString() {
      return Action + "," + EntityId + "," + Language;
    }

  }

}
