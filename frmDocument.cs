using devkit2.Properties;
using Markdig;

namespace devkit2
{
    public partial class frmDocument : Form
    {
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

        private async void WebView21_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var url = "https://raw.githubusercontent.com/minhnguyenerp/devkit2/refs/heads/main/Document/Overview.md";
            using var client = new HttpClient();
            string markdown = await client.GetStringAsync(url);
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
            webView21.NavigateToString(htmlPage);
        }
    }
}
