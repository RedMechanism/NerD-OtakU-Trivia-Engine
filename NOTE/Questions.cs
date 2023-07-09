﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace NOTE
{
    public class Category: INotifyPropertyChanged
    {
        public string CategoryType { get; set; }
        public string CategoryName { get; set; }
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }
        public string Curator { get; set; }
        public string BackgroundImagePath { get; set; }
        public string IconPath { get; set; }
        public bool IsSpecial { get; set; }
        public int QuestionCount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private Visibility isExpanded;
        public ObservableCollection<Question> Questions { get; set; }

        public Visibility IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public Category()
        {
            isExpanded = Visibility.Collapsed;
            IsSpecial = false;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Question
    {
        public string CategoryType { get; set; }
        public string CategoryName { get; set; }
        public int QuestionNumber { get; set; }
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }
        public Tuple<string, int, int> QuestionTextPos { get; set; }
        public Tuple<string, int, int> AnswerTextPos { get; set; }
        public int QuestionTextFontSize { get; set; }
        public SolidColorBrush QuestionTextColor { get; set; }
        public string Answer { get; set; }
        public Uri MediaPath { get; set; }
        public bool ClearClock { get; set; }
        public string BackgroundImagePath { get; set; }
        public string SubCategoryName { get; set; }
    }
}
