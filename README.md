# Sank Sounds
- Define custom join messages, join sounds, and chat sounds.

- If you find any bugs, please report them so I can fix them
- If you have any ideas for this project, I'm more than happy to hear your request.

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/788f605f-f112-45e8-8ccf-904149e447e1" />

# Requirements
> [CounterStrikeSharp](https://docs.cssharp.dev/) installed.

> [CSSharpPatcher](https://github.com/samyycX/CSSharpPatcher) for volume to work.

# Features

1. Custom Join Messages
- Personalized messages triggered when a player joins the server.
- Supports multiple messages that can include the player’s name using {player}.

2. Join Sounds
- Play custom sounds for specific players when they join the server.
- Volume adjustable per sound.

3. Chat Sounds
- Trigger sounds when players type certain words or phrases in chat.
- Multiple triggers can be linked to a single sound.
- Volume adjustable per sound.

# Roadmap
- Implement custom flag-based access for features
- Add player preferences for volume control and toggling sounds on/off
- Prepare a default sound list for the workshop (still in progress)
- Some other stuff that I can't think about rn

# Installation
- Download the [latest release](https://github.com/phara1/advanced-ff-cs2/releases)
- Paste the ```sank_sounds``` folder inside your plugins folder
- Edit the sank_sounds.keno file to edit sounds.

# Config
```json
########################
CUSTOM_JOIN_MESSAGES
########################
msg1: "{player} joined the server"
msg2: "Big boss {player} has arrived!"
msg3: "Everyone welcome {player}!"

########################
JOIN_SOUNDS
########################
76561198328633919 SOUNDEVENTNAME 0.8 msg1

########################
CHAT_SOUNDS
########################
gg;goodgame SOUNDEVENTNAME 0.2
bruh SOUNDEVENTNAME 1.0
```

