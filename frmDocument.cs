using devkit2.Properties;
using Markdig;
using Microsoft.Web.WebView2.Core;
using System.Text;

namespace devkit2
{
    public partial class frmDocument : Form
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string OldPrefix = "https://github.com/minhnguyenerp/devkit2/blob/main/";
        private const string NewPrefix = "https://raw.githubusercontent.com/minhnguyenerp/devkit2/refs/heads/main/";

        public frmDocument()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevKit2", "runtimes"));
            webView21.CoreWebView2InitializationCompleted += WebView21_CoreWebView2InitializationCompleted;
            webView21.CreationProperties = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties()
            {
                UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevKit2", "runtimes")
            };
            webView21.EnsureCoreWebView2Async();
        }

        private async void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (e.ResourceContext != CoreWebView2WebResourceContext.Document)
                return;

            var uri = e.Request.Uri;
            if (string.IsNullOrWhiteSpace(uri))
                return;

            if (!uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return;

            var deferral = e.GetDeferral();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.TryAddWithoutValidation(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome Safari");

                var response = await _httpClient.SendAsync(request);
                var mediaType = response.Content.Headers.ContentType?.MediaType ?? "text/plain";

                if (!mediaType.Contains("text/plain", StringComparison.OrdinalIgnoreCase))
                    return;

                var html = await response.Content.ReadAsStringAsync();
                html = MarkDownToHtmlPage(html.Replace(OldPrefix, NewPrefix, StringComparison.OrdinalIgnoreCase));
                var bytes = Encoding.UTF8.GetBytes(html);
                var stream = new MemoryStream(bytes);

                e.Response = webView21.CoreWebView2.Environment.CreateWebResourceResponse(
                    stream,
                    (int)response.StatusCode,
                    response.ReasonPhrase ?? "OK",
                    "Content-Type: text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                deferral.Complete();
            }
        }

        private string MarkDownToHtmlPage(string markdown)
        {
            string htmlPage = $@"
<html>
<head>
<meta charset='utf-8'>
<style>
::-webkit-scrollbar {{ width: 12px; height: 12px; }}
::-webkit-scrollbar-track {{ background: #f1f1f1; }}
::-webkit-scrollbar-thumb {{ background: #888; border-radius: 8px; }}
::-webkit-scrollbar-thumb:hover {{ background: #666; }}
img {{ max-width:100%; }}
h1 {{ font-size: 22px; }}
h2 {{ font-size: 18px; }}
h3 {{ font-size: 16px; }}
h4 {{ font-size: 14px; }}
p {{ text-align: justify; }}
body {{
    font-family: Segoe UI, Arial, sans-serif;
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
    line-height: 1.6;
    background-color: white;
}}
pre {{
    background: #f4f4f4;
    padding: 10px;
    overflow-x: auto;
}}
code {{
    background: #f4f4f4;
    padding: 2px 4px;
}}
</style>
</head>
<body>
{Markdown.ToHtml(markdown)}
</body>
</html>";
            return htmlPage;
        }

        private async void WebView21_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Document);
            webView21.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            var url = "https://raw.githubusercontent.com/minhnguyenerp/devkit2/refs/heads/main/README.md";
            try
            {
                using var client = new HttpClient();
                string markdown = await client.GetStringAsync(url);
                webView21.NavigateToString(MarkDownToHtmlPage(markdown.Replace(OldPrefix, NewPrefix, StringComparison.OrdinalIgnoreCase)));
            }
            catch { }
        }
    }
}
