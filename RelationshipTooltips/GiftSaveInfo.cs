using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    public class GiftSaveInfo
    {
        public List<NPCGift> giftsMade;
        public GiftSaveInfo()
        {
            giftsMade = new List<NPCGift>();
        }
        public bool PlayerHasGifted(string npcName, string giftName)
        {
            return giftsMade.FirstOrDefault(e => e.NPC == npcName && e.Gift == giftName) != null;
        }
        public class NPCGift
        {
            public string NPC;
            public string Gift;

            public NPCGift(string npc, string gift)
            {
                NPC = npc;
                Gift = gift;
            }
        }
    }
}
