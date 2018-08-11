using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    /// <summary>
    /// Data class storing configuration for RelationshipTooltips Mod
    /// </summary>
    public class ModConfig
    {
        #region MainConfig
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
        public StardewModdingAPI.SButton toggleDisplayKey = StardewModdingAPI.SButton.F8;
        #endregion
        #region NPCHeartLevel
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
        #endregion
        #region Gift
        /// <summary>
        /// If true, will bypass all checks on if the player knows the gift's response.
        /// </summary>
        public bool playerKnowsAllGifts = false;
        /// <summary>
        /// The required level of friendship before a player is granted full knowledge of all gift responses.
        /// </summary>
        public int heartLevelToKnowAllGifts = 7;
        /// <summary>
        /// Prefix text for gift display.
        /// </summary>
        public string gift = "Gift";
        /// <summary>
        /// Displayed if NPC Loves this gift.
        /// </summary>
        public string giftLoves = "Loves";
        /// <summary>
        /// Displayed if NPC Likes this gift.
        /// </summary>
        public string giftLikes = "Likes";
        /// <summary>
        /// Displayed if NPC is Neutral about the gift.
        /// </summary>
        public string giftNeutral = "Neutral";
        /// <summary>
        /// Displayed if NPC Dislikes this gift.
        /// </summary>
        public string giftDislikes = "Dislikes";
        /// <summary>
        /// Displayed if NPC hates this gift.
        /// </summary>
        public string giftHates = "Hates";
        /// <summary>
        /// Displayed if the gift is a quest item for the NPC.
        /// </summary>
        public string giftQuestItem = "Quest Item";
        /// <summary>
        /// Text displayed on the tooltip when the gift response is unknown.
        /// </summary>
        public string giftUnknown = "???";
        #endregion
        #region Animals
        public string animalPetted = "Is happy to have seen you today.";
        public string animalNotPetted = "Needs some love.";
        public string animalHappiness = "Happiness";
        public string animalFriendship = "Friendship";
        #endregion
        #region HelperGets
        public string GetAnimalPetString(bool petted)
        {
            return petted ? animalPetted : animalNotPetted;
        }
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
        #endregion
    }
}
