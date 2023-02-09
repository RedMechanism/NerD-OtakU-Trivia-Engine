using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NOTE
{
    public class MediaController
    {
        /// <summary>
        /// Controls behavior of a MediaElement control and an Image control
        /// Regulates what file type gets passed to either control
        /// </summary>
        private static MediaElement mediaElement;
        private static Image imageElement;

        public MediaController(MediaElement media, Image image)
        {
            mediaElement = media;
            imageElement = image;
            OnStop();
        }
        static string[] audioExtensions = {
            ".mp3", ".wav", ".wma", ".aac", ".mid", ".midi", ".adts"
        };

        static string[] imageExtensions = {
            ".jpeg", ".jpg", ".png", ".wmf", ".emf", ".tiff", ".gif", ".bmp"
        };

        static string[] videoExtensions = {
            ".mp4", ".avi", ".m4a", ".m4v", ".mov", ".3g2", ".3gp", ".3gp2", ".3gpp", ".asf", ".wmv"
        };

        public static bool IsAudioFile(string pathString)
        {
            return -1 != Array.IndexOf(audioExtensions, System.IO.Path.GetExtension(pathString).ToLowerInvariant());
        }

        public static bool IsImageFile(string pathString)
        {
            return -1 != Array.IndexOf(imageExtensions, System.IO.Path.GetExtension(pathString).ToLowerInvariant());
        }

        public static bool IsVideoFile(string pathString)
        {
            return -1 != Array.IndexOf(videoExtensions, System.IO.Path.GetExtension(pathString).ToLowerInvariant());
        }

        public MediaState Status { get; set; }
        public enum MediaState
        {
            Playing,
            Paused,
            Stopped
        }
        public MediaType Type { get; set; }
        public enum MediaType
        {
            Video,
            Audio,
            Image
        }

        private Uri path;
        public Uri Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    Reset();
                    path = value;
                    mediaElement.Visibility = Visibility.Hidden;
                    imageElement.Visibility = Visibility.Hidden;
                    if (IsAudioFile(Path.ToString()))
                    {
                        Type = MediaType.Audio;
                        mediaElement.Visibility = Visibility.Visible;
                        mediaElement.Source = path;
                        mediaElement.Stop();
                        OnStop();
                    }
                    else if (IsImageFile(Path.ToString()))
                    {
                        Type = MediaType.Image;
                        imageElement.Source = new BitmapImage(path);
                        OnStop();
                    }
                    else if (IsVideoFile(Path.ToString()))
                    {
                        Type = MediaType.Video;
                        mediaElement.Visibility = Visibility.Visible;
                        mediaElement.Source = path;
                        mediaElement.Stop();
                        OnStop();
                    }
                }
            }
        }

        public void Play()
        {
            if (Type == MediaType.Audio || Type == MediaType.Video)
            {
                mediaElement.Play();
                mediaElement.MediaEnded += (o1, p1) =>
                {
                    OnStop();
                };
                OnPlay();
            }
            else
            {
                imageElement.Visibility = Visibility.Visible;
            }
        }

        public void Pause()
        {
            mediaElement.Pause();
            OnPause();
        }

        public void Stop()
        {
            if (Type == MediaType.Audio || Type == MediaType.Video)
            {
                mediaElement.Stop();
            }
            else
            {
                imageElement.Visibility = Visibility.Hidden;
            }
            OnStop();
        }

        public void Reset()
        {
            Stop();
            OnReset();
        }


        private void OnPlay()
        {
            Status = MediaState.Playing;
        }

        private void OnPause()
        {
            Status = MediaState.Paused;
        }

        private void OnStop()
        {
            Status = MediaState.Stopped;
        }
        private void OnReset()
        {
            Status = MediaState.Stopped;
        }
    }
}
