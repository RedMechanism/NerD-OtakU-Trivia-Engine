# NerD|OtakU Trivia Engine (NOTE)

A trivia companion application developed for [NerD|OtakU Zambia](https://www.nerdotaku.org/) events.

## How it works
The trivia app is composed of two parts (i) NOTE Control Center and (ii) the N|O Trivia player

### 1. NOTE Control Center
This is the window that opens when the executable is launched. The Control Center manages everything, from selecting what files to display on the N|O Trivia player to making trivia settings. The N|O Trivia player can be opened from this window.

### 2. N|O Trivia player
The function of the player is just to display trivia content for the audience, without any distracting buttons and knobs on screen.

## Discord Functionality
The trivia program contains the Discord bot which can be used for tasks such as registering players or audience feedback. To use the bot:
1. Define the connection token for your Discord bot in the `config.json` file.
2. Run the trivia app and check "Enable Discord bot" in settings under the "Discord Integration" section.