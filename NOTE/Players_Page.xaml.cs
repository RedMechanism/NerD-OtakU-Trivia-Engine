using System.Windows;
using System.Windows.Controls;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for Players_Page.xaml
    /// </summary>
    public partial class Players_Page : Page
    {
        public Players_Page()
        {
            InitializeComponent();

            foreach (var team in ControlCenter.Instance.TeamsList)
            {
                var teamNode = new TreeViewItem { Header = team.Name };
                playersTreeView.Items.Add(teamNode);

                foreach (var member in team.Members)
                {
                    var memberNode = new TreeViewItem { Header = member.Name };
                    if (member.IsCurator == true)
                    {
                        memberNode.FontWeight = FontWeights.Bold;
                    }

                    memberNode.MouseDoubleClick += (sender, args) =>
                    {
                        member.IsCurator = !member.IsCurator;
                        memberNode.FontWeight = member.IsCurator ? FontWeights.Bold : FontWeights.Normal;
                    };

                    AddMemberContextMenu(memberNode);
                    teamNode.Items.Add(memberNode);
                }
            }
        }

        private void AddMemberContextMenu(TreeViewItem memberNode)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem deleteMenuItem = new MenuItem { Header = "Delete" };
            deleteMenuItem.Click += DeleteMenuItem_Click;
            contextMenu.Items.Add(deleteMenuItem);
            memberNode.ContextMenu = contextMenu;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem) || !(menuItem.Parent is ContextMenu contextMenu) || !(contextMenu.PlacementTarget is TreeViewItem selectedNode))
            {
                return;
            }

            if (selectedNode.Parent is TreeViewItem parentNode) // Member node
            {
                parentNode.Items.Remove(selectedNode);
                int teamIndex = ControlCenter.Instance.TeamsList.FindIndex(team => team.Name == (string)parentNode.Header);
                if (teamIndex >= 0)
                {
                    ControlCenter.Instance.TeamsList[teamIndex].Members.RemoveAll(member => member.Name == (string)selectedNode.Header);
                }
            }
        }

        public void ClearTreeView()
        {
            playersTreeView.Items.Clear();
        }

        private void AddPlayer_Button(object sender, RoutedEventArgs e)
        {
            if (playersTreeView.SelectedItem is TreeViewItem selectedNode)
            {
                PlayerPageInput dialog = new PlayerPageInput();
                if (dialog.ShowDialog() == true)
                {
                    string newMemberName = dialog.PlayerName;
                    bool newMemberIsCurator = dialog.IsCuratorBox;
                    TreeViewItem teamNode;

                    if (selectedNode.Parent is TreeView) // Team node
                    {
                        teamNode = selectedNode;
                    }
                    else if (selectedNode.Parent is TreeViewItem) // Member node
                    {
                        teamNode = (TreeViewItem)selectedNode.Parent;
                    }
                    else
                    {
                        return;
                    }

                    // Add the new member to the treeview
                    TreeViewItem memberNode = new TreeViewItem { Header = newMemberName };
                    if (newMemberIsCurator)
                    {
                        memberNode.FontWeight = FontWeights.Bold;
                    }

                    // Add the context menu and double-click event to the new member node
                    AddMemberContextMenu(memberNode);
                    memberNode.MouseDoubleClick += (snd, args) =>
                    {
                        newMemberIsCurator = !newMemberIsCurator;
                        memberNode.FontWeight = newMemberIsCurator ? FontWeights.Bold : FontWeights.Normal;
                    };

                    teamNode.Items.Add(memberNode);

                    // Find the index of the team in the TeamsList
                    int teamIndex = ControlCenter.Instance.TeamsList.FindIndex(team => team.Name == (string)teamNode.Header);

                    // Add the new member to the corresponding team's Members list
                    if (teamIndex >= 0)
                    {
                        ControlCenter.Instance.TeamsList[teamIndex].Members.Add(
                            new Teams.Member
                            {
                                Name = newMemberName,
                                IsCurator = newMemberIsCurator
                            }
                        );
                    }
                }
            }
        }

        private void ShowTeams_Button(object sender, RoutedEventArgs e)
        {
            if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
            {
                if (TriviaPlayer.Instance.TriviaPlayer_Frame.Content is Teams_Page)
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = new MediaPlayer_Page();
                }
                else
                {
                    TriviaPlayer.Instance.TriviaPlayer_Frame.Content = new Teams_Page();
                }
            }
        }
    }
}
