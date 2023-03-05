using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
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
        public ObservableCollection<string> TeamNames { get; set; }

        public Questions_Page()
        {

            InitializeComponent();

            TeamNames = new ObservableCollection<string>();
            foreach (Teams team in ControlCenter.Instance.TeamsList)
            {
                TeamNames.Add(team.Name);
            }

            Instance = this;
            ObservableCollection<Category> mainCategories = new ObservableCollection<Category>();
            CategoryGrid.ItemsSource = mainCategories;
            CategoryGrid.IsReadOnly = true;
            DataContext = this;
        }

        private void AddCategory_Button(object sender, RoutedEventArgs e)
        {
            AddCategory_Dialog addRowDialog = new AddCategory_Dialog();
            addRowDialog.ShowDialog();
        }

        private void ExpandNestedDatagrid()
        {
            var selectedRow = CategoryGrid.ItemContainerGenerator.ContainerFromItem(CategoryGrid.SelectedItem) as DataGridRow;
            if (selectedRow != null)
            {
                selectedRow.DetailsVisibility = Visibility.Visible;
            }
        }
        private void AddQuestion_Button(object sender, RoutedEventArgs e)
        {
            var selectedItem = CategoryGrid.SelectedItem as Category;
            if (selectedItem != null)
            {
                if (!selectedItem.IsSpecial) {
                    selectedItem.Questions.Add(new Question { CategoryName = "New Sub-Category" });
                    selectedItem.IsExpanded = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Special!");
                }
                    
            }

            // Immediately expands Datagrid
            ExpandNestedDatagrid();
        }

        private void AddQuestionFile_Button(object sender, RoutedEventArgs e)
        {
            var selectedItem = CategoryGrid.SelectedItem as Category;
            if (selectedItem != null)
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    int teamNumber = 1;
                    int questionNumber = 1;
                    foreach (string file in openFileDialog.FileNames)
                    {
                        selectedItem.Questions.Add(new Question
                        {
                            CategoryName = selectedItem.CategoryName,
                            QuestionNumber = questionNumber,
                            QuestionText = selectedItem.QuestionText,
                            Team = ControlCenter.Instance.TeamsList[teamNumber - 1],
                            Points = selectedItem.Points,
                            BonusPoints = selectedItem.BonusPoints,
                            Penalty =   selectedItem.Penalty,
                            Time = selectedItem.Time,
                            FilePath = new Uri(file)
                        });

                        teamNumber++;
                        questionNumber++;
                        if (teamNumber > 4)
                        {
                            teamNumber = 1;
                        }

                        selectedItem.IsExpanded = Visibility.Visible;
                    }
                }
            }

            // Immediately displays added media files
            ExpandNestedDatagrid();
        }

        public DataGrid QuestionGrid;
        private void RemoveQuestion_RClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = CategoryGrid.SelectedItem as Category;
            if (selectedItem != null)
            {
                var selectedFiles = QuestionGrid.SelectedItems;
                while (selectedFiles.Count > 0)
                {
                    selectedItem.Questions.Remove(selectedFiles[0] as Question);
                }
            }
        }

        private void RemoveCategory_RClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = CategoryGrid.SelectedItem as Category;
            if (selectedItem != null)
            {
                var mainCategories = CategoryGrid.ItemsSource as ObservableCollection<Category>;
                mainCategories.Remove(selectedItem);
            }
        }

        private void DisplayCategory_RClick(object sender, RoutedEventArgs e)
        {
            Category category = (Category)CategoryGrid.SelectedItem;

            if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
            {
                TriviaPlayer._media.Path = new Uri(category.IconPath);
                TriviaPlayer._media.Play();
            }
        }

        private void CategoryGrid_LButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = CategoryGrid.ItemContainerGenerator.ContainerFromItem(CategoryGrid.SelectedItem) as DataGridRow;
            if (selectedRow != null)
            {
                if (CategoryGrid.SelectedItem != null)
                {
                    if (selectedRow.DetailsVisibility == Visibility.Collapsed)
                    {
                        selectedRow.DetailsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        selectedRow.DetailsVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void QuestionGrid_LButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Question question = (Question)QuestionGrid.SelectedItem;

            if (question != null)
            {
                if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
                {
                    TriviaPlayer._media.Path = question.FilePath;
                    ControlCenter.Instance._Timer.Duration = question.Time;
                    if (question.ClearClock)
                    {
                        ControlCenter.Instance.ClearTimer();
                    }
                }
            }
        }
        private void MainGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            QuestionGrid = e.DetailsElement as DataGrid;
            if (QuestionGrid == null) return;
        }
        public void ColourRow(SolidColorBrush solidColorBrush)
        {
            DataGridRow dataGridRow = CategoryGrid.ItemContainerGenerator.ContainerFromItem(CategoryGrid.SelectedItem) as DataGridRow;
            if (dataGridRow != null)
                dataGridRow.Background = solidColorBrush;
        }

        private void CategoryGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (CategoryGrid.SelectedItem == null)
            {
                e.Handled = true;
            }
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
        }

    }
}
