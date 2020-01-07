using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;

namespace Innofactor.SuomiFiIdentificationClient.Saml {

  public class Saml2AuthRequest {

    public static readonly XNamespace Vetuma = XNamespace.Get("urn:vetuma:SAML:2.0:extensions");

    private IEnumerable<XAttribute> GetRedirectAttribute(string assertionConsumerServiceUrl) {

      var bindingPost = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";

      if (!string.IsNullOrEmpty(assertionConsumerServiceUrl)) {
        yield return new XAttribute("AssertionConsumerServiceURL", assertionConsumerServiceUrl);
      } else {
        yield return new XAttribute("ProtocolBinding", bindingPost);
      }

    }

    public Saml2AuthRequest() {
      Id = new Saml2Id("id" + Guid.NewGuid().ToString("N"));
    }

    public Saml2Id Id { get; }

    public XElement ToXml(string entityId, string destination, string assertionConsumerServiceUrl, string language) {

      var issueInstant = DateTime.UtcNow.ToSaml2DateTimeString();

      var x = new XElement(Saml2Namespaces.Saml2P + "AuthnRequest",
        new XAttribute(XNamespace.Xmlns + "samlp", Saml2Namespaces.Saml2P),
        GetRedirectAttribute(assertionConsumerServiceUrl),
        new XAttribute("Destination", destination),
        new XAttribute("ID", Id),
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
