using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
            GameEvents.QuarterSecondTick += CheckForNPCUnderMouse;
            GraphicsEvents.OnPostRenderHudEvent += GraphicsEvents_OnPostRenderHudEvent;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
        }

        private bool notifiedLogger = false;
        private void CheckForNPCUnderMouse(object sender, EventArgs e)
        {
            if (Game1.gameMode == Game1.playingGameMode && Game1.player != null && Game1.player.currentLocation != null)
            {
                if (NPCUtils.TryGetNPCUnderCursor(out selectedNPC))
                {
                    if (!notifiedLogger) {
                        Monitor.Log(String.Format("NPC '{0}' under cursor.", selectedNPC.Name));
                        notifiedLogger = true;
                    }
                }else
                {
                    notifiedLogger = false;
                }
            }
        }

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
        /// The NPC currently under the mouse
        /// </summary>
        private NPC selectedNPC;

        /// <summary>
        /// Cached value of an NPC's location
        /// </summary>
        private Vector2 tileLocation;
        /// <summary>
        /// Checks if there is an NPC in the CachedNPCs list, which shares the same tileLocation as the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (selectedNPC != null && displayTooltip)
            {
                string display = selectedNPC.name + "\n";
                if (selectedNPC == Game1.player.getSpouse())
                    display += GetHeartString(Game1.player.getFriendshipHeartLevelForNPC(selectedNPC.name), 12, Game1.player.getFriendshipLevelForNPC(selectedNPC.name));
                else
                    display += GetHeartString(Game1.player.getFriendshipHeartLevelForNPC(selectedNPC.name), 10, Game1.player.getFriendshipLevelForNPC(selectedNPC.name));
                Vector2 size = Game1.smallFont.MeasureString(display);
                int boxX = Game1.getMouseX() + offset.X;
                int boxY = Game1.getMouseY() + offset.Y - ((int)size.Y + padding.Y * 2);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, boxX, boxY, (int)size.X + (padding.X * 2), (int)size.Y + (padding.Y * 2), Color.White);
                Utility.drawTextWithShadow(Game1.spriteBatch, display, Game1.smallFont, new Vector2(boxX + padding.X, boxY + padding.Y), Game1.textColor);
            }
        }
    }
}
