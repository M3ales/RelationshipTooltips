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
        public virtual int Priority => 200;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Farmer; };

        public virtual string GetDisplayText<T>(T character, Item item = null) where T : Character
        {
            return "Another Farmer.";
        }

        public virtual string GetHeaderText<T>(T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
