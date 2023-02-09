using System;
using System.Collections.ObjectModel;
using System.Windows;
using static NOTE.Questions_Page;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for AddCategory_Dialog.xaml
    /// </summary>
    public partial class AddCategory_Dialog : Window
    {
        public AddCategory_Dialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as ControlCenter;
            if (mainWindow != null)
            {
                var mainCategories = Instance.CategoryGrid.ItemsSource as ObservableCollection<Category>;
                if (mainCategories != null)
                {
                    mainCategories.Add(new Category { CategoryName = CategoryTextBox.Text, 
                        Points = int.Parse(PointsTextBox.Text),
                        BonusPoints = int.Parse(BonusPointsTextBox.Text),
                        Penalty = int.Parse(PenaltyPointsTextBox.Text),
                        Time = TimeSpan.FromSeconds(int.Parse(TimeTextBox.Text)),
                        QuestionText = QuestionTextTextBox.Text,
                        Questions = new ObservableCollection<Question>() });
                }
            }
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
