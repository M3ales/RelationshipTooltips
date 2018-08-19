using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;

namespace RelationshipTooltips.Relationships
{
    public class HorseRelationship : IRelationship
    {
        public Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Monster; };

        public int Priority => throw new NotImplementedException();

        public bool BreakAfter => throw new NotImplementedException();

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
