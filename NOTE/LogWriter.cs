using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOTE
{
    internal class LogWriter
    {
        public static string fileName = Directory.GetCurrentDirectory() + $@"\Logs\{GetShortTimestamp(DateTime.Now)}_NOtrivia_logs.txt";
        public static void LogWriterInitialize()
        {
            string currentDirectory = Environment.CurrentDirectory;
            string folderPath = Path.Combine(currentDirectory, "Logs");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!File.Exists(fileName))
            {
                using (StreamWriter writer = File.CreateText(fileName))
                {
                    writer.WriteLine($"Trivia stated at {GetTimestamp(DateTime.Now)}");
                }
            }
            else
            {
                using (StreamWriter writer = File.AppendText(fileName))
                {
                    writer.WriteLine($"Trivia stated at {GetTimestamp(DateTime.Now)}");
                }
            }

        }

        public void WriterCorrect(Teams TeamX, int points)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {
                var selectedItem = Questions_Page.Instance.QuestionGrid.SelectedItem;

                if (selectedItem != null)
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, {((Question)selectedItem).CategoryName}, Q#{((Question)selectedItem).QuestionNumber}, +{points} pts, Total = {TeamX.Score} pts");
                }
                else
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, +{points} pts, Total = {TeamX.Score} pts");
                }
            }
        }
        public void WriterIncorrect(Teams TeamX)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {
                var selectedItem = Questions_Page.Instance.QuestionGrid.SelectedItem;

                if (selectedItem != null)
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, {((Question)selectedItem).CategoryName}, Q#{((Question)selectedItem).QuestionNumber} incorrect answer");
                }
                else
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name} incorrect answer");
                }
            }
        }
        public void WriterDeduct(Teams TeamX, int points)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {
                var selectedItem = Questions_Page.Instance.QuestionGrid.SelectedItem;

                if (selectedItem != null)
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, {((Question)selectedItem).CategoryName}, Q#{((Question)selectedItem).QuestionNumber}, -{points} pts, Total = {TeamX.Score} pts");
                }
                else
                {
                    writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name}, -{points} pts, Total = {TeamX.Score} pts");
                }
            }
        }
        public static void WriterScoreChanged(Teams TeamX, string score)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine($"{GetTimestamp(DateTime.Now)} - {TeamX.Name} score manually changed to {TeamX.Score} points");
                ControlCenter.Instance.Status_disp.Content = $"Set {TeamX.Name} points -> {score}";
            }
        }
        static string GetTimestamp(DateTime value)
        {
            return value.ToString("yy.MM.dd-HH:mm:ss");
        }

        static string GetShortTimestamp(DateTime value)
        {
            return value.ToString("yy.MM.dd");
        }
    }
}