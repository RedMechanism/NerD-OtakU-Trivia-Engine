using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DSharpPlus;

namespace NOTE
{
    /// <summary>
    /// The Discord Bot
    /// </summary>
    public partial class DiscordBot
    {
        public Button StartBotButton { get; private set; }
        public Button EnableCommandsButton { get; private set; }

        private DiscordClient discord;
        private bool _isListening;
        private Image listeningIcon;
        private bool _commandsEnabled;
        private dynamic config;
        private bool dIntercepted;
        public DiscordBot(Grid centralGrid)
        {
            _isListening = false;
            CreateStartBotButton(centralGrid);
            CreateEnableCommandsButton(centralGrid);
        }

        private async Task ConnectBot()
        {
            using (var reader = new StreamReader("config.json"))
            {
                var json = await reader.ReadToEndAsync();
                config = JsonSerializer.Deserialize<dynamic>(json);
            }

            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.GetProperty("Token").GetString(),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            // Register the MessageCreated event handler
            discord.MessageCreated += Discord_MessageCreated;

            await discord.ConnectAsync();
        }
        private async Task DisconnectBot()
        {
            // Unregister the MessageCreated event handler
            discord.MessageCreated -= Discord_MessageCreated;

            await discord.DisconnectAsync();
        }

        public async void StartBot()
        {
            await ConnectBot();
            // Change the StartBotButton content to "Stop Bot"
            StartBotButton.Content = "Stop Bot";
            _isListening = true;

            // Enable the EnableCommandsButton
            EnableCommandsButton.IsEnabled = true;
        }
        public async void StopBot()
        {
            await DisconnectBot();
            // Change the StartBotButton content to "Start Bot"
            StartBotButton.Content = "Start Bot";
            _isListening = false;

            // Disable the EnableCommandsButton and reset its state
            EnableCommandsButton.IsEnabled = false;
            EnableCommandsButton.Content = "Listen";
            _commandsEnabled = false;
            if (listeningIcon != null)
            {
                listeningIcon.Visibility = Visibility.Collapsed;
            }
        }

        private List<string> lateDMessages = new List<string>();
        private async Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (!_commandsEnabled) return;

            List<string> users = new List<string>();

            // Define a list of possible response messages
            List<string> responseMessages = new List<string>()
            {
                "{username}, now's your chance",
                "{username}, show 'em how it is done",
                "{username}, it's all you now",
                "{username}, grab these points",
                "{username} got this!"
            };

            List<string> slowMessages = new List<string>()
            {
                "{username}, too late",
                "{username}, too slow",
                "{username}, sorry",
                "{username}, maybe next time",
                "{username} not quick enough"
            };

            if (e.Message.Content.ToLower() == "+r" && TeamShuffler_Page.Instance != null)
            {
                bool isUsernameInNameListBox = false;

                TeamShuffler_Page.Instance.nameListBox.Dispatcher.Invoke(() =>
                {
                    isUsernameInNameListBox = TeamShuffler_Page.Instance.nameListBox.Items.Contains(e.Author.Username);
                });

                if (isUsernameInNameListBox)
                {
                    // Send a Discord reply message to the user
                    await e.Message.RespondAsync($"{e.Author.Username}, you are already registered.");
                }
                else
                {
                    // Add the username to the list and the ListBox control
                    users.Add(e.Author.Username);
                    TeamShuffler_Page.Instance.nameListBox.Dispatcher.Invoke(() =>
                    {
                        TeamShuffler_Page.Instance.nameListBox.Items.Add(e.Author.Username);
                    });

                    // Send a Discord reply message to the user
                    await e.Message.RespondAsync($"{e.Author.Username}, you have been registered.");
                }
            }

            else if (e.Message.Content.ToLower() == "+d")
            {
                Animations Animation = new Animations();
                if (!dIntercepted)
                {
                    // This is not the first "+d"
                    dIntercepted = true;

                    // Store the intercepted message in the InterceptedTextBox
                   TriviaPlayer.Instance.BotD_name.Dispatcher.Invoke(() =>
                    {
                        TriviaPlayer.Instance.BotD_name.Content = $"{e.Author.Username}";
                        // Animations
                        Animation.FadeInOut_Label(TriviaPlayer.Instance.BotD_name, 2, 2);
                        Animation.FadeInOut_Image(TriviaPlayer.Instance.BotD_icon, 2, 2);
                    });

                    // Send a random response message to the Discord server
                    string responseMessage = responseMessages[new Random().Next(0, responseMessages.Count)];
                    responseMessage = responseMessage.Replace("{username}", e.Author.Username);
                    await e.Message.RespondAsync(responseMessage);
                }
                else
                {
                    // This is not the first user to send the "+d" command, or it is the first user who sent it but it has already been intercepted
                    // Send a Discord reply message to the user
                    await e.Message.RespondAsync("Too late.");
                }
            }
        }


