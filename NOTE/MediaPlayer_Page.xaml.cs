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

namespace NOTE
{
    /// <summary>
    /// Interaction logic for MediaPlayer_Page.xaml
    /// </summary>
    public partial class MediaPlayer_Page : Page
    {
        public static MediaPlayer_Page Instance;
        public static MediaController _media;
        public Uri _currentMediaPath = null;
        public MediaPlayer_Page()
        {
            InitializeComponent();
            Instance = this;
            _media = new MediaController(Media_player, Image_player);
        }
    }
}
