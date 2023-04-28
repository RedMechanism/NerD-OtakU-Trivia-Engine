using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static NOTE.Teams;

namespace NOTE
{
    /// <summary>
    /// Interaction logic for TeamShuffler_Page.xaml
    /// </summary>
    public partial class TeamShuffler_Page : Page
    {
        // List of names to display
        private static List<string> names = new List<string>();

        // List of TextBlock elements representing the names
        private List<TextBlock> nameTextBlocks = new List<TextBlock>();

        // Random number generator
        private Random rand = new Random();

        public static TeamShuffler_Page Instance;
        public TeamShuffler_Page()
        {
            InitializeComponent();
            Instance = this;

            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team1, teamOneListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team2, teamTwoListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team3, teamThreeListBox);
            ControlCenter.AddTeamMembersToListBox(ControlCenter.Instance.Team4, teamFourListBox);


            // Add TextBlock elements for each name
            foreach (string name in names)
            {
                TextBlock tb = new TextBlock();
                tb.Text = name;
                tb.FontSize = 24;
                nameCanvas.Children.Add(tb);
                nameTextBlocks.Add(tb);

                // Initialize the shuffle velocity of a name
                tb.Tag = new Point(rand.NextDouble() * 2 - 1, rand.NextDouble() * 2 - 1);

                nameListBox.Items.Add(name);
            }

            // Set up the timer to update the positions of the names every 20 milliseconds
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // 
            Team1Header.Text = ControlCenter.Instance.Team1.Name;
            Team2Header.Text = ControlCenter.Instance.Team2.Name;
            Team3Header.Text = ControlCenter.Instance.Team3.Name;
            Team4Header.Text = ControlCenter.Instance.Team4.Name;

            // Shuffle the names in the nameCanvas
            Shuffle();
        }



