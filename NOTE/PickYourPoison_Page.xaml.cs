using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for PickYourPoison_Page.xaml
    /// </summary>
    public partial class PickYourPoison_Page : Page
    {
        public List<Question> Questions { get; set; }
        public List<Question> CurrentCategoryQuestions { get; set; }
        private int CurrentQuestionIndex { get; set; }

        public static PickYourPoison_Page Instance;
        public PickYourPoison_Page(List<Question> questions)
        {
            InitializeComponent();
            Instance = this;

            Questions = questions;

            // Generate buttons for each category.
            var categories = Questions.Select(q => q.SubCategoryName).Distinct().ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                Button button = new Button
                {
                    Content = categories[i],
                    Style = (Style)FindResource("ModernButton") // apply style here
                };
                button.Click += Button_Click;
                Grid.SetRow(button, i / 4);
                Grid.SetColumn(button, i % 4);
                buttonGrid.Children.Add(button);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get all questions for this category.
            Button button = (Button)sender;
            string category = button.Content.ToString();
            CurrentCategoryQuestions = Questions.Where(q => q.SubCategoryName == category).ToList();

            if (CurrentCategoryQuestions.Any())
            {
                // Display the first question and manage buttons.
                CurrentQuestionIndex = 0;
                UpdateQuestionDisplay();

                // Switch to the question grid.
                buttonGrid.Visibility = Visibility.Hidden;
                questionGrid.Visibility = Visibility.Visible;

                // Disable the clicked button.
                button.IsEnabled = false;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                UpdateQuestionDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentQuestionIndex < CurrentCategoryQuestions.Count - 1)
            {
                CurrentQuestionIndex++;
                UpdateQuestionDisplay();
            }
        }

        private void UpdateQuestionDisplay()
        {
            // Update the question text.
            questionText.Text = CurrentCategoryQuestions[CurrentQuestionIndex].QuestionText;

            // Enable or disable the previous and next buttons based on the current question index.
            prevButton.Visibility = CurrentQuestionIndex > 0 ? Visibility.Visible : Visibility.Hidden;
            nextButton.Visibility = CurrentQuestionIndex < CurrentCategoryQuestions.Count - 1 ? Visibility.Visible : Visibility.Hidden;
        }

        public void ReenableButton(Question question)
        {
            // Get the clicked row.
            //Question question = (Question)dataGrid.SelectedItem;
            if (question != null)
            {
                // Find the corresponding button and re-enable it.
                foreach (Button button in buttonGrid.Children)
                {
                    if (button.Content.ToString() == question.SubCategoryName)
                    {
                        button.IsEnabled = true;
                        break;
                    }
                }
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            // When the go back button is clicked, show the button grid.
            buttonGrid.Visibility = Visibility.Visible;
            questionGrid.Visibility = Visibility.Hidden;
        }

    }
}
