using System.Collections;
using System.Windows.Media;

namespace NOTE
{
    public class Teams
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string LogoPath { get; set; }
        public string SoundPath { get; set; }
        public SolidColorBrush PrimaryColour { get; set; }
        public SolidColorBrush SecondaryColour { get; set; }

        //public Teams(string name, int score, string imagePath, string soundPath, Color primaryColour , Color secondaryColour)
        //{
        //    Name = name;
        //    Score = score;
        //    LogoPath = imagePath;
        //    SoundPath = soundPath;
        //    PrimaryColour = new SolidColorBrush(primaryColour);
        //    SecondaryColour = new SolidColorBrush(secondaryColour);
        //}

        private class SortDescending : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                Teams t1 = (Teams)a;
                Teams t2 = (Teams)b;

                if (t1.Score < t2.Score)
                    return 1;
                if (t1.Score > t2.Score)
                    return -1;
                else
                    return 0;
            }
        }

        private class SortAscending : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                Teams t1 = (Teams)a;
                Teams t2 = (Teams)b;

                if (t1.Score > t2.Score)
                    return 1;
                if (t1.Score < t2.Score)
                    return -1;
                else
                    return 0;
            }
        }

        public static IComparer SortScoreAscending()
        {
            return (IComparer)new SortAscending();
        }

        public static IComparer SortScoreDescending()
        {
            return (IComparer)new SortDescending();
        }
    }
}
