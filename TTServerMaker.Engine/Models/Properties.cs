﻿// <copyright file="Properties.cs" company="TThread">
// Copyright (c) TThread. All rights reserved.
// </copyright>

namespace TTServerMaker.Engine.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using TTServerMaker.Engine.Models.Servers;

/// <summary>
/// Handles the server.properties file.
/// </summary>
public class Properties : ObservableObject, IEnumerable<KeyValuePair<string, string>>, ICollection<string>
{
    private const string Filename = "server.properties";

    /// <summary>
    /// The parent server.
    /// </summary>
    private readonly ServerBase server;

    private Dictionary<string, string> properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="Properties"/> class.
    /// </summary>
    /// <param name="server">The server the properties belong to.</param>
    public Properties(ServerBase server)
    {
        this.server = server;
    }

    /// <inheritdoc/>
    public int Count => this.properties.Count;

    /// <inheritdoc/> // Required for collection
    public bool IsReadOnly => false;

    private string PropertiesFilePath => Path.Combine(this.server.BasicInfo.Folder, Filename);

    /// <summary>
    /// Gives the value of a server property setting, or an empty string if not found.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>Value or an empty string.</returns>
    public string this[string propertyName]
    {
        get
        {
            return this.properties.ContainsKey(propertyName) ? this.properties[propertyName] : string.Empty;
        }

        set
        {
            this.properties[propertyName] = value;
            this.OnPropertyChanged(Binding.IndexerName); // TODO <-- nem tom működik-e
        }
    }

