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

        public int questionPoints = 10;
        public int bonusPoints = 5;
        public int penaltyPoints = 5;

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
            SetTimerAndStart(60);
        }

        private void Timer30_Button(object sender, RoutedEventArgs e)
        {
            SetTimerAndStart(30);
        }

        private void Timer15_Button(object sender, RoutedEventArgs e)
        {
            SetTimerAndStart(15);
        }

        private void SetTimerAndStart(int duration)
        {
            SetTimer(duration);
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
        #endregion

        #region Scoring
        public void AddPoints(Teams TeamX, int points)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            TeamX.Score += points;
            Status_disp.Content = $"{TeamX.Name} +{points}";
            logWriter.WriterCorrect(TeamX, points);

            if (PlayerWindowCounter() >= 1)
            {
                UpdatePlayerPoints($"+{points}", Colors.Green, TeamX.LogoPath, Animation);
            }

            RefreshScoresIfNeeded();
        }

        private void DeductPoints(Teams TeamX, int points)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            TeamX.Score -= points;
            Status_disp.Content = $"{TeamX.Name} -{points}";
            logWriter.WriterDeduct(TeamX, points);

            if (PlayerWindowCounter() >= 1)
            {
                UpdatePlayerPoints($"-{points}", Colors.Red, TeamX.LogoPath, Animation);
            }

            RefreshScoresIfNeeded();
            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }

        public void AddDeductPoints(Teams TeamAdd, Teams TeamDeduct, int AddedPoints, int DeductedPoints)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            TeamAdd.Score += AddedPoints;
            TeamDeduct.Score -= DeductedPoints;

            Status_disp.Content = $"{TeamAdd.Name} +{AddedPoints} and {TeamDeduct.Name} -{DeductedPoints}";

            logWriter.WriterCorrect(TeamAdd, AddedPoints);
            logWriter.WriterDeduct(TeamDeduct, DeductedPoints);

            if (PlayerWindowCounter() >= 1)
            {
                UpdatePlayerPointsDual(
                    $"+{AddedPoints}", Colors.Green, TeamAdd.LogoPath,
                    $"-{DeductedPoints}", Colors.Red, TeamDeduct.LogoPath,
                    Animation
                );
            }

            RefreshScoresIfNeeded();
            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }

        private void WrongAnswer(Teams TeamX)
        {
            Animations Animation = new Animations();
            LogWriter logWriter = new LogWriter();

            Status_disp.Content = $"{TeamX.Name} incorrect answer";
            logWriter.WriterIncorrect(TeamX);

            if (PlayerWindowCounter() >= 1)
            {
                TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(TeamX.LogoPath, UriKind.Relative));
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Wrong_answer_image);
                Animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
            }

            RefreshScoresIfNeeded();
            playSound(FileBrowser.SelectRandomFile("Audio/Incorrect"));
        }

        private void UpdatePlayerPoints(string pointsText, Color color, string logoPath, Animations animation)
        {
            TriviaPlayer.Instance.Points_awarded_disp.Content = pointsText;
            TriviaPlayer.Instance.Points_awarded_disp.Foreground = new SolidColorBrush(color);
            TriviaPlayer.Instance.Team_logo.Source = new BitmapImage(new Uri(logoPath, UriKind.Relative));
            animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp);
            animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo);
        }

        private void UpdatePlayerPointsDual(
            string topPointsText, Color topColor, string topLogoPath,
            string bottomPointsText, Color bottomColor, string bottomLogoPath,
            Animations animation)
        {
            TriviaPlayer.Instance.Points_awarded_disp_top.Content = topPointsText;
            TriviaPlayer.Instance.Points_awarded_disp_top.Foreground = new SolidColorBrush(topColor);
            TriviaPlayer.Instance.Team_logo_top.Source = new BitmapImage(new Uri(topLogoPath, UriKind.Relative));
            animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp_top, 5);
            animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo_top, 5);

            TriviaPlayer.Instance.Points_awarded_disp_bottom.Content = bottomPointsText;
            TriviaPlayer.Instance.Points_awarded_disp_bottom.Foreground = new SolidColorBrush(bottomColor);
            TriviaPlayer.Instance.Team_logo_bottom.Source = new BitmapImage(new Uri(bottomLogoPath, UriKind.Relative));
            animation.FadeInOut_Label(TriviaPlayer.Instance.Points_awarded_disp_bottom, 5);
            animation.FadeInOut_Image(TriviaPlayer.Instance.Team_logo_bottom, 5);
        }

        private void RefreshScoresIfNeeded()
        {
            if (Page_Frame.Content.GetType() == new Scores_Page().GetType())
            {
                Page_Frame.Content = new Scores_Page();
            }
        }
        #endregion

        #region Button Click Events
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

                UpdateNavigationButtons();
                UpdateBonusButton();

                HandleQuestionType(currentQuestion);
                HighlightCurrentQuestion();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateQuestion(1);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateQuestion(-1);
        }

        private void NavigateQuestion(int direction)
        {
            if (PlayerWindowCounter() < 1) return;

            currentQuestionIndex += direction;

            if (currentQuestionIndex >= 0 && currentQuestionIndex < Questions_Page.Instance.gridItems.Count)
            {
                Questions_Page.Instance.CategoryGrid.SelectedIndex = currentQuestionIndex;
                currentQuestion = Questions_Page.Instance?.CategoryGrid?.SelectedItem as Question;
            }

            UpdateNavigationButtons();
            UpdateBonusButton();

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

            HandleQuestionType(currentQuestion);
            HighlightCurrentQuestion();
        }

        private void UpdateNavigationButtons()
        {
            Previous_Button.IsEnabled = currentQuestionIndex > 0;
            Next_Button.IsEnabled = currentQuestionIndex < Questions_Page.Instance.gridItems.Count - 1;
        }

        private void UpdateBonusButton()
        {
            Bonus_button.IsEnabled = currentQuestion.BonusPoints > 0;
        }

        private void HandleQuestionType(Question question)
        {
            switch (question.Type)
            {
                case "Q&A":
                    HandleQAQuestion(question);
                    break;
                case "Media":
                    HandleMediaQuestion(question, false);
                    break;
                case "PayItForward":
                    HandleMediaQuestion(question, true);
                    break;
                case "Banner":
                    HandleBannerQuestion(question);
                    break;
            }
        }

        private void HandleQAQuestion(Question question)
        {
            TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
            ShowQuestion(question);
        }

        private void HandleMediaQuestion(Question question, bool isPayItForward)
        {
            Questions_Page.Instance.ClearQuestionAnswerText();
            TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
            Correct_button.Content = "Correct";
            Correct_button.IsEnabled = true;

            if (isPayItForward)
            {
                Bonus_button.IsEnabled = false;
            }

            MediaController media = MediaPlayer_Page._media;
            media.Path = question.FilePath;

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

        private void HandleBannerQuestion(Question question)
        {
            Questions_Page.Instance.ClearQuestionAnswerText();
            TriviaPlayer.Instance.TriviaPlayer_Frame.Content = TriviaPlayer.Instance._mediaPlayer_page;
            Correct_button.Content = "Correct";
            Correct_button.IsEnabled = true;

            MediaController media = MediaPlayer_Page._media;
            media.Path = question.FilePath;
            media.Play();
            ClearTimer();
        }

        private void Answer_correct_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1 && currentQuestion.Team != null)
            {
                if (currentQuestion.Type == "Media")
                {
                    AddPoints(currentQuestion.Team, currentQuestion.Points);
                    playSound(currentQuestion.Team.SoundPath);
                }
                else if (currentQuestion.Type == "PayItForward")
                {
                    Teams nextTeam = GetNextTeam(currentQuestion.Team);
                    AddPoints(nextTeam, currentQuestion.Points);
                }
                _Timer.Stop();
            }
        }

        private void Bonus_correct_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1 && currentQuestion.Team != null)
            {
                AddPoints(currentQuestion.Team, currentQuestion.BonusPoints);
                playSound(currentQuestion.Team.SoundPath);
                _Timer.Stop();
            }
        }

        private void Answer_wrong_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1 && currentQuestion.Team != null)
            {
                if (currentQuestion.Type == "Media")
                {
                    HandleMediaWrongAnswer();
                }
                else if (currentQuestion.Type == "PayItForward")
                {
                    HandlePayItForwardWrongAnswer();
                }
                _Timer.Stop();
            }
        }

        private void HandleMediaWrongAnswer()
        {
            if (currentQuestion.Penalty == 0)
            {
                WrongAnswer(currentQuestion.Team);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to deduct points?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeductPoints(currentQuestion.Team, currentQuestion.Penalty);
                }
            }
        }

        private void HandlePayItForwardWrongAnswer()
        {
            Teams nextTeam = GetNextTeam(currentQuestion.Team);
            AddDeductPoints(nextTeam, currentQuestion.Team, currentQuestion.BonusPoints, currentQuestion.Penalty);
        }

        private Teams GetNextTeam(Teams currentTeam)
        {
            int currentIndex = TeamsList.IndexOf(currentTeam);
            int nextIndex = (currentIndex + 1) % TeamsList.Count;
            return TeamsList[nextIndex];
        }

        private void Answer_wrong_penalty_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1 && currentQuestion.Team != null)
            {
                DeductPoints(currentQuestion.Team, currentQuestion.TricklePenalty);
                _Timer.Stop();
            }
        }

        private void ShowAnswer_button_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerWindowCounter() >= 1 && currentQuestion?.AnswerText != null)
            {
                Questions_Page.Instance.DisplayAnswerText(currentQuestion);
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
                MessageBoxResult result = MessageBox.Show("Do you really want to end the game?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TriviaPlayer.Instance.FinalScores();
                }
            }
        }

        private void Bonus_RClick_Team1(object sender, RoutedEventArgs e) => AddBonusToTeam(Team1);
        private void Bonus_RClick_Team2(object sender, RoutedEventArgs e) => AddBonusToTeam(Team2);
        private void Bonus_RClick_Team3(object sender, RoutedEventArgs e) => AddBonusToTeam(Team3);
        private void Bonus_RClick_Team4(object sender, RoutedEventArgs e) => AddBonusToTeam(Team4);

        private void AddBonusToTeam(Teams team)
        {
            AddPoints(team, bonusPoints);
            playSound(team.SoundPath);
        }

        private void Answer_wrong_penalty_RClick_Team1(object sender, RoutedEventArgs e) => ApplyPenaltyToTeam(Team1);
        private void Answer_wrong_penalty_RClick_Team2(object sender, RoutedEventArgs e) => ApplyPenaltyToTeam(Team2);
        private void Answer_wrong_penalty_RClick_Team3(object sender, RoutedEventArgs e) => ApplyPenaltyToTeam(Team3);
        private void Answer_wrong_penalty_RClick_Team4(object sender, RoutedEventArgs e) => ApplyPenaltyToTeam(Team4);

        private void ApplyPenaltyToTeam(Teams team)
        {
            DeductPoints(team, penaltyPoints);
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
                    SetTimerAndStart(timerInput);
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }
        #endregion

        #region Sounds
        private void playSound(string path)
        {
            Tick_sound.Source = new Uri(path, UriKind.Relative);
            Tick_sound.Play();
        }
        #endregion

        #region Methods
        public void ShowQuestion(Question question)
        {
            _Timer.Duration = question.Time;
            if (question.ClearClock)
            {
                ClearTimer();
            }

            if (TriviaPlayer.Instance.Clock_face_image.Visibility == Visibility.Hidden)
            {
                TriviaPlayer.Instance.Clock_face_image.Visibility = Visibility.Visible;
                TriviaPlayer.Instance.Timer_display.Visibility = Visibility.Visible;
            }

            Questions_Page.Instance.DisplayQuestionText(question);
            MediaPlayer_Page._media.Path = question.BackgroundImagePath;
            MediaPlayer_Page._media.Play();
            _Timer.Start();
        }

        public int PlayerWindowCounter()
        {
            return Application.Current.Windows.OfType<TriviaPlayer>().Count();
        }

        private void ControlCenter_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
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

        private void HighlightCurrentQuestion()
        {
            ResetRowColors();
            var selectedItem = (Question)Questions_Page.Instance.CategoryGrid.SelectedItem;
            if (selectedItem != null)
            {
                selectedItem.RowColor = new SolidColorBrush(Colors.Yellow);
            }
        }

        private void GenerateContextMenu(List<Teams> TeamList)
        {
            ContextMenu contextMenu_penalty = new ContextMenu();
            ContextMenu contextMenu_bonus = new ContextMenu();

            var teamHandlers = new Dictionary<string, (RoutedEventHandler penalty, RoutedEventHandler bonus)>
            {
                { Team1.Name, (Answer_wrong_penalty_RClick_Team1, Bonus_RClick_Team1) },
                { Team2.Name, (Answer_wrong_penalty_RClick_Team2, Bonus_RClick_Team2) },
                { Team3.Name, (Answer_wrong_penalty_RClick_Team3, Bonus_RClick_Team3) },
                { Team4.Name, (Answer_wrong_penalty_RClick_Team4, Bonus_RClick_Team4) }
            };

            foreach (Teams team in TeamList)
            {
                if (teamHandlers.ContainsKey(team.Name))
                {
                    var handlers = teamHandlers[team.Name];

                    MenuItem menuItem_penalty = new MenuItem { Header = team.Name };
                    menuItem_penalty.Click += handlers.penalty;
                    contextMenu_penalty.Items.Add(menuItem_penalty);

                    MenuItem menuItem_bonus = new MenuItem { Header = team.Name };
                    menuItem_bonus.Click += handlers.bonus;
                    contextMenu_bonus.Items.Add(menuItem_bonus);
                }
            }

            Bonus_button.ContextMenu = contextMenu_bonus;
        }
        #endregion
    }
}