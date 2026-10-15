# MorePlayers

Raises Stonewards' multiplayer cap above the default 4 players. Defaults to 8.

## Install

Install with r2modman. Only the host needs the mod; players who join without it can still play, but their hub player count still shows "/ 4" and their session screen only lists 4 players.

## Config

`BepInEx/config/com.darkharasho.stonewards.moreplayers.cfg`, or in-game with ModSettings:

- **MaxPlayers** (default 8, range 1–64): most players in a hosted session, including the host. Takes effect the next time you host.


### Scaling

Everything in `[Scaling]` only kicks in once a run has had more than 4 players, and never makes the game easier than vanilla. The game's own enemy strength and forge cost scaling already keeps growing for every player past 4; these add to it.

- **EnemyCountPerExtraPlayer** (default 0.25): extra enemies per wave for each player beyond 4. 0.25 means 8 players fight twice as many. Single spawns like bosses stay single.
- **ResourceYieldPerExtraPlayer** (default 0.15): extra mining, barrel and enemy drop yield for each player beyond 4. 0.15 means 8 players get 1.6x.
- **CombatDifficultyMultiplier** (default 1): multiplies the game's enemy strength scaling. 1 is vanilla.
- **ForgeCostMultiplier** (default 1): multiplies the game's forge cost scaling. 1 is vanilla. Applied in steps of about one player's worth of cost.

All scaling runs on the host, so clients still don't need the mod. Changes apply from the next run.