        private int currentTeamIndex = 0;
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }

        private void Shuffle()
        {
            // Assign new random positions and velocities to each name
            foreach (TextBlock tb in nameTextBlocks)
            {
                double x = rand.NextDouble() * (nameCanvas.ActualWidth - tb.ActualWidth);
                double y = rand.NextDouble() * (nameCanvas.ActualHeight - tb.ActualHeight);
                tb.SetValue(Canvas.LeftProperty, x);
                tb.SetValue(Canvas.TopProperty, y);
                tb.Tag = new Point(15 * (rand.NextDouble() * 2 - 1), 15 * (rand.NextDouble() * 2 - 1));
            }
        }

        private List<ListBox> teamListboxes => new List<ListBox> { teamOneListBox, teamTwoListBox, teamThreeListBox, teamFourListBox };

        private ListBox GetCurrentTeamListBox()
        {
            switch (currentTeamIndex)
            {
                case 0:
                    return teamOneListBox;
                case 1:
                    return teamTwoListBox;
                case 2:
                    return teamThreeListBox;
                case 3:
                    return teamFourListBox;
                default:
                    throw new InvalidOperationException("Invalid team index");
            }
        }

        private int CountBoldNamesInTeam(ListBox teamListBox)
        {
            return teamListBox.Items
                .Cast<ListBoxItem>()
                .Count(item => item.FontWeight == FontWeights.Bold);
        }

        private bool AreAllTeamsFilledWithBold()
        {
            int minBoldCount = teamListboxes.Min(team => CountBoldNamesInTeam(team));
            return minBoldCount >= 1; // change 1 to the number of desired bold names per team
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBlocks.Count != 0)
            {
                int index = rand.Next(nameTextBlocks.Count);
                TextBlock tb = nameTextBlocks[index];
                bool isBold = tb.FontWeight == FontWeights.Bold;
                string selectedName = tb.Text;

                ListBoxItem? selectedListItem = nameListBox.Items
                    .Cast<object>()
                    .Select(item => nameListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem)
                    .FirstOrDefault(listBoxItem => listBoxItem?.Content?.ToString() == selectedName);

                if (selectedListItem != null && selectedListItem.FontWeight == FontWeights.Bold)
                {
                    isBold = true;
                }

                if (AreAllTeamsFilledWithBold() || !isBold)
                {
                    ListBoxItem newItem = new ListBoxItem { Content = tb.Text, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal, Foreground = isBold ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black) };

                    if (isBold)
                    {
                        newItem.Effect =  ControlCenter.CreateDropShadowEffect();
                    }

                    ListBox targetTeam = GetCurrentTeamListBox();
                    targetTeam.Items.Add(newItem);

                    int targetTeamIndex = teamListboxes.IndexOf(targetTeam);
                    Teams currentTeam = ControlCenter.Instance.TeamsList[targetTeamIndex];
                    currentTeam.Members.Add(new Member { Name = selectedName, IsCurator = false });

                    currentTeamIndex = (currentTeamIndex + 1) % 4;

                    // Only remove the name from the nameCanvas if it has been successfully added to a team
                    nameTextBlocks.RemoveAt(index);
                    nameCanvas.Children.Remove(tb);
                }
                else if (isBold)
                {
                    int minBoldCount = teamListboxes.Min(team => CountBoldNamesInTeam(team));
                    ListBox teamWithLeastBold = teamListboxes.FirstOrDefault(team => CountBoldNamesInTeam(team) == minBoldCount);

                    if (teamWithLeastBold != null)
                    {
                        ListBoxItem newItem = new ListBoxItem { Content = tb.Text, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal, Foreground = isBold ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black) };

                        if (isBold)
                        {
                            newItem.Effect = ControlCenter.CreateDropShadowEffect();
                        }

                        teamWithLeastBold.Items.Add(newItem);

                        int targetTeamIndex = teamListboxes.IndexOf(teamWithLeastBold);
                        Teams currentTeam = ControlCenter.Instance.TeamsList[targetTeamIndex];
                        currentTeam.Members.Add(new Member { Name = selectedName, IsCurator = true });

                        // Only remove the name from the nameCanvas if it has been successfully added to a team
                        nameTextBlocks.RemoveAt(index);
                        nameCanvas.Children.Remove(tb);
                    }
                }

                // Deactivate the remove button when all names have been added to teams
                if (nameTextBlocks.Count == 0)
                {
                    assignButton.IsEnabled = false;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the positions of the names based on their velocities
            foreach (TextBlock tb in nameTextBlocks)
            {
                Point velocity = (Point)tb.Tag;
                double x = (double)tb.GetValue(Canvas.LeftProperty) + velocity.X;
                double y = (double)tb.GetValue(Canvas.TopProperty) + velocity.Y;

                // Check if the name has hit the edge of the box
                if (x < 0 || x + tb.ActualWidth > nameCanvas.ActualWidth)
                {
                    // Reverse the x velocity
                    velocity.X *= -1;
                    tb.Tag = velocity;
                }
                if (y < 0 || y + tb.ActualHeight > nameCanvas.ActualHeight)
                {
                    // Reverse the y velocity
                    velocity.Y *= -1;
                    tb.Tag = velocity;
                }

                tb.SetValue(Canvas.LeftProperty, x);
                tb.SetValue(Canvas.TopProperty, y);
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get the selected ListBoxItem
            if (sender is ListBox listBox && listBox.SelectedItem != null)
            {
                ListBoxItem selectedItem = listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) as ListBoxItem;
                if (selectedItem != null)
                {
                    if (selectedItem.FontWeight == FontWeights.Normal)
                    {
                        selectedItem.FontWeight = FontWeights.Bold;
                        selectedItem.Foreground = new SolidColorBrush(Colors.Purple);
                    }
                    else
                    {
                        selectedItem.FontWeight = FontWeights.Normal;
                        selectedItem.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove names from all team list boxes and return them to the shuffle canvas
            ListBox[] teamListBoxes = { teamOneListBox, teamTwoListBox, teamThreeListBox, teamFourListBox };

            foreach (ListBox teamListBox in teamListBoxes)
            {
                while (teamListBox.Items.Count > 0)
                {
                    ListBoxItem listItem = teamListBox.Items[0] as ListBoxItem;
                    string name = listItem.Content.ToString();
                    teamListBox.Items.RemoveAt(0);

                    // Remove the existing name from the nameListBox
                    var existingItem = nameListBox.Items.Cast<string>().FirstOrDefault(item => item == name);
                    if (existingItem != null)
                    {
                        nameListBox.Items.Remove(existingItem);
                    }

                    TextBlock tb = new TextBlock();
                    tb.Text = name;
                    tb.FontSize = 24;
                    nameCanvas.Children.Add(tb);
                    nameTextBlocks.Add(tb);

                    // Initialize the shuffle velocity of a name
                    tb.Tag = new Point(rand.NextDouble() * 2 - 1, rand.NextDouble() * 2 - 1);

                    nameListBox.Items.Add(name);
                }
            }

            // Iterate over each team and remove all members
            foreach (var team in ControlCenter.Instance.TeamsList)
            {
                team.Members.Clear();
            }

            // Enable the assign button
            assignButton.IsEnabled = true;

            Shuffle();
        }

        private void AddNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddName();
            }
        }

        private void AddName()
        {
            string newName = addNameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(newName) && !names.Contains(newName))
            {
                names.Add(newName);

                TextBlock tb = new TextBlock();
                tb.Text = newName;
                tb.FontSize = 24;
                nameCanvas.Children.Add(tb);
                nameTextBlocks.Add(tb);

                // Initialize the shuffle velocity of a name
                tb.Tag = new Point(rand.NextDouble() * 2 - 1, rand.NextDouble() * 2 - 1);

                Shuffle(); // Shuffle the names immediately after adding a new name

                nameListBox.Items.Add(newName);
            }

            addNameTextBox.Clear();
        }
    }
}
