using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            LoadLogFile();
        }

        private void LoadLogFile()
        {
            var logEntries = new List<LogEntry>();
            var lines = System.IO.File.ReadAllLines(LogWriter.fileName);

            foreach (var line in lines)
            {
                if (line.StartsWith("Trivia")) continue;
                var logEntry = ParseLogEntry(line);
                if (logEntry != null)
                    logEntries.Add(logEntry);
            }

            LogDataGrid.ItemsSource = logEntries;
        }

        private LogEntry ParseLogEntry(string line)
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
    }
}
