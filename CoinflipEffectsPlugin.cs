using CoinflipEffectsPlugin.Handlers;
using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinflipEffectsPlugin
{
    public class CoinflipEffectsPlugin : Plugin<Config>
    {
        public static CoinflipEffectsPlugin Singleton;
        public static CoinflipEffectsPlugin Instance => Singleton;
        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private PlayerHandler playerHandler;
        private ServerHandler serverHandler;

        public CoinflipEffectsPlugin()
        {
        }

        public override void OnEnabled()
        {
            Singleton = this;
            playerHandler = new PlayerHandler(this);
            serverHandler = new ServerHandler(this);

            Exiled.Events.Handlers.Player.FlippingCoin += playerHandler.OnFlippingCoin;
            Exiled.Events.Handlers.Server.RoundStarted += serverHandler.OnRoundStarted;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= playerHandler.OnFlippingCoin;
            Exiled.Events.Handlers.Server.RoundStarted -= serverHandler.OnRoundStarted;
            playerHandler = null;
            serverHandler = null;
            Singleton = null;
            base.OnDisabled();
        }
    }
}
