using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static NOTE.Questions_Page;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for AddCategory_Dialog.xaml
    /// </summary>
    public partial class AddCategory_Dialog : Window
    {
        private string categoryLogoPath;
        public AddCategory_Dialog()
        {
            InitializeComponent();
            InitializeTextBox(CategoryTextBox, "");
            InitializeTextBox(PointsTextBox,"10");
            InitializeTextBox(BonusPointsTextBox, "5");
            InitializeTextBox(PenaltyPointsTextBox, "0");
            InitializeTextBox(TimeTextBox, "60");
            InitializeTextBox(QuestionTextBox, "This is a sickness");
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Save_category();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                categoryLogoPath = openFileDialog.FileName;
                string fileName = Path.GetFileName(categoryLogoPath);
                string parentFolder = Path.GetDirectoryName(categoryLogoPath);
                string parentFolderName = new DirectoryInfo(parentFolder).Name;
                CategoryLogoPath.Text = parentFolderName + "\\" + fileName;
            }
        }

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save_category();
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

        private void Save_category()
        {
            var mainWindow = Application.Current.MainWindow as ControlCenter;
            if (mainWindow != null)
            {
                var mainCategories = Instance.CategoryGrid.ItemsSource as ObservableCollection<Category>;
                if (mainCategories != null)
                {
                    mainCategories.Add(new Category
                    {
                        CategoryType = CategoryComboBox.Text,
                        CategoryName = CategoryTextBox.Text,
                        Points = int.Parse(PointsTextBox.Text),
                        BonusPoints = int.Parse(BonusPointsTextBox.Text),
                        Penalty = int.Parse(PenaltyPointsTextBox.Text),
                        Time = TimeSpan.FromSeconds(int.Parse(TimeTextBox.Text)),
                        QuestionText = QuestionTextBox.Text,
                        IconPath = categoryLogoPath,
                        Curator = CuratorTextBox.Text,
                        QuestionCount = 0,
                        Questions = new ObservableCollection<Question>()
                    });
                }
            }
            Close();
        }

        private void Other_Selected(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Other Selected");
        }
    }
}
