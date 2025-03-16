using Microsoft.Web.WebView2.WinForms;

namespace LostKit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            RenderWebPage();
        }

        private void RenderWebPage()
        {
            var webView = new WebView2
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(webView);

            webView.Source = new Uri("https://2004.lostcity.rs/detail");
            webView.EnsureCoreWebView2Async();
        }
    }
}
