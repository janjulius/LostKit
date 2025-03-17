using HtmlAgilityPack;
using LostKit.Helpers;
using LostKit.Models;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Forms;

namespace LostKit
{
    public partial class Form1 : Form
    {
        public WebView2 webView;

        private List<WorldData> worldList = new List<WorldData>();
        private readonly HttpClient httpClient = new HttpClient();

        private string notesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotesApp", "notes.txt");


        public Form1()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(notesFilePath));
            InitializeComponent();
            LoadWorldData();

            RenderWebPage();
            RenderMarketPage();
            RenderMapPage();
            LoadNotes();

            this.FormClosing += Application_FormClosing;
        }

        private void Application_FormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveNotes();
        }

        private void LoadNotes()
        {
            if (File.Exists(notesFilePath))
            {
                NotesTextBox.Text = File.ReadAllText(notesFilePath);
            }
        }

        private void SaveNotes()
        {
            File.WriteAllText(notesFilePath, NotesTextBox.Text);
        }

        private void RenderMapPage()
        {
            webView23.Source = new Uri($"https://2004.lostcity.rs/worldmap");
            webView23.EnsureCoreWebView2Async();
            webView23.NavigationCompleted += (s, e) => ResizeMap();

        }

        private async void ResizeMap()
        {
            if (webView23.CoreWebView2 != null)
            {
                await webView23.CoreWebView2.ExecuteScriptAsync(
                    """
                    var canv = document.getElementById('canvas');
                    canv.width = 330;
                    canv.height = 543; 
                    var secondCenter = document.querySelectorAll('center')[1]; 
                    if (secondCenter) { secondCenter.remove(); }
                                    var brElements = document.querySelectorAll('br'); // Select all <br> elements
                brElements.forEach(function(br) {
                    br.remove(); // Remove each <br> element
                });
                """
                    );
            }
        }

        private void RenderMarketPage()
        {
            webView22.Source = new Uri($"https://lostcity.markets/");
            webView22.EnsureCoreWebView2Async();
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

        private async Task<List<SkillRecord>> SearchPlayerByName(string playerName)
        {
            string url = $"https://2004.lostcity.rs/api/hiscores/player/{playerName}";

            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send GET request to the API
                    string jsonResponse = await client.GetStringAsync(url);

                    // Deserialize the JSON response into a list of SkillRecord objects
                    List<SkillRecord> skillRecords = JsonConvert.DeserializeObject<List<SkillRecord>>(jsonResponse);

                    // Print out the results
                    //foreach (var record in skillRecords)
                    //{
                    //    //MessageBox.Show($"{record.Type} - Level: {record.Level}, Value: {record.Value / 10}, Date: {record.Date}, Rank: {record.Rank}");
                    //}

                    return skillRecords;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }

        private async void LoadWorldData()
        {
            try
            {
                worldList.Clear();
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
                        var playerCountNode = worldNode.Ancestors("tr").FirstOrDefault()?.SelectSingleNode("./td[2]");
                        int playerCount = 0;
                        if (playerCountNode != null)
                        {
                            string playerText = playerCountNode.InnerText.Trim();
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
            TabPage tabPage = new TabPage("Worlds");
            int yOffset = 10; // Start at the top of the TabPage
            List<Label> worldLabels = new List<Label>();

            CheckBox toggleLabelsCheckBox = new CheckBox
            {
                Text = "Allow double-click to swap worlds",
                Location = new Point(10, yOffset),
                AutoSize = true,
                Checked = true // Labels start enabled
            };
            yOffset += 20;

            Button reloadButton = new Button
            {
                Text = "Reload",
                Location = new Point(10, yOffset),
                AutoSize = true
            };

            reloadButton.Click += (sender, e) =>
            {
                foreach (TabPage tab in TabControl1.TabPages)
                {
                    if (tab.Text == "Worlds")
                    {
                        tab.Controls.Clear();
                    }
                }

                for (int i = 0; i < TabControl1.TabPages.Count; i++)
                {
                    if (TabControl1.TabPages[i].Text == "Worlds")
                    {
                        TabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }

                LoadWorldData();
            };
            yOffset += 25;
            tabPage.Controls.Add(reloadButton);


            foreach (var world in worldList)
            {

                Label worldNameLabel = new Label
                {
                    Text = $"{world.WorldName} ({world.PlayerCount} players)",
                    Location = new Point(10, yOffset),
                    AutoSize = true
                };
                yOffset += worldNameLabel.Height + 5;

                worldLabels.Add(worldNameLabel);
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

                tabPage.Controls.Add(worldNameLabel);
                //tabPage.Controls.Add(countryLabel);
                //tabPage.Controls.Add(playerCountLabel);
                //tabPage.Controls.Add(flagPictureBox);
            }
            toggleLabelsCheckBox.CheckedChanged += (sender, e) =>
            {
                foreach (var label in worldLabels)
                {
                    label.Visible = toggleLabelsCheckBox.Checked;
                }
            };

            tabPage.Controls.Add(toggleLabelsCheckBox);
            TabControl1.TabPages.Add(tabPage);
        }

        private void OnWorldDoubleClick(WorldData world)
        {
            webView.Source = new Uri($"https://2004.lostcity.rs/client?world={world.WorldId}&detail={Settings.FavDetailSettings.ToString().ToLower()}&method=0");
            webView.EnsureCoreWebView2Async();
        }

        private void LoadHighscoreTab()
        {

        }

        private async void HigscoreSearch_onclick(object sender, MouseEventArgs e)
        {
            var searchName = HiscoreSearchBox.Text.Trim().ToLower().Replace(' ', '_');
            var skillRecords = await SearchPlayerByName(searchName);

            if (skillRecords != null)
            {
                foreach (var skillRecord in skillRecords)
                {
                    LoadHiscoreSkill(skillRecord);
                }

                SetCombatLevelHiScore(GetCombatLevel(skillRecords[(int)SkillType.Attack].Level,
                    skillRecords[(int)SkillType.Strength].Level,
                    skillRecords[(int)SkillType.Hitpoints].Level,
                    skillRecords[(int)SkillType.Defence].Level,
                    skillRecords[(int)SkillType.Ranged].Level,
                    skillRecords[(int)SkillType.Magic].Level,
                    skillRecords[(int)SkillType.Prayer].Level));
            }

            LoadAdventureLog(searchName);

        }

        private async void LoadAdventureLog(string searchName)
        {
            string url = $"https://2004.lostcity.rs/player/adventurelog/{searchName}";
            string htmlContent = await httpClient.GetStringAsync(url);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);

            List<string> extractedData = new List<string>();

            var divs = doc.DocumentNode.SelectNodes("//div[@style='text-align: left']");


            if (divs != null)
            {
                foreach (var div in divs)
                {
                    var dateNode = div.SelectSingleNode(".//span");
                    var eventText = div.InnerText.Trim();

                    if (dateNode != null)
                    {
                        string date = dateNode.InnerText.Trim();
                        string eventDetail = eventText.Replace(date, "").Trim();
                        extractedData.Add($"{date} - {eventDetail}");
                    }
                }
            }
            richTextBox1.Text = extractedData.Count > 0
                ? string.Join(Environment.NewLine + "---" + Environment.NewLine, extractedData)
                : "No events found.";
        }

        private void LoadHiscoreSkill(SkillRecord record)
        {
            switch (record.Type)
            {
                case SkillType.Overall:
                    HiScoreOverall.Text = record.Level.ToString();
                    toolTip1.SetToolTip(HiScoreOverall, $"Total exp: {NumberHelper.MakeBigNumberReadable(record.Value)}" +
                        $"\nRank: {NumberHelper.MakeBigNumberReadable(record.Rank)}");
                    break;
                case SkillType.Attack:
                    LoadHiscoreSkill(record, ref HiscoreAttack);
                    break;
                case SkillType.Strength:
                    LoadHiscoreSkill(record, ref HiscoresStrength);
                    break;
                case SkillType.Defence:
                    LoadHiscoreSkill(record, ref HiscoresDefense);
                    break;
                case SkillType.Ranged:
                    LoadHiscoreSkill(record, ref HiscoresRanged);
                    break;
                case SkillType.Magic:
                    LoadHiscoreSkill(record, ref HiscoresMagic);
                    break;
                case SkillType.Prayer:
                    LoadHiscoreSkill(record, ref HiscoresPrayer);
                    break;
                case SkillType.Runecrafting:
                    LoadHiscoreSkill(record, ref HiscoresRunecraft);
                    break;
                case SkillType.Hitpoints:
                    LoadHiscoreSkill(record, ref HiscoresHitpoints);
                    break;
                case SkillType.Agility:
                    LoadHiscoreSkill(record, ref HiscoresAgility);
                    break;
                case SkillType.Thieving:
                    LoadHiscoreSkill(record, ref HiscoresThieving);
                    break;
                case SkillType.Herblore:
                    LoadHiscoreSkill(record, ref HiscoresHerblore);
                    break;
                case SkillType.Crafting:
                    LoadHiscoreSkill(record, ref HiscoresCrafting);
                    break;
                case SkillType.Fletching:
                    LoadHiscoreSkill(record, ref HiscoresFletching);
                    break;
                case SkillType.Mining:
                    LoadHiscoreSkill(record, ref HiscoresMining);
                    break;
                case SkillType.Smithing:
                    LoadHiscoreSkill(record, ref HiscoresSmithing);
                    break;
                case SkillType.Fishing:
                    LoadHiscoreSkill(record, ref HiscoresFishing);
                    break;
                case SkillType.Cooking:
                    LoadHiscoreSkill(record, ref HiscoresCooking);
                    break;
                case SkillType.Firemaking:
                    LoadHiscoreSkill(record, ref HiscoresFiremaking);
                    break;
                case SkillType.Woodcutting:
                    LoadHiscoreSkill(record, ref HiscoresWoodcutting);
                    break;


            }
        }

        private void LoadHiscoreSkill(SkillRecord record, ref Label label)
        {
            label.Text = record.Level.ToString();
            toolTip1.SetToolTip(label, $"Total exp: {NumberHelper.MakeBigNumberReadable(record.Value)}" +
                $"\nRank: {NumberHelper.MakeBigNumberReadable(record.Rank)}");
            //add next lv etc
        }

        private void SetCombatLevelHiScore(double lvl)
        {
            HiScoreCombat.Text = lvl.ToString();
        }


        private double GetCombatLevel(int attack, int strength, int hitpoints, int defense, int ranged, int magic, int prayer)
        {
            int base_lvl = Convert.ToInt32(prayer / 2 - 0.5 + hitpoints + defense) / 4;
            int melee = Convert.ToInt32((attack + strength) * 0.325);
            int range = Convert.ToInt32((ranged * 1.5 - 0.5) * 0.325);
            int mage = Convert.ToInt32((magic * 1.5 - 0.5) * 0.325);

            if (range > melee || mage > melee)
            {
                return range >= mage ? base_lvl + range : base_lvl + mage;
            }

            return base_lvl + melee;
            
        }
    }
}
