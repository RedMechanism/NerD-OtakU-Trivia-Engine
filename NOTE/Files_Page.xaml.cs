using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

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
                    ControlCenter.Instance.ClearClock();
                    ControlCenter.Instance.currentTime = ControlCenter.Instance.questionTime;
                    TriviaPlayer.Instance.mediaPlayer.Source = new Uri(dirTree.SelectedItem.ToString());
                    ControlCenter.Instance.MediaPlaying = true;
                    TriviaPlayer.Instance.mediaPlayer.Stop();
                }
            }
        }
    }
}
