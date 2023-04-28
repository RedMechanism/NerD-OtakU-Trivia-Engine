using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static NOTE.Questions_Page;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for AddQuestion_Dialog.xaml
    /// </summary>
    public partial class AddQuestion_Dialog : Window
    {
        public string Question;
        public AddQuestion_Dialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionTextBox.Text;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
