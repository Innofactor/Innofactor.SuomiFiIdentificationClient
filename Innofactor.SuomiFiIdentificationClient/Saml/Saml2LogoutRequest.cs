using System;
using System.IdentityModel.Tokens;
using System.Xml.Linq;
using Kentor.AuthServices;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  public class Saml2LogoutRequest {

    public Saml2LogoutRequest() {
      Id = new Saml2Id("id" + Guid.NewGuid().ToString("N"));
    }

    public Saml2Id Id { get; }

    public XElement ToXml(string entityId, string destination) {

      var issueInstant = DateTime.UtcNow.ToSaml2DateTimeString();

      var x = new XElement(Saml2Namespaces.Saml2P + "LogoutRequest",
        new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
        new XAttribute("Destination", destination),
        new XAttribute("ID", Id),
        new XAttribute("IssueInstant", issueInstant),
        new XAttribute("Version", "2.0"),
        new XElement(Saml2Namespaces.Saml2 + "Issuer",
          new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2), entityId)
      );

      return x;

    }

  }

}
