using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace ItemRestrictions
{
    public class PluginIR : RocketPlugin<Config>
    {
        public static PluginIR Instance;
        public string version = "2.0";

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "item_notPermitted", "Item not permitted: {0}" },
                    { "translation_version_dont_edit", "2" }
                };
            }
        }

        protected override void Load()
        {
            PluginIR.Instance = this;

            Rocket.Core.Logging.Logger.LogWarning("Setting up Item Restrictions by LeeIzaZombie. v" + version);
            Rocket.Core.Logging.Logger.LogWarning("--");
            int count = 0; foreach (ushort item in this.Configuration.Instance.Items) { count++; }
            Rocket.Core.Logging.Logger.LogWarning("Black listed items found: " + count);
            Rocket.Core.Logging.Logger.LogWarning("--");
            Rocket.Core.Logging.Logger.LogWarning("Item Restrictions is ready!");
        }

        private void CheckInventory(UnturnedPlayer player)
        {
            foreach (ushort item in this.Configuration.Instance.Items)
            {
                try
                {
                    for (byte page = 0; page < PlayerInventory.PAGES; page++)
                    {
                        byte itemCount = player.Player.inventory.getItemCount(page);
                        for (byte index = 0; index < itemCount; index++)
                        {
                            if (player.Player.inventory.getItem(page, index).Item.ItemID == item)
                            {
                                UnturnedChat.Say(player, Translate("item_notPermitted", UnturnedItems.GetItemAssetById(item).name), Color.red);
                                player.Player.inventory.removeItem(page, index);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        protected override void Unload() { }

        public List<UnturnedPlayer> Players()
        {
            List<UnturnedPlayer> list = new List<UnturnedPlayer>();

            foreach (SteamPlayer sp in Provider.Players)
            {
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(sp);
                list.Add(p);
            }

            return list;
        }

        public void FixedUpdate()
        {
            if (Level.isLoaded && Provider.Players.Count > 0)
                IntegrityCheck();
        }

        DateTime Second = DateTime.Now;
        private void IntegrityCheck()
        {
            if ((DateTime.Now - this.Second).TotalSeconds > Configuration.Instance.CheckInterval)
            {
                if (Provider.Players.Count > 0)
                {
                    foreach (UnturnedPlayer player in Players())
                    {
                        if (!R.Permissions.HasPermission(player, "ir.safe"))
                            CheckInventory(player);
                        else if (player.IsAdmin && !Configuration.Instance.ignoreAdmin && !R.Permissions.HasPermission(player, "ir.safe"))
                            CheckInventory(player);
                    }
                }

                Second = DateTime.Now;
            }
        }
    }

    public class Config : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Item")]
        public List<ushort> Items;

        public bool ignoreAdmin;
        public float CheckInterval = 1.0F;

        public void LoadDefaults()
        {
            this.ignoreAdmin = true;
            this.Items = new List<ushort>()
            {
                519,
                1050
            };
        }
    }
}
