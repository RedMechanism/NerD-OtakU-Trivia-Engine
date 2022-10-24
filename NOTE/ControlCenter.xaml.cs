using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace NOTE
{
    public partial class ControlCenter : Window
    {
        public Teams Team1 = new Teams("Team 1", 0,
                "Images/Team1.png",
                "Audio/Teams/Team1.mp3",
                Colors.DeepSkyBlue, Colors.DodgerBlue);

        public Teams Team2 = new Teams("Team 2", 0,
                "Images/Team2.png",
                "Audio/Teams/Team2.mp3",
                Colors.Gold, Colors.Goldenrod);

        public Teams Team3 = new Teams("Team 3", 0,
                "Images/Team3.png",
                "Audio/Teams/Team3.mp3",
                Colors.Lime, Colors.LimeGreen);

        public Teams Team4 = new Teams("Team 4", 0,
                "Images/Team4.png",
                "Audio/Teams/Team4.mp3",
                Colors.Red, Colors.Crimson);

        public bool PlayerRunning = false;

        public bool MediaPlaying = false;

        public static ControlCenter Instance;

        public string fileName = @$"C:/{GetShortTimestamp(DateTime.Now)}_NOtrivia_logs.txt";
        public ControlCenter()
        {
            InitializeComponent();

            Instance = this;

            if (!File.Exists(fileName))
            {
                using (StreamWriter writer = File.CreateText(fileName))
                {
                    writer.WriteLine($"Trivia stated at {GetTimestamp(DateTime.Now)}");
                } 
            }
            else
            {
                using (StreamWriter writer = File.AppendText(fileName))
                {
                    writer.WriteLine($"Trivia stated at {GetTimestamp(DateTime.Now)}");
                }
            }
        }

        public int questionPoints = 10;
        public int bonusPoints = 5;
        public int tricklePoints = 5;
        public int tricklePenalty = 5;
        public int penaltyPoints = 5;
        public int questionTime = 60;
        public int currentTime;

        private void LaunchPlayer_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerRunning)
            {
                MessageBox.Show("An instance of the trivia player is already running");
            }
            else
            {
                TriviaPlayer player = new TriviaPlayer();
                player.Show();
                PlayerRunning = true;
            }
        }
        private void Play_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Files_Page.Instance.dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    if (Files_Page.Instance.playState)
                    {
                        TriviaPlayer.Instance.mediaPlayer.Pause();
                        Files_Page.Instance.playState = false;
                    }
                    else
                    {
                        TriviaPlayer.Instance.mediaPlayer.Play();
                        Files_Page.Instance.playState = true;
                    }
                }
            }
        }
        private void Stop_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Files_Page.Instance.dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    TriviaPlayer.Instance.mediaPlayer.Stop();
                }
            }
        }

        private object interNode = null; //transfer tree node info between functions
        private void Load_Folder(object sender, RoutedEventArgs e)
        {
            Files_Page scoresPage = new Files_Page();
            Page_Frame.Content = scoresPage;
            System.Windows.Forms.FolderBrowserDialog dirBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (dirBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String pathName = dirBrowser.SelectedPath;

                foreach (string dirPath in Directory.GetDirectories(pathName))
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = dirPath.Substring(dirPath.LastIndexOf("\\") + 1);
                    item.Tag = dirPath;
                    item.Items.Add(interNode);
                    item.Expanded += new RoutedEventHandler(treeExpansion);
                    Files_Page.Instance.dirTree.Items.Add(item);
                }
            }
        }
        void treeExpansion(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == interNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subDirItem = new TreeViewItem();
                        //subitem.Header = subDir.Substring(subDir.LastIndexOf("\\") + 1);
                        subDirItem.Header = subDir;
                        subDirItem.Tag = subDir;
                        subDirItem.Items.Add(interNode);
                        subDirItem.Expanded += new RoutedEventHandler(treeExpansion);
                        item.Items.Add(subDir.Substring(subDir.LastIndexOf("\\") + 1));
                    }

                    foreach (string fileName in Directory.GetFiles(item.Tag.ToString()))
                    {
                        TreeViewItem fileItem = new TreeViewItem();
                        // fileItem.Header = fileName.Substring(fileName.LastIndexOf("\\") + 1)
                        fileItem.Header = fileName;
                        fileItem.Tag = fileName;
                        item.Items.Add(fileName);
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            e.Handled = true;
        }
        private void Timer60_Button(object sender, RoutedEventArgs e)
        {
            StartTimer(60);
        }
        private void Timer30_Button(object sender, RoutedEventArgs e)
        {
            StartTimer(30);
        }
        private void Timer15_Button(object sender, RoutedEventArgs e)
        {
            StartTimer(15);
        }

        private void Start_custom_timer_Button(object sender, RoutedEventArgs e)
        {
            if (Custom_timer_input.Text.All(char.IsDigit))
            {
                int timerInput = int.Parse(Custom_timer_input.Text);
                StartTimer(timerInput);
            }
            else
            {
                MessageBox.Show("Enter only positive digits");
            }
        }

        private bool countdownRunning = false;
        private void StartTimer(int duration)
        {
            if (PlayerRunning)
            {
                if (countdownRunning)
                {
                    MessageBox.Show("Clear running timer first!");
                }
                else
                {

                    TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                    Countdown(duration, TimeSpan.FromSeconds(1), count => TriviaPlayer.Instance.displayTimer.Text = count.ToString());
                }
            }
        }
        private void ClearTimer_Button(object sender, RoutedEventArgs e)
        {
            ClearClock();
        }

        public void ClearClock()
        {
            if (PlayerRunning)
            {
                countdownRunning = false;
                TriviaPlayer.Instance.displayTimer.Text = "";
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Collapsed;
            }
        }

        void Countdown(int count, TimeSpan interval, Action<int> timerStart)
        {
            countdownRunning = true;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = interval;
            timer.Tick += (_, a) =>
            {
                if (count-- == 0)
                {
                    Tick_sound.Stop();
                    timer.Stop();
                }
                else if (!countdownRunning)
                {
                    timer.Stop();
                    Tick_sound.Stop();
                }
                else
                    timerStart(count);
            };
            timerStart(count);
            timer.Start();
        }

        private void Settings_Page_Button(object sender, RoutedEventArgs e)
        {
            Settings_Page settingsPage = new Settings_Page();
            Page_Frame.Content = settingsPage;
        }

        private void File_viewer_Button(object sender, RoutedEventArgs e)
        {
            Page_Frame.Content = Files_Page.Instance;
        }

        private void Scores_page_Button(object sender, RoutedEventArgs e)
        {
            Page_Frame.Content = new Scores_Page();
        }

        private void Team1_moniker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MessageBox.Show(Settings_Page.Instance.Team1_moniker.Text);
            }
        }

        #region Scoring

        private void AddPoints(Teams TeamX, int points)
        {
            Animations Animation = new Animations();

            TeamX.Score += points;

            // Update control info
            Status_disp.Content = $"{TeamX.Name} +{points}";
            if (PlayerRunning)
            {
                TriviaPlayer.Instance.Points_awarded_disp.Content = $"+{points}";
                TriviaPlayer.Instance.Points_awarded_disp.Foreground = new SolidColorBrush(Colors.Green);

                // Animations
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }
            
            //Write logs
            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, +{points}, New total = {TeamX.Score} points");
            }

            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
        }

        private void DeductPoints(Teams TeamX, int points)
        {
            Animations Animation = new Animations();

            TeamX.Score -= points;

            // Update labels 
            Status_disp.Content = $"{TeamX.Name} -{points}";
            if (PlayerRunning)
            {
                TriviaPlayer.Instance.Points_awarded_disp.Content = $"-{points}";
                TriviaPlayer.Instance.Points_awarded_disp.Foreground = new SolidColorBrush(Colors.Red);

                // Animations
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }

            //Write logs
            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, -{points}, New total = {TeamX.Score} points");
            }

            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
        }

        private void WrongAnswer(Teams TeamX)
        {
            Animations Animation = new Animations();

            //Write logs
            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name} incorrect answer");
                Status_disp.Content = $"{TeamX.Name} incorrect answer";
            }
            if (PlayerRunning)
            {
                // Animations
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Wrong_answer_image);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }

            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm:ss-yyyy.MM.dd");
        }

        public static String GetShortTimestamp(DateTime value)
        {
            return value.ToString("yyyy.MM.dd");
        }
        private void Answer_correct_Button(object sender, RoutedEventArgs e)
        {
            if (Team1_radio_button.IsChecked == true)
            {
                AddPoints(Team1, questionPoints);
                playSound(Team1.SoundPath);
            }
            if (Team2_radio_button.IsChecked == true)
            {
                AddPoints(Team2, questionPoints);
                playSound(Team2.SoundPath);
            }
            if (Team3_radio_button.IsChecked == true)
            {
                AddPoints(Team3, questionPoints);
                playSound(Team3.SoundPath);
            }
            if (Team4_radio_button.IsChecked == true)
            {
                AddPoints(Team4, questionPoints);
                playSound(Team4.SoundPath);
            }
        }

        private void Trickle_correct_Button(object sender, RoutedEventArgs e)
        {
            if (Team1_radio_button.IsChecked == true)
            {
                AddPoints(Team1, tricklePoints);
            }
            if (Team2_radio_button.IsChecked == true)
            {
                AddPoints(Team2, tricklePoints);
            }
            if (Team3_radio_button.IsChecked == true)
            {
                AddPoints(Team3, tricklePoints);
            }
            if (Team4_radio_button.IsChecked == true)
            {
                AddPoints(Team4, tricklePoints);
            }
        }

        private void Bonus_correct_Button(object sender, RoutedEventArgs e)
        {
            if (Team1_radio_button.IsChecked == true)
            {
                AddPoints(Team1, bonusPoints);
            }
            if (Team2_radio_button.IsChecked == true)
            {
                AddPoints(Team2, bonusPoints);
            }
            if (Team3_radio_button.IsChecked == true)
            {
                AddPoints(Team3, bonusPoints);
            }
            if (Team4_radio_button.IsChecked == true)
            {
                AddPoints(Team4, bonusPoints);
            }
        }

        private void Answer_wrong_Button(object sender, RoutedEventArgs e)
        {
            if (Team1_radio_button.IsChecked == true)
            {
                WrongAnswer(Team1);
            }
            if (Team2_radio_button.IsChecked == true)
            {
                WrongAnswer(Team2);
            }
            if (Team3_radio_button.IsChecked == true)
            {
                WrongAnswer(Team3);
            }
            if (Team4_radio_button.IsChecked == true)
            {
                WrongAnswer(Team4);
            }

            playSound(selectRandomFile("Audio/Incorrect"));
        }

        private void Trickle_wrong_Button(object sender, RoutedEventArgs e)
        {

            if (Team1_radio_button.IsChecked == true)
            {
                DeductPoints(Team1, tricklePenalty);
            }
            if (Team2_radio_button.IsChecked == true)
            {
                DeductPoints(Team2, tricklePenalty);
            }
            if (Team3_radio_button.IsChecked == true)
            {
                DeductPoints(Team3, tricklePenalty);
            }
            if (Team4_radio_button.IsChecked == true)
            {
                DeductPoints(Team4, tricklePenalty);
            }

            playSound(selectRandomFile("Audio/Incorrect"));
        }

        private void Points_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Points_input.Text.All(char.IsDigit))
                {
                    questionPoints = int.Parse(Points_input.Text);
                    Points_avail_disp.Content = $"{questionPoints}pts";
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }

        private void Trickle_points_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Trickle_points_input.Text.All(char.IsDigit))
                {
                    tricklePoints = int.Parse(Trickle_points_input.Text);
                    Trickle_points_avail_disp.Content = $"{tricklePoints}pts";
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }

        private void Bonus_points_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Bonus_points_input.Text.All(char.IsDigit))
                {
                    bonusPoints = int.Parse(Bonus_points_input.Text);
                    Bonus_points_avail_disp.Content = $"{bonusPoints}pts";
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }

        private void Trickle_penalty_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Trickle_penalty_input.Text.All(char.IsDigit))
                {
                    tricklePenalty = int.Parse(Trickle_penalty_input.Text);
                    Trickle_penalty_avail_disp.Content = $"-{tricklePenalty}pts";
                }
                else
                {
                    MessageBox.Show("Enter only POSITIVE digits");
                }
            }
        }

        # endregion

        private void Start_pause_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Files_Page.Instance.dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    if (Files_Page.Instance.playState)
                    {
                        TriviaPlayer.Instance.mediaPlayer.Pause();
                        currentTime = int.Parse(TriviaPlayer.Instance.displayTimer.Text.ToString());
                        Files_Page.Instance.playState = false;
                        countdownRunning = false;
                    }
                    else
                    {
                        TriviaPlayer.Instance.mediaPlayer.Play();
                        Files_Page.Instance.playState = true;
                        StartTimer(currentTime);
                    }
                }
            }
        }

        private void Time_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Time_input.Text.All(char.IsDigit))
                {
                    questionTime = int.Parse(Time_input.Text);
                    Time_avail_disp.Content = $"{questionTime}sec";
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }

        private void playSound(string path)
        {
            Tick_sound.Source = new Uri(path, UriKind.Relative);
            Tick_sound.Play();
        }

        private string selectRandomFile(string dir)
        {
            var directoryInfo = new DirectoryInfo(dir).GetFiles("*.*");
            Random random = new Random();
            string randomFile = directoryInfo.ElementAt(random.Next(0, directoryInfo.Length)).FullName;
            return randomFile;
        }
        private void Show_scores_Click(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.ShowScores();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.FinalScores();
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.Options.Visibility = Visibility.Visible;
        }

        private void NotCehcked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.Options.Visibility = Visibility.Hidden;
        }
    }

}
