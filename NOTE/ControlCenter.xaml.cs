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
using System.Windows.Media.Animation;

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
        public Question currentQuestion;
        private int currentQuestionIndex = -1;
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

        public void AddPoints(Teams TeamX, int points)
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


        public void AddDeductPoints(Teams TeamAdd, Teams TeamDeduct, int AddedPoints, int DeductedPoints)
        {
            // Use this to add point to one team and deduct from another
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            TeamAdd.Score += AddedPoints;
            TeamDeduct.Score -= DeductedPoints;

            // Update control info
            Status_disp.Content = $"{TeamAdd.Name} +{AddedPoints} and {TeamDeduct.Name} -{DeductedPoints}";

            //Write logs
            logWriter.WriterCorrect(TeamAdd, AddedPoints);
            logWriter.WriterDeduct(TeamDeduct, DeductedPoints);

            if (PlayerWindowCounter() >= 1)
            {
                // Add points - Top space
                TriviaPlayer.Instance.Points_awarded_disp_top.Content = $"+{AddedPoints}";             
                TriviaPlayer.Instance.Points_awarded_disp_top.Foreground = new SolidColorBrush(Colors.Green);

                // Top space animations
                TriviaPlayer.Instance.Team_logo_top.Source = new BitmapImage(new Uri(TeamAdd.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp_top, 5);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo_top, 5);

                // Deduct points - Bottom space
                TriviaPlayer.Instance.Points_awarded_disp_bottom.Content = $"-{DeductedPoints}";
                TriviaPlayer.Instance.Points_awarded_disp_bottom.Foreground = new SolidColorBrush(Colors.Red);

                //Bottom space animations
                TriviaPlayer.Instance.Team_logo_bottom.Source = new BitmapImage(new Uri(TeamDeduct.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp_bottom, 5);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo_bottom, 5);
            }

            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }

        private void WrongAnswer(Teams TeamX)
        {
            //if (TeamX == null) return; // Add this line to ignore the method if TeamX is null

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
            var newQuestion = Questions_Page.Instance?.CategoryGrid?.SelectedItem as Question;
            if (newQuestion != currentQuestion)
            {
                _Timer.Reset();
                currentQuestion = newQuestion;
                currentQuestionIndex = Questions_Page.Instance.CategoryGrid.SelectedIndex;
            }

            if (PlayerWindowCounter() >= 1)
            {
                Timer_Visibility();
                _Timer.Duration = currentQuestion.Time;

                if (currentQuestionIndex == 0)
                {
                    Previous_Button.IsEnabled = false;
                }
                else
                {
                    // If it's not the first item, enable the Previous button
                    Previous_Button.IsEnabled = true;
                }

                // If the last item is selected, disable the Next button
                if (currentQuestionIndex == Questions_Page.Instance.gridItems.Count - 1)
                {
                    Next_Button.IsEnabled = false;
                }
                else
                {
                    // If it's not the last item, enable the Next button
                    Next_Button.IsEnabled = true;
                }

                // Enable/disable flex button based on point availability
                if (currentQuestion.BonusPoints == 0)
                {
                    Bonus_button.IsEnabled = false;
                }
                else
                {
                    Bonus_button.IsEnabled = true;
                }

                if (currentQuestion.Type == "Media")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    MediaController media = MediaPlayer_Page._media;

                    media.Path = currentQuestion.FilePath;

                    if (media.Status == MediaController.MediaState.Playing)
                    {
                        media.Pause();
                        if (!Questions_Page.Instance.noClock) _Timer.Stop();
                    }
                    else if (media.Path == null)
                    {
                        TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Hidden;
                        TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        media.Play();
                        if (!Questions_Page.Instance.noClock) _Timer.Start();
                    }
                }
                else if (currentQuestion.Type == "PayItForward")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    MediaController media = MediaPlayer_Page._media;

                    Bonus_button.IsEnabled = false;

                    media.Path = currentQuestion.FilePath;

                    if (media.Status == MediaController.MediaState.Playing)
                    {
                        media.Pause();
                        if (!Questions_Page.Instance.noClock) _Timer.Stop();
                    }
                    else if (media.Path == null)
                    {
                        TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Hidden;
                        TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        media.Play();
                        if (!Questions_Page.Instance.noClock) _Timer.Start();
                    }
                }
                else if (currentQuestion.Type == "Banner")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    MediaController media = MediaPlayer_Page._media;

                    media.Path = currentQuestion.FilePath;
                    media.Play();
                    ClearTimer();
                }

                ResetRowColors();
                var selectedItem = (Question)Questions_Page.Instance.CategoryGrid.SelectedItem;
                if (selectedItem != null)
                {
                    selectedItem.RowColor = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                currentQuestionIndex++;
                if (currentQuestionIndex < Questions_Page.Instance.gridItems.Count)
                {
                    Questions_Page.Instance.CategoryGrid.SelectedIndex = currentQuestionIndex;
                    currentQuestion = Questions_Page.Instance?.CategoryGrid?.SelectedItem as Question;
                }

                // Enable Previous button because we're not at the first item anymore
                Previous_Button.IsEnabled = true;

                // If we have reached the last item, disable the Next button
                if (currentQuestionIndex >= Questions_Page.Instance.gridItems.Count - 1)
                {
                    Next_Button.IsEnabled = false;
                }

                // Enable/disable flex button based on point availability
                if (currentQuestion.BonusPoints == 0)
                {
                    Bonus_button.IsEnabled = false;
                }
                else
                {
                    Bonus_button.IsEnabled = true;
                }

                Timer_Visibility();
                _Timer.Duration = currentQuestion.Time;

                if (!currentQuestion.NoClock)
                {
                    _Timer.Reset();
                    _Timer.Start();
                }
                else
                {
                    ClearTimer();
                    _Timer.Reset();
                }

                if (currentQuestion.Type == "Media")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    SeekerBar.Value = 0;
                    MediaController media = MediaPlayer_Page._media;
                    media.Path = currentQuestion.FilePath;
                    media.Play();
                }
                else if (currentQuestion.Type == "PayItForward")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    SeekerBar.Value = 0;
                    MediaController media = MediaPlayer_Page._media;
                    media.Path = currentQuestion.FilePath;
                    media.Play();

                    Bonus_button.IsEnabled = false;
                }
                else if (currentQuestion.Type == "Banner")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    MediaController media = MediaPlayer_Page._media;

                    media.Path = currentQuestion.FilePath;
                    media.Play();
                    ClearTimer();
                }

                ResetRowColors();
                ResetRowColors();
                var selectedItem = (Question)Questions_Page.Instance.CategoryGrid.SelectedItem;
                if (selectedItem != null)
                {
                    selectedItem.RowColor = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                currentQuestionIndex--;
                if (currentQuestionIndex >= 0)
                {
                    Questions_Page.Instance.CategoryGrid.SelectedIndex = currentQuestionIndex;
                    currentQuestion = Questions_Page.Instance?.CategoryGrid?.SelectedItem as Question;
                }

                // Enable Next button because we're not at the last item anymore
                Next_Button.IsEnabled = true;

                // If we have reached the first item, disable the Previous button
                if (currentQuestionIndex <= 0)
                {
                    Previous_Button.IsEnabled = false;
                }

                // Enable/disable flex button based on point availability
                if (currentQuestion.BonusPoints == 0)
                {
                    Bonus_button.IsEnabled = false;
                }
                else
                {
                    Bonus_button.IsEnabled = true;
                }

                Timer_Visibility();
                _Timer.Duration = currentQuestion.Time;

                if (!currentQuestion.NoClock)
                {
                    _Timer.Reset();
                    _Timer.Start();
                }
                else
                {
                    ClearTimer();
                    _Timer.Reset();
                }

                if (currentQuestion.Type == "Media")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    SeekerBar.Value = 0;
                    MediaController media = MediaPlayer_Page._media;
                    media.Path = currentQuestion.FilePath;
                    media.Play();
                }
                else if (currentQuestion.Type == "PayItForward")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    SeekerBar.Value = 0;
                    MediaController media = MediaPlayer_Page._media;
                    media.Path = currentQuestion.FilePath;
                    media.Play();

                    Bonus_button.IsEnabled = false;
                }
                else if (currentQuestion.Type == "Banner")
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
                    Correct_button.Content = "Correct";
                    Correct_button.IsEnabled = true;
                    MediaController media = MediaPlayer_Page._media;

                    media.Path = currentQuestion.FilePath;
                    media.Play();
                    ClearTimer();
                }

                ResetRowColors();
                ResetRowColors();
                var selectedItem = (Question)Questions_Page.Instance.CategoryGrid.SelectedItem;
                if (selectedItem != null)
                {
                    selectedItem.RowColor = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        private void Answer_correct_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                if (currentQuestion.Team != null)
                {
                    if (currentQuestion.Type == "Media")
                    {
                        AddPoints(currentQuestion.Team, currentQuestion.Points);
                        playSound(currentQuestion.Team.SoundPath);
                    }
                    else if (currentQuestion.Type == "PayItForward")
                    {
                        Teams nextTeam;

                        switch (TeamsList.IndexOf(currentQuestion.Team))
                        {
                            case 0:
                                nextTeam = Team2;
                                break;
                            case 1:
                                nextTeam = Team3;
                                break;
                            case 2:
                                nextTeam = Team4;
                                break;
                            case 3:
                                nextTeam = Team1;
                                break;
                            default:
                                nextTeam = currentQuestion.Team;
                                break;
                        }

                        AddPoints(nextTeam, currentQuestion.Points);
                    }
                }
                _Timer.Stop();
            }
        }

        private void Bonus_correct_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                if (currentQuestion.Team != null)
                {
                    AddPoints(currentQuestion.Team, currentQuestion.BonusPoints);
                    playSound(currentQuestion.Team.SoundPath);
                }
                _Timer.Stop();
            }
        }

        private void Answer_wrong_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                if (currentQuestion.Team != null)
                {
                    if (currentQuestion.Type == "Media")
                    {
                        if (currentQuestion.Penalty == 0)
                        {
                            WrongAnswer(currentQuestion.Team);
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Are you sure you want to deduct points?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                DeductPoints(currentQuestion.Team, currentQuestion.Penalty);
                            }
                        }
                    }
                    else if (currentQuestion.Type == "PayItForward")
                    {
                        Teams nextTeam;

                        switch (TeamsList.IndexOf(currentQuestion.Team))
                        {
                            case 0:
                                nextTeam = Team2;
                                break;
                            case 1:
                                nextTeam = Team3;
                                break;
                            case 2:
                                nextTeam = Team4;
                                break;
                            case 3:
                                nextTeam = Team1;
                                break;
                            default:
                                nextTeam = currentQuestion.Team;
                                break;
                        }

                        AddDeductPoints(nextTeam, currentQuestion.Team, currentQuestion.BonusPoints, currentQuestion.Penalty);
                    }
                }
                _Timer.Stop();
            }
        }

        private void Answer_wrong_penalty_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                if (currentQuestion.Team != null)
                {
                    DeductPoints(currentQuestion.Team, currentQuestion.TricklePenalty);
                }
                _Timer.Stop();
            }
        }

        private void Play_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                MediaPlayer_Page._media.Play();
            }
        }

        private void Pause_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                MediaPlayer_Page._media.Pause();
            }
        }
        private void Stop_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                MediaPlayer_Page._media.Stop();
            }
        }

        private void Show_scores_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer.Instance.ShowScores();
            }
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
            if (PlayerWindowCounter() >= 1)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to end the game?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TriviaPlayer.Instance.FinalScores();
                }
            }    
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

        #region Keystroke
        private void Timer_avail_changed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Custom_timer_input.Text.All(char.IsDigit))
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

        private void ControlCenter_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get all open windows
            foreach (Window window in Application.Current.Windows)
            {
                // Don't close the main window again
                if (window != this)
                {
                    // Close the window
                    window.Close();
                }
            }
        }

        private void Timer_Visibility()
        {
            if (Questions_Page.Instance.noClock)
            {
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Hidden;
                TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Hidden;
                ClearTimer();
            }
            else
            {
                if (TriviaPlayer.Instance.Clock_face_image.Visibility == Visibility.Hidden)
                {
                    TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                    TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Visible;
                }
            }
        }

        private void ResetRowColors()
        {
            foreach (Question question in Questions_Page.Instance.CategoryGrid.Items)
            {
                question.RowColor = new SolidColorBrush(Colors.White);
            }
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
            Bonus_button.ContextMenu = contextMenu_bonus;
        }
        #endregion

    }

}
