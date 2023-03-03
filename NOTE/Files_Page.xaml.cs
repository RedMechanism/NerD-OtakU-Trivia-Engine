using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows;
using System.Linq;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Files_Page.xaml
    /// </summary>
    public partial class Files_Page : Page
    {
        public static Files_Page Instance;
        public Files_Page()
        {
            InitializeComponent();
            Instance = this;
        }
        private void DirTree_SelectedItemChanged(object sender, MouseButtonEventArgs e)
        {
            Questions_Page.Instance.QuestionGrid.SelectedItem = null;

            var playerWindowCount = Application.Current.Windows.OfType<TriviaPlayer>().Count();

            if (File.Exists(dirTree.SelectedItem?.ToString()))
            {
                if (playerWindowCount >= 1)
                {
                    ControlCenter.Instance.ClearTimer();
                    TriviaPlayer._media.Path = new Uri(dirTree.SelectedItem.ToString());
                }
            }
        }

        private void Load_Folder(object sender, RoutedEventArgs e)
        {
            FileBrowser fileBrowser = new FileBrowser();
            fileBrowser.OpenFolder(dirTree);
        }
    }
}
