using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    public class ModConfig
    {
        public bool displayTooltipByDefault = true;
        public bool displayGiftInfo = true;
        public Microsoft.Xna.Framework.Input.Keys toggleDisplayKey = Microsoft.Xna.Framework.Input.Keys.F8;
        public string friendshipAcquaintance = "Acquaintance";
        public string friendshipFriend = "Friend";
        public string friendshipCloseFriend = "Close Friend";
        public string friendshipBestFriend = "Best of Friends";
        public string datingMale = "Boyfriend";
        public string datingFemale = "Girlfriend";
        public string engagedMale = "Betrothed";
        public string engagedFemale = "Betrothed";
        public string marriedMale = "Husband";
        public string marriedFemale = "Wife";
        public string divorcedMale = "Ex-Husband";
        public string divorcedFemale = "Ex-Wife";
        public string gift = "Gift";
        public string giftLoves = "Loves";
        public string giftLikes = "Likes";
        public string giftNeutral = "Neutral";
        public string giftDislikes = "Dislikes";
        public string giftHates = "Hates";
        public bool playerKnowsAllGifts = false;
        public string giftUnknown = "???";
    }
}
