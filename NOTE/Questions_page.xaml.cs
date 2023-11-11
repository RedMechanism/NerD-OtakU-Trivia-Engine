using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            gridItems.Add(new Question
            {
                QuestionName = "Geography Category Banner",
                Type = "Banner",
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Geography\\Geography.png"),
            });

            gridItems.Add(new Question
            {
                QuestionName = "Geography 1",
                Type = "Media",
                Points = 10,
                BonusPoints = 5,
                Penalty = 0,
                Time = TimeSpan.FromSeconds(30),
                Team = ControlCenter.Instance.Team1,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Geography\\1.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Geography 2",
                Type = "Media",
                Points = 10,
                BonusPoints = 5,
                Penalty = 0,
                Time = TimeSpan.FromSeconds(30),
                Team = ControlCenter.Instance.Team2,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Geography\\2.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Geography 3",
                Type = "Media",
                Points = 10,
                BonusPoints = 5,
                Penalty = 0,
                Time = TimeSpan.FromSeconds(30),
                Team = ControlCenter.Instance.Team3,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Geography\\3.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Geography 4",
                Type = "Media",
                Points = 10,
                BonusPoints = 5,
                Penalty = 0,
                Time = TimeSpan.FromSeconds(30),
                Team = ControlCenter.Instance.Team4,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Geography\\4.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Mythology Category Banner",
                Type = "Banner",
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Mythology\\Mythology.png"),
                NoClock = true,
            });

            gridItems.Add(new Question
            {
                QuestionName = "Mythology 1",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 0,
                TricklePenalty = 0,
                Time = TimeSpan.FromSeconds(40),
                Team = ControlCenter.Instance.Team1,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Mythology\\1.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Mythology 2",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 0,
                TricklePenalty = 0,
                Time = TimeSpan.FromSeconds(40),
                Team = ControlCenter.Instance.Team2,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Mythology\\2.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Mythology 3",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 0,
                TricklePenalty = 0,
                Time = TimeSpan.FromSeconds(40),
                Team = ControlCenter.Instance.Team3,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Mythology\\3.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Mythology 4",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 0,
                TricklePenalty = 0,
                Time = TimeSpan.FromSeconds(40),
                Team = ControlCenter.Instance.Team4,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Mythology\\4.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Movies Category Banner",
                Type = "Banner",
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Movies\\Movies.png"),
                NoClock = true,
            });

            gridItems.Add(new Question
            {
                QuestionName = "Movies 1",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 5,
                Time = TimeSpan.FromSeconds(15),
                Team = ControlCenter.Instance.Team1,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Movies\\1.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Movies 2",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 5,
                Time = TimeSpan.FromSeconds(15),
                Team = ControlCenter.Instance.Team2,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Movies\\2.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Movies 3",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 5,
                Time = TimeSpan.FromSeconds(15),
                Team = ControlCenter.Instance.Team3,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Movies\\3.jpg")
            });

            gridItems.Add(new Question
            {
                QuestionName = "Movies 4",
                Type = "Media",
                Points = 10,
                BonusPoints = 0,
                Penalty = 5,
                Time = TimeSpan.FromSeconds(15),
                Team = ControlCenter.Instance.Team4,
                FilePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}TriviaFiles\\Movies\\4.jpg")
            });

            ControlCenter.Instance.currentQuestion = CategoryGrid.SelectedItem as Question;
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