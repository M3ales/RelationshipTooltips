using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RelationshipTooltips.Relationships
{
    public class VillagerNameScreenRelationship : IRelationship, IInputListener
    {
        public VillagerNameScreenRelationship(ModConfig config)
        {
            Display = false;
            Config = config;
        }
        private ModConfig Config;
        public bool Display { get; set; }
        public SButton DisplayKey { get { return Config.toggleDisplayKey; } }
        public Func<Character, Item, bool> ConditionsMet => (c, i) => { return Display && c.GetType() == typeof(NPC) && !((NPC)c).IsMonster; };

        public int Priority => 30000;

        public bool BreakAfter => false;

        public Action<Character, Item, EventArgsInput> ButtonPressed => (c, i, e) => { if (e.Button == DisplayKey) { Display = true; } };

        public Action<Character, Item, EventArgsInput> ButtonReleased => (c, i, e) => { if (e.Button == DisplayKey) { Display = false; } };

        public string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            return "";
        }

        public string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            NPC n = character as NPC;
            if (n == null)
                return "";
            return character.displayName;
        }
    }
}
