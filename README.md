# MorePlayers

Raises Stonewards' multiplayer cap above the default 4 players. Defaults to 8.

## Install

Install with r2modman. Only the host needs the mod; players who join without it can still play, but their hub player count still shows "/ 4" and their session screen only lists 4 players.

## Config

`BepInEx/config/com.darkharasho.stonewards.moreplayers.cfg`, or in-game with ModSettings:

- **MaxPlayers** (default 8, range 1–64): most players in a hosted session, including the host. Takes effect the next time you host.

## Notes

The game was balanced for up to 4 players. Enemy and forge scaling keeps growing per player, so big lobbies get hard fast. Expect rough edges.
