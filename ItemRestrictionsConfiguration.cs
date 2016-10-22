using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LeeIzaZombie.ItemRestrictions
{
    public class ItemRestrictionsConfiguration : IRocketPluginConfiguration
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
