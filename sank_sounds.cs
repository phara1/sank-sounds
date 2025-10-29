using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using System.Reflection;

public class Sank_Sounds : BasePlugin
{
    public override string ModuleName => "Sank Sounds";
    public override string ModuleAuthor => "keno";
    public override string ModuleVersion => "1.0";
    public override string ModuleDescription => "Define custom join messages, join sounds, and chat sounds.";

    private Dictionary<string, string> joinMessages = new();
    private Dictionary<ulong, (string path, float volume, string msgKey)> joinSounds = new();
    private List<(List<string> aliases, string path, float volume)> chatSounds = new();

    public override void Load(bool hotReload)
    {
        AddCommandListener("say", OnCommandSay);
        AddCommandListener("say_team", OnCommandSay);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
        LoadConfig();
    }

    private void LoadConfig()
    {
        string configPath = Path.Combine(Server.GameDirectory, "csgo/addons/counterstrikesharp/plugins/sank_sounds/sank_sounds.keno");

        if (!System.IO.File.Exists(configPath))
        {
            Console.WriteLine($"[Sank Sounds] Config not found at: {configPath}");
            return;
        }

        joinMessages.Clear();
        joinSounds.Clear();
        chatSounds.Clear();

        string[] lines = System.IO.File.ReadAllLines(configPath);
        string currentSection = "";

        foreach (var raw in lines)
        {
            string line = raw.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            if (line.All(c => c == '#'))
                continue;

            if (line.Equals("CUSTOM_JOIN_MESSAGES", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("JOIN_SOUNDS", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("CHAT_SOUNDS", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = line.ToUpper();
                continue;
            }

            switch (currentSection)
            {
                case "CUSTOM_JOIN_MESSAGES":
                    {
                        var parts = line.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string msg = parts[1].Trim().Trim('"')
                                .Replace("/n", "\n")
                                .Replace("\\n", "\n");
                            joinMessages[key] = msg;
                        }
                        break;
                    }

                case "JOIN_SOUNDS":
                    {
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4 &&
                            ulong.TryParse(parts[0], out var steamid) &&
                            float.TryParse(parts[2], out var volume))
                        {
                            string path = parts[1].Trim('"');
                            string msgKey = parts[3];
                            joinSounds[steamid] = (path, volume, msgKey);
                        }
                        break;
                    }

                case "CHAT_SOUNDS":
                    {
                        var parts = line.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 3 && float.TryParse(parts[2], out var volume))
                        {
                            var aliases = parts[0].Split(';').Select(a => a.Trim().ToLower()).ToList();
                            string path = parts[1].Trim('"');
                            chatSounds.Add((aliases, path, volume));
                        }
                        break;
                    }
            }
        }

        Console.WriteLine("█▀▄▀█ █▀▀█ █▀▀▄ █▀▀ 　 █▀▀▄ █░░█ 　 █░█ █▀▀ █▀▀▄ █▀▀█");
        Console.WriteLine("█░▀░█ █▄▄█ █░░█ █▀▀ 　 █▀▀▄ █▄▄█ 　 █▀▄ █▀▀ █░░█ █░░█");
        Console.WriteLine("▀░░░▀ ▀░░▀ ▀▀▀░ ▀▀▀ 　 ▀▀▀░ ▄▄▄█ 　 ▀░▀ ▀▀▀ ▀░░▀ ▀▀▀▀");
        Console.WriteLine($"[Sank Sounds] Loaded {joinSounds.Count} join sounds and {chatSounds.Count} chat sounds.");

    }

    [GameEventHandler]
    public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;

        ulong steamid = player.AuthorizedSteamID?.SteamId64 ?? 0;
        if (joinSounds.TryGetValue(steamid, out var data))
        {
            string msg = joinMessages.TryGetValue(data.msgKey, out var text)
                ? text.Replace("{player}", player.PlayerName)
                : $"{player.PlayerName} joined the server";

            Server.PrintToChatAll($" \x0E• {msg}");
            PlaySoundToAll(data.path, data.volume);
        }

        return HookResult.Continue;
    }

    private HookResult OnCommandSay(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null) return HookResult.Continue;

        string message = commandinfo.ArgString.ToLower();

        foreach (var (aliases, path, volume) in chatSounds)
        {
            if (aliases.Any(a => message.Contains(a)))
            {
                PlaySoundToAll(path, volume);
                break;
            }
        }

        return HookResult.Continue;
    }

    private void PlaySoundToAll(string path, float volume)
    {

        foreach (CCSPlayerController player in GetAllValidPlayers())
        {
            RecipientFilter filter = player;
            player.EmitSound(path, filter, volume);
            Server.PrintToChatAll($"[Sank Sounds] Playing '{path}' at volume {volume}");

        }
    }


    private List<CCSPlayerController> GetAllValidPlayers()
    {
        return Utilities.GetPlayers().Where(IsValidClient).ToList();
    }

    private bool IsValidClient(CCSPlayerController client)
    {
        return client != null &&
               client.IsValid &&
               client.PlayerPawn.IsValid &&
               client.PlayerPawn.Value != null &&
               !client.IsBot;
    }
}
