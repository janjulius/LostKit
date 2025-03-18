using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LostKit
{
    public partial class PreferencesForm : Form
    {
        private readonly string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data", "settings.json");
        private Settings settings;


        public PreferencesForm()
        {
            settings = Settings.Load();
            InitializeComponent();

            FavWorldTextBox.Text = settings.FavWorld.ToString();
            comboBox1.DataSource = Enum.GetValues(typeof(DetailSetting));
            comboBox1.SelectedIndex = (int)settings.FavDetailSettings;
        }

        private void SaveButton_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                settings.FavWorld = Convert.ToInt32(FavWorldTextBox.Text);
            }
            catch (FormatException er)
            { 
                MessageBox.Show($"The input value of the world needs to be an integer\n{er.Message}");
            }
            settings.FavDetailSettings = (DetailSetting)comboBox1.SelectedValue;

            settings.Save();

            this.Close();
        }

        private void SaveSettings()
        {
            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            }

            JsonConvert.SerializeObject(new Settings());

        }
    }
}
