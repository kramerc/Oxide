﻿using System;
using System.Net;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using SDG.Unturned;
using Steamworks;

namespace Oxide.Game.Unturned.Libraries.Covalence
{
    /// <summary>
    /// Represents the server hosting the game instance
    /// </summary>
    public class UnturnedServer : IServer
    {
        #region Information

        /// <summary>
        /// Gets/sets the public-facing name of the server
        /// </summary>
        public string Name
        {
            get { return Provider.serverName; }
            set { Provider.serverName = value; }
        }

        private static IPAddress address;

        /// <summary>
        /// Gets the public-facing IP address of the server, if known
        /// </summary>
        public IPAddress Address
        {
            get
            {
                try
                {
                    if (address == null)
                    {
                        var webClient = new WebClient();
                        address = IPAddress.Parse(webClient.DownloadString("http://api.ipify.org"));
                        return address;
                    }
                    return address;
                }
                catch (Exception ex)
                {
                    RemoteLogger.Exception("Couldn't get server IP address", ex);
                    return new IPAddress(0);
                }
            }
        }

        /// <summary>
        /// Gets the public-facing network port of the server, if known
        /// </summary>
        public ushort Port => Provider.port;

        /// <summary>
        /// Gets the version or build number of the server
        /// </summary>
        public string Version => Provider.APP_VERSION;

        /// <summary>
        /// Gets the network protocol version of the server
        /// </summary>
        public string Protocol => Version;

        /// <summary>
        /// Gets the total of players currently on the server
        /// </summary>
        public int Players => Provider.clients.Count;

        /// <summary>
        /// Gets/sets the maximum players allowed on the server
        /// </summary>
        public int MaxPlayers
        {
            get { return Provider.maxPlayers; }
            set { Provider.maxPlayers = (byte)value; }
        }

        /// <summary>
        /// Gets/sets the current in-game time on the server
        /// </summary>
        public DateTime Time
        {
            get { return DateTime.Today.AddSeconds(LightingManager.time * 120); }
            set { LightingManager.time = (uint)(value.Second / 120); }
        }

        #endregion

        #region Administration

        /// <summary>
        /// Saves the server and any related information
        /// </summary>
        public void Save() => SaveManager.save();

        #endregion

        #region Chat and Commands

        /// <summary>
        /// Broadcasts a chat message to all users
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(string message) => ChatManager.sendChat(EChatMode.GLOBAL, message);

        /// <summary>
        /// Runs the specified server command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void Command(string command, params object[] args)
        {
            Commander.execute(CSteamID.Nil, $"{command} {string.Join(" ", Array.ConvertAll(args, x => x.ToString()))}");
        }

        #endregion
    }
}
