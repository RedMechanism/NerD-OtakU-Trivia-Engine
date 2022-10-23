using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Scores_Page.xaml
    /// </summary>
    public partial class Scores_Page : Page
    {
        public static Scores_Page Instance;
        public Scores_Page()
        {
            InitializeComponent();
            Instance = this;

            Scores_team1_name.Text = ControlCenter.Instance.Team1.Name;
            Scores_team1_points.Text = ControlCenter.Instance.Team1.Score.ToString();

            Scores_team2_name.Text = ControlCenter.Instance.Team2.Name;
            Scores_team2_points.Text = ControlCenter.Instance.Team2.Score.ToString();

            Scores_team3_name.Text = ControlCenter.Instance.Team3.Name;
            Scores_team3_points.Text = ControlCenter.Instance.Team3.Score.ToString();

            Scores_team4_name.Text = ControlCenter.Instance.Team4.Name;
            Scores_team4_points.Text = ControlCenter.Instance.Team4.Score.ToString();
        }

        private void score_Changed(Teams TeamX, string score)
        {
                if (Scores_team1_points.Text.All(char.IsDigit))
                {
                    TeamX.Score = int.Parse(score);
                    using (StreamWriter writer = File.AppendText(ControlCenter.Instance.fileName))
                    {
                        writer.WriteLine($"{ControlCenter.GetTimestamp(DateTime.Now)} - {TeamX.Name} score manually changed to {TeamX.Score} points");
                        ControlCenter.Instance.Status_disp.Content = $"Set {TeamX.Name} points -> {score}";
                    }
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
        }
        private void Score_changed_team1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                score_Changed(ControlCenter.Instance.Team1, Scores_team1_points.Text);
            }
        }
        private void Score_changed_team2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                score_Changed(ControlCenter.Instance.Team2, Scores_team2_points.Text);
            }
        }
        private void Score_changed_team3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                score_Changed(ControlCenter.Instance.Team3, Scores_team3_points.Text);
            }
        }
        private void Score_changed_team4(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                score_Changed(ControlCenter.Instance.Team4, Scores_team4_points.Text);
            }
        }
    }
}
