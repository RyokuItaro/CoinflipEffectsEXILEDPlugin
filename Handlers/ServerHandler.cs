using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinflipEffectsPlugin.Handlers
{
    internal sealed class ServerHandler
    {
        private Config config;
        private CoinflipEffectsPlugin plugin;
        public ServerHandler(CoinflipEffectsPlugin plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
        }

        public void OnRoundStarted()
        {
            if (config.GiveEveryoneACoin)
            {
                foreach(var player in Player.List)
                {
                    if(player.Role.Team != Team.SCPs)
                    {
                        player.AddItem(ItemType.Coin, 1);
                    }
                }
            }
        }
    }
}
