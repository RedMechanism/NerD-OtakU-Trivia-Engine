using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NOTE
{
    public class FileBrowser
    {
        private object interNode = null; //transfer tree node info between functions
        public void OpenFolder(TreeView dirTree)
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
                    item.Expanded += new RoutedEventHandler(folderExpansion);
                    dirTree.Items.Add(item);
                }
            }
        }
        private void folderExpansion(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == interNode)
            {
                item.Items.Clear();
                foreach (string subDir in Directory.GetDirectories(item.Tag.ToString()))
                {
                    TreeViewItem subDirItem = new TreeViewItem();
                    subDirItem.Header = subDir;
                    subDirItem.Tag = subDir;
                    subDirItem.Items.Add(interNode);
                    subDirItem.Expanded += new RoutedEventHandler(folderExpansion);
                    item.Items.Add(subDir.Substring(subDir.LastIndexOf("\\") + 1));
                }

                foreach (string fileName in Directory.GetFiles(item.Tag.ToString()))
                {
                    TreeViewItem fileItem = new TreeViewItem();
                    fileItem.Header = fileName;
                    fileItem.Tag = fileName;
                    fileItem.Items.Add(interNode);
                    item.Items.Add(fileName);
                }
            }
        }
        public static string SelectRandomFile(string dir)
        {
            var directoryInfo = new DirectoryInfo(dir).GetFiles("*.*");
            Random random = new Random();
            string randomFile = directoryInfo.ElementAt(random.Next(0, directoryInfo.Length)).FullName;
            return randomFile;
        }
    }
}
