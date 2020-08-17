using System.IO;
using System.Reflection;
using System.Text;

namespace EmailServer
{
    /// <summary>
    /// File helpers
    /// </summary>
    public static class FileUtil
    {
        public static StreamReader GetEmailTemplate(string templateName)
        {
            return GetEmbededResource($"Templates.{templateName}");
        }

        public static StreamReader GetEmbededResource(string namespaceAndFileName)
        {
            return new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream($"EmailServer.{namespaceAndFileName}"), Encoding.UTF8);
        }
    }
}
