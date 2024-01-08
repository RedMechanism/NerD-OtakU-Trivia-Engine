using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for MediaPlayer_Page.xaml
    /// </summary>
    public partial class MediaPlayer_Page : Page
    {
        public static MediaPlayer_Page Instance;
        public static MediaController _media;
        private DispatcherTimer timer;
        public MediaPlayer_Page()
        {
            InitializeComponent();
            InitializeSeekBarTimer();
            Instance = this;
            _media = new MediaController(Media_player, Image_player);

            Media_player.MediaOpened += Media_player_MediaOpened;
            ControlCenter.Instance.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            ControlCenter.Instance.SeekerBar.ValueChanged += SeekBar_ValueChanged;
        }

        private void InitializeSeekBarTimer()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
            timer.Start(); // Start the timer
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ControlCenter.Instance.SeekerBar.Value = Media_player.Position.TotalSeconds;
            if (Media_player.NaturalDuration.HasTimeSpan)
            {
                ControlCenter.Instance.SeekerBar.Maximum = Media_player.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void Media_player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Media_player.NaturalDuration.HasTimeSpan)
            {
                ControlCenter.Instance.SeekerBar.Maximum = Media_player.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Media_player != null)
            {
                Media_player.Volume = e.NewValue;
            }
        }

        private void SeekBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Media_player != null)
            {
                Media_player.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }
    }
}
