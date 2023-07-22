using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Crossword_Page.xaml
    /// </summary>
    public partial class Crossword_Page : Page
    {
        private readonly SolidColorBrush background = Brushes.Black;
        private readonly SolidColorBrush textTileColor = Brushes.White;
        public List<Question> Questions { get; set; }


        private List<string> _words = new List<string>();
        private List<string> _order;

        public static Crossword_Page Instance;

        Crossword _board = new Crossword(16, 18);
        Random _rand = new Random();

        public Crossword_Page(List<Question> questions)
        {
            InitializeComponent();
            Instance = this;
            DataContext = this;
            Questions = questions;

            // Dynamically create grid rows and columns
            for (int i = 0; i < _board.N; i++)
            {
                grid1.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < _board.M; j++)
            {
                grid1.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var i = 0; i < _board.N; i++)
            {
                for (var j = 0; j < _board.M; j++)
                {
                    var b = new Button
                    {
                        Background = background,
                        Content = "",
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 20,
                        Foreground = Brushes.Transparent // Set the initial foreground color to Transparent
                    };

                    b.MouseDoubleClick += Button_MouseDoubleClick; // Add the event handler for double-click

                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                    grid1.Children.Add(b);

                }
            }

            blackSquaresLabel.Content = (_board.N * _board.M).ToString();
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (clickedButton.Background != textTileColor) return;

            if (clickedButton.Foreground == Brushes.Transparent)
            {
                RevealWord(clickedButton);
            }
            else
            {
                HideWord(clickedButton);
            }
        }

        private void RevealWord(Button clickedButton)
        {
            int row = Grid.GetRow(clickedButton);
            int col = Grid.GetColumn(clickedButton);
            char letter = clickedButton.Content.ToString()[0];

            // Check horizontal words
            int left = col;
            int right = col;
            while (left > 0 && _board.GetBoard[row, left - 1] != '*' && _board.GetBoard[row, left - 1] != ' ')
            {
                left--;
            }

            while (right < _board.M - 1 && _board.GetBoard[row, right + 1] != '*' && _board.GetBoard[row, right + 1] != ' ')
            {
                right++;
            }

            if (right - left + 1 >= 2)
            {
                for (int j = left; j <= right; j++)
                {
                    Button b = (Button)grid1.Children[row * _board.M + j];
                    if (b.Background == textTileColor)
                    {
                        b.Foreground = Brushes.Black;
                    }
                }
            }

            // Check vertical words
            int top = row;
            int bottom = row;
            while (top > 0 && _board.GetBoard[top - 1, col] != '*' && _board.GetBoard[top - 1, col] != ' ')
            {
                top--;
            }

            while (bottom < _board.N - 1 && _board.GetBoard[bottom + 1, col] != '*' && _board.GetBoard[bottom + 1, col] != ' ')
            {
                bottom++;
            }

            if (bottom - top + 1 >= 2)
            {
                for (int i = top; i <= bottom; i++)
                {
                    Button b = (Button)grid1.Children[i * _board.M + col];
                    if (b.Background == textTileColor)
                    {
                        b.Foreground = Brushes.Black;
                    }
                }
            }
        }

        private void HideWord(Button clickedButton)
        {
            int row = Grid.GetRow(clickedButton);
            int col = Grid.GetColumn(clickedButton);
            char letter = clickedButton.Content.ToString()[0];

            // Check horizontal words
            int left = col;
            int right = col;
            while (left > 0 && _board.GetBoard[row, left - 1] != '*' && _board.GetBoard[row, left - 1] != ' ')
            {
                left--;
            }

            while (right < _board.M - 1 && _board.GetBoard[row, right + 1] != '*' && _board.GetBoard[row, right + 1] != ' ')
            {
                right++;
            }

            if (right - left + 1 >= 2)
            {
                for (int j = left; j <= right; j++)
                {
                    Button b = (Button)grid1.Children[row * _board.M + j];
                    if (b.Background == textTileColor)
                    {
                        b.Foreground = Brushes.Transparent;
                    }
                }
            }

            // Check vertical words
            int top = row;
            int bottom = row;
            while (top > 0 && _board.GetBoard[top - 1, col] != '*' && _board.GetBoard[top - 1, col] != ' ')
            {
                top--;
            }

            while (bottom < _board.N - 1 && _board.GetBoard[bottom + 1, col] != '*' && _board.GetBoard[bottom + 1, col] != ' ')
            {
                bottom++;
            }

            if (bottom - top + 1 >= 2)
            {
                for (int i = top; i <= bottom; i++)
                {
                    Button b = (Button)grid1.Children[i * _board.M + col];
                    if (b.Background == textTileColor)
                    {
                        b.Foreground = Brushes.Transparent;
                    }
                }
            }
        }

        static int Comparer(string a, string b)
        {
            var temp = a.Length.CompareTo(b.Length);
            return temp == 0 ? a.CompareTo(b) : temp;
        }

        public void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            _words = Questions.Select(wc => wc.QuestionText).ToList();
            _words.Sort(Comparer);
            _words.Reverse();
            _order = _words;
            GenCrossword();
        }

        void GenCrossword()
        {
            horizontalWordsListView.Items.Clear();
            verticalWordsListView.Items.Clear();
            _board.Reset();
            ClearBoard();

            foreach (var word in _order)
            {
                //var wordToInsert = ((bool)RTLRadioButton.IsChecked) ? word.Reverse().Aggregate("",(x,y) => x + y) : word;

                switch (_board.AddWord(word))
                {
                    case 0:
                        horizontalWordsListView.Items.Add(word);
                        break;
                    case 1:
                        verticalWordsListView.Items.Add(word);
                        break;

                }
            }

            ActualizeData();
        }

        void ActualizeData()
        {

            var count = _board.N * _board.M;

            var board = _board.GetBoard;
            var p = 0;

            for (var i = 0; i < _board.N; i++)
            {
                for (var j = 0; j < _board.M; j++)
                {
                    var letter = board[i, j] == '*' ? ' ' : board[i, j];
                    if (letter != ' ') count--;
                    ((Button)grid1.Children[p]).Content = letter.ToString();
                    ((Button)grid1.Children[p]).Background = letter != ' ' ? textTileColor : background;
                    ((Button)grid1.Children[p]).Foreground = letter != ' ' ? Brushes.Transparent : background;
                    p++;
                }
            }

            blackSquaresLabel.Content = count.ToString();
        }

        void ClearBoard()
        {
            var p = 0;
            for (var i = 0; i < _board.N; i++)
            {
                for (var j = 0; j < _board.M; j++)
                {
                    ((Button)grid1.Children[p]).Content = "";
                    ((Button)grid1.Children[p]).Background = background;
                    p++;
                }
            }
            blackSquaresLabel.Content = (_board.N * _board.M).ToString();
        }

    }
}
