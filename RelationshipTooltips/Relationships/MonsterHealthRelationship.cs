using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Monsters;

namespace RelationshipTooltips.Relationships
{
    public class MonsterHealthRelationship : IRelationship
    {
        public Func<Character, Item, bool> ConditionsMet => (c, i) => c.IsMonster && c is Monster && !((Monster)c).IsInvisible;

        public int Priority => -30000;

        public bool BreakAfter => false;

        public string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            Monster m = character as Monster;
            if (m != null)
                return $"{m.Health}/{m.MaxHealth}";
            return "";
        }

        public string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return "";
        }
    }
}
