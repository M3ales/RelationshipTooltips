using RelationshipTooltips.Relationships;
using RelationshipTooltips.UI;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using RelationshipTooltips.API;

namespace RelationshipTooltips
{
    public class RelationshipTooltipsMod : Mod
    {
        public RelationshipAPI RelationshipAPI { get; private set; }
        internal ModConfig Config { get; private set; }
        /// <summary>
        /// Ordered list of relationships to check against every quater second tick. Higher priorities are first.
        /// </summary>
        public List<IRelationship> Relationships { get; private set; }
        public override void Entry(IModHelper helper)
        {
            RelationshipAPI = new RelationshipAPI();
            hasInitRelationships = false;
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            displayEnabled = Config.displayTooltipByDefault;
            tooltip = new Tooltip(0, 0, Color.White, anchor: FrameAnchor.BottomLeft);
            Relationships = new List<IRelationship>();//LEAST SPECIFIC GOES LAST IN THIS LIST, IT IS ORDERED BY PRIORITY DESCENDING
            RelationshipAPI.RegisterRelationships += RegisterDefaultRelationships;
            GameEvents.SecondUpdateTick += CheckRelationshipsHaveInit;
            InputEvents.ButtonPressed += (obj, e) => { if (e.Button == Config.toggleDisplayKey) { displayEnabled = !displayEnabled; } };
            GameEvents.QuarterSecondTick += QuaterSecondUpdate;
            GraphicsEvents.OnPostRenderEvent += DrawTooltip;
            helper.WriteConfig(Config);
            Monitor.Log("Entry Complete", LogLevel.Trace);
        }
        /// <summary>
        /// Checks if the init step has been called for relationships, cannot be done in first frame since other mods need to add to API before init.
        /// Likely to be replaced with Bookcase implementation if/when it's included as a dependancy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckRelationshipsHaveInit(object sender, EventArgs e)
        {
            if(!hasInitRelationships)
            {
                InitRelationships();
                hasInitRelationships = true;
            }
        }

        private void RegisterDefaultRelationships(object sender, EventArgsRegisterRelationships e)
        {
            e.Relationships.AddRange(new List<IRelationship>()
            {
                new PlayerRelationship(),
                new EasterEgg(),
                new PetRelationship(),
                new FarmAnimalRelationship(Config),
                new NPCGiftingRelationship(Config, Monitor),
                new NPCRelationship(Config, Monitor),
                new NonFriendNPCRelationship()
            });
            if (Config.displayBirthday)
                e.Relationships.Add(new VillagerBirthdayRelationship(Config));
        }

        public override object GetApi()
        {
            return RelationshipAPI;
        }
        /// <summary>
        /// If the relationship Init step has been called.
        /// </summary>
        private bool hasInitRelationships;
        /// <summary>
        /// Subscribes the stored Relationships to the relevant events
        /// </summary>
        private void InitRelationships()
        {
            //Fire registration event
            Relationships.AddRange(RelationshipAPI.FireRegistrationEvent());
            //Sort by Priority
            Relationships.Sort((x, y) => y.Priority - x.Priority);
            //Log
            Monitor.Log($"API found {Relationships.Count()} registered types.", LogLevel.Info);
            string str = "Types Found:";
            str += String.Format("{0}\t{1,10} :: {2}", Environment.NewLine, "<Priority>", "<Fully Qualified Type>");
            foreach (IRelationship r in Relationships)
            {
                str += String.Format("{0}\t{1,10} :: {2}", Environment.NewLine, r.Priority, r.GetType().ToString());
            }
            Monitor.Log(str);
            //subscribe to events
            foreach (IRelationship r in Relationships)
            {
                if(r is Relationships.IUpdateable)
                {
                    GameEvents.UpdateTick += (obj, args) => { ((Relationships.IUpdateable)r)?.OnTick(selectedCharacter, heldItem); };
                    GameEvents.QuarterSecondTick += (obj, args) => { ((Relationships.IUpdateable)r)?.OnQuaterSecondTick(selectedCharacter, heldItem); };
                }
                if(r is IInputListener)
                {
                    InputEvents.ButtonPressed += (obj, args) => { ((IInputListener)r)?.ButtonPressed(selectedCharacter, heldItem, args); };
                    InputEvents.ButtonReleased += (obj, args) => { ((IInputListener)r)?.ButtonReleased(selectedCharacter, heldItem, args); };
                }
                if(r is IPerSaveSerializable)
                {
                    SaveEvents.AfterSave += (obj, args) => { ((IPerSaveSerializable)r)?.SaveData(Helper); };
                    SaveEvents.AfterLoad += (obj, args) => { ((IPerSaveSerializable)r)?.LoadData(Helper); };
                }
            }
            Monitor.Log("Relationship Event Subscription Complete.");
        }
        private IEnumerable<Character> GetLocationCharacters(GameLocation location, Event currentEvent = null)
        {
            if (location is AnimalHouse)
            {
                return (location as AnimalHouse).animals.Values
                    .Cast<Character>()
                    .Union(location.getCharacters())
                    .Union(location.getFarmers());
            }
            if (currentEvent != null && Config.displayTooltipDuringEvent)
            {
                return currentEvent.actors
                    .Cast<Character>()
                    .Union(currentEvent.farmerActors.Cast<Character>());
            }
            return location.getCharacters()
                    .Cast<Character>()
                    .Union(location.getFarmers());
        }
        /// <summary>
        /// Attempts to get a Character under the mouse, allows for more specific filtering via specification of T other than Character.
        /// </summary>
        /// <typeparam name="T">A more specific type to filter for, default to Character if you don't want a specific derived Type.</typeparam>
        /// <param name="output">The found Character or null</param>
        /// <returns>If there is a Character found under the mouse.</returns>
        private bool TryGetAtMouse<T>(out T output) where T : Character
        {
            output = null;
            if (Game1.currentLocation == null)
                return false;
            IEnumerable<Character> locationCharacters = GetLocationCharacters(Game1.currentLocation, Game1.CurrentEvent);
            foreach (Character c in locationCharacters)
            {
                if (c == null || c == Game1.player)
                    continue;
                if (c.getTileLocation() == Game1.currentCursorTile || c.getTileLocation() - Vector2.UnitY == Game1.currentCursorTile)
                {
                    output = c as T;
                    if (output != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #region ModLoop
        /// <summary>
        /// Whether to display the tooltip or not - togglable.
        /// </summary>
        private bool displayEnabled;
        /// <summary>
        /// The character under the mouse at the last 250ms tick.
        /// </summary>
        internal Character selectedCharacter;
        /// <summary>
        /// The item in the player's hands at the last 250ms tick.
        /// </summary>
        internal Item heldItem;
        /// <summary>
        /// The tooltip used for display
        /// </summary>
        internal Tooltip tooltip;
        /// <summary>
        /// Used for logging
        /// </summary>
        private bool isFirstTickForMouseHover;
        /// <summary>
        /// Mod updates every 250ms for performance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuaterSecondUpdate(object sender, EventArgs e)
        {
            CheckUnderMouse();
        }

        private void CheckUnderMouse()
        {
            if (Game1.gameMode == Game1.playingGameMode && Game1.player != null && Game1.player.currentLocation != null)
            {
                heldItem = Game1.player.CurrentItem;
                if (TryGetAtMouse<Character>(out selectedCharacter))
                {
                    if (isFirstTickForMouseHover)
                    {
                        Monitor.Log($"Character '{selectedCharacter.Name}' under mouse. Type: '{selectedCharacter.GetType()}'");
                        Monitor.Log($"Held item is '{heldItem}'.");
                        isFirstTickForMouseHover = false;
                    }
                    tooltip.header.text = "";
                    tooltip.body.text = "";
                    foreach(IRelationship relationship in Relationships)
                    {
                        if (relationship.ConditionsMet(selectedCharacter, heldItem))
                        {
                            if (relationship.BreakAfter)
                            {
                                try
                                {
                                    string header = relationship.GetHeaderText(selectedCharacter, heldItem);
                                    string body = relationship.GetDisplayText(selectedCharacter, heldItem);
                                    tooltip.header.text += header;
                                    if (tooltip.body.text != "")
                                        tooltip.body.text += "\n";
                                    tooltip.body.text += body;
                                    break;//Finds the FIRST match, ignores later matches -- may want to change this later
                                }
                                catch (ArgumentException e)
                                {
                                    Monitor.Log(e.Message, LogLevel.Error);
                                }
                            }else
                            {
                                string header = relationship.GetHeaderText(selectedCharacter, heldItem);
                                string body = relationship.GetDisplayText(selectedCharacter, heldItem);
                                tooltip.header.text =  header == "" ? tooltip.header.text : tooltip.header.text + header;
                                if (tooltip.body.text != "")
                                    tooltip.body.text = body == "" ? tooltip.body.text : tooltip.body.text + "\n" + body;
                                else
                                    tooltip.body.text = body;
                            }
                        }
                    }
                }
                else
                    isFirstTickForMouseHover = true;
            }
            else
            {
                heldItem = null;
                selectedCharacter = null;
                isFirstTickForMouseHover = true;
            }
        }
        private void DrawTooltip(object sender, EventArgs e)
        {
            if (displayEnabled && selectedCharacter != null && tooltip.header.text != "")
            {
                tooltip.localX = Game1.getMouseX();
                tooltip.localY = Game1.getMouseY();
                tooltip.Draw(Game1.spriteBatch, null);
            }
        }
        #endregion
    }
}
