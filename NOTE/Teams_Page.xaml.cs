using System.Windows.Controls;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Teams_Page.xaml
    /// </summary>
    public partial class Teams_Page : Page
    {
        public Teams_Page()
        {
            InitializeComponent();

            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team1, teamOneListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team2, teamTwoListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team3, teamThreeListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team4, teamFourListBox);

            Team1Header.Text = ControlCenter.Instance.Team1.Name;
            Team2Header.Text = ControlCenter.Instance.Team2.Name;
            Team3Header.Text = ControlCenter.Instance.Team3.Name;
            Team4Header.Text = ControlCenter.Instance.Team4.Name;
        }
    }
}
