## Splitshift Protocol
A 2D puzzle platformer built in **Unity** with **C#**, that's centered aorund a "split-and-merge" mechanic. The player control a slime that can divide into smaller, weaker slimes or merge into larger, stronger ones to navigate different obstacles and solve puzzles. Each Slime size has their own health and moisture stats 

🎮 Play it here: [Splitshift Protocol on itch.io](https://mischief-labs.itch.io/splitshift-protocol)

## Features
- **Split & Merge System —**
   Slimes can split into smaller units or merge into larger ones. Their size affects their stats, abilities, and what combinations are valid. A Large splits into a Medium + Small; two Smalls merge into a Medium, and so on.
- **Multi-slime management —**
  Players can control multiple slimes independently and switch between them with *Tab*. Each has its own health, moisture, and position in the world.
- **Moisture mechanic —**
  Slimes dehydrate over time when they're out of water, which triggers damage when moisture hits zero. Finding water sources is as important as dodging hazards.
- **Emotion Bubble system —**
  The active slime displays a contextual emotion bubbles (hydrating, drying out, idle, warning) so the player always know what's going on even with their inactive slimes.
- **Inventory & Consumables —**
  Player can collect and use jam to restore health mid-run.
- **Scene Transitions & Level Flow —**
- Smooth async scene loading with animated transitions and a progress bar, plus checkpoint saving and respawn handling.
  
## What I Learned
- **Multi-entity player control —**
  Managing a list of active slimes, switching focus between them, and keeping UI (health bars, portraits, emotion bubbles) in sync with whichever is currently active.
- **Component-driven architecture —**
  Keeping `Player`, `Slime`, `PlayerHealth`, `PlayerMoisture`, and `PlayerMovement` as separate, loosely-coupled components that communicate through references rather than inheritance chains.
- **Singleton pattern in practice —**
  `GameManager`, `InventoryManager`, `LevelManager`, and `AudioManager` persist across scenes using `DontDestroyOnLoad`, with `BindUIReferences()` called after each scene load to reconnect UI elements.
- **2D physics & detection —**
  Using OverlapCircleAll for merge range detection and OverlapCircle for ground and water checks, with layer-based collision toggling for invulnerability i-frames.
