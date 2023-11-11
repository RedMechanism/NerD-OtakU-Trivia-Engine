using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for NerdFeud_Page.xaml
    /// </summary>
    public partial class NerdFeud_Page : Page
    {
        private int currentIndex = 0;
        private List<Button> rowButtons = new List<Button>();

        public NerdFeud_Page()
        {
            InitializeComponent();
        }

        public void RevealAnswers(Dictionary<string, int> answersAndScores)
        {
            currentIndex = 0;

            foreach (var answerScore in answersAndScores)
            {
                AddRow(answerScore.Key, answerScore.Value);
                currentIndex++;
            }
        }

        private void AddRow(string answer, int score)
        {
            Button rowButton = CreateRowButton(answer, score);
            rowButton.Click += (sender, e) => RowButton_Click(sender, e, score);

            var flipAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            ((ScaleTransform)rowButton.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, flipAnimation);

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(rowButton);
            rowButtons.Add(rowButton);

            Grid.SetRow(rowButton, currentIndex);
            Grid.SetColumn(rowButton, 0);
            Grid.SetColumnSpan(rowButton, 2);
        }

        public void ClearAnswers()
        {
            foreach (Button button in rowButtons)
            {
                grid.Children.Remove(button);
            }
            rowButtons.Clear();
            grid.RowDefinitions.Clear();
            currentIndex = 0;
        }

        private Button CreateRowButton(string answer, int score)
        {
            return new Button
            {
                Height = 60,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        CreateAnswerBlock(answer),
                        CreateScoreBlock(score)
                    }
                },
                Margin = new Thickness(10),
                RenderTransform = new ScaleTransform(),
                BorderThickness = new Thickness(4),
                RenderTransformOrigin = new Point(0.5, 0.5),
                Style = (Style)FindResource("RoundedButtonStyle"),
                Effect = new DropShadowEffect
                {
                    Color = new Color { A = 255, R = 0, G = 0, B = 0 },
                    Direction = 270,
                    ShadowDepth = 1,
                    Opacity = 1,
                    BlurRadius = 10
                },

                Cursor = Cursors.Hand
            };
        }

        private TextBlock CreateAnswerBlock(string answer)
        {
            return new TextBlock
            {
                Text = answer,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold
            };
        }

        private Grid CreateScoreBlock(int score)
        {
            var border = new Border
            {
                Width = 40,
                Height = 100,
                Background = Brushes.LightBlue,
                CornerRadius = new CornerRadius(0, 30, 0, 0),
            };

            var scoreBlock = new Grid
            {
                Children =
        {
            border,
            new TextBlock
            {
                Text = score.ToString(),
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold
            }
        }
            };

            return scoreBlock;
        }

        private void RowButton_Click(object sender, RoutedEventArgs e, int score)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Create the animation.
                var clickAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0.9,
                    Duration = TimeSpan.FromMilliseconds(100),
                    AutoReverse = true
                };

                // Apply the animation.
                clickedButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, clickAnimation);
                clickedButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, clickAnimation);

                // Disable the button.
                clickedButton.IsEnabled = false;
            }
            ControlCenter.Instance.AddPoints(ControlCenter.Instance.currentQuestion.Team, score);
        }
    }
}
