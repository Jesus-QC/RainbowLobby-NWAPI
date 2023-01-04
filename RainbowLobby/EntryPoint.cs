﻿using System;
using System.Threading.Tasks;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RainbowLobby.Configs;
using UnityEngine;

namespace RainbowLobby;

public class EntryPoint
{
    public const string RainbowLobbyVersion = "1.0.0.0";

    [PluginConfig]
    public static RainbowLobbyConfig Config;

    [PluginEntryPoint("RainbowLobby", RainbowLobbyVersion, "Plugin that allows you to show a hint inside the lobby", "Jesus-QC")]
    private void Init()
    {
        Log.Raw($"Loading RainbowLobby {RainbowLobbyVersion} by Jesus-QC");
        
        EventManager.RegisterEvents(this, this);
    }
    
    [PluginEvent(ServerEventType.WaitingForPlayers)]
    private void OnWaitingForPlayers()
    {
        Task.Run(RunRainbowLobby);
    }

    private static async Task RunRainbowLobby()
    {
        int r = 255, g = 0, b = 0;
        
        Config.Text = Config.Text.Replace("</rainbow>", "</color>");
        
        while (!Round.IsRoundStarted)
        {
            string hex = $"{r:X2}{g:X2}{b:X2}"; // X = Hex, 2 = 2 characters
            string text = Config.Text.Replace("<rainbow>", $"<color=#{hex}>");
            
            foreach (Player player in Player.GetPlayers())
            {
                player.ReceiveHint(text);
            }

            if (r > 0 && b == 0)
            {
                r += 3;
                g += 3;
            }

            if (g > 0 && r == 0)
            {
                g += 3;
                b += 3;
            }

            if (b > 0 && g == 0)
            {
                b += 3;
                r += 3;
            }

            r = Mathf.Clamp(r, 0, 255);
            g = Mathf.Clamp(g, 0, 255);
            b = Mathf.Clamp(b, 0, 255);
            
            await Task.Delay(500);
        }
    }
}