        // Add a button click event handler for starting or stopping the bot indefinitely
        private async void StartBotButton_Click(object sender, RoutedEventArgs e)
        {
            if (ControlCenter.Instance.PlayerWindowCounter() >= 1)
            {
                if (!_isListening)
                {
                    StartBot();
                }
                else
                {
                    StopBot();
                }
            }
            else
            {
                MessageBox.Show("Launch trivia player first");
            }
        }

        private void InitializeListeningImageControl(string fileName)
        {
            listeningIcon = new Image();
            listeningIcon.Width = 60;
            listeningIcon.Height = 60;
            listeningIcon.Stretch = Stretch.UniformToFill;
            listeningIcon.HorizontalAlignment = HorizontalAlignment.Center;
            listeningIcon.VerticalAlignment = VerticalAlignment.Top;
            listeningIcon.Margin = new Thickness(0, 10, 0, 0);
            listeningIcon.Visibility = Visibility.Collapsed; // Set initial visibility to Collapsed

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(fileName, UriKind.Relative);
            bitmapImage.EndInit();

            listeningIcon.Source = bitmapImage;

            // Create and add a drop shadow effect
            var dropShadow = new DropShadowEffect();
            dropShadow.Color = Colors.Black;
            dropShadow.Direction = 320;
            dropShadow.ShadowDepth = 5;
            dropShadow.BlurRadius = 10;
            listeningIcon.Effect = dropShadow;

            // Add the Image control to the Grid
            TriviaPlayer.Instance.TriviaPlayerGrid.Children.Add(listeningIcon);
        }


        // Add a button click event handler for enabling or disabling commands
        private void EnableCommandsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_commandsEnabled)
            {
                EnableCommandsButton.Content = "Silence";
                _commandsEnabled = true;
                // Reset the dIntercepted variable when starting the bot
                dIntercepted = false;

                if (listeningIcon == null)
                {
                    InitializeListeningImageControl("Images/DiscordBotListening.png");
                }
                listeningIcon.Visibility = Visibility.Visible;
            }
            else
            {
                EnableCommandsButton.Content = "Listen";
                _commandsEnabled = false;
                listeningIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateStartBotButton(Grid centralGrid)
        {
            // Create a new button
            StartBotButton = new Button();
            StartBotButton.Content = "Start Bot";

            // Set the button's height and width
            StartBotButton.Height = 30;
            StartBotButton.Width = 100;

            // Set the button's margin and padding
            StartBotButton.Margin = new Thickness(0, 0, 150, 11); // (left, top, right and bottom)

            // Set the button's position in the grid
            Grid.SetRow(StartBotButton, 1); // set row to the last row of the grid
            Grid.SetColumn(StartBotButton, 1); // set column to the middle column of the grid
            Grid.SetRowSpan(StartBotButton, 1);
            Grid.SetColumnSpan(StartBotButton, 1);
            StartBotButton.HorizontalAlignment = HorizontalAlignment.Right; // set horizontal alignment to center
            StartBotButton.VerticalAlignment = VerticalAlignment.Bottom; // set vertical alignment to bottom

            // Add the button click event handler
            StartBotButton.Click += StartBotButton_Click;

            // Add the button to the grid
            centralGrid.Children.Add(StartBotButton);
        }

        private void CreateEnableCommandsButton(Grid centralGrid)
        {
            // Create a new button
            EnableCommandsButton = new Button();
            EnableCommandsButton.Content = "Listen";
            EnableCommandsButton.IsEnabled = false;

            // Set the button's height and width
            EnableCommandsButton.Height = 30;
            EnableCommandsButton.Width = 100;

            // Set the button's margin and padding
            EnableCommandsButton.Margin = new Thickness(0, 0, 150, 50); // (left, top, right and bottom)

            // Set the button's position in the grid
            Grid.SetRow(EnableCommandsButton, 1); // set row to the last row of the grid
            Grid.SetColumn(EnableCommandsButton, 0); // set column to the first column of the grid
            Grid.SetRowSpan(EnableCommandsButton, 1);
            Grid.SetColumnSpan(EnableCommandsButton, 1);
            EnableCommandsButton.HorizontalAlignment = HorizontalAlignment.Right; // set horizontal alignment to center
            EnableCommandsButton.VerticalAlignment = VerticalAlignment.Bottom; // set vertical alignment to bottom

            // Add the button click event handler
            EnableCommandsButton.Click += EnableCommandsButton_Click;

            // Add the button to the grid
            centralGrid.Children.Add(EnableCommandsButton);
        }
    }
}
