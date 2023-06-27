using System;
using System.Collections.Generic;
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
        public List<string> filePaths;
        public string Question;
        public string QuestionTextPosition;
        public AddQuestion_Dialog()
        {
            InitializeComponent();
            filePaths = new List<string>();
            QPos_Combobox.SelectedIndex = 4;
        }

        private void LoadMedia_CLick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    filePaths.Add(file);
                }
            }
        }

        public void SavePosition()
        {
            var selectedItem = QPos_Combobox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                QuestionTextPosition = selectedItem.Content.ToString();
            }
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionTextBox.Text;
            DialogResult = true;
            SavePosition();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
