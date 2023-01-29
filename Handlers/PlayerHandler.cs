using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Usables.Scp244;
using Exiled.API.Features.DamageHandlers;
using InventorySystem.Items.Usables.Scp330;

namespace CoinflipEffectsPlugin.Handlers
{
    internal sealed class PlayerHandler
    {
        private readonly CoinflipEffectsPlugin plugin;
        private Config config;
        System.Random rnd = new System.Random();
        public List<string> SCPListForFakeTerminationPhonetic { get; set; } = new List<string>()
        {
            "scp 1 7 3",
            "scp 0 4 9",
            "scp 0 9 6",
            "scp 1 0 6",
            "scp 9 3 9"
        };

        public List<string> SCPListForFakeTerminationMsg { get; set; } = new List<string>()
        {
            "SCP-173",
            "SCP-049",
            "SCP-096",
            "SCP-106",
            "SCP-939"
        };

        public PlayerHandler(CoinflipEffectsPlugin plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
        }

        public void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            Timing.CallDelayed(2.5f, () =>
            {
                int determineEvent = rnd.Next(0, 101);
                if (ev.IsTails)
                {
                    if (1 <= determineEvent && determineEvent <= 10)
                    {
                        ev.Player.EnableEffect(config.NegativeEffects.ToList().RandomItem(), 10, false);
                        ev.Player.Broadcast(1, "Random negative effect");
                    }
                    else if (11 <= determineEvent && determineEvent <= 20)
                    {
                        ev.Player.DropItems();
                        ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.None);
                        ev.Player.Broadcast(1, "Turn into SCP049-2");
                    }
                    else if (21 <= determineEvent && determineEvent <= 25)
                    {
                        ev.Player.Hurt(ev.Player.Health - 1);
                        ev.Player.Broadcast(1, "Set HP to 1");
                    }
                    else if (26 <= determineEvent && determineEvent <= 30)
                    {
                        if (Player.Get(Side.Scp).Any())
                        {
                            Player scpPlayer = Player.Get(Side.Scp).Where(p => p.Role.Type != RoleTypeId.Scp079).ToList().RandomItem();
                            ev.Player.Position = scpPlayer.Position;
                            ev.Player.Broadcast(1, "TP to SCP Player");
                        }
                    }
                    else if (31 <= determineEvent && determineEvent <= 40)
                    {
                        switch (rnd.Next(1, 5))
                        {
                            case 1:
                                Cassie.MessageTranslated(
                                    $"{SCPListForFakeTerminationPhonetic[rnd.Next(5)]} SUCCESSFULLY TERMINATED. TERMINATION CAUSE UNSPECIFIED",
                                    $"{SCPListForFakeTerminationMsg[rnd.Next(5)]} pomyślnie zneutralizowany, przyczyna nieznana."
                                    );
                                break;
                            case 2:
                                Cassie.MessageTranslated(
                                    $"{SCPListForFakeTerminationPhonetic[rnd.Next(5)]} CONTAINED SUCCESSFULLY BY CLASS D PERSONNEL",
                                    $"{SCPListForFakeTerminationMsg[rnd.Next(5)]} pomyślnie zabezpieczony przez personel klasy D."
                                    );
                                break;
                            case 3:
                                Cassie.MessageTranslated(
                                    $"{SCPListForFakeTerminationPhonetic[rnd.Next(5)]} CONTAINED SUCCESSFULLY BY SCIENCE PERSONNEL",
                                    $"{SCPListForFakeTerminationMsg[rnd.Next(5)]} pomyślnie zabezpieczony przez personel badawczy."
                                    );
                                break;
                            case 4:
                                Cassie.MessageTranslated(
                                    $"{SCPListForFakeTerminationPhonetic[rnd.Next(5)]} SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM",
                                    $"{SCPListForFakeTerminationMsg[rnd.Next(5)]} pomyślnie zneutralizowany przez automatyczny system bezpieczeństwa."
                                    );
                                break;
                        }
                        ev.Player.Broadcast(1, "Fake SCP Termination message");
                    }
                    else if (41 <= determineEvent && determineEvent <= 50)
                    {
                        ev.Player.Handcuff(ev.Player);
                        Timing.CallDelayed(15f, () =>
                        {
                            if (ev.Player.IsCuffed)
                            {
                                ev.Player.RemoveHandcuffs();
                            }
                        });
                        ev.Player.Broadcast(1, "Handcuffs");
                    }
                    else if (51 <= determineEvent && determineEvent <= 60)
                    {
                        ev.Player.Teleport(Door.Get(DoorType.PrisonDoor));
                        ev.Player.Broadcast(1, "TP to Class D Prison");
                    }
                    else if (61 <= determineEvent && determineEvent <= 65)
                    {
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                        grenade.FuseTime = 0.01f;
                        grenade.SpawnActive(ev.Player.Position + Vector3.up);
                        Cassie.MessageTranslated("Explosion anomalies has been noticed in the facility", "W placówce wykryto dziwne anomalie wybuchowe");
                    }
                    else if (66 <= determineEvent && determineEvent <= 70)
                    {
                        ev.Player.EnableEffect(EffectType.Corroding);
                        ev.Player.Broadcast(1, "TP to Pocket Dimension");
                    }
                    else if (71 <= determineEvent && determineEvent <= 80)
                    {
                        Map.TurnOffAllLights(config.BlackoutTime, ev.Player.Zone);
                        ev.Player.Broadcast(1, "Lights out");
                    }
                    else if (81 <= determineEvent && determineEvent <= 90)
                    {
                        if (!Warhead.IsDetonated)
                        {
                            if (Warhead.IsInProgress)
                            {
                                Warhead.Stop();
                            }
                            else
                            {
                                Warhead.Start();
                                Timing.CallDelayed(15f, () =>
                                {
                                    if (!Warhead.IsDetonated && Warhead.IsInProgress)
                                    {
                                        Warhead.Stop();

                                        foreach (var door in Door.List)
                                        {
                                            door.IsOpen = false;
                                        }
                                        Cassie.MessageTranslated("WARHEAD ERROR", "Sory, nie ten przycisk");
                                    }
                                });
                            }
                        }
                        ev.Player.Broadcast(1, "Fake WARHEAD detonation");
                    }
                    else if (91 <= determineEvent && determineEvent <= 100)
                    {
                        Scp244Pickup scp244 = (Scp244Pickup)Pickup.Create(ItemType.SCP244b);
                        scp244.State = Scp244State.Active;
                        scp244.CurrentSizePercent = 200;
                        scp244.Position = ev.Player.Position;
                        scp244.IsLocked = true;
                        scp244.Spawn();
                        ev.Player.Broadcast(1, "A VASE?!");
                    }
                }
                else
                {
                    if (1 <= determineEvent && determineEvent <= 10)
                    {
                        var keycardRnd = rnd.Next(0, 6);
                        if (0 <= keycardRnd && keycardRnd <= 2)
                        {
                            Pickup.CreateAndSpawn(ItemType.KeycardNTFLieutenant, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        }
                        else if (3 <= keycardRnd && keycardRnd <= 4)
                        {
                            Pickup.CreateAndSpawn(ItemType.KeycardFacilityManager, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        }
                        else if (keycardRnd == 5)
                        {
                            Pickup.CreateAndSpawn(ItemType.KeycardO5, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        }
                        ev.Player.Broadcast(1, "Keycard");
                    }
                    if (11 <= determineEvent && determineEvent <= 20)
                    {
                        ev.Player.MaxHealth *= 1.15f;
                        ev.Player.Heal(ev.Player.MaxHealth);
                        ev.Player.Broadcast(1, "Max HP increased");
                    }
                    else if (21 <= determineEvent && determineEvent <= 25)
                    {
                        Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        ev.Player.Broadcast(1, "Pills");
                    }
                    else if (26 <= determineEvent && determineEvent <= 30)
                    {
                        //DAJ BRON
                        Firearm gun = Firearm.Create((FirearmType)new[] { 1, 2, 3, 5, 7, 8, 9, 10 }.RandomItem());
                        gun.Ammo = gun.MaxAmmo;
                        var ammoType = gun.AmmoType;
                        gun.CreatePickup(ev.Player.Position);
                        ev.Player.AddAmmo(ammoType, gun.MaxAmmo);
                        ev.Player.Broadcast(1, "Brand new gun");
                    }
                    else if (31 <= determineEvent && determineEvent <= 40)
                    {
                        ev.Player.Teleport(Door.Get(DoorType.GateA));
                        ev.Player.Broadcast(1, "TP to Gate A");
                    }
                    else if (41 <= determineEvent && determineEvent <= 50)
                    {
                        ev.Player.EnableEffect(config.PositiveEffects.ToList().RandomItem(), 10, false);
                        ev.Player.Broadcast(1, "Random positive effect");
                    }
                    else if (51 <= determineEvent && determineEvent <= 60)
                    {
                        for(int i = 0; i < 8; i++)
                            Pickup.CreateAndSpawn(ItemType.Adrenaline, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        ev.Player.Broadcast(1, "Steroids");
                    }
                    else if (61 <= determineEvent && determineEvent <= 70)
                    {
                        MicroHid hid = (MicroHid)Item.Create(ItemType.MicroHID);
                        hid.Energy = 100;
                        hid.CreatePickup(ev.Player.Position);
                        ev.Player.Broadcast(1, "A WHAT?!");
                    }
                    else if (71 <= determineEvent && determineEvent <= 80)
                    {
                        Scp330 bag = (Scp330)Item.Create(ItemType.SCP330);
                        IEnumerable<CandyKindID> candiesToAdd = new[] {
                            (CandyKindID)rnd.Next(0,8),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,7),
                            (CandyKindID)rnd.Next(0,8)
                            };
                        bag.AddCandy(candiesToAdd, out var stat);
                        ev.Player.AddItem(bag);
                        ev.Player.Broadcast(1, "Candies!");
                    }
                    else if (81 <= determineEvent && determineEvent <= 90)
                    {
                        Pickup.CreateAndSpawn(ItemType.SCP268, ev.Player.Position, new Quaternion(0, 0, 0, 0));
                        ev.Player.Broadcast(1, "M'lady");
                    }
                    else if (91 <= determineEvent && determineEvent <= 100)
                    {
                        if (Player.Get(Side.Scp).Any())
                        {
                            var scps = Player.Get(Side.Scp).Where(x => x.Role.Type != RoleTypeId.Scp079).ToList();
                            if (scps.Any())
                            {
                                string scpsString = "";
                                foreach (var scp in scps)
                                {
                                    var difference = Math.Abs((int)scp.Zone - (int)ev.Player.Zone);
                                    string distance;
                                    if (ev.Player.Zone != 0)
                                    {
                                        switch (difference)
                                        {
                                            case 0:
                                                distance = "really close";
                                                break;
                                            case 1:
                                                distance = "close";
                                                break;
                                            case 2:
                                                distance = "far";
                                                break;
                                            case 3:
                                            default:
                                                distance = "really far";
                                                break;
                                        }
                                        scpsString += $"You can sense {scp.Role.Name} - {distance} \n";
                                    }
                                    else
                                    {
                                        scpsString = "You only sense death";
                                    }
                                }
                                ev.Player.Broadcast(3, scpsString);
                            }
                        }
                    }
                }
                ev.Player.RemoveHeldItem();
            });
        }
    }
}
