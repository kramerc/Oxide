﻿using System;
using System.Net;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Game.Hurtworld.Libraries.Covalence
{
    /// <summary>
    /// Represents the server hosting the game instance
    /// </summary>
    public class HurtworldServer : IServer
    {
        #region Information

        /// <summary>
        /// Gets/sets the public-facing name of the server
        /// </summary>
        public string Name
        {
            get { return GameManager.Instance.ServerConfig.GameName; }
            set { GameManager.Instance.ServerConfig.GameName = value; }
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
        public ushort Port => (ushort)uLink.MasterServer.port;

        /// <summary>
        /// Gets the version or build number of the server
        /// </summary>
        public string Version => GameManager.Instance.Version.ToString();

        /// <summary>
        /// Gets the network protocol version of the server
        /// </summary>
        public string Protocol => GameManager.PROTOCOL_VERSION.ToString();

        /// <summary>
        /// Gets the total of players currently on the server
        /// </summary>
        public int Players => GameManager.Instance.GetPlayerCount();

        /// <summary>
        /// Gets/sets the maximum players allowed on the server
        /// </summary>
        public int MaxPlayers
        {
            get { return GameManager.Instance.ServerConfig.MaxPlayers; }
            set { GameManager.Instance.ServerConfig.MaxPlayers = value; }
        }

        /// <summary>
        /// Gets/sets the current in-game time on the server
        /// </summary>
        public DateTime Time
        {
            get
            {
                var time = TimeManager.Instance.GetCurrentGameTime();
                return Convert.ToDateTime($"{time.Hour}:{time.Minute}:{Math.Floor(time.Second)}");
            }
            set
            {
                var currentOffset = TimeManager.Instance.GetCurrentGameTime().offset;
                var daysPassed = TimeManager.Instance.GetCurrentGameTime().Day + 1;
                var newOffset = 86400 * daysPassed - currentOffset + value.TimeOfDay.TotalSeconds;
                TimeManager.Instance.InitialTimeOffset += (float)newOffset;
            }
        }

        #endregion

        #region Administration

        /// <summary>
        /// Saves the server and any related information
        /// </summary>
        public void Save() => Command("saveserver");

        #endregion

        #region Chat and Commands

        /// <summary>
        /// Broadcasts a chat message to all users
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(string message)
        {
            ConsoleManager.SendLog($"[Broadcast] {message}");
            ChatManagerServer.Instance.RPC("RelayChat", uLink.RPCMode.Others, message);
        }

        /// <summary>
        /// Runs the specified server command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void Command(string command, params object[] args)
        {
            ConsoleManager.Instance.ExecuteCommand($"{command} {string.Join(" ", Array.ConvertAll(args, x => x.ToString()))}");
        }

        #endregion
    }
}
