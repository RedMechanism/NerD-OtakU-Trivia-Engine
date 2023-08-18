using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace NOTE
{
    public partial class Logs_Page : Page
    {
        public static Logs_Page Instance;
        public class LogEntry
        {
            public string Date { get; set; }
            public string Time { get; set; }
            public string Team { get; set; }
            public string Category { get; set; }
            public string Question { get; set; }
            public string Score { get; set; }
            public string Total { get; set; }
        }

        public Logs_Page()
        {
            InitializeComponent();

            Instance = this;
            LogDataGrid.ItemsSource = LoadLogFile();
        }

        public static List<LogEntry> LoadLogFile()
        {
            var logEntries = new List<LogEntry>();
            var lines = File.ReadAllLines(LogWriter.fileName);

            foreach (var line in lines)
            {
                if (line.StartsWith("Trivia")) continue;
                var logEntry = ParseLogEntry(line);
                if (logEntry != null)
                    logEntries.Add(logEntry);
            }

            return logEntries;
        }

        public static void UpdateLogDataGrid()
        {
            Instance.LogDataGrid.ItemsSource = LoadLogFile();
        }

        public static LogEntry ParseLogEntry(string line)
        {
            var components = line.Split(new[] { " - ", ", ", ", Q#", ", ", " pts, Total = ", " pts" }, StringSplitOptions.None);

            if (components.Length >= 7)
            {
                var scoreComponent = components[4];
                var score = scoreComponent.Contains("incorrect") ? "incorrect" : scoreComponent;
                var totalComponent = components[5];
                var total = totalComponent.Contains("Total = ") ? totalComponent.Replace("Total = ", "") : totalComponent;

                return new LogEntry
                {
                    Date = components[0].Split('-')[0],
                    Time = components[0].Split('-')[1],
                    Team = components[1],
                    Category = components[2],
                    Question = components[3],
                    Score = score,
                    Total = total
                };
            }

            return null;
        }

        public void LogDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            dataGrid.Items.SortDescriptions.Clear();

            ListSortDirection newSortDirection = ListSortDirection.Ascending;
            if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
            {
                newSortDirection = ListSortDirection.Ascending;
            }
            else
            {
                newSortDirection = ListSortDirection.Descending;
            }

            // Set sort direction for the clicked column
            e.Column.SortDirection = newSortDirection;

            // Add the primary sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, newSortDirection));

            // Add secondary sort description for "Time"
            if (e.Column.SortMemberPath != "Time")
            {
                dataGrid.Items.SortDescriptions.Add(new SortDescription("Time", newSortDirection));
            }

            // Prevent the built-in sort from sorting again
            e.Handled = true;
        }

        private void Show_logs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
            {
                MediaPlayer_Page._media.Stop();
                ControlCenter.Instance.ClearTimer();
                Questions_Page.Instance.ClearQuestionText();

                if (TriviaPlayer.Instance.TriviaPlayer_Frame.Content is Logs_Display_Page)
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = new MediaPlayer_Page();
                }
                else if (TriviaPlayer.Instance.TriviaPlayer_Frame.Content is MediaPlayer_Page)
                {
                    if (Logs_Display_Page.Instance == null)
                        TriviaPlayer.Instance.TriviaPlayer_Frame.Content = new Logs_Display_Page(LoadLogFile());
                    else
                    {
                        Logs_Display_Page.Instance.LogDataGrid.ItemsSource = LoadLogFile();
                        TriviaPlayer.Instance.TriviaPlayer_Frame.Content = Logs_Display_Page.Instance;
                    }
                }
            }
        }
    }
}
