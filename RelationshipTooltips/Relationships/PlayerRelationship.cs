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
        public bool BreakAfter => false;
        public virtual int Priority => 800;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Farmer && c != Game1.player; };

        public virtual string GetDisplayText<T>(T character, Item item = null) where T : Character
        {
            if (character == Game1.MasterPlayer && character != Game1.player)
                return "The Farm owner.";
            return "Another Farmhand.";
        }

        public virtual string GetHeaderText<T>(T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
