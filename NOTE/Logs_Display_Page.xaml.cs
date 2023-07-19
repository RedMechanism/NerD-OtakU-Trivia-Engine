using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Logs_Display_Page.xaml
    /// </summary>
    public partial class Logs_Display_Page : Page
    {
        public Logs_Display_Page(List<Logs_Page.LogEntry> logEntry)
        {
            InitializeComponent();
            LogDataGrid.ItemsSource = logEntry;
        }

        private void LogDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            Logs_Page.Instance.LogDataGrid_Sorting(sender, e);
        }
    }
}
