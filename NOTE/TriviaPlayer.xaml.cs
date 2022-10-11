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
using System.Windows.Shapes;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for TriviaPlayer.xaml
    /// </summary>
    /// 
    public partial class TriviaPlayer : Window
    {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Create a variable to save the instance of the form
        public static TriviaPlayer Instance;
        #pragma warning restore CS8618
        public MediaElement mediaPlayer;

        public Label displayTimer;
        public Image clock_face;
        public TriviaPlayer()
        {
            InitializeComponent();
            Instance = this;
            mediaPlayer = myMedia;
            displayTimer = Timer_display;
            clock_face = Clock_face_image;
        }
    }
}
