using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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
                        gridItems.Add(new Question
                        {
                            QuestionName = q.GetProperty("questionName").GetString(),
                            Type = q.GetProperty("type").GetString(),
                            Points = q.GetProperty("points").GetInt32(),
                            BonusPoints = q.GetProperty("bonusPoints").GetInt32(),
                            Penalty = q.GetProperty("penalty").GetInt32(),
                            TricklePenalty = q.GetProperty("tricklePenalty").GetInt32(),
                            Time = TimeSpan.FromSeconds(q.GetProperty("timeSeconds").GetInt32()),
                            Team = GetTeamByIndex(q.GetProperty("teamIndex").GetInt32()),
                            FilePath = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, q.GetProperty("filePath").GetString())),
                            ClearClock = q.TryGetProperty("clearClock", out var cc) ? cc.GetBoolean() : true,
                            NoClock = q.TryGetProperty("noClock", out var nc) && nc.GetBoolean()
                        });
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