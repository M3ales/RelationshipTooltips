﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    public class EasterEgg : PetRelationship
    {
        public override int Priority => 400;
        public override Func<Character, Item, bool> ConditionsMet => (c, i) => { return base.ConditionsMet(c,i) && c is Cat && c.displayName.Trim() == "drakkensong"; };

        public override string GetDisplayText<T>(T character, Item item = null)
        {
            return "(Not dead)";
        }

        public override string GetHeaderText<T>(T character, Item item = null)
        {
            return base.GetHeaderText(character, item);
        }
    }
}
