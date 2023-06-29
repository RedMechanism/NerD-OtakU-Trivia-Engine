using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static NOTE.Questions_Page;
using static NOTE.Teams;

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
        public int QuestionTextFontSize;
        public AddQuestion_Dialog()
        {
            InitializeComponent();
            filePaths = new List<string>();
            QPos_Combobox.SelectedIndex = 4;
            QuestionTextFontSize = 60;
        }

        private void LoadMedia_Click(object sender, RoutedEventArgs e)
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

        private void ColorPicker_Botton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of ColorDialog
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            // Display the dialog and get the selected color
            System.Windows.Forms.DialogResult result = colorDialog.ShowDialog();
        }

        private void Font_size_changed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (FontSize_input.Text.All(char.IsDigit))
                {
                    int fontSizeVal = int.Parse(FontSize_input.Text);
                    if (fontSizeVal >= 1 && fontSizeVal < 1000)
                    {
                        QuestionTextFontSize = fontSizeVal;
                        FontSize_current_disp.Content = $"{QuestionTextFontSize}px";
                    }
                    else
                    {
                        MessageBox.Show("Font size restricted to between 1 and 999pt");
                    }
                }
                else
                {
                    MessageBox.Show("Enter only positive digits");
                }
            }
        }
    }
}
