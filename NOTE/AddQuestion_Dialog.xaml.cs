using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        public int QuestionTextFontSize;
        public SolidColorBrush QuestionTextColor;
        public Tuple<string, int, int> QuestionTextPosition;
        public string SubCategoryText;
        public AddQuestion_Dialog()
        {
            InitializeComponent();
            filePaths = new List<string>();
            QPos_Combobox.SelectedIndex = 4;
            QuestionTextFontSize = 60;
            QuestionTextColor = new SolidColorBrush(Colors.White);
            SubCategoryText = "Null";
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
                QuestionTextPosition = new Tuple<string, int, int>(selectedItem.Content.ToString(), int.Parse(xOffset.Text), int.Parse(yOffset.Text));
            }
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionTextBox.Text;
            SubCategoryText = SubCategoryTextBox.Text;
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

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                QuestionTextColor = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                ColorPreview.Fill = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }

        private void HandleTextChange(object sender, KeyEventArgs e, TextBox input, Action<int> callback)
        {
            if (e.Key == Key.Enter)
            {
                if (!input.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Enter only positive digits");
                    return;
                }

                int val = int.Parse(input.Text);
                if (val >= 1 && val < 1000) callback(val);
                else MessageBox.Show("Restricted to between 1 and 999pt");
            }
        }

        private void Font_size_changed(object sender, KeyEventArgs e)
        {
            HandleTextChange(sender, e, FontSize_input, val => {
                QuestionTextFontSize = val;
                FontSize_current_disp.Content = $"{QuestionTextFontSize}px";
            });
        }

        private void Question_xOffset_changed(object sender, KeyEventArgs e)
        {
            HandleTextChange(sender, e, xOffset, _ => { });
        }

        private void Question_yOffset_changed(object sender, KeyEventArgs e)
        {
            HandleTextChange(sender, e, yOffset, _ => { });
        }
    }
}
