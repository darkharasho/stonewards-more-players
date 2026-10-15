## Project Context

A quick BepInEx/Harmony mod for Stonewards that raises the multiplayer player cap above 4. It sets `NetworkManager.maxConnections` from config. That value sizes the Steam lobby, caps Mirror's server and sizes the session settings player list. It also fixes the hub's hardcoded "x / 4" label. Only the host needs it. Packaged for Thunderstore under `darkharasho`.

## Layout

- `src/MorePlayers/Plugin.cs` — config and Harmony bootstrap
- `src/MorePlayers/Patches/MaxPlayersPatches.cs` — all patches
- `scripts/package.sh` builds the Thunderstore zip; `scripts/update-refs.sh` regenerates `lib/refs` for CI
- Release: bump version in `Plugin.cs`, the csproj, `thunderstore/manifest.json` and `CHANGELOG.md`, then push tag `v<version>`
