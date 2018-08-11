using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips.Relationships
{
    public interface IRelationship
    {
        Func<Character, Item, bool> ConditionsMet { get; }
        string GetHeaderText<T>(T character, Item item = null) where T : Character;
        string GetDisplayText<T>(T character, Item item = null) where T : Character;
    }
}
