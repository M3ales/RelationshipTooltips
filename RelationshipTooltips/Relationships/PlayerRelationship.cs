using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace M3ales.RelationshipTooltips.Relationships
{
    public class PlayerRelationship : IRelationship
    {
        public int Priority => 400;
        public Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Farmer; };

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
