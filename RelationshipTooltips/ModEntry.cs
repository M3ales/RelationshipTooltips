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
        private ModConfig config;
        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>();
            cachedNPCs = new List<NPC>();
            offset = new Point(30, 0);
            padding = new Point(20, 20);
            displayTooltip = config.displayTooltipByDefault;
            GameEvents.QuarterSecondTick += GameEvents_QuaterSecondTick;
            GameEvents.SecondUpdateTick += GameEvents_SecondUpdateTick;
            LocationEvents.LocationsChanged += LocationEvents_LocationsChanged;
            GraphicsEvents.OnPostRenderHudEvent += GraphicsEvents_OnPostRenderHudEvent;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
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

        private void LocationEvents_LocationsChanged(object sender, EventArgsLocationsChanged e)
        {
            cachedNPCs.Clear();
            selectedNPC = null;
        }

        private Point offset;
        private Point padding;
        private bool displayTooltip;
        private IList<NPC> cachedNPCs;
        private NPC selectedNPC;

        private void GameEvents_SecondUpdateTick(object sender, EventArgs e)
        {
            CacheNPCs();
        }

        private void GameEvents_QuaterSecondTick(object sender, EventArgs e)
        {
            if (cachedNPCs.Count > 0 && displayTooltip)
            {
                foreach (NPC n in cachedNPCs)
                {
                    if (Game1.currentCursorTile == n.getTileLocation() || Game1.currentCursorTile == (n.getTileLocation() - Vector2.UnitY))//tile -1y = 1 tile above
                    {
                        selectedNPC = n;
                        break;
                    }
                    else
                        selectedNPC = null;
                }
            }
        }

        private void CacheNPCs()
        {
            if (Game1.gameMode == 3)
            {
                foreach (Character c in Game1.player.currentLocation.getCharacters())
                {
                    if (!c.IsMonster && cachedNPCs.Count((x) => x.name == c.name) == 0 && c is NPC)
                    {
                        cachedNPCs.Add(c as NPC);
                    }
                }
            }
        }

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
