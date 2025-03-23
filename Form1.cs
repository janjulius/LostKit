using HtmlAgilityPack;
using LostKit.Helpers;
using LostKit.Models;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Svg;
using System.Net;
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

        private Settings settings;

        public Form1()
        {
            settings = Settings.Load();
            Directory.CreateDirectory(Path.GetDirectoryName(notesFilePath));
            InitializeComponent();
            //LoadWorldData();
            LoadWorldData2();

            RenderWebPage();
            RenderMarketPage();
            RenderDropTablesPage();
            RenderMapPage();
            LoadNotes();
            SkillLabel_MouseLeave(null, null);
            menuStrip1.Visible = false;

            this.FormClosing += Application_FormClosing;
        }

        private void RenderDropTablesPage()
        {
            DroptableWebview.Source = new Uri($"https://thesneilert.github.io/2004scape/");
            DroptableWebview.EnsureCoreWebView2Async();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Y <= 1)
            {
                if (!menuStrip1.Visible)
                {
                    menuStrip1.Visible = true;
                    menuStrip1.BringToFront();
                }
            }
            else
            {
                if (menuStrip1.Visible)
                {
                    menuStrip1.Visible = false;
                }
            }
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

            webView.Source = new Uri($"https://2004.lostcity.rs/client?world={settings.FavWorld}&detail={settings.FavDetailSettings.ToString().ToLower()}&method=0");
            webView.EnsureCoreWebView2Async();
            SetTitle($"LostKit (World {settings.FavWorld}) (Detail mode: {settings.FavDetailSettings.ToString()})");
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

                    for (int i = 0; i < skillRecords.Count; i++)
                    {
                        var nextLvlXP = RunescapeCalculators.GetExperienceForLevel(skillRecords[i].Level + 1);
                        skillRecords[i].NextLevelExp = nextLvlXP;
                    }

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

        public static Bitmap DownloadImage(string url)
        {
            using WebClient client = new WebClient();
            byte[] imageData = client.DownloadData(url);

            using var stream = new System.IO.MemoryStream(imageData);
            return new Bitmap(stream);
        }

        private Bitmap DownloadAndConvertSvgToBitmap(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    // Download SVG content as a string
                    string svgContent = client.DownloadString(url);

                    if (string.IsNullOrEmpty(svgContent))
                        throw new Exception("SVG content is empty.");

                    // Load the SVG content into an SvgDocument
                    SvgDocument svgDocument = SvgDocument.FromSvg<SvgDocument>(svgContent);

                    // Set up the size for the Bitmap (can be adjusted)
                    int width = (int)svgDocument.Width;
                    int height = (int)svgDocument.Height;

                    // Create a Bitmap based on the SVG size
                    Bitmap bitmap = new Bitmap(width, height);

                    // Draw the SVG content to the Bitmap
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);  // Optional: set background color to transparent
                        svgDocument.Draw(g);  // Render SVG onto the Graphics object
                    }

                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or rendering SVG from {url}: {ex.Message}");
                return null;
            }
        }

        private async void LoadWorldData2()
        {
            worldList.Clear();

            string url = "https://2004.lostcity.rs/serverlist?hires.x=112&hires.y=16&method=0";
            string htmlContent = await httpClient.GetStringAsync(url);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);

            var table = doc.DocumentNode.SelectSingleNode("//table"); // Select the table
            var rows = table.SelectNodes(".//tr");

            string currentRegion = "";
            string currentFlag = "";
            var worlds = new List<WorldData>();

            foreach (var row in rows)
            {
                var cols = row.SelectNodes(".//td");
                if (cols == null) continue;

                if (cols.Count == 1)
                {
                    // Region Row (contains flag and region name)
                    var imgNode = cols[0].SelectSingleNode(".//img");
                    var text = cols[0].InnerText.Trim();

                    if (imgNode != null && !string.IsNullOrEmpty(text))
                    {
                        currentFlag = imgNode.GetAttributeValue("src", "");  // Extract flag URL
                        currentRegion = text; // Extract region name
                    }
                }
                else if (cols.Count == 2)
                {
                    // World Row (contains world name and player count)
                    var worldNode = cols[0].SelectSingleNode(".//a");
                    string world = worldNode != null ? worldNode.InnerText.Trim() : cols[0].InnerText.Trim();
                    string players = cols[1].InnerText.Trim();
                    int.TryParse(players.Split(' ')[0], out int playerCountInt);
                    int worldID = int.TryParse(System.Text.RegularExpressions.Regex.Match(world, @"\d+").Value, out int id) ? id : -1;

                    var flagUrl = $"https://2004.lostcity.rs{currentFlag}";
                    if (world.ToLower().Contains("world"))
                    {
                        worldList.Add(new WorldData
                        {
                            WorldName = world,
                            PlayerCount = playerCountInt,
                            Country = currentRegion.Replace("&nbsp;&nbsp; ", "").Trim(),
                            FlagUrl = flagUrl,
                            Offline = players == "OFFLINE",
                            FlagImage = DownloadAndConvertSvgToBitmap(flagUrl)
                        });
                    }
                }
            }
            PopulateWorldTabPage();

        }

        private void PopulateWorldTabPage()
        {
            TabPage tabPage = new TabPage("Worlds");
            tabPage.AutoScroll = true;
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

                LoadWorldData2();
            };
            yOffset += 25;
            tabPage.Controls.Add(reloadButton);


            foreach (var world in worldList)
            {
                PictureBox flagBox = new PictureBox
                {
                    Image = world.FlagImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(32, 20), // Adjust size if needed
                    Location = new Point(10, yOffset)
                };
                Label worldNameLabel = new Label
                {
                    Text = world.Offline ? $"{world.WorldName} (OFFLINE)" : $"{world.WorldName} ({world.PlayerCount} players) ({world.Country})",
                    Location = new Point(10 + flagBox.Width, yOffset),
                    AutoSize = true
                };
                yOffset += worldNameLabel.Height + 5;

                worldLabels.Add(worldNameLabel);

                if (!world.Offline)
                {
                    worldNameLabel.DoubleClick += (sender, e) => OnWorldDoubleClick(world); // Double-click handler for flag image
                }

                tabPage.Controls.Add(worldNameLabel);
                tabPage.Controls.Add(flagBox);
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
            settings = Settings.Load();
            webView.Source = new Uri($"https://2004.lostcity.rs/client?world={world.WorldId}&detail={settings.FavDetailSettings.ToString().ToLower()}&method=0");
            webView.EnsureCoreWebView2Async();
            SetTitle($"LostKit (World {world.WorldId}) (Detail mode: {settings.FavDetailSettings.ToString()})");
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

                var attackLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Attack))?.Level;
                var strengthLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Strength))?.Level;
                var hitpointsLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Hitpoints))?.Level;
                var defenceLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Defence))?.Level;
                var rangedLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Ranged))?.Level;
                var magicLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Magic))?.Level;
                var prayerLevel = skillRecords.Find(x => x.Type.Equals(SkillType.Prayer))?.Level;

                SetCombatLevelHiScore(GetCombatLevel(
                    attackLevel ?? 1,
                    strengthLevel ?? 1,
                    hitpointsLevel ?? 1,
                    defenceLevel ?? 1,
                    rangedLevel ?? 1,
                    magicLevel ?? 1,
                    prayerLevel ?? 1
                ));
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
            //toolTip1.SetToolTip(label, $"Total exp: {NumberHelper.MakeBigNumberReadable(record.Value)}" +
            //    $"\nRank: {NumberHelper.MakeBigNumberReadable(record.Rank)}");

            label.MouseEnter += (sender, e) => SkillLabel_MouseEnter(sender, e, record);
            label.MouseLeave += SkillLabel_MouseLeave;
        }

        private void SkillLabel_MouseLeave(object? sender, EventArgs e)
        {
            SkillExtraInfoBox.Text = "Skill:\n\nRank:\n\nExperience:\n\nXp To Next Lvl:\n\n";
        }

        private void SkillLabel_MouseEnter(object? sender, EventArgs e, SkillRecord record)
        {
            SkillExtraInfoBox.Text = $"Skill:\n{record.Type.ToString()}\nRank:\n{record.Rank}\nExperience:\n{NumberHelper.MakeBigNumberReadable(record.Value)}\nXp To Next Lvl:\n{NumberHelper.MakeBigNumberReadable(record.NextLevelExp - record.Value)}\n";
        }

        private void SetCombatLevelHiScore(double lvl)
        {
            HiScoreCombat.Text = lvl.ToString();
        }


        private double GetCombatLevel(int attack, int strength, int hitpoints, int defense, int ranged, int magic, int prayer)
        {
            double base_lvl = Convert.ToInt32(prayer / 2 - 0.5 + hitpoints + defense) / 4;
            double melee = Convert.ToInt32((attack + strength) * 0.325);
            double range = Convert.ToInt32((ranged * 1.5 - 0.5) * 0.325);
            double mage = Convert.ToInt32((magic * 1.5 - 0.5) * 0.325);

            if (range > melee || mage > melee)
            {
                return range >= mage ? base_lvl + range : base_lvl + mage;
            }

            return base_lvl + melee;

        }

        private void SetTitle(string title)
        {
            this.Text = title;
        }

        private void preferencesToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            PreferencesForm pForm = new PreferencesForm();
            pForm.ShowDialog();
        }
    }
}
