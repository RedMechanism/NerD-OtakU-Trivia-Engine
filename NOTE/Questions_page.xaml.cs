using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Linq;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Questions_Page.xaml
    /// </summary>
    public partial class Questions_Page : Page
    {
        public static Questions_Page Instance;
        private string categoryBackgroundPath;

        private TextBlock displayedTextBlock;
        private TextBlock displayedAnswerTextBlock;
        public Questions_Page()
        {

            InitializeComponent();

            Instance = this;
            ObservableCollection<Category> mainCategories = new ObservableCollection<Category>();
            CategoryGrid.ItemsSource = mainCategories;
            CategoryGrid.IsReadOnly = true;
            DataContext = this;
            categoryBackgroundPath = "pack://application:,,,/Images/CategoryBackground.jpg";
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
            if (sender is MenuItem clickedSubItem)
            {
                string header = clickedSubItem.Header.ToString();

                if (header.StartsWith("Team"))
                {
                    int teamIndex = int.Parse(header.Replace("Team", "")) - 1;
                    if (teamIndex >= 0 && teamIndex < ControlCenter.Instance.TeamsList.Count)
                    {
                        // Loop over all selected items in the grid
                        foreach (Question selectedQuestion in QuestionGrid.SelectedItems)
                        {
                            selectedQuestion.Team = ControlCenter.Instance.TeamsList[teamIndex];
                        }
                        QuestionGrid.Items.Refresh(); // Refresh the DataGrid to reflect the changes
                    }
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
                InitializeTextBox(dialog.SubCategoryTextBox, "N/A");
                InitializeTextBox(dialog.PointsTextBox, selectedCategory.Points.ToString());
                InitializeTextBox(dialog.BonusPointsTextBox, selectedCategory.BonusPoints.ToString());
                InitializeTextBox(dialog.PenaltyTextBox, selectedCategory.Penalty.ToString());
                InitializeTextBox(dialog.TimeTextBox, selectedCategory.Time.TotalSeconds.ToString());

                if (selectedCategory.CategoryType == "Media")
                {
                    dialog.LoadMedia_Text.Visibility = Visibility.Visible;
                    dialog.LoadMedia_Button.Visibility = Visibility.Visible;
                    dialog.LoadMediaBatch_CheckBox.Visibility = Visibility.Visible;
                    
                }
                else if (selectedCategory.CategoryType == "Pick your poison")
                {
                    dialog.SubCategoryLabel.Visibility = Visibility.Visible;
                    dialog.SubCategoryTextBox.Visibility = Visibility.Visible;
                    dialog.TextOptions_Expander.Visibility = Visibility.Collapsed;
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
                                QuestionTextPos = dialog.QuestionTextPosition,
                                QuestionTextFontSize = dialog.QuestionTextFontSize,
                                QuestionTextColor = dialog.QuestionTextColor,
                                CategoryType = selectedCategory.CategoryType,
                                CategoryName = selectedCategory.CategoryName,
                                Points = int.Parse(dialog.PointsTextBox.Text),
                                BonusPoints = int.Parse(dialog.BonusPointsTextBox.Text),
                                Penalty = int.Parse(dialog.PenaltyTextBox.Text),
                                Time = TimeSpan.FromSeconds(int.Parse(dialog.TimeTextBox.Text)),
                                QuestionNumber = questionNumber,
                                MediaPath = new Uri(mediaPath),
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
                                QuestionTextPos = dialog.QuestionTextPosition,
                                QuestionTextFontSize = dialog.QuestionTextFontSize,
                                QuestionTextColor = dialog.QuestionTextColor,
                                Answer = dialog.Answer,
                                CategoryType = selectedCategory.CategoryType,
                                CategoryName = selectedCategory.CategoryName,
                                SubCategoryName = dialog.SubCategoryText,
                                BackgroundImagePath = selectedCategory.BackgroundImagePath,
                                Points = int.Parse(dialog.PointsTextBox.Text),
                                BonusPoints = int.Parse(dialog.BonusPointsTextBox.Text),
                                Penalty = int.Parse(dialog.PenaltyTextBox.Text),
                                Time = TimeSpan.FromSeconds(int.Parse(dialog.TimeTextBox.Text)),
                                QuestionNumber = questionNumber,
                                MediaPath = null,
                                Team = ControlCenter.Instance.TeamsList[teamIndex]
                            }); ;
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
            DeleteSelectedQuestions();
        }
        
        public void RevealQuestionText(Question question)
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

            if (ControlCenter.Instance._settings_Page.DropShadow_checkbox.IsChecked == true)
            {
                displayedTextBlock.Effect = new DropShadowEffect
                {
                    BlurRadius = 5,
                    ShadowDepth = 2,
                    Opacity = 0.5,
                    Color = Colors.Black
                };
            }

            // Set the position of the TextBlock based on the selected  question attribute
            
            if (question.QuestionTextPos.Item1 == "Top Left")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (question.QuestionTextPos.Item1 == "Top Middle")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (question.QuestionTextPos.Item1 == "Top Right")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (question.QuestionTextPos.Item1 == "Middle Left")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (question.QuestionTextPos.Item1 == "Center")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (question.QuestionTextPos.Item1 == "Middle Right")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (question.QuestionTextPos.Item1 == "Bottom Left")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else if (question.QuestionTextPos.Item1 == "Bottom Middle")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else if (question.QuestionTextPos.Item1 == "Bottom Right")
            {
                displayedTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                displayedTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            }

            int xPos = question.QuestionTextPos.Item2;
            int yPos = question.QuestionTextPos.Item3;

            displayedTextBlock.Margin = new Thickness(xPos, yPos, 0, 0);

            Grid.SetColumnSpan(displayedTextBlock, 2);
            Grid.SetRow(displayedTextBlock, 0);
            Grid.SetColumn(displayedTextBlock, 0);

            TriviaPlayer.Instance.TriviaPlayerGrid.Children.Add(displayedTextBlock);
        }

        public void RevealAnswerText(Question question)
        {
            // Check if the TextBlock is already displayed
            if (displayedAnswerTextBlock != null && TriviaPlayer.Instance.TriviaPlayerGrid.Children.Contains(displayedAnswerTextBlock))
            {
                // Remove the TextBlock
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedAnswerTextBlock);
                displayedAnswerTextBlock = null;
                ControlCenter.Instance.ShowAnswer_button.Content = "Show answer";
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
                displayedAnswerTextBlock.Text = question.Answer;
                displayedAnswerTextBlock.FontSize = question.QuestionTextFontSize;
                displayedAnswerTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                displayedAnswerTextBlock.FontWeight = FontWeights.Bold;
                displayedAnswerTextBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                if (ControlCenter.Instance._settings_Page.DropShadow_checkbox.IsChecked == true)
                {
                    displayedAnswerTextBlock.Effect = new DropShadowEffect
                    {
                        BlurRadius = 2,
                        ShadowDepth = 1,
                        Opacity = 0.5,
                        Color = Colors.Black
                    };
                }

                displayedAnswerTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                displayedAnswerTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                displayedAnswerTextBlock.Margin = new Thickness(0, 0, 0, 100);

                Grid.SetColumnSpan(displayedAnswerTextBlock, 2);
                Grid.SetRow(displayedAnswerTextBlock, 0);
                Grid.SetColumn(displayedAnswerTextBlock, 0);

                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Add(displayedAnswerTextBlock);
            }
        }

        public void ClearQuestionText()
        {
            if (displayedTextBlock != null)
            {
                TriviaPlayer.Instance.TriviaPlayerGrid.Children.Remove(displayedTextBlock);
                displayedTextBlock = null;
            }
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

        private BitmapImage CategoryLogo(string filePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        private void DisplayCategory_RClick(object sender, RoutedEventArgs e)
        {
            TriviaPlayer.Instance.Category_logo.Visibility = Visibility.Collapsed;

            if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
            {
                Category category = (Category)CategoryGrid.SelectedItem;

                ControlCenter.Instance.ClearTimer();
                ClearQuestionText();

                TriviaPlayer.Instance.Category_logo.Visibility = Visibility.Visible;
                TriviaPlayer.Instance.Category_logo.Source = CategoryLogo(category.IconPath);

                MediaPlayer_Page._media.Path = new Uri(categoryBackgroundPath);
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
            // Keep the categroy grid from immediately collapsing
            e.Handled = true;
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

        public void DeleteSelectedQuestions()
        {
            var selectedCategory = CategoryGrid.SelectedItem as Category;

            if (selectedCategory != null)
            {
                List<Question> itemsToRemove = new List<Question>();

                foreach (var item in QuestionGrid.SelectedItems)
                {
                    var question = item as Question;
                    if (question != null)
                    {
                        itemsToRemove.Add(question);
                    }
                }

                foreach (var question in itemsToRemove)
                {
                    selectedCategory.Questions.Remove(question);
                }

                if (!itemsToRemove.Any())
                {
                    MessageBox.Show("Please select one or more questions to delete.");
                }

                int counter = 1;
                foreach (var question in selectedCategory.Questions)
                {
                    question.QuestionNumber = counter++;
                }

                QuestionGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select a category.");
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

        private void QuestionContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ContextMenu)sender;
            var selectedItem = CategoryGrid.SelectedItem as Category;

            if (selectedItem != null && selectedItem.CategoryType == "Pick your poison")
            {
                var reenableButtonMenuItem = (MenuItem)contextMenu.ItemContainerGenerator.ContainerFromIndex(2);
                if (reenableButtonMenuItem != null)
                    reenableButtonMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                var reenableButtonMenuItem = (MenuItem)contextMenu.ItemContainerGenerator.ContainerFromIndex(2);
                if (reenableButtonMenuItem != null)
                    reenableButtonMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void ReenableButton_RClick(object sender, RoutedEventArgs e)
        {
            Question question = QuestionGrid.SelectedItem as Question;

            PickYourPoison_Page.Instance.ReenableButton(question);
        }
    }
}