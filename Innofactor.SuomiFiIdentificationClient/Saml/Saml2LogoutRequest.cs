using System;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  public class Saml2LogoutRequest {

    public Saml2LogoutRequest() {
      Id = new Saml2Id("id" + Guid.NewGuid().ToString("N"));
    }

    public Saml2Id Id { get; }

    public XElement ToXml(string entityId, string idpEntityId, string destination, string sessionId, string sessionIndex) {

      var issueInstant = DateTime.UtcNow.ToSaml2DateTimeString();

      var x = new XElement(Saml2Namespaces.Saml2P + "LogoutRequest",
        new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
        new XAttribute("Destination", destination),
        new XAttribute("ID", Id),
        new XAttribute("IssueInstant", issueInstant),
        new XAttribute("Version", "2.0"),
        new XElement(Saml2Namespaces.Saml2 + "Issuer",
          new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2), entityId),
        new XElement(Saml2Namespaces.Saml2 + "NameID",
          new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2),
          new XAttribute("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"),
          new XAttribute("NameQualifier", idpEntityId),
          new XAttribute("SPNameQualifier", entityId),
            sessionId),
        new XElement(Saml2Namespaces.Saml2P + "SessionIndex", sessionIndex)
      );

      return x;

    }

  }

}
