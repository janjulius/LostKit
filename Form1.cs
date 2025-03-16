using LostKit.Configuration;
using Microsoft.Web.WebView2.WinForms;

namespace LostKit
{
    public partial class Form1 : Form
    {
        public WebView2 webView;

        public Form1()
        {
            InitializeComponent();

            RenderWebPage();
        }

        private void RenderWebPage()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(webView);

            webView.Source = new Uri("https://2004.lostcity.rs/client?world=5&detail=high&method=0");
            webView.EnsureCoreWebView2Async();

            webView.NavigationCompleted += (s, e) => RemoveGameFrameTop();
            webView.NavigationCompleted += (s, e) => RemoveControls();

        }

        private async void RemoveGameFrameTop()
        {
            if (Settings.ShowTopBar)
                return;

            if (webView.CoreWebView2 != null)
            {
                await webView.CoreWebView2.ExecuteScriptAsync(
                    "document.getElementById('gameframe-top')?.remove();"
                );
            }
        }

        private async void RemoveControls()
        {
            if (Settings.ShowSettings)
                return;

            if (webView.CoreWebView2 != null)
            {
                await webView.CoreWebView2.ExecuteScriptAsync(
                    "document.getElementById('controls')?.remove();"
                );
            }
        }
    }
}
