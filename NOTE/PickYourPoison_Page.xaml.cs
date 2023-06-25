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
        private List<Question> CurrentCategoryQuestions { get; set; }
        private int CurrentQuestionIndex { get; set; }

        public PickYourPoison_Page()
        {
            InitializeComponent();

            // Initialize the list of questions.
            Questions = new List<Question>
        {
            new Question { SubCategoryName = "Geography", QuestionText = "What is the capital of France?", Answer = "Paris" },
            new Question { SubCategoryName = "Geography", QuestionText = "What is the capital of Italy?", Answer = "Rome" },
            new Question { SubCategoryName = "Geography", QuestionText = "What is the capital of Japan?", Answer = "Tokyo" },
            new Question { SubCategoryName = "Geography", QuestionText = "What is the capital of Russia?", Answer = "Moscow" },
            new Question { SubCategoryName = "History", QuestionText = "In which year did World War II end?", Answer = "1945" },
            new Question { SubCategoryName = "Science", QuestionText = "What is the chemical symbol for gold?", Answer = "Au" },
            new Question { SubCategoryName = "Science", QuestionText = "Do human cells have a cell wall?", Answer = "Yes" },
            new Question { SubCategoryName = "Literature", QuestionText = "Who wrote the novel 'Pride and Prejudice'?", Answer = "Jane Austen" },
            new Question { SubCategoryName = "Sports", QuestionText = "Which country hosted the 2016 Summer Olympics?", Answer = "Brazil" },
            new Question { SubCategoryName = "Art", QuestionText = "Who painted 'Starry Night'?", Answer = "Vincent van Gogh" },
            new Question { SubCategoryName = "Music", QuestionText = "Which band released the album 'Abbey Road'?", Answer = "The Beatles" },
            new Question { SubCategoryName = "Technology", QuestionText = "Who co-founded Apple Inc. alongside Steve Jobs?", Answer = "Steve Wozniak" },
            new Question { SubCategoryName = "Movies", QuestionText = "Who directed the movie 'The Shawshank Redemption'?", Answer = "Frank Darabont" },
            new Question { SubCategoryName = "Food and Drink", QuestionText = "What is the main ingredient in guacamole?", Answer = "Avocado" },
            new Question { SubCategoryName = "Mathematics", QuestionText = "What is the value of pi (π)?", Answer = "3.14159..." },
            new Question { SubCategoryName = "Fashion", QuestionText = "Which fashion designer is known for creating the little black dress?", Answer = "Coco Chanel" }
        };

            // Bind the data grid to the list of questions.
            dataGrid.ItemsSource = Questions;

            // Generate buttons for each category.
            var categories = Questions.Select(q => q.SubCategoryName).Distinct().ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                Button button = new Button
                {
                    Content = categories[i],
                    Width = 100,
                    Height = 50,
                    Margin = new Thickness(5)
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

        private void ReenableButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked row.
            Question question = (Question)dataGrid.SelectedItem;
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