    /// <summary>
    /// Returns the description of the given property. Returns null if not supported.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Retruns the property description.</returns>
    public static string GetDescriptionByName(string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "allow-flight" => "Allows users to use flight on your server while in Survival mode, if they have a mod that provides flight installed.\r\n" +
                                "With allow-flight enabled, griefers will possibly be more common, because it will make their work easier. In Creative mode " +
                                "this has no effect.",
            "allow-nether" => "Allows players to travel to the Nether.",
            "difficulty" => "Defines the difficulty (such as damage dealt by mobs and the way hunger and poison affects players) of the server.",
            "enable-command-block" => "Enables command blocks. Duh",
            "enable-query" => "Enables GameSpy4 protocol server listener. Used to get information about server.",
            "enable-rcon" => "Enables remote access to the server console.",
            "force-gamemode" => "Force players to join in the default game mode.\r\n" +
                                   "Off - Players will join in the gamemode they left in.\r\n" +
                                   "on - Players will always join in the default gamemode.",
            "gamemode" => "Defines the mode of gameplay.",
            "generate-structures" => "Defines whether structures (such as villages) will be generated.\r\n" +
                                   "Note: Dungeons will still generate if this is set to false.",
            "generator-settings" => "The settings used to customize world generation. See Superflat and Customized for possible settings and examples.",
            "hardcore" => "If turned on, server difficulty is ignored and set to hard and players will be set to spectator mode if they die.",
            "level-name" => "The 'level name' value will be used as the world name and its folder name. You may also copy your saved game into " +
                                   "the server folder, and change the name to the same as that folder's to load it instead.",
            "level-seed" => "Add a seed for your world, as in Singleplayer.",
            "level-type" => "Determines the type of map that is generated.",
            "max-build-height" => "The maximum height in which building is allowed. Terrain may still naturally generate above a low height limit.",
            "max-players" => "The maximum number of players that can play on the server at the same time. Note that if more players are on the " +
                                   "server it will use more resources. Note also, op player connections are not supposed to count against the max players.",
            "max-tick-time" => "The maximum number of milliseconds a single tick may take before the server watchdog stops the server with the message, " +
                                "A single server tick took 60.00 seconds (should be max 0.05); Considering it to be crashed, server will forcibly shutdown. " +
                                "Set it to -1 to disable the feature",
            "max-world-size" => "This sets the maximum possible size in blocks, expressed as a radius, that the world border can obtain. Setting the " +
                                   "world border bigger causes the commands to complete successfully but the actual border will not move past this block " +
                                   "limit. Setting the max-world-size higher than the default doesn't appear to do anything.",
            "network-compression-threshold" => "By default it allows packets that are n-1 bytes big to go normally, but a packet that n bytes or more will be compressed " +
                                   "down. So, lower number means more compression but compressing small amounts of bytes might actually end up with a larger " +
                                   "result than what went in.\r\n" +
                                   "- 1 - disable compression entirely\r\n" +
                                   "0 - compress everything",
            "online-mode" => "Server checks connecting players against Minecraft account database. Only set this to false if your server is not connected " +
                                   "to the Internet. Hackers with fake accounts can connect if this is set to false! If minecraft.net is down or inaccessible, " +
                                   "no players will be able to connect if this is set to true. Setting this variable to off purposely is called \"cracking\" " +
                                   "a server, and servers that are presently with online mode off are called \"cracked\" servers, allowing players with " +
                                   "unlicensed copies of Minecraft to join.",
            "op-permission-level" => "Sets the default permission level for ops when using /op",
            "player-idle-timeout" => "If non-zero, players are kicked from the server if they are idle for more than that many minutes.",
            "prevent-proxy-connections" => "If the ISP/AS sent from the server is different from the one from Mojang's authentication server, the player is kicked",
            "pvp" => "Enable PvP on the server. Players shooting themselves with arrows will only receive damage if PvP is enabled.",
            "query.port" => "Sets the port for the query server.",
            "rcon.password" => "Sets the password to rcon",
            "rcon.port" => "Sets the port to rcon.",
            "resource-pack" => "Optional URI to a resource pack. The player may choose to use it.",
            "resource-pack-sha1" => "Optional SHA-1 digest of the resource pack, in lowercase hexadecimal. It's recommended to specify this. " +
                                   "This is not yet used to verify the integrity of the resource pack, but improves the effectiveness and " +
                                   "reliability of caching.",
            "server-port" => "Changes the port the server is hosting (listening) on. This port must be forwarded if the server is " +
                                   "hosted in a network using NAT (If you have a home router/firewall).",
            "snooper-enabled" => "Sets whether the server sends snoop data regularly to http://snoop.minecraft.net.",
            "spawn-animals" => "Determines if animals will be able to spawn.",
            "spawn-monsters" => "Determines if monsters will be spawned.",
            "spawn-npcs" => "Determines whether villagers will be spawned.",
            "spawn-protection" => "Determines the radius of the spawn protection as 2x+1. Setting this to 0 will not disable spawn protection. " +
                                   "0 will protect the single block at the spawn point. 1 will protect a 3x3 area centered on the spawn point. 2 " +
                                   "will protect 5x5, 3 will protect 7x7, etc. This option is not generated on the first server start and appears " +
                                   "when the first player joins. If there are no ops set on the server, the spawn protection will be disabled automatically.",
            "view-distance" => "Sets the amount of world data the server sends the client, measured in chunks in each direction of the player (radius, " +
                                   "not diameter). It determines the server-side viewing distance. (see Render distance)",
            "white-list" => "With a whitelist enabled, users not on the whitelist will be unable to connect. Intended for private servers, such as those for " +
                                "real-life friends or strangers carefully selected via an application process, for example.",
            "enforce-whitelist" => "Enforces the whitelist on the server. When this option is enabled, users who are not present on the whitelist(if it's enabled) " +
                                   "will be kicked from the server after the server reloads the whitelist file.",
            "server-ip" => "This is the ip you will be using. If you are not sure what this means, please leave it empty!",
            "motd" => "This is the message the players see under the server name, when they are in the multiplayer menu inside Minecraft.",
            _ => null,
        };
    }

    /// <summary>
    /// Reads the default server.properties file (from the resources folder), and sets the properties to default.
    /// </summary>
    public async void SetToDefault()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "ServerEngine.Resources.defaultServerProperties.txt";

        this.properties = new Dictionary<string, string>();

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new (stream))
        {
            while (!reader.EndOfStream)
            {
                string row = await reader.ReadLineAsync();
                string[] spl = row.Split('=');

                if (spl.Length > 1)
                {
                    this.properties.Add(spl[0], spl[1]);
                }
            }
        }
    }

    /// <summary>
    /// Reads the .properties file.
    /// </summary>
    public void LoadFromFile()
    {
        // If the file does not exist yet, we load the default values to the properties, and save the file.
        if (!File.Exists(this.PropertiesFilePath))
        {
            this.SetToDefault();
            this.SaveToFile();
            return;
        }

        this.properties = new Dictionary<string, string>();

        using (StreamReader olv = new (this.PropertiesFilePath))
        {
            while (!olv.EndOfStream)
            {
                string[] splitLine = olv.ReadLine().Split(new[] { '=' }, 2);

                if (splitLine.Length > 1)
                {
                    this.properties.Add(splitLine[0], splitLine[1]);
                }
            }
        }
    }

    /// <summary>
    /// Saves to properties to the server.properties file.
    /// </summary>
    public async void SaveToFile()
    {
        using (StreamWriter writer = new (this.PropertiesFilePath))
        {
            foreach (KeyValuePair<string, string> property in this.properties)
            {
                // Removing invalid characters from the property value
                string value = property.Value.Replace("=", string.Empty);

                await writer.WriteLineAsync($"{property.Key}={value}");
            }

            writer.Close();
        }
    }

    /// <summary>
    /// Returns the dictionary containing the properties.
    /// </summary>
    /// <returns>Converts the properties to a dictionary.</returns>
    public Dictionary<string, string> ToDictionary() => this.properties;

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach (var keyValue in this.properties)
        {
            yield return keyValue;
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.properties.GetEnumerator();

    /// <inheritdoc/>
    public void Add(string item) => throw new System.NotSupportedException();

    /// <inheritdoc/>
    public void Clear() => this.properties.Clear();

    /// <inheritdoc/>
    public bool Contains(string item) => this.properties.ContainsValue(item);

    /// <inheritdoc/>
    public void CopyTo(string[] array, int arrayIndex) => this.properties.Values.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(string item) => this.properties.Remove(item);

    /// <inheritdoc/>
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => this.properties.Values.GetEnumerator();
}