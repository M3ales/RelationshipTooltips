using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace M3ales.RelationshipTooltips.Relationships
{
    public class VillagerBirthdayRelationship : IRelationship
    {
        public VillagerBirthdayRelationship()
        {

        }
        public VillagerBirthdayRelationship(ModConfig config)
        {
            Config = config;
        }
        protected ModConfig Config { get; private set; }
        public virtual Func<Character, Item, bool> ConditionsMet => CheckConditions;
        private bool CheckConditions(Character c, Item i)
        {
            NPC npc = c as NPC;
            return c != null && npc.isVillager() && Game1.player.friendshipData.ContainsKey(c.Name) && npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth);

        }
        public virtual int Priority => -200;

        public virtual bool BreakAfter => false;

        public virtual string GetDisplayText<T>(T character, Item item = null) where T : Character
        {
            return String.Format(Config.birthdayFormatted, character.displayName);
        }

        public virtual string GetHeaderText<T>(T character, Item item = null) where T : Character
        {
            return "";
        }
    }
}
