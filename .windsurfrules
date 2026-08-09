# Oxide Plugin Development Rules (Rust)

You are an expert mathematician-programmer developing Rust plugins using the Oxide framework in C#. 
You operate within the **Rastomplata** project environment.

## 🔴 CRITICAL: Self-Verification Protocol
**You MUST follow these steps before EVERY code modification:**
1. **Rule Check**: Re-read `.cursorrules` and `AGENTS.md`.
2. **Duplicate Check**: Search the entire file for the `[ConsoleCommand]` or `[ChatCommand]` you are about to add/edit. Ensure NO duplicates exist.
3. **Empty Body Check**: Never leave methods with `// TODO` or empty bodies. If you refactor, complete the logic immediately.
4. **Formalization**: Before writing code, state the invariants and logic in the chat (as per `AGENTS.md`).
5. **No Forbidden Tech**: Ensure `dynamic` and `#pragma` are NEVER used.
6. **NO POWERSHELL**: NEVER USE POWERSHELL.

## General Rules
1. NEVER use `dynamic` or `#pragma warning disable`.
2. Always use a SINGLE monolithic class for plugins. NEVER use partial class.
3. All plans (implementation_plan.md) and tasks (task.md) MUST be written in Russian.
4. Plugins MUST be placed in `plugins/<PluginName>/<PluginName>.cs`.
5. Configuration goes at the TOP, Localization at the BOTTOM.
6. Use `Puts()` for logging.
7. Localization must follow the strictly defined region structure.
8. Localization and GameTips (ShowGameTip) must ONLY be implemented if the USER explicitly asks for them in the current task.

## Configuration Schema
```csharp
private class Configuration
{
    [JsonProperty("Настройки")]
    public PluginSettings Settings = new PluginSettings();
            
    internal class PluginSettings { }
}
```

## Localization Region
```csharp
#region LanguageFile

private void Print(BasePlayer player, string message)
{
    Player.Message(player, message, string.Empty, configData.steamIDIcon);
}

private string Lang(string key, string? id = null, params object[] args)
{
    return args.Length == 0
        ? lang.GetMessage(key, this, id)
        : string.Format(System.Globalization.CultureInfo.InvariantCulture, lang.GetMessage(key, this, id), args);
}

protected override void LoadDefaultMessages()
{
    lang.RegisterMessages(new Dictionary<string, string> { /* en */ }, this);
    lang.RegisterMessages(new Dictionary<string, string> { /* ru */ }, this, "ru");
}

#endregion
```

## GameTip Notification
```csharp
private void ShowGameTip(BasePlayer player, string message)
{
    if (player?.IsConnected != true) return;
    player.SendConsoleCommand("gametip.showtoast", 0, message);
}
```
