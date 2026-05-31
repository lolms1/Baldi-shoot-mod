# Baldi Shoot Mod

Adds a shooting mechanic to Baldi. When Baldi spots the player, he stops,
aims, shows 3 targeting beams, then shoots 3 bullets along the same paths (by default 3 bullets). Bullets slow down whoever they hit,
then bananas spawn under them.

## How it works

Baldi enters a new custom state (`Baldi_ShootState`) where he:
- Stops moving and aims at the player
- Aims at the player with 3 laser sights (visual only)
- Fires 3 bullets that follow the laser paths
- Spawns bananas that make targets slip

The shooting speed scales with Baldi's anger — the angrier he gets, the faster he shoots.

## Mod structure

This mod comes as two separate `.dll` files:

| File | Purpose |
|:---|:---|
| `BaldiShootCore.dll` | The shooting mechanic (state, bullets, lasers, bananas) |
| `BaldiShootTexturePack.dll` | Custom sprites and sounds for Baldi |

You can use just the Core if you want the mechanic without custom visuals.
Other modders can extend the Core without touching the texture pack.

## Install

1. Install [BepInEx](https://github.com/BepInEx/BepInEx) and [MTM101BaldAPI](https://gamebanana.com/mods/383711).
2. Copy both `.dll` files into `BepInEx/plugins/`.
3. (Optional) To disable custom sprites, remove `BaldiShootTexturePack.dll`.

## Compatibility

- Designed to work alongside other Baldi mods.
- Does not replace Baldi's default behavior — adds a new state.
- Other mods can check for `Baldi_ShootState` to add compatibility.

## For modders

Key classes:

| Class | Purpose |
|:---|:---|
| `Baldi_ShootState` | Custom NPC state — the core of the shooting logic |
| `BulletComponent` | Bullet behavior (movement, collision, hit tracking) |
| `BaldiShootingCfg` | All configurable values (timers, speeds, formula) |

To extend: create a new mod that references `BaldiShootCore.dll` and use
`[BepInDependency]` attribute.

## Credits

- [MTM101BaldAPI](https://github.com/benjaminpants/MTM101BMDE) by benjaminpants
- [BaldiTexturePacks](https://github.com/benjaminpants/BaldiTexturePacks) for `SpriteOverlay` component

## Misc.
this is my first ever C# project!!111!!11!!11!!!!1
