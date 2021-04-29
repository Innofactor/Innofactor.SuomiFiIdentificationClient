using System;
using System.Linq;

namespace Innofactor.SuomiFiIdentificationClient {

  public interface IRelayState {
    
  }

  public class RelayState<TAction> : IRelayState where TAction : Enum {

    public static RelayState<TAction> Parse(string relayState) {

      var parts = relayState.Split(',');

      if (parts.Length < 1)
        return null;

      return new RelayState<TAction>((TAction)Enum.Parse(typeof(TAction), parts[0]), parts.ElementAtOrDefault(1), parts.ElementAtOrDefault(2));

    }
    public RelayState(TAction action, string entityId, string language) {
      Action = action;
      EntityId = entityId;
      Language = language;
    }

    public TAction Action { get; }

    public string EntityId { get; }

    /// <summary>
    /// Selected UI language code
    /// </summary>
    public string Language { get; }

    public override string ToString() {
      return Action + "," + EntityId + "," + Language;
    }

  }

}
