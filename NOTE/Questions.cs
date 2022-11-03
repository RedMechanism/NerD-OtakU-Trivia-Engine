using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOTE
{
    public class Questions
    {
        public int Position { get; set; }
        public string QuestionID { get; set; }
        public string Category { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
        public TimeSpan Time { get; set; }
        public string Text { get; set; }
    }
}
