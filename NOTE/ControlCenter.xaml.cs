using System;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Media;
using static NOTE.CountdownTimer;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NOTE
{
    public partial class ControlCenter : Window
    {
        public List<Teams> TeamsList = new List<Teams>();

        public Teams Team1 = new Teams
        {
            Name = "Team1",
            Score = 0,
            LogoPath = "Images/Team1.png",
            SoundPath = "Audio/Teams/Team1.mp3",
            PrimaryColour = new SolidColorBrush(Colors.DeepSkyBlue),
            SecondaryColour = new SolidColorBrush(Colors.DodgerBlue)
        };

        public Teams Team2 = new Teams
        {
            Name = "Team2",
            Score = 0,
            LogoPath = "Images/Team2.png",
            SoundPath = "Audio/Teams/Team2.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Gold),
            SecondaryColour = new SolidColorBrush(Colors.Goldenrod)
        };

        public Teams Team3 = new Teams
        {
            Name = "Team3",
            Score = 0,
            LogoPath = "Images/Team3.png",
            SoundPath = "Audio/Teams/Team3.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Lime),
            SecondaryColour = new SolidColorBrush(Colors.LimeGreen)
        };

        public Teams Team4 = new Teams
        {
            Name = "Team4",
            Score = 0,
            LogoPath = "Images/Team4.png",
            SoundPath = "Audio/Teams/Team4.mp3",
            PrimaryColour = new SolidColorBrush(Colors.Red),
            SecondaryColour = new SolidColorBrush(Colors.Crimson)
        };

        public static ControlCenter Instance;

        public CountdownTimer _Timer;
        public ControlCenter()
        {
            InitializeComponent();
            LogWriter.LogWriterInitialize();

            Instance = this;

            TeamsList.AddRange(new[] { Team1, Team2, Team3, Team4 });

            _Timer = new CountdownTimer(new TimeSpan(0, 0, 60))
            {
                SoundPath_Tick = "Audio/Core/tick_sound.mp3",
                SoundPath_TimeUp = "Audio/Core/time_up.wav"
            };

            _Timer.TickEvent += new TimerTickHandler(TimerDisplay);

            GenerateContextMenu(TeamsList);
        }

        public int questionPoints = 10;
        public int bonusPoints = 5;
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
            
            if (PlayerWindowCounter() >= 1)
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

            if (PlayerWindowCounter() >= 1)
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
            LogWriter logWriter = new LogWriter();

            TeamX.Score += points;

            // Update control info
            Status_disp.Content = $"{TeamX.Name} +{points}";

            //Write logs
            logWriter.WriterCorrect(TeamX, points);

            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer.Instance.Points_awarded_disp.Content = $"+{points}";
                TriviaPlayer.Instance.Points_awarded_disp.Foreground = new SolidColorBrush(Colors.Green);

                // Animations
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }
            
            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
        }

        private void DeductPoints(Teams TeamX, int points)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            TeamX.Score -= points;

            // Update labels 
            Status_disp.Content = $"{TeamX.Name} -{points}";

            //Write logs
            logWriter.WriterDeduct(TeamX, points);

            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer.Instance.Points_awarded_disp.Content = $"-{points}";
                TriviaPlayer.Instance.Points_awarded_disp.Foreground = new SolidColorBrush(Colors.Red);

                // Animations
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }

            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }

            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }

        private void WrongAnswer(Teams TeamX)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            // Update labels 
            Status_disp.Content = $"{TeamX.Name} incorrect answer";

            //Write logs
            logWriter.WriterIncorrect(TeamX);

            if (PlayerWindowCounter() >= 1)
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
            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }
        # endregion

        # region Button click events
        private void LaunchPlayer_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() < 1)
            {
                TriviaPlayer player = new TriviaPlayer();
                player.Show();
            }
            else
            {
                Application.Current.Windows.OfType<TriviaPlayer>().First().Topmost = true;
                SystemSounds.Beep.Play();
            }
        }
        private void Play_pause_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Visible;

                if (_Timer.Status == TimerState.Running)
                {
                    TriviaPlayer._media.Pause();
                    _Timer.Stop();
                }
                else
                {
                    TriviaPlayer._media.Play();
                    _Timer.Start();
                }
            }
        }
        private void Answer_correct_Button(object sender, RoutedEventArgs e)
        {
            Question question = (Question)Questions_Page.Instance.QuestionGrid.SelectedItem;
            if(question != null)
            {
                AddPoints(question.Team, question.Points);
                playSound(question.Team.SoundPath);

                // Colour selected row to green
                var selectedItem = Questions_Page.Instance.QuestionGrid.SelectedItem;

                if (selectedItem != null)
                {
                    
                    DataGridRow row = (DataGridRow)Questions_Page.Instance.QuestionGrid.ItemContainerGenerator.ContainerFromItem(selectedItem);
                    if (row != null)
                    {
                        row.Background = Brushes.LightGreen;
                    }
                }
            } 
            _Timer.Stop();
        }

        private void Bonus_correct_Button(object sender, RoutedEventArgs e)
        {
            Question question = (Question)Questions_Page.Instance.QuestionGrid.SelectedItem;
            if (question != null)
            {
                AddPoints(question.Team, question.BonusPoints);
                playSound(question.Team.SoundPath);
            }
            _Timer.Stop();
        }

        private void Answer_wrong_Button(object sender, RoutedEventArgs e)
        {
            Question question = (Question)Questions_Page.Instance.QuestionGrid.SelectedItem;
            if (question != null)
            {
                if (question.Penalty == 0)
                {
                    WrongAnswer(question.Team);
                }
                else
                {
                    DeductPoints(question.Team, question.Penalty);
                }

                // Colour selected row to red
                var selectedItem = Questions_Page.Instance.QuestionGrid.SelectedItem;

                if (selectedItem != null)
                {

                    DataGridRow row = (DataGridRow)Questions_Page.Instance.QuestionGrid.ItemContainerGenerator.ContainerFromItem(selectedItem);
                    if (row != null)
                    {
                        row.Background = Brushes.Tomato;
                    }
                }
            }
            _Timer.Stop();
        }

        private void Answer_wrong_penalty_Button(object sender, RoutedEventArgs e)
        {
            Question question = (Question)Questions_Page.Instance.QuestionGrid.SelectedItem;
            if (question != null)
            {
                DeductPoints(question.Team, question.Penalty);
            }
            _Timer.Stop();
        }

        private void Play_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer._media.Play();
            }
        }

        private void Pause_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer._media.Pause();
            }
        }
        private void Stop_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer._media.Stop();
            }
        }

        private void Show_scores_Click(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.ShowScores();
        }

        private void Settings_Page_Button(object sender, RoutedEventArgs e)
        {
            if (Settings_Page.Instance == null)
                Page_Frame.Content = new Settings_Page();
            Page_Frame.Content = Settings_Page.Instance;
        }

        private void File_viewer_Button(object sender, RoutedEventArgs e)
        {
            if (Files_Page.Instance == null)
                Page_Frame.Content = new Files_Page();
            Page_Frame.Content = Files_Page.Instance;
        }

        private void Scores_page_Button(object sender, RoutedEventArgs e)
        {
            if (Scores_Page.Instance == null)
                Page_Frame.Content = new Scores_Page();
            Page_Frame.Content = Scores_Page.Instance;
        }

        private void Questions_Page_Button(object sender, RoutedEventArgs e)
        {
            if (Questions_Page.Instance == null)
                Page_Frame.Content = new Questions_Page();
            Page_Frame.Content = Questions_Page.Instance;
        }
        private void End_game(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter()>=1)
                TriviaPlayer.Instance.FinalScores();
        }

        private void Bonus_RClick_Team1(object sender, RoutedEventArgs e)
        {
            AddPoints(Team1, bonusPoints);
            playSound(Team1.SoundPath);
        }

        private void Bonus_RClick_Team2(object sender, RoutedEventArgs e)
        {
            AddPoints(Team2, bonusPoints);
            playSound(Team2.SoundPath);
        }

        private void Bonus_RClick_Team3(object sender, RoutedEventArgs e)
        {
            AddPoints(Team3, bonusPoints);
            playSound(Team3.SoundPath);
        }

        private void Bonus_RClick_Team4(object sender, RoutedEventArgs e)
        {
            AddPoints(Team4, bonusPoints);
            playSound(Team4.SoundPath);
        }

        private void Answer_wrong_penalty_RClick_Team1(object sender, RoutedEventArgs e)
        {
            DeductPoints(Team1, penaltyPoints);
            _Timer.Stop();
        }

        private void Answer_wrong_penalty_RClick_Team2(object sender, RoutedEventArgs e)
        {
            DeductPoints(Team2, penaltyPoints);
            _Timer.Stop();
        }

        private void Answer_wrong_penalty_RClick_Team3(object sender, RoutedEventArgs e)
        {
            DeductPoints(Team3, penaltyPoints);
            _Timer.Stop();
        }

        private void Answer_wrong_penalty_RClick_Team4(object sender, RoutedEventArgs e)
        {
            DeductPoints(Team4, penaltyPoints);
            _Timer.Stop();
        }
        #endregion

        #region Keystroke Events
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
                    penaltyPoints = int.Parse(Trickle_penalty_input.Text);
                    Trickle_penalty_avail_disp.Content = $"-{penaltyPoints}pts";
                }
                else
                {
                    MessageBox.Show("Enter only POSITIVE digits");
                }
            }
        }

        # endregion

        #region Sounds
        private void playSound(string path)
        {
            Tick_sound.Source = new Uri(path, UriKind.Relative);
            Tick_sound.Play();
        }

        #endregion

        #region Methods
        public int PlayerWindowCounter()
        {
            return Application.Current.Windows.OfType<TriviaPlayer>().Count();
        }

        private void GenerateContextMenu(List<Teams> TeamList)
        {
            ContextMenu contextMenu_penalty = new ContextMenu();
            ContextMenu contextMenu_bonus = new ContextMenu();

            List<Teams> menuItems = TeamList;

            foreach (Teams menuItem in menuItems)
            {
                MenuItem menuItem_penalty = new MenuItem();
                menuItem_penalty.Header = menuItem.Name;

                MenuItem menuItem_bonus = new MenuItem();
                menuItem_bonus.Header = menuItem.Name;

                if (menuItem.Name == Team1.Name)
                {
                    menuItem_penalty.Click += Answer_wrong_penalty_RClick_Team1;
                    menuItem_bonus.Click += Bonus_RClick_Team1;
                }
                else if (menuItem.Name == Team2.Name)
                {
                    menuItem_penalty.Click += Answer_wrong_penalty_RClick_Team2;
                    menuItem_bonus.Click += Bonus_RClick_Team2;
                }
                else if (menuItem.Name == Team3.Name)
                {
                    menuItem_penalty.Click += Answer_wrong_penalty_RClick_Team3;
                    menuItem_bonus.Click += Bonus_RClick_Team3;
                }
                else if (menuItem.Name == Team4.Name)
                {
                    menuItem_penalty.Click += Answer_wrong_penalty_RClick_Team4;
                    menuItem_bonus.Click += Bonus_RClick_Team4;
                }

                contextMenu_penalty.Items.Add(menuItem_penalty);
                contextMenu_bonus.Items.Add(menuItem_bonus);
            }

            Penalty_button.ContextMenu = contextMenu_penalty;
            Bonus_button.ContextMenu = contextMenu_bonus;
        }

        #endregion
    }

}
