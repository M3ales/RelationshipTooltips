using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;

namespace M3ales.RelationshipTooltips.Relationships
{
    public class PetRelationship : IRelationship
    {
        public virtual int Priority => 300;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Pet; };

        public virtual string GetDisplayText<T>(T character, Item item = null) where T : Character
        {
            return "";
        }

        public virtual string GetHeaderText<T>(T character, Item item = null) where T : Character
        {
            return (character as Pet).displayName;
        }
    }
}
