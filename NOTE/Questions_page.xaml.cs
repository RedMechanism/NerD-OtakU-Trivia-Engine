using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Questions_Page.xaml
    /// </summary>
    public partial class Questions_Page : Page
    {
        public static Questions_Page Instance;

        public ObservableCollection<Question> gridItems = new ObservableCollection<Question>();

        public Uri selectedMediaFilePath = null;
        public Dictionary<string, int> selectedNerdFeudAnswers = null;
        public string selectedQuestionType = null;
        public bool noClock;
        public bool clearClock;

        private TextBlock displayedTextBlock;
        private TextBlock displayedAnswerTextBlock;
        public Questions_Page()
        {

            InitializeComponent();
            Instance = this;

            CategoryGrid.ItemsSource = gridItems;
            CategoryGrid.IsReadOnly = true;
            DataContext = this;
            CategoryGrid.SelectedIndex = 0;

            LoadQuestionsFromJson();

            ControlCenter.Instance.currentQuestion = CategoryGrid.SelectedItem as Question;
        }

        private void LoadQuestionsFromJson()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "questions.json");
                string jsonContent = File.ReadAllText(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<JsonDocument>(jsonContent, options);

                foreach (var category in data.RootElement.GetProperty("categories").EnumerateArray())
                {
                    string categoryName = category.GetProperty("name").GetString();
                    string bannerPath = category.GetProperty("bannerPath").GetString();
                    bool noClock = category.TryGetProperty("noClock", out var noClockProp) && noClockProp.GetBoolean();

                    // Add category banner
                    gridItems.Add(new Question
                    {
                        QuestionName = $"{categoryName} Category Banner",
                        Type = "Banner",
                        FilePath = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bannerPath)),
                        NoClock = noClock
                    });

                    // Add questions
                    foreach (var q in category.GetProperty("questions").EnumerateArray())
                    {
                        var question = CreateQuestionFromJson(q);
                        gridItems.Add(question);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error loading questions from JSON: {ex.Message}",
                    "Configuration Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private Question CreateQuestionFromJson(JsonElement q)
        {
            var question = new Question();

            // Required properties (with safe checks)
            if (q.TryGetProperty("questionName", out var qn))
                question.QuestionName = qn.GetString();

            if (q.TryGetProperty("type", out var t))
                question.Type = t.GetString();

            // Optional numeric properties
            if (q.TryGetProperty("points", out var pts))
                question.Points = pts.GetInt32();

            if (q.TryGetProperty("bonusPoints", out var bp))
                question.BonusPoints = bp.GetInt32();

            if (q.TryGetProperty("penalty", out var pen))
                question.Penalty = pen.GetInt32();

            if (q.TryGetProperty("tricklePenalty", out var tp))
                question.TricklePenalty = tp.GetInt32();

            if (q.TryGetProperty("timeSeconds", out var ts))
                question.Time = TimeSpan.FromSeconds(ts.GetInt32());

            if (q.TryGetProperty("teamIndex", out var ti))
                question.Team = GetTeamByIndex(ti.GetInt32());

            // Optional boolean properties
            question.ClearClock = q.TryGetProperty("clearClock", out var cc) ? cc.GetBoolean() : true;
            question.NoClock = q.TryGetProperty("noClock", out var nc) && nc.GetBoolean();

            // Optional text properties
            if (q.TryGetProperty("questionText", out var qt))
                question.QuestionText = qt.GetString();

            if (q.TryGetProperty("answerText", out var at))
                question.AnswerText = at.GetString();

            if (q.TryGetProperty("categoryName", out var cn))
                question.CategoryName = cn.GetString();

            // Optional file paths
            if (q.TryGetProperty("filePath", out var fp))
                question.FilePath = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fp.GetString()));

            if (q.TryGetProperty("backgroundImagePath", out var bip))
                question.BackgroundImagePath = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bip.GetString()));

            // Handle nested question list (for "Pick your poison" type)
            if (q.TryGetProperty("questionList", out var ql))
            {
                question.QuestionList = new List<Question>();
                foreach (var nestedQ in ql.EnumerateArray())
                {
                    var nestedQuestion = CreateQuestionFromJson(nestedQ);
                    question.QuestionList.Add(nestedQuestion);
                }
            }

            return question;
        }

        private Teams GetTeamByIndex(int index)
        {
            switch (index)
            {
                case 1: return ControlCenter.Instance.Team1;
                case 2: return ControlCenter.Instance.Team2;
                case 3: return ControlCenter.Instance.Team3;
                case 4: return ControlCenter.Instance.Team4;
                default: return null;
            }
        }

        public void ClearQuestionAnswerText()
        {
            if (displayedTextBlock != null)
            {
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedTextBlock);
                displayedTextBlock = null;
            }

            if (displayedAnswerTextBlock != null)
            {
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedAnswerTextBlock);
                displayedAnswerTextBlock = null;
            }
        }

        private void CategoryGrid_LButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Question question = (Question)CategoryGrid.SelectedItem;

            if (question != null)
            {
                if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
                {
                    MediaPlayer_Page._media.Path = question.FilePath;
                    
                    ControlCenter.Instance._Timer.Duration = question.Time;
                }
            }
        }

        public void DisplayQuestionText(Question question)
        {
            // Remove the previously displayed TextBlock (if any)
            if (displayedTextBlock != null)
            {
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedTextBlock);
                displayedTextBlock = null;
            }

            if (displayedAnswerTextBlock != null)
            {
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedAnswerTextBlock);
                displayedAnswerTextBlock = null;
                ControlCenter.Instance.ShowAnswer_button.Content = "Show answer";
            }

            // Create a new TextBlock
            displayedTextBlock = new TextBlock();
            displayedTextBlock.Text = question.QuestionText;
            displayedTextBlock.FontSize = question.QuestionTextFontSize;
            displayedTextBlock.Foreground = question.QuestionTextColor;
            displayedTextBlock.FontWeight = FontWeights.Bold;
            displayedTextBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            displayedTextBlock.Margin = new Thickness(100);

            // Set the position of the TextBlock based on the selected  question attribute

            if (question.QuestionTextPos.Item1 == "Top")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (question.QuestionTextPos.Item1 == "Center")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (question.QuestionTextPos.Item1 == "Bottom")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            }


            int xPos = question.QuestionTextPos.Item2;
            int yPos = question.QuestionTextPos.Item3;

            if (xPos != 0 || yPos != 0)
            {
                TranslateTransform translate = new TranslateTransform();
                translate.X = xPos;
                translate.Y = yPos;
                displayedTextBlock.RenderTransform = translate;
            }

            Grid.SetColumnSpan(displayedTextBlock, 2);
            Grid.SetRow(displayedTextBlock, 0);
            Grid.SetColumn(displayedTextBlock, 0);

            TriviaPlayer.Instance.TriviaPlayerGrid.Children.Add(displayedTextBlock);
        }

        public void DisplayAnswerText(Question question)
        {
            // Check if the TextBlock is already displayed
            if (displayedAnswerTextBlock != null && TriviaPlayer.Instance.TriviaPlayerGrid.Children.Contains(displayedAnswerTextBlock))
            {
                // Remove the TextBlock
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedAnswerTextBlock);
                displayedAnswerTextBlock = null;
                ControlCenter.Instance.ShowAnswer_button.Content = "Reveal answer";
            }
            else
            {
                // Remove the previously displayed TextBlock (if any)
                if (displayedAnswerTextBlock != null)
                {
                    TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedAnswerTextBlock);
                    displayedAnswerTextBlock = null;
                }

                ControlCenter.Instance.ShowAnswer_button.Content = "Hide answer";
                displayedAnswerTextBlock = new TextBlock();
                displayedAnswerTextBlock.Text = question.AnswerText;
                displayedAnswerTextBlock.FontSize = question.QuestionTextFontSize;
                displayedAnswerTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                displayedAnswerTextBlock.FontWeight = FontWeights.Bold;
                displayedAnswerTextBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                if (question.AnswerTextPos.Item1 == "Top")
                {
                    displayedAnswerTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    displayedAnswerTextBlock.VerticalAlignment = VerticalAlignment.Top;
                }
                else if (question.AnswerTextPos.Item1 == "Center")
                {
                    displayedAnswerTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    displayedAnswerTextBlock.VerticalAlignment = VerticalAlignment.Center;
                }
                else if (question.AnswerTextPos.Item1 == "Bottom")
                {
                    displayedAnswerTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    displayedAnswerTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                }

                int xAnsPos = question.QuestionTextPos.Item2;
                int yAnsPos = question.QuestionTextPos.Item3;

                if (xAnsPos != 0 || yAnsPos != 0)
                {
                    TranslateTransform translate = new TranslateTransform();
                    translate.X = xAnsPos;
                    translate.Y = yAnsPos;
                    displayedAnswerTextBlock.RenderTransform = translate;
                }

                Grid.SetColumnSpan(displayedAnswerTextBlock, 2);
                Grid.SetRow(displayedAnswerTextBlock, 0);
                Grid.SetColumn(displayedAnswerTextBlock, 0);

                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Add(displayedAnswerTextBlock);
            }
        }

        private void MainGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            CategoryGrid = e.DetailsElement as DataGrid;
            if (CategoryGrid == null) return;          
        }

        private void CategoryGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if the clicked item is not a row
            var visual = e.OriginalSource as Visual;
            while (visual != null && visual != CategoryGrid)
            {
                if (visual is DataGridRow)
                {
                    return; // Clicked on a row, do nothing
                }
                visual = VisualTreeHelper.GetParent(visual) as Visual;
            }

            // Clicked outside of a row, clear selection
            CategoryGrid.SelectedItem = null;

            if (!(e.OriginalSource is DataGridCell || e.OriginalSource is TextBlock))
            {
                e.Handled = true;
            }
        }
    }
}