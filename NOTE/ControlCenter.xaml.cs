using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace NOTE
{
    public partial class ControlCenter : Window
    {
        private bool PlayerRunning = false;
        public ControlCenter()
        {
            InitializeComponent();
        }
        private void LaunchPlayer_Button(object sender, RoutedEventArgs e)
        {
            if (PlayerRunning)
            {
                MessageBox.Show("An instance of the trivia player is already running");
            }
            else
            {
                TriviaPlayer player = new TriviaPlayer();
                player.Show();
                PlayerRunning = true;
            }
        }
        private void Play_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    TriviaPlayer.Instance.mediaPlayer.Source = new Uri(dirTree.SelectedItem.ToString());
                    TriviaPlayer.Instance.mediaPlayer.Play();
                }
            }
        }
        private void Reset_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    TriviaPlayer.Instance.mediaPlayer.Stop();
                    TriviaPlayer.Instance.mediaPlayer.Play();
                }
            }
        }
        private void Pause_Button(object sender, RoutedEventArgs e)
        {
            if (File.Exists(dirTree.SelectedItem?.ToString()))
            {
                if (PlayerRunning)
                {
                    TriviaPlayer.Instance.mediaPlayer.Pause();
                }
            }
        }

        private object interNode = null; //transfer tree node info between functions
        private void Load_Folder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dirBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (dirBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String pathName = dirBrowser.SelectedPath;

                foreach (string dirPath in Directory.GetDirectories(pathName))
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = dirPath.Substring(dirPath.LastIndexOf("\\") + 1);
                    item.Tag = dirPath;
                    item.Items.Add(interNode);
                    item.Expanded += new RoutedEventHandler(treeExpansion);
                    dirTree.Items.Add(item);
                }
            }
        }
        void treeExpansion(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == interNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subDirItem = new TreeViewItem();
                        //subitem.Header = subDir.Substring(subDir.LastIndexOf("\\") + 1);
                        subDirItem.Header = subDir;
                        subDirItem.Tag = subDir;
                        subDirItem.Items.Add(interNode);
                        subDirItem.Expanded += new RoutedEventHandler(treeExpansion);
                        item.Items.Add(subDir.Substring(subDir.LastIndexOf("\\") + 1));
                    }

                    foreach (string fileName in Directory.GetFiles(item.Tag.ToString()))
                    {
                        TreeViewItem fileItem = new TreeViewItem();
                        // fileItem.Header = fileName.Substring(fileName.LastIndexOf("\\") + 1)
                        fileItem.Header = fileName;
                        fileItem.Tag = fileName;
                        item.Items.Add(fileName);
                    }

                }
                catch (Exception)
                {
                }
            }
        }
    }

}
