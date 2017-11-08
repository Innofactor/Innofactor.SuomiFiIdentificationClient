using System;
using System.Runtime.Serialization;

namespace Innofactor.SuomiFiIdentificationClient.Exceptions {

  public class ConfigurationErrorsException : Exception {

    public ConfigurationErrorsException() {
    }

    public ConfigurationErrorsException(string message) : base(message) {
    }

    public ConfigurationErrorsException(string message, Exception innerException) : base(message, innerException) {
    }

    protected ConfigurationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }

  }

}
