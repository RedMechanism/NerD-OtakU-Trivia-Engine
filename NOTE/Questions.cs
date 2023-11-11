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
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public int TricklePenalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }
        public Uri FilePath { get; set; }
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
