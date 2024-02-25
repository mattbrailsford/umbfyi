using Hangfire.Server;
using System.Net;
using System.Text;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class MediaExtractorBase : IMediaExtractor
    {
        public string[] Tags { get; }

        public MediaExtractorBase(string[] tags)
        {
            Tags = tags;
        }

        public abstract Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default);

        protected static async Task<string> FetchAsync(string url, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient(IgnoreTlsErrorsHandler()))
            {
                // Pretend to be the Edge browser, v99 because otherwise some feeds block you as a bot 
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.82 Safari/537.36 Edg/99.0.1150.36");

                using (var result = await client.GetAsync(url, cancellationToken))
                {
                    if (result.IsSuccessStatusCode == false)
                    {
                        //context.SetTextColor(ConsoleTextColor.Red);
                        //context.WriteLine( $"Getting {url} not successful, status: {result.StatusCode}, reason: {result.ReasonPhrase}");
                        //context.ResetTextColor();
                        return string.Empty;
                    }

                    // Force read with UTF-8 encoding
                    var buffer = await result.Content.ReadAsByteArrayAsync(cancellationToken);
                    var byteArray = buffer.ToArray();
                    var raw = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                    return raw.Trim(new char[] { '\uFEFF', '\u200B' });
                }
            }
        }

        private static HttpClientHandler IgnoreTlsErrorsHandler()
        {
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };

            return handler;
        }
    }
}
