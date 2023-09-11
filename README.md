# CyberSteel Rapture

This is my entry for the [MechJam IV game jam hosted on itch.io](https://itch.io/jam/mechjam4)!

Run, jump, and shoot your way through a hostile alien environment while completing mission objectives.

Project hosted at: https://github.com/krazkidd/mech-jam-iv

Itch.io page: https://reckless.itch.io/cybersteel-rapture

Game jam submission: https://itch.io/jam/mechjam4/rate/2178643

License: TBD

## Story

You are a space marine sent to an alien planet to collect CyberSteel replicators. You must spare no cost to collect all replicator orbs and return to your ship for exfiltration. Fire at will.

### Backstory

CyberSteel is a self-organizing nanomaterial that can be programmed to replicate sophisticated, near-sentient biomachines. Generally, one needs at a minimum several kilograms of CyberSteel to store the replicator logic for any one kind of biomachine blueprint. This material is usually assembled into "replicator orbs" for ease of transport and use. The replication process requires a substrate specific to the biomachine, though significant effort is put toward engineering universal substrates.

During wartime, replicator orbs are considered quite precious as they allow for planetary scaling of offensive or defensive forces. The replication substrates are the real limiting factor as they usually require off-world collection of natural resources and subsequent manufacturing, although existing biomachines are easily recycled.

### Inspiration

In John Conway's Game of Life, simple rules beget complex patterns. Some of these patterns are classifiable as self-repeating or [self-replicating](https://conwaylife.com/wiki/Replicator) patterns.

Now imagine a world where macro-sized machines can be built at the molecular level through nanobots, but how do you program individual nanobots with limited physical memory? Like DNA, you give them simple patterns to repeat. You might be able to build a biomachine like any natural organism simply by accelerating the "growing" part of their lifetime.

## Features

- Platforming
- Hitscan and projectile weapons
- Robot buddy
- Mech and "Troid" enemy types
- Explosions
- Pickups

## Development

### Prerequisites

- [Godot 4.1.1 C# engine](https://downloads.tuxfamily.org/godotengine/4.1.1/mono/)

Set the `GODOT4` environment variable to the install location of the Godot 4 runtime executable (e.g. `/opt/godot/Godot_v4.1.1-stable_mono_linux.x86_64`).

For more information about this environment variable, see [here](https://github.com/godotengine/godot-csharp-vscode/issues/43#issuecomment-1258321229).

### Godot Editor Recommended Extensions

- [Kanban Tasks - Todo Manager 2](https://godotengine.org/asset-library/asset/1474)

### VS Code Recommended Extensions

- [ms-dotnettools.csharp](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)

### Starting the game (Godot editor)

1. Select **Run Project** in the Godot editor.

### Starting the game (VS Code)

Press **Build** in the Godot editor at least once to populate dependencies. (I'm not sure how else to do this.)

1. In the Run and Debug view, launch **Debug Game**.

### Other

* Debug builds will show line-of-sight raycasts.

* The planet animation was made with the [Pixel Planet Generator](https://deep-fold.itch.io/pixel-planet-generator) and the following inputs:

  * Planet Type: Terran Dry
  * Seed: 2508822703
  * Colors:

    ```Text
    #ff8933
    #e64539
    #ad2f45
    #52333f
    #3d2936
    ```

## Attributions

Special thanks to my spouse for supporting my efforts in this game jam!

Thanks to Ziuz for the story concept!

And thanks very much to the following artists for making their work freely available under open licenses:

- [Emcee Flesher](https://opengameart.org/users/emcee-flesher)

  - [Super Dead Space Gunner Merc Redux: Platform Shmup Hero](https://opengameart.org/content/super-dead-space-gunner-merc-redux-platform-shmup-hero)
  - [Super Dead Gunner: Batch 2](https://opengameart.org/content/super-dead-gunner-batch-2)
  - [Space Merc Redux: Platform Shmup tileset](https://opengameart.org/content/space-merc-redux-platform-shmup-tileset)

- [surt](https://opengameart.org/users/surt)

  - [Space Merc](https://opengameart.org/content/space-merc)

- [clonethirteen](https://itch.io/profile/clonethirteen)

  - [The "GIGA-PACK" Sounds](https://clonethirteen.itch.io/giga-pack)

- [Deep-Fold](https://itch.io/profile/deep-fold)

  - [Pixel Planet Generator](https://deep-fold.itch.io/pixel-planet-generator)

- [ansimuz](https://itch.io/profile/ansimuz)

  - [Sideview Sci-Fi - Patreon Collection](https://ansimuz.itch.io/sideview-sci-fi)

- [Rad Potato](https://rad-potato.itch.io/)

  - [Pixel Perfect: Ultimate Crosshair kit](https://rad-potato.itch.io/pixel-perfect-ultimate-crosshair-kit)
