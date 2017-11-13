using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Innofactor.SuomiFiIdentificationClient.Test.TestSupport {

  public class TestDataReader {

    private static Stream GetFileStream(string fileName) {

      var asm = typeof(TestDataReader).Assembly;
      return asm.GetManifestResourceStream(asm.GetName().Name + ".TestData." + fileName);

    }

    public static X509Certificate2 ReadCertificate(string fileName) {

      byte[] bytes;

      using (var stream = GetFileStream(fileName)) {
        bytes = new byte[stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
      }

      var cert = new X509Certificate2(bytes, "", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable); // Mark key as exportable, http://stackoverflow.com/a/9373407
      return cert;

    }

    public static string ReadTextFile(string fileName) {

      using (var stream = GetFileStream(fileName))
      using (var reader = new StreamReader(stream)) {
        return reader.ReadToEnd();
      }

    }

  }

}
