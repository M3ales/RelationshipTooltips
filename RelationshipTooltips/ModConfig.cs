using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    public class ModConfig
    {
        /// <summary>
        /// The starting value of displayTooltip
        /// </summary>
        public bool displayTooltipByDefault = true;
        /// <summary>
        /// 
        /// </summary>
        public bool displayGiftInfo = true;
        /// <summary>
        /// Whether to save/listen for gifting events
        /// </summary>
        public bool recordGiftInfo = true;
        /// <summary>
        /// The key used to toggle the UI display of the Relationship Tooltips tooltip.
        /// </summary>
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
        public string animalPetted = "Is happy to have seen you.";
        public string animalNotPetted = "Needs some love.";
        public string GetAnimalPetString(bool petted)
        {
            return petted ? animalPetted : animalNotPetted;
        }
        /// <summary>
        /// If true, will bypass all checks on if the player knows the gift's response.
        /// </summary>
        public bool playerKnowsAllGifts = false;
        /// <summary>
        /// Text displayed on the tooltip when the gift response is unknown.
        /// </summary>
        public string giftUnknown = "???";
        /// <summary>
        /// The required level of friendship before a player is granted full knowledge of all gift responses.
        /// </summary>
        public int heartLevelToKnowAllGifts = 7;

        public string GetEngagedString(int Gender)
        {
            return GetEngagedString(Gender == NPC.male);
        }
        public string GetEngagedString(bool isMale)
        {
            return isMale ? engagedMale : engagedFemale;
        }
        public string GetDatingString(int Gender)
        {
            return GetDatingString(Gender == NPC.male);
        }
        public string GetDatingString(bool isMale)
        {
            return isMale ? datingMale : datingFemale;
        }
        public string GetMarriageString(int Gender)
        {
            return GetMarriageString(Gender == NPC.male);
        }
        public string GetMarriageString(bool isMale)
        {
            return isMale ? marriedMale : marriedFemale;
        }
        public string GetDivorcedString(int Gender)
        {
            return GetDivorcedString(Gender == NPC.male);
        }
        public string GetDivorcedString(bool isMale)
        {
            return isMale ? divorcedMale : divorcedFemale;
        }
    }
}
