﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for AddQuestion_Dialog.xaml
    /// </summary>
    public partial class AddQuestion_Dialog : Window
    {
        public List<string> filePaths;
        public string Question;
        public string Answer;
        public int QuestionTextFontSize;
        public SolidColorBrush QuestionTextColor;
        public Tuple<string, int, int> QuestionTextPosition;
        public Tuple<string, int, int> AnswerTextPosition;
        public string SubCategoryText;
        public bool ResetClock;
        public string BackgroundImagePath;
        public AddQuestion_Dialog(Question question = null)
        {
            InitializeComponent();
            if (question == null)
            {
                filePaths = new List<string>();
                QuestionPos_Combobox.SelectedIndex = 1;
                AnswerPos_Combobox.SelectedIndex = 2;
                QuestionTextFontSize = 60;
                QuestionTextColor = new SolidColorBrush(Colors.White);
                SubCategoryText = "Null";
            }
            else
            {
                // Update text boxes
                QuestionTextBox.Text = question.QuestionText;
                AnswerTextBox.Text = question.Answer;
                xOffset.Text = question.QuestionTextPos.Item2.ToString();
                yOffset.Text = question.QuestionTextPos.Item3.ToString();
                FontSize_input.Text = question.QuestionTextFontSize.ToString();
                FontSize_current_disp.Content = $"{question.QuestionTextFontSize}px";
                xOffset_Answer.Text = question.AnswerTextPos.Item2.ToString();
                yOffset_Answer.Text = question.AnswerTextPos.Item3.ToString();
                SubCategoryTextBox.Text = question.SubCategoryName;
                PointsTextBox.Text = question.Points.ToString();
                BonusPointsTextBox.Text = question.BonusPoints.ToString();
                PenaltyTextBox.Text = question.Penalty.ToString();
                TimeTextBox.Text = ((int)question.Time.TotalSeconds).ToString();

                // Update variables
                QuestionTextColor = question.QuestionTextColor;
                QuestionTextFontSize = question.QuestionTextFontSize;
                QuestionPos_Combobox.SelectedIndex = 1;
                AnswerPos_Combobox.SelectedIndex = 2;
                SubCategoryText = question.SubCategoryName;

            }
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

                LoadMedia_filenames_label.Text = $"{openFileDialog.FileNames.Length} selected";
            }
        }

        private void BackgroundSelect_Click(object sender, RoutedEventArgs e)
        {
            BackgroundImagePath = FileBrowser.ImageSelect(BackgroundPath_label, BackgroundImagePath);
        }

        public void SaveQuestionPosition()
        {
            var selectedItem = QuestionPos_Combobox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                QuestionTextPosition = new Tuple<string, int, int>(selectedItem.Content.ToString(), int.Parse(xOffset.Text), int.Parse(yOffset.Text));
            }
        }

        public void SaveAnswerPosition()
        {
            var selectedItem = AnswerPos_Combobox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                AnswerTextPosition = new Tuple<string, int, int>(selectedItem.Content.ToString(), int.Parse(xOffset_Answer.Text), int.Parse(yOffset_Answer.Text));
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionTextBox.Text;
            Answer = AnswerTextBox.Text;
            SubCategoryText = SubCategoryTextBox.Text;
            ResetClock = ResetClock_CheckBox.IsChecked ?? false;
            SaveQuestionPosition();
            SaveAnswerPosition();
            DialogResult = true;
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

        private void Answer_xOffset_changed(object sender, KeyEventArgs e)
        {
            HandleTextChange(sender, e, xOffset_Answer, _ => { });
        }

        private void Answer_yOffset_changed(object sender, KeyEventArgs e)
        {
            HandleTextChange(sender, e, yOffset_Answer, _ => { });
        }
    }
}
