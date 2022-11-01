using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;

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

        public bool playState;
        private void DirTree_SelectedItemChanged(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(Files_Page.Instance.dirTree.SelectedItem?.ToString()))
            {
                if (ControlCenter.Instance.PlayerRunning)
                {
                    playState = false;
                    ControlCenter.Instance.ClearTimer();
                    ControlCenter.Instance._Timer.Reset();
                    TriviaPlayer.Instance.mediaPlayer.Source = new Uri(dirTree.SelectedItem.ToString());
                    if (ControlCenter.Instance.IsImageCheck.IsChecked == true)
                    {
                        TriviaPlayer.Instance.ImagePlayer.Source = new BitmapImage(new Uri(dirTree.SelectedItem.ToString()));
                    }
                    ControlCenter.Instance.MediaPlaying = true;
                    TriviaPlayer.Instance.mediaPlayer.Stop();
                }
            }
        }
    }
}
