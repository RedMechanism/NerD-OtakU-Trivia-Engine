﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for TriviaPlayer.xaml
    /// </summary>
    /// 
    public partial class TriviaPlayer : Window
    {
        
        public static TriviaPlayer Instance;

        public MediaElement mediaPlayer;

        public Label displayTimer;
        public Image clock_face;
        public TriviaPlayer()
        {
            InitializeComponent();

            Instance = this;
            mediaPlayer = myMedia;
            displayTimer = Timer_display;
            clock_face = Clock_face_image;
        }

        private void playbackEnded(object sender, RoutedEventArgs e)
        {
           ControlCenter.Instance.MediaPlaying = false;
        }

        private void Window_left_click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }
            else
            {
                DragMove();
            }
        }

        private void Context_menu_scores(object sender, RoutedEventArgs e)
        {
            ShowScores();
        }

        private void Context_menu_clear_timer(object sender, RoutedEventArgs e)
        {
            ControlCenter.Instance.ClearClock();
        }

        private void Context_menu_reset_window(object sender, RoutedEventArgs e)
        {
            this.Width = 1280;
            this.Height = 720;
        }

        private void Context_menu_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool scoresDisplayed = false;

        private Teams[] TeamsArr = new Teams[]
        {
            ControlCenter.Instance.Team1,
            ControlCenter.Instance.Team2,
            ControlCenter.Instance.Team3,
            ControlCenter.Instance.Team4
        };

        private void teamSort()
        {
            Array.Sort(TeamsArr, Teams.SortScoreDescending());

            Pos1_team_name.Content = TeamsArr[0].Name;
            Pos1_score.Content = TeamsArr[0].Score.ToString();
            Pos1_team_bg.Background = TeamsArr[0].PrimaryColour;
            Pos1_score_bg.Background = TeamsArr[0].SecondaryColour;

            Pos2_team_name.Content = TeamsArr[1].Name;
            Pos2_score.Content = TeamsArr[1].Score.ToString();
            Pos2_team_bg.Background = TeamsArr[1].PrimaryColour;
            Pos2_score_bg.Background = TeamsArr[1].SecondaryColour;

            Pos3_team_name.Content = TeamsArr[2].Name;
            Pos3_score.Content = TeamsArr[2].Score.ToString();
            Pos3_team_bg.Background = TeamsArr[2].PrimaryColour;
            Pos3_score_bg.Background = TeamsArr[2].SecondaryColour;

            Pos4_team_name.Content = TeamsArr[3].Name;
            Pos4_score.Content = TeamsArr[3].Score.ToString();
            Pos4_team_bg.Background = TeamsArr[3].PrimaryColour;
            Pos4_score_bg.Background = TeamsArr[3].SecondaryColour;
        }
        public void ShowScores()
        {
            Scores_view_container.Visibility = Visibility.Visible;
            Animations translations = new Animations();

            teamSort();

            if (scoresDisplayed)
            {
                translations.SlideOutAnimation(Scores_view_container);
                scoresDisplayed = false;
            }
            else
            {
                translations.SlideInAnimation(Scores_view_container);
                scoresDisplayed = true;
            }
        }

        public void FinalScores()
        {
            ControlCenter.Instance.ClearClock();
            MediaPlayer finalScores = new MediaPlayer();
            finalScores.Open(new Uri("Audio/Core/final_scores.mp3", UriKind.Relative));
            finalScores.Play();

            teamSort();

            Scores_view_container.Visibility = Visibility.Visible;
            Animations animations = new Animations();

            if (!scoresDisplayed)
            {
                animations.ScoresAppear(Scores_view_container);
            }

            myMedia.Source = new Uri("Videos/VictoryBackground_EjraVFX.mp4", UriKind.Relative);
            myMedia.Play();

            animations.FadeIn_Grid(Position_numbers);
            animations.BounceDownStory(Pos4_box, 4);
            animations.BounceDownStory(Pos3_box, 6);
            animations.BounceDownStory(Pos2_box, 8);
            animations.BounceDownStory(Pos1_box, 10);
        }
    }
}
