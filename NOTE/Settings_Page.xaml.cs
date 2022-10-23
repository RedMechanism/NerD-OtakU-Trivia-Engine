using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;


namespace NOTE
{
    /// <summary>
    /// Interaction logic for Settings_Page.xaml
    /// </summary>
    public partial class Settings_Page : Page
    {
        public static Settings_Page Instance;

        public Settings_Page()
        {
            InitializeComponent();
            Instance = this;
            // Specify default values
            Team1_moniker.Text = ControlCenter.Instance.Team1.Name;
            Team2_moniker.Text = ControlCenter.Instance.Team2.Name;
            Team3_moniker.Text = ControlCenter.Instance.Team3.Name;
            Team4_moniker.Text = ControlCenter.Instance.Team4.Name;

            Change_default_points.Text = ControlCenter.Instance.questionPoints.ToString();
        }

        private void Team1_moniker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ControlCenter.Instance.Team1_radio_button.Content = Team1_moniker.Text;
                ControlCenter.Instance.Team1.Name = Team1_moniker.Text;
            }
        }

        private void Team2_moniker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ControlCenter.Instance.Team2_radio_button.Content = Team2_moniker.Text;
                ControlCenter.Instance.Team2.Name = Team2_moniker.Text;
            }
        }

        private void Team3_moniker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ControlCenter.Instance.Team3_radio_button.Content = Team3_moniker.Text;
                ControlCenter.Instance.Team3.Name = Team3_moniker.Text;
            }
        }

        private void Team4_moniker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ControlCenter.Instance.Team4_radio_button.Content = Team4_moniker.Text;
                ControlCenter.Instance.Team4.Name = Team4_moniker.Text;
            }
        }

        private void Default_points_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Change_default_points.Text.All(char.IsDigit))
                {
                    ControlCenter.Instance.questionPoints = int.Parse(Change_default_points.Text);
                    Settings_points_value.Text = ControlCenter.Instance.questionPoints.ToString();
                    ControlCenter.Instance.Points_avail_disp.Content = $"{int.Parse(Change_default_points.Text)}pts";
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }
    }
}
