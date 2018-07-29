using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3ales.RelationshipTooltips.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
using StardewValley.Menus;
using StardewValley.Characters;

namespace M3ales.RelationshipTooltips
{
    public class ModEntry : Mod
    {
        /// <summary>
        /// Config file instance
        /// </summary>
        private ModConfig config;
        /// <summary>
        /// Per Save Gifting Data - what the player has gifted to who in this save
        /// </summary>
        private GiftSaveInfo giftSaveInfo;
        /// <summary>
        /// Mod Entry point - used by SMAPI to initialize before gamestart
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>();

            if (!config.recordGiftInfo && config.displayGiftInfo && !config.playerKnowsAllGifts)
                config.playerKnowsAllGifts = true;//This must be true given the other checks.
            Helper.WriteConfig<ModConfig>(config);
            offset = new Point(30, 0);
            padding = new Point(20, 20);
            displayTooltip = config.displayTooltipByDefault;
            GameEvents.QuarterSecondTick += CheckForNPCUnderMouse;
            GraphicsEvents.OnPostRenderHudEvent += GraphicsEvents_OnPostRenderHudEvent;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            t = new Tooltip(0, 0, Color.White, anchor: FrameAnchor.BottomLeft);
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            SaveEvents.AfterSave += SaveEvents_AfterSave;
        }
        /// <summary>
        /// Saves the giftSaveInfo if recordGiftInfo is true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            if (config.recordGiftInfo)
            {
                Helper.WriteJsonFile(GiftInfoPath, giftSaveInfo);
                Monitor.Log($"Saved {GiftInfoPath}");
            }
            else
            {
                Monitor.Log("Session data not saved, recordedGiftInfo: " + config.recordGiftInfo);
            }
        }
        private bool CheckForPlayer(out Farmer player)
        {
            player = null;
            if (!Game1.IsMultiplayer || Game1.getOnlineFarmers().Count() < 2)
                return false;
            foreach(Farmer f in Game1.getOnlineFarmers())
            {
                if (f == Game1.player)
                    return false;
                Vector2 tileLoc = f.getTileLocation();
                if(Game1.currentCursorTile == tileLoc || Game1.currentCursorTile == (tileLoc - Vector2.UnitY))
                {
                    player = f;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// The path to the current save's GiftInfo json
        /// </summary>
        private string GiftInfoPath
        {
            get
            {
                return $"{Constants.CurrentSavePath}\\{Constants.SaveFolderName}_RelationshipTooltips_GiftInfo.json";
            }
        }
        /// <summary>
        /// Loads the giftSaveInfo, if recordGiftInfo is true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            if (config.recordGiftInfo)
            {
                giftSaveInfo = this.Helper.ReadJsonFile<GiftSaveInfo>(GiftInfoPath) ?? new GiftSaveInfo();
                Monitor.Log($"Loaded {GiftInfoPath}");
            }
            else
                giftSaveInfo = new GiftSaveInfo();
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
        /// The currently held gift
        /// </summary>
        private Item selectedGift;
        /// <summary>
        /// The currently selected player
        /// </summary>
        private Farmer selectedPlayer;//Todo, change to Character, and merge with selectedNPC
        /// <summary>
        /// If the player has never given the gift, and the feature is enabled then this represents an unknown gift response
        /// </summary>
        private const int NPC_GIFT_OPINION_UNKNOWN = -2;
        private const int NPC_GIFT_OPINION_QUEST_ITEM = -3;
        /// <summary>
        /// The NPC's taste value towards an item, nullable
        /// </summary>
        private int? selectedNPCGiftOpinion;
        /// <summary>
        /// Used for preventing log spam
        /// </summary>
        private bool firstHoverTick = false;
        /// <summary>
        /// If high enough friendship exists, then the player is auto granted knowledge of all applicable gifts
        /// </summary>
        /// <param name="npc">The npc with which the player friendship query will be checked</param>
        /// <returns>True if exists and of high enough level, false otherwise</returns>
        private bool FriendshipKnowsGifts(NPC npc)
        {
            return Game1.player.getFriendshipHeartLevelForNPC(npc.name) >= config.heartLevelToKnowAllGifts;
        }
        /// <summary>
        /// Is the gift being given part of an item delivery quest?
        /// </summary>
        /// <param name="target">NPC to be gifted</param>
        /// <param name="heldItem">Item currently held (must be a gift)</param>
        /// <returns>If the item is part of a quest delivery - true, else false</returns>
        public bool IsItemQuestGift(NPC target, Item heldItem)
        {
            if (target == null || heldItem == null)
                return false;
            foreach (Quest q in Game1.player.questLog)
            {
                if (q.questType == Quest.type_itemDelivery)
                {
                    ItemDeliveryQuest quest = q as ItemDeliveryQuest;
                    if (quest == null)
                        break;
                    if (quest.target?.Value == target.Name && quest.item.Value == heldItem.ParentSheetIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks for NPC under the mouse, if one is found a gift may also be cached if the player is holding an applicable item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForNPCUnderMouse(object sender, EventArgs e)
        {
            if (Game1.gameMode == Game1.playingGameMode && Game1.player != null && Game1.player.currentLocation != null)
            {
                if (CheckForPlayer(out selectedPlayer))
                {
                    selectedNPCGiftOpinion = null;
                    selectedNPC = null;
                    selectedGift = null;
                    if (firstHoverTick)
                    {
                        Monitor.Log(String.Format("Player '{0}' under cursor.", selectedPlayer.Name));
                    }
                }
                else if (NPCUtils.TryGetNPCUnderCursor(out selectedNPC))
                {
                    selectedPlayer = null;
                    if (selectedNPC.IsMonster)
                    {
                        selectedNPC = null;
                        selectedGift = null;
                        selectedNPCGiftOpinion = null;
                        return;
                    }
                    if (firstHoverTick)
                    {
                        Monitor.Log(String.Format("NPC '{0}' under cursor.", selectedNPC.Name));
                    }
                    if (config.displayGiftInfo && Game1.player.CurrentItem != null && Game1.player.CurrentItem.canBeGivenAsGift())
                    {
                        selectedGift = Game1.player.CurrentItem;
                        if (!IsItemQuestGift(selectedNPC, selectedGift))
                        {
                            if (config.playerKnowsAllGifts || (config.recordGiftInfo && giftSaveInfo.PlayerHasGifted(selectedNPC.name, selectedGift.Name)) || (config.recordGiftInfo && FriendshipKnowsGifts(selectedNPC)))
                            {
                                selectedNPCGiftOpinion = selectedNPC.getGiftTasteForThisItem(selectedGift);
                            }
                            else
                            {
                                selectedNPCGiftOpinion = NPC_GIFT_OPINION_UNKNOWN;
                            }
                        }
                        else
                        {
                            selectedNPCGiftOpinion = NPC_GIFT_OPINION_QUEST_ITEM;
                        }
                        if (firstHoverTick)
                        {
                            Monitor.Log(String.Format("Gift '{0}' in player's hands. It's response is {1}. :: giftSaveInfo?{2}, heartlevel? {3}", Game1.player.CurrentItem.Name, selectedNPCGiftOpinion == NPC_GIFT_OPINION_UNKNOWN ? "unknown" : "known", giftSaveInfo.PlayerHasGifted(selectedNPC.name, selectedGift.Name), FriendshipKnowsGifts(selectedNPC)));
                        }
                    }
                    else
                    {
                        selectedNPCGiftOpinion = null;
                        selectedGift = null;
                    }
                    firstHoverTick = false;
                }
                else
                {
                    firstHoverTick = true;
                    selectedGift = null;
                }
            }
        }
        /// <summary>
        /// Handles Keyboard Input to Toggle Display of Tooltip, and the gifting of items (hackish solution, but it works)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (config.recordGiftInfo && (e.Button.IsActionButton() || e.Button.IsUseToolButton()) && selectedNPC != null && selectedGift != null)
            {//not great solution but these are conditions needed to work as far as I can see.
                if (Game1.player.friendshipData.ContainsKey(selectedNPC.name))
                {
                    if (Game1.player.friendshipData[selectedNPC.name].GiftsToday == 0)
                    {
                        Monitor.Log($"Recorded gift '{selectedGift.Name}' to '{selectedNPC.Name}'");
                        giftSaveInfo.AddGift(selectedNPC, selectedGift);
                    }
                }
            }
            else if (e.Button.TryGetStardewInput(out InputButton k))
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
            if (Game1.player.friendshipData.TryGetValue(selectedNPC.name, out Friendship friendship))
            {
                FriendshipStatus status = friendship.Status;
                switch (status)
                {
                    case FriendshipStatus.Dating:
                        {
                            flavourText = config.GetDatingString(selectedNPC.Gender) + ": ";
                            break;
                        }
                    case FriendshipStatus.Married:
                        {
                            flavourText = config.GetMarriageString(selectedNPC.Gender) + ": ";
                            break;
                        }
                    case FriendshipStatus.Engaged:
                        {
                            flavourText = config.GetEngagedString(selectedNPC.Gender) + ": ";
                            break;
                        }
                    case FriendshipStatus.Divorced:
                        {
                            flavourText = config.GetDivorcedString(selectedNPC.Gender) + ": ";
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
            }
            return flavourText + level + "/" + maxLevel + " (" + amount + ")";
        }
        /// <summary>
        /// Appends the gift info to a given string
        /// </summary>
        /// <param name="display">The string to append the giftinfo to</param>
        private void AddGiftString(ref string display)
        {
            display += "\n" + config.gift + ": ";
            switch (selectedNPCGiftOpinion)
            {
                case (int)NPCUtils.GiftResponse.Love:
                    {
                        display += config.giftLoves;
                        break;
                    }
                case (int)NPCUtils.GiftResponse.Like:
                case NPC_GIFT_OPINION_QUEST_ITEM:
                    {
                        display += config.giftLikes;
                        break;
                    }
                case (int)NPCUtils.GiftResponse.Neutral:
                    {
                        display += config.giftNeutral;
                        break;
                    }
                case (int)NPCUtils.GiftResponse.Dislike:
                    {
                        display += config.giftDislikes;
                        break;
                    }
                case (int)NPCUtils.GiftResponse.Hate:
                    {
                        display += config.giftHates;
                        break;
                    }
                case NPC_GIFT_OPINION_UNKNOWN:
                    {
                        display += config.giftUnknown;
                        break;
                    }
                default:
                    {
                        display = $"null({selectedNPCGiftOpinion})";
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
        /// <summary>
        /// The relationship tooltip to display
        /// </summary>
        private Tooltip t;
        /// <summary>
        /// Draws the Tooltip over UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (displayTooltip)
            {
                string npcName = "???";
                string display = "";
                if (selectedPlayer != null)
                {
                    npcName = selectedPlayer.Name;
                    if(selectedPlayer.isMarried())
                    {
                        if(Game1.player.spouse == selectedPlayer.Name)
                        {
                            display = config.GetMarriageString(selectedPlayer.IsMale);
                        }
                    }
                    t.localX = Game1.getMouseX();
                    t.localY = Game1.getMouseY();
                    t.header.text = npcName;
                    t.body.text = display;
                    t.Draw(Game1.spriteBatch, null);
                }
                else if (selectedNPC != null)
                {
                    if (Game1.player.friendshipData.ContainsKey(selectedNPC.name))
                    {
                        npcName = selectedNPC.displayName;
                        int numHearts = 10;//default is 10 for friends
                        if (selectedNPC == Game1.player.getSpouse())
                            numHearts = 12;
                        display += GetHeartString(Game1.player.getFriendshipHeartLevelForNPC(selectedNPC.name), numHearts, Game1.player.getFriendshipLevelForNPC(selectedNPC.name));
                        if (selectedGift != null)
                        {
                            AddGiftString(ref display);
                        }
                    }
                    else if (selectedNPC.Name == Game1.player.getPetName())
                    {
                        // your pet
                        npcName = Game1.player.getPetDisplayName();
                        if (Game1.player.Name == "Darkosto")
                            display = "(No I am not dead)";
                    }
                    else if (selectedNPC is Pet)
                    {
                        npcName = selectedNPC.displayName;
                    }
                    t.localX = Game1.getMouseX();
                    t.localY = Game1.getMouseY();
                    t.header.text = npcName;
                    t.body.text = display;
                    t.Draw(Game1.spriteBatch, null);
                }
            }
        }
    }
}