using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace NOTE
{
    public class Question : INotifyPropertyChanged
    {
        public string Type { get; set; }
        public string QuestionName { get; set; }
        public string CategoryName { get; set; }
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public int TricklePenalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }
        public Tuple<string, int, int> QuestionTextPos { get; set; }
        public Tuple<string, int, int> AnswerTextPos { get; set; }
        public int QuestionTextFontSize { get; set; }
        public SolidColorBrush QuestionTextColor { get; set; }
        public string AnswerText { get; set; }
        public Uri FilePath { get; set; }
        public Uri BackgroundImagePath { get; set; }
        public bool ClearClock { get; set; }
        public bool NoClock { get; set; }
        public Dictionary<string, int> NerdFeudAnswers { get; set; }
        public Question()
        {
            NoClock = false;
            ClearClock = true;
            NerdFeudAnswers = new Dictionary<string, int>();
            Points = 0;
            BonusPoints = 0;
            Penalty = 0;
            TricklePenalty = 0;

            // Predefined attributed for Q&A questions
            QuestionTextPos = new Tuple<string, int, int>("Center", 0, 0);
            QuestionTextFontSize = 60;
            QuestionTextColor = new SolidColorBrush(Colors.White);
            AnswerTextPos = new Tuple<string, int, int>("Bottom", 0, 0);
            BackgroundImagePath = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images\\QuestionBackground.jpg");
        }

        private SolidColorBrush rowColor = new SolidColorBrush(Colors.White);
        public SolidColorBrush RowColor
        {
            get { return rowColor; }
            set
            {
                if (rowColor != value)
                {
                    rowColor = value;
                    OnPropertyChanged("RowColor");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
