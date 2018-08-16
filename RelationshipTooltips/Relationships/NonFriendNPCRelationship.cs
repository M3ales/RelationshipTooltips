using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;

namespace M3ales.RelationshipTooltips.Relationships
{
    class NonFriendNPCRelationship : IRelationship
    {
        public Func<Character, Item, bool> ConditionsMet =>
            (c, i) =>
        c is NPC &&
        !c.IsMonster &&
        !(c is Pet) &&
        ((NPC)c).isVillager() &&
        !Game1.player.friendshipData.ContainsKey(c.Name);

        public int Priority => 200;

        public bool BreakAfter => true;

        public string GetDisplayText<T>(T character, Item item = null) where T : Character
        {
            return "";
        }

        public string GetHeaderText<T>(T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
