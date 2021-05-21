using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  public class Saml2AuthRequest {

    public static readonly XNamespace Vetuma = XNamespace.Get("urn:vetuma:SAML:2.0:extensions");

    public Saml2AuthRequest() {
      Id = new Saml2Id("id" + Guid.NewGuid().ToString("N"));
    }

    public Saml2Id Id { get; }

    public XElement ToXml(string entityId, string destination, string assertionConsumerServiceUrl = "", string language = "fi") {

      var issueInstant = DateTime.UtcNow.ToSaml2DateTimeString();

      var x = new XElement(Saml2Namespaces.Saml2P + "AuthnRequest",
        new XAttribute(XNamespace.Xmlns + "samlp", Saml2Namespaces.Saml2P),
        !string.IsNullOrEmpty(assertionConsumerServiceUrl) ? new XAttribute("AssertionConsumerServiceURL", assertionConsumerServiceUrl) : null,
        new XAttribute("Destination", destination),
        new XAttribute("ID", Id.Value),
        new XAttribute("IssueInstant", issueInstant),
        new XAttribute("Version", "2.0"),
        new XElement(Saml2Namespaces.Saml2 + "Issuer",
          new XAttribute(XNamespace.Xmlns + "saml", Saml2Namespaces.Saml2),
          entityId),
        new XElement(Saml2Namespaces.Saml2P + "Extensions",
          new XElement(Vetuma + "vetuma", new XElement(Vetuma + "LG", language == "" ? "fi" : language))
        ),
        new XElement(Saml2Namespaces.Saml2P + "NameIDPolicy",
          new XAttribute("AllowCreate", "true"),
          new XAttribute("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:transient")
        )
      );

      return x;

    }

  }

}
