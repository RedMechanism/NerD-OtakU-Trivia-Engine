using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Media;

namespace NOTE
{
    public partial class ControlCenter : Window
    {
        public Teams Team1 = new Teams
        {
            Name = "Team 1",
            Score = 0,
            LogoPath = "Images/Team1.png",
            SoundPath = "Audio/Teams/Team1.mp3",
            PrimaryColour = new SolidColorBrush(Colors.DeepSkyBlue),
            SecondaryColour = new SolidColorBrush(Colors.DodgerBlue)
        };

        public Teams Team2 = new Teams
        {
            Name = "Team 2",
            Score = 0,
            LogoPath = "Images/Team2.png",
            SoundPath = "Audio/Teams/Team2.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Gold),
            SecondaryColour = new SolidColorBrush(Colors.Goldenrod)
        };

        public Teams Team3 = new Teams
        {
            Name = "Team 3",
            Score = 0,
            LogoPath = "Images/Team3.png",
            SoundPath = "Audio/Teams/Team3.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Lime),
            SecondaryColour = new SolidColorBrush(Colors.LimeGreen)
        };

        public Teams Team4 = new Teams
        {
            Name = "Team 4",
            Score = 0,
            LogoPath = "Images/Team4.png",
            SoundPath = "Audio/Teams/Team4.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Red),
            SecondaryColour = new SolidColorBrush(Colors.Crimson)
        };

        public bool PlayerRunning = false;

        public bool MediaPlaying = false;

        public static ControlCenter Instance;

        public CountdownTimer _Timer;

        public string fileName = @$"C:/Users/Robin/Desktop/{GetShortTimestamp(DateTime.Now)}_NOtrivia_logs.txt";
        public ControlCenter()
        {
            InitializeComponent();

            Instance = this;

            _Timer = new CountdownTimer(new TimeSpan(0, 0, 60));
            _Timer.TickEvent += new CountdownTimer.TimerTickHandler(TimerDisplay);

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

        #region Timing
        protected void TimerDisplay(TimeSpan timerValue)
        {
            Timer_display.Content = timerValue.TotalSeconds;
        }
        private void TimerStart_Button(object sender, RoutedEventArgs e)
        {
            _Timer.Start();
        }

        private void TimerStop_Button(object sender, RoutedEventArgs e)
        {
            _Timer.Stop();
        }

        private void TimerReset_Button(object sender, RoutedEventArgs e)
        {
            _Timer.Reset();
        }

        private void TimerClear_Button(object sender, RoutedEventArgs e)
        {
            ClearTimer();
        }

        private void Timer60_Button(object sender, RoutedEventArgs e)
        {
            SetTimer(60);
            _Timer.Start();
        }
        private void Timer30_Button(object sender, RoutedEventArgs e)
        {
            SetTimer(30);
            _Timer.Start();
        }
        private void Timer15_Button(object sender, RoutedEventArgs e)
        {
            SetTimer(15);
            _Timer.Start();
        }

        private void Timer_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Points_input.Text.All(char.IsDigit))
                {
                    int timerInput = int.Parse(Custom_timer_input.Text);
                    SetTimer(timerInput);
                    _Timer.Start();
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
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

        private void SetTimer(int duration)
        {
            if (PlayerRunning)
            {
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Visible;
            }
            _Timer.Reset();
            _Timer.Duration = new TimeSpan(0, 0, duration);
            Timer_display.Content = _Timer.CurrentTime.TotalSeconds;
        }

        public void ClearTimer()
        {
            _Timer.Reset();

            if (PlayerRunning)
            {
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Hidden;
                TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Hidden;
            }
            Timer_display.Content = "";
        }
        # endregion

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

            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
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

            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
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

        # region Button click events
        private void LaunchPlayer_Button(object sender, RoutedEventArgs e)
        {
            var playerWindowCount = Application.Current.Windows.OfType<TriviaPlayer>().Count();

            if (playerWindowCount < 1)
            {
                TriviaPlayer player = new TriviaPlayer();
                player.Show();
                PlayerRunning = true;
            }
            else
            {
                Application.Current.Windows.OfType<TriviaPlayer>().First().Topmost = true;
                SystemSounds.Beep.Play();
            }
        }
        private void Start_pause_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Files_Page.Instance.dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                    TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Visible;

                    if (Files_Page.Instance.playState)
                    {
                        //TriviaPlayer.Instance.mediaPlayer.Pause();
                        Files_Page.Instance.playState = false;
                        _Timer.Stop();
                    }
                    else
                    {
                        //TriviaPlayer.Instance.mediaPlayer.Play();
                        Files_Page.Instance.playState = true;
                        _Timer.Start();
                    }
                }
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

        private void Show_scores_Click(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.ShowScores();
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

        private void Questions_Page_Button(object sender, RoutedEventArgs e)
        {
            Page_Frame.Content = new Questions_Page();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.FinalScores();
        }

        #endregion

        #region Sounds
        private void playSound(string path)
        {
            Tick_sound.Source = new Uri(path, UriKind.Relative);
            Tick_sound.Play();
        }

        #endregion
        private void Checked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.Options.Visibility = Visibility.Visible;
        }

        private void NotCehcked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.Options.Visibility = Visibility.Hidden;
        }

        private void ImChecked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.myMedia.Visibility = Visibility.Hidden;
            TriviaPlayer.Instance.ImagePlayer.Visibility = Visibility.Visible;
        }

        private void ImNotCehcked(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.myMedia.Visibility = Visibility.Visible;
            TriviaPlayer.Instance.ImagePlayer.Visibility = Visibility.Hidden;
        }

        private void Page_Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

    }

}
