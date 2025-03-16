using HtmlAgilityPack;
using Microsoft.Web.WebView2.WinForms;
using System.Net.Http;

namespace LostKit
{
    public partial class Form1 : Form
    {
        public WebView2 webView;


        private List<WorldData> worldList = new List<WorldData>();
        private readonly HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            LoadWorldData();

            RenderWebPage();
        }


        private void RenderWebPage()
        {
            webView = new WebView2
            {
                //Dock = DockStyle.Fill,
                Location = new Point(10, 10),
                Size = new Size(800, 600)
            };
            this.Controls.Add(webView);

            webView.Source = new Uri($"https://2004.lostcity.rs/client?world={Settings.FavWorld}&detail={Settings.FavDetailSettings.ToString().ToLower()}&method=0");
            webView.EnsureCoreWebView2Async();

            webView.NavigationCompleted += (s, e) => RemoveGameFrameTop();

        }

        private async void RemoveGameFrameTop()
        {
            //if (Settings.ShowTopBar)
            //    return;

            if (webView.CoreWebView2 != null)
            {
                await webView.CoreWebView2.ExecuteScriptAsync(
                    "document.getElementById('gameframe-top')?.remove();"
                );
            }
        }

        private async void LoadWorldData()
        {
            try
            {
                // Fetch HTML content from the URL
                string url = "https://2004.lostcity.rs/serverlist?hires.x=112&hires.y=16&method=0";
                string htmlContent = await httpClient.GetStringAsync(url);
                string currentFlagUrl = "";

                // Parse the HTML content
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Find all <a> tags inside <td> tags with world data
                var worldNodes = htmlDoc.DocumentNode.SelectNodes("//td//a[contains(@href, 'world=')]");

                if (worldNodes != null)
                {
                    foreach (var worldNode in worldNodes)
                    {
                        // Extract world name
                        string worldName = worldNode.InnerText.Trim();

                        // Extract world ID from the href
                        string href = worldNode.GetAttributeValue("href", "");
                        string worldIdStr = href.Split(new string[] { "world=" }, StringSplitOptions.None)[1].Split('&')[0];
                        int worldId = int.TryParse(worldIdStr, out int id) ? id : 0;

                        // Check if the current <td> has an img tag with a game flag and update currentFlagUrl
                        var flagImgNode = worldNode.Ancestors("td").FirstOrDefault()?.PreviousSibling?.SelectSingleNode(".//img[starts-with(@src, '/img/gamewin/')]");
                        if (flagImgNode != null)
                        {
                            currentFlagUrl = flagImgNode.GetAttributeValue("src", "");
                        }

                        // If no flag is set, use the previous world flag
                        string flagUrl = string.IsNullOrEmpty(currentFlagUrl) ? "" : currentFlagUrl;

                        // Find the country from the row below the world
                        var countryNode = worldNode.Ancestors("tr").FirstOrDefault()?.SelectSingleNode(".//td//img[@title]");
                        string country = countryNode?.GetAttributeValue("title", "Unknown");

                        // Find the number of players in the next <td> cell
                        var playerCountNode = worldNode.Ancestors("tr").FirstOrDefault()?.SelectNodes("following-sibling::tr[1]//td[2]");
                        int playerCount = 0;
                        if (playerCountNode != null)
                        {
                            string playerText = playerCountNode.First().InnerText.Trim();
                            if (int.TryParse(playerText.Split(' ')[0], out int players))
                            {
                                playerCount = players;
                            }
                        }

                        // Create a new WorldData object
                        var worldData = new WorldData
                        {
                            WorldName = worldName,
                            WorldId = worldId,
                            FlagUrl = flagUrl,
                            Country = country,
                            PlayerCount = playerCount
                        };

                        // Add the world data to the list
                        worldList.Add(worldData);
                    }

                    // Populate UI elements
                    PopulateWorldTabPage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching world data: {ex.Message}");
            }
        }

        private void PopulateWorldTabPage()
        {
            // Create a new TabPage
            TabPage tabPage = new TabPage("Worlds");

            int yOffset = 10; // Start at the top of the TabPage

            foreach (var world in worldList)
            {
                // Create labels for world name, country, player count
                Label worldNameLabel = new Label
                {
                    Text = $"{world.WorldName} ({world.PlayerCount} players)",
                    Location = new Point(10, yOffset),
                    AutoSize = true
                };
                yOffset += worldNameLabel.Height + 5;

                //Label countryLabel = new Label
                //{
                //    Text = $"Country: {world.Country}",
                //    Location = new Point(10, yOffset),
                //    AutoSize = true
                //};
                //yOffset += countryLabel.Height + 5;

                //Label playerCountLabel = new Label
                //{
                //    Text = $"Players: {world.PlayerCount}",
                //    Location = new Point(10, yOffset),
                //    AutoSize = true
                //};
                //yOffset += playerCountLabel.Height + 5;

                // Create a PictureBox for the flag
                //PictureBox flagPictureBox = new PictureBox
                //{
                //    Location = new Point(10, yOffset),
                //    Size = new Size(50, 30),
                //    SizeMode = PictureBoxSizeMode.StretchImage,
                //    ImageLocation = "",
                //};
                //flagPictureBox.LoadAsync(world.FlagUrl);
                //yOffset += flagPictureBox.Height + 10;

                worldNameLabel.DoubleClick += (sender, e) => OnWorldDoubleClick(world); // Double-click handler for flag image


                // Add the controls to the TabPage
                tabPage.Controls.Add(worldNameLabel);
                //tabPage.Controls.Add(countryLabel);
                //tabPage.Controls.Add(playerCountLabel);
                //tabPage.Controls.Add(flagPictureBox);
            }

            // Add the TabPage to the TabControl
            tabControl1.TabPages.Add(tabPage);
        }

        private void OnWorldDoubleClick(WorldData world)
        {
            MessageBox.Show($"Loading world {world.WorldName}.");
            webView.Source = new Uri($"https://2004.lostcity.rs/client?world={world.WorldId}&detail={Settings.FavDetailSettings.ToString().ToLower()}&method=0");
            webView.EnsureCoreWebView2Async();
        }


    }
}
