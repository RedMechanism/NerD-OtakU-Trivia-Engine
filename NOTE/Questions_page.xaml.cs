using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        public Questions_Page()
        {

            InitializeComponent();

            Instance = this;
            ObservableCollection<Category> mainCategories = new ObservableCollection<Category>();
            CategoryGrid.ItemsSource = mainCategories;
            CategoryGrid.IsReadOnly = true;
            DataContext = this;
        }

        //Load team change context menu
        private void ChangeTeamMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem changeTeamMenuItem = sender as MenuItem;
            if (changeTeamMenuItem != null)
            {
                changeTeamMenuItem.Items.Clear(); // Clear existing subitems before adding them again

                foreach (var teamContext in ControlCenter.Instance.TeamsList)
                {
                    MenuItem subItem = new MenuItem { Header = teamContext.Name };
                    subItem.Click += SubItem_Click; // Add the click event handler to the subitem
                    changeTeamMenuItem.Items.Add(subItem);
                }
            }
        }

        private void SubItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedSubItem = sender as MenuItem;
            if (clickedSubItem != null && QuestionGrid.SelectedItem != null)
            {
                Question selectedQuestion = QuestionGrid.SelectedItem as Question;
                if (selectedQuestion != null)
                {
                    // Create a new Team object with the new name and assign it to the selected question
                    Teams updatedTeam = new Teams();
                    CopyProperties(selectedQuestion.Team, updatedTeam);
                    updatedTeam.Name = clickedSubItem.Header.ToString();

                    selectedQuestion.Team = updatedTeam;

                    QuestionGrid.Items.Refresh(); // Refresh the DataGrid to reflect the changes
                }
            }
        }

        private void AddCategory_Button(object sender, RoutedEventArgs e)
        {
            AddCategory_Dialog addCategoryDialog = new AddCategory_Dialog();
            addCategoryDialog.ShowDialog();
        }

        private void AddQuestion_Button(object sender, RoutedEventArgs e)
        {
            int questionNumber;
            int teamIndex;
            var dialog = new AddQuestion_Dialog();
            var selectedCategory = Instance.CategoryGrid.SelectedItem as Category;
            List<string> itemList = new List<string>();


            if (selectedCategory != null)
            {
                InitializeTextBox(dialog.QuestionTextBox, selectedCategory.QuestionText);
                InitializeTextBox(dialog.PointsTextBox, selectedCategory.Points.ToString());
                InitializeTextBox(dialog.BonusPointsTextBox, selectedCategory.BonusPoints.ToString());
                InitializeTextBox(dialog.PenaltyTextBox, selectedCategory.Penalty.ToString());
                InitializeTextBox(dialog.TimeTextBox, selectedCategory.Time.TotalSeconds.ToString());
                List<string> mediaPaths = new List<string>();
                if (selectedCategory.CategoryType == "Media")
                {
                    dialog.LoadMedia_Text.Visibility = Visibility.Visible;
                    dialog.LoadMedia_Button.Visibility = Visibility.Visible;
                    dialog.LoadMediaBatch_CheckBox.Visibility = Visibility.Visible;

                    // Assume dialog.MediaFiles is the array of string paths to media files selected by "LoadMedia"
                    //mediaPaths = dialog.MediaFiles;
                    itemList = new List<string> { "Audio/Teams/Team1.mp3", "Audio/Teams/Team2.mp3", "Audio/Teams/Team3.mp3", "Audio/Teams/Team4.mp3" };
                    
                }

                if (dialog.ShowDialog() == true)
                {
                    if (selectedCategory.CategoryType == "Media")
                    {
                        if (selectedCategory.QuestionCount == 0)
                        {
                            questionNumber = 1;
                            teamIndex = 1;
                        }
                        else
                        {
                            questionNumber = selectedCategory.QuestionCount;
                            teamIndex = questionNumber % 4;
                        }
                        itemList = dialog.filePaths;
                        foreach (string mediaPath in itemList)
                        {
                            selectedCategory.Questions.Add(new Question
                            {
                                QuestionText = dialog.QuestionTextBox.Text,
                                CategoryType = selectedCategory.CategoryType,
                                CategoryName = selectedCategory.CategoryName,
                                Points = int.Parse(dialog.PointsTextBox.Text),
                                BonusPoints = int.Parse(dialog.BonusPointsTextBox.Text),
                                Penalty = int.Parse(dialog.PenaltyTextBox.Text),
                                Time = TimeSpan.FromSeconds(int.Parse(dialog.TimeTextBox.Text)),
                                QuestionNumber = questionNumber,
                                MediaPath = new Uri(mediaPath, UriKind.Relative),
                                Team = ControlCenter.Instance.TeamsList[teamIndex - 1]
                            });
                            teamIndex++;
                            questionNumber++;
                            if (teamIndex > 4)
                            {
                                teamIndex = 1;
                            }

                            selectedCategory.IsExpanded = Visibility.Visible;
                        }

                        selectedCategory.QuestionCount = questionNumber - 1;
                        selectedCategory.IsExpanded = Visibility.Visible;
                    }
                    else
                    {
                        if (selectedCategory != null)
                        {
                            if (selectedCategory.QuestionCount == 0)
                            {
                                questionNumber = 1;
                                teamIndex = 0;
                            }
                            else
                            {
                                questionNumber = selectedCategory.QuestionCount;
                                teamIndex = (questionNumber - 1) % 4;
                            }

                            selectedCategory.Questions.Add(new Question
                            {
                                QuestionText = dialog.QuestionTextBox.Text,
                                CategoryType = selectedCategory.CategoryType,
                                CategoryName = selectedCategory.CategoryName,
                                Points = int.Parse(dialog.PointsTextBox.Text),
                                BonusPoints = int.Parse(dialog.BonusPointsTextBox.Text),
                                Penalty = int.Parse(dialog.PenaltyTextBox.Text),
                                Time = TimeSpan.FromSeconds(int.Parse(dialog.TimeTextBox.Text)),
                                QuestionNumber = questionNumber,
                                Team = ControlCenter.Instance.TeamsList[teamIndex]
                            });
                            questionNumber++;
                            selectedCategory.QuestionCount = questionNumber;
                            selectedCategory.IsExpanded = Visibility.Visible;
                        }
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Add or select a category first");
            }
            ExpandNestedDatagrid();
        }

        public DataGrid QuestionGrid;
        private void RemoveQuestion_RClick(object sender, RoutedEventArgs e)
        {
            Delete_category();
        }
        

        private void CategoryGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Delete_category();
                e.Handled = true;
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
                MediaPlayer_Page._media.Path = new Uri(category.IconPath);
                MediaPlayer_Page._media.Play();
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
                    MediaPlayer_Page._media.Path = question.MediaPath;
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

        private void Delete_category()
        {
            var selectedItem = CategoryGrid.SelectedItem as Category;
            if (selectedItem != null)
            {
                var mainCategories = CategoryGrid.ItemsSource as ObservableCollection<Category>;
                mainCategories.Remove(selectedItem);
            }
        }
        private void ExpandNestedDatagrid()
        {
            var selectedRow = CategoryGrid.ItemContainerGenerator.ContainerFromItem(CategoryGrid.SelectedItem) as DataGridRow;
            if (selectedRow != null)
            {
                selectedRow.DetailsVisibility = Visibility.Visible;
            }
        }

        public void InitializeTextBox(TextBox textbox, string text)
        {
            textbox.Text = text;
            textbox.Foreground = Brushes.Gray;
            textbox.TextChanged += TextBox_TextChanged;
        }

        public void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Foreground = Brushes.Black;
            textbox.FontWeight = FontWeights.Bold;
        }

        // CopyProperties method for copying properties from one object to another
        // The CopyProperties method is used in the SubItem_Click event handler to copy all the properties from the existing Team object to the new one
        public static void CopyProperties<T>(T source, T target)
        {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and/or target objects cannot be null.");

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    object value = property.GetValue(source, null);
                    property.SetValue(target, value, null);
                }
            }
        }

    }
}