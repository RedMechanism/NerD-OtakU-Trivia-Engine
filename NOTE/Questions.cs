using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace NOTE
{
    public class Category: INotifyPropertyChanged
    {
        public string CategoryName { get; set; }
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }

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
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Question
    {
        public string CategoryName { get; set; }
        public int QuestionNumber { get; set; }
        public Teams Team { get; set; }
        public int Points { get; set; }
        public int BonusPoints { get; set; }
        public int Penalty { get; set; }
        public TimeSpan Time { get; set; }
        public string QuestionText { get; set; }
        public Uri FilePath { get; set; }
        public bool ClearClock { get; set; }

        public Question()
        {
            ClearClock = true;
        }
    }
}
