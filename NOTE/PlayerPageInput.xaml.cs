using System;
using System.Windows;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for PlayerPageInput.xaml
    /// </summary>
    public partial class PlayerPageInput : Window
    {
        public string PlayerName { get; private set; }
        public bool IsCuratorBox { get; private set; }

        public PlayerPageInput()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = playerNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(PlayerName))
            {
                MessageBox.Show("Please enter a valid player name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                IsCuratorBox = isCuratorCheckBox.IsChecked ?? false;
                DialogResult = true;
                Close();
            }
        }
    }

}