using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
namespace M3ales.RelationshipTooltips
{
    public class ModEntry : Mod
    {
        /// <summary>
        /// Config file instance
        /// </summary>
        private ModConfig config;
        /// <summary>
        /// Mod Entry point - used by SMAPI to initialize before gamestart
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>();
            offset = new Point(30, 0);
            padding = new Point(20, 20);
            displayTooltip = config.displayTooltipByDefault;
            displayGiftInfo = config.displayGiftInfo;
            GameEvents.QuarterSecondTick += CheckForNPCUnderMouse;
            GraphicsEvents.OnPostRenderHudEvent += GraphicsEvents_OnPostRenderHudEvent;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
        }
        /// <summary>
        /// Tooltip display offset
        /// </summary>
        private Point offset;
        /// <summary>
        /// Text padding for Tooltip
        /// </summary>
        private Point padding;
        /// <summary>
        /// Whether the Relationship Tooltip should be displayed or not
        /// </summary>
        private bool displayTooltip;
        /// <summary>
        /// Whether to display gifting information
        /// </summary>
        private bool displayGiftInfo;
        /// <summary>
        /// The NPC currently under the mouse
        /// </summary>
        private NPC selectedNPC;
        /// <summary>
        /// Not the most elegant solution, but is a value to check against that isn't used by npc_taste
        /// </summary>
        private const int NPCGiftOpinionNull = -1;
        /// <summary>
        /// The NPC's taste value towards an item, Null if = NPCGiftOpinionNull
        /// </summary>
        private int selectedNPCGiftOpinion;
        /// <summary>
        /// Used for preventing log spam
        /// </summary>
        private bool firstHoverTick = false;
        private void CheckForNPCUnderMouse(object sender, EventArgs e)
        {
            if (Game1.gameMode == Game1.playingGameMode && Game1.player != null && Game1.player.currentLocation != null)
            {
                if (NPCUtils.TryGetNPCUnderCursor(out selectedNPC))
                {
                    if (firstHoverTick) {
                        Monitor.Log(String.Format("NPC '{0}' under cursor.", selectedNPC.Name));
                    }
                    if(displayGiftInfo && Game1.player.CurrentItem != null && Game1.player.CurrentItem.canBeGivenAsGift())
                    {
                        selectedNPCGiftOpinion = selectedNPC.getGiftTasteForThisItem(Game1.player.CurrentItem);
                        if (firstHoverTick)
                        {
                            Monitor.Log(String.Format("Gift '{0}' in player's hands.", Game1.player.CurrentItem.Name));
                        }
                    }else
                    {
                        selectedNPCGiftOpinion = NPCGiftOpinionNull;
                    }
                    firstHoverTick = false;
                }
                else
                {
                    firstHoverTick = true;
                }
            }
        }
        /// <summary>
        /// Handles Keyboard Input to Toggle Display of Tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (e.Button.TryGetStardewInput(out InputButton k))
            {
                if (k.key == config.toggleDisplayKey)
                {
                    displayTooltip = !displayTooltip;
                }
            }
        }
        /// <summary>
        /// The flavour text that is displayed below an NPC's name on the Tooltip.
        /// </summary>
        /// <param name="level">The current Heart Level of the relationship</param>
        /// <param name="maxLevel">The max Heart Level of the relationship (default 10, 12 for spouses)</param>
        /// <param name="amount">The number of friendship 'points'</param>
        /// <returns>A formatted string containing the relevant Relationship data</returns>
        private string GetHeartString(int level, int maxLevel, int amount)
        {
            string flavourText = "null";
            if (selectedNPC == null)
                return "null";
            FriendshipStatus status = Game1.player.friendshipData[selectedNPC.name].Status;
            switch (status)
            {
                case FriendshipStatus.Dating:
                    {
                        flavourText = (selectedNPC.Gender == 0 ? config.datingMale : config.datingFemale) + ": ";
                        break;
                    }
                case FriendshipStatus.Married:
                    {
                        flavourText = (selectedNPC.Gender == 0 ? config.marriedMale : config.marriedFemale) + ": ";
                        break;
                    }
                case FriendshipStatus.Engaged:
                    {
                        flavourText = (selectedNPC.Gender == 0 ? config.engagedMale : config.engagedFemale) + ": ";
                        break;
                    }
                case FriendshipStatus.Divorced:
                    {
                        flavourText = (selectedNPC.Gender == 0 ? config.divorcedMale : config.engagedFemale) + ": ";
                        break;
                    }
                case FriendshipStatus.Friendly:
                    {
                        switch (level)
                        {
                            case 0:
                            case 1:
                                {
                                    flavourText = config.friendshipAcquaintance + ": ";
                                    break;
                                }
                            case 2:
                            case 3:
                            case 4:
                                {
                                    flavourText = config.friendshipFriend + ": ";
                                    break;
                                }
                            case 5:
                            case 6:
                            case 7:
                                {
                                    flavourText = config.friendshipCloseFriend + ": ";
                                    break;
                                }
                            case 8:
                            case 9:
                            case 10:
                                {
                                    flavourText = config.friendshipBestFriend + ": ";
                                    break;
                                }
                        }
                        break;
                    }
            }
            return flavourText + level + "/" + maxLevel + " (" + amount + ")";
        } 
        /// <summary>
        /// Appends the gift info to a given string
        /// </summary>
        /// <param name="display">The string to append the giftinfo to</param>
        private void AddGiftString(ref string display)
        {
            display += "\n" +config.gift + ": ";
            switch (selectedNPCGiftOpinion)
            {
                case NPC.gift_taste_love:
                    {
                        display += config.giftLoves;
                        break;
                    }
                case NPC.gift_taste_like:
                    {
                        display += config.giftLikes;
                        break;
                    }
                case NPC.gift_taste_neutral:
                    {
                        display += config.giftNeutral;
                        break;
                    }
                case NPC.gift_taste_dislike:
                    {
                        display += config.giftDislikes;
                        break;
                    }
                case NPC.gift_taste_hate:
                    {
                        display += config.giftHates;
                        break;
                    }
                default:
                    {
                        display = "null";
                        break;
                    }
            }
        }
        private void DrawTooltip(int x, int y, string text, string header, bool drawAbove = true, bool centered = false)
        {
            Vector2 headerSize = Game1.dialogueFont.MeasureString(header);
            Vector2 textSize = Game1.smallFont.MeasureString(text);
            Vector2 combined = (textSize + headerSize);
            int boxX = x - (centered ? ((int)combined.X + padding.X * 2) / 2 : 0);
            int boxY = y - (drawAbove ? ((int)combined.Y + padding.Y * 2) : 0);
            IClickableMenu.drawTextureBox(Game1.spriteBatch, boxX, boxY, (int)textSize.X + padding.X * 2, (int)combined.Y + padding.Y, Color.White);
            Utility.drawTextWithShadow(Game1.spriteBatch, header, Game1.dialogueFont, new Vector2(boxX + (padding.X), boxY + padding.Y), Game1.textColor);
            Utility.drawTextWithShadow(Game1.spriteBatch, text, Game1.smallFont, new Vector2(boxX + (padding.X), boxY + (int)headerSize.Y - 10), Game1.textColor);
        }

        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (selectedNPC != null && displayTooltip)
            {
                string npcName = selectedNPC.displayName;
                string display = "\n";
                if (selectedNPC == Game1.player.getSpouse())
                    display += GetHeartString(Game1.player.getFriendshipHeartLevelForNPC(selectedNPC.name), 12, Game1.player.getFriendshipLevelForNPC(selectedNPC.name));
                else
                    display += GetHeartString(Game1.player.getFriendshipHeartLevelForNPC(selectedNPC.name), 10, Game1.player.getFriendshipLevelForNPC(selectedNPC.name));
                if (selectedNPCGiftOpinion != NPCGiftOpinionNull)
                {
                    AddGiftString(ref display);
                }
                DrawTooltip(Game1.getMouseX() + offset.X, Game1.getMouseY() + offset.Y, display, npcName);
            }
        }
    }
}
