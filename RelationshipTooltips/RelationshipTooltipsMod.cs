using M3ales.RelationshipTooltips.Relationships;
using M3ales.RelationshipTooltips.UI;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    public class RelationshipTooltipsMod : Mod
    {
        internal ModConfig Config { get; private set; }
        public List<IRelationship> RelationshipTypes { get; private set; }
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            displayEnabled = Config.displayTooltipByDefault;
            tooltip = new Tooltip(0, 0, Color.White, anchor: FrameAnchor.BottomLeft);
            RelationshipTypes = new List<IRelationship>()
            {
                new PlayerRelationship(),
                new EasterEgg(),
                new PetRelationship(),
                new FarmAnimalRelationship(Config),
                new NPCGiftingRelationship(Config, Monitor),
                new NPCRelationship(Config, Monitor)
            };//LEAST SPECIFIC GOES LAST IN THIS LIST
            InitRelationshipTypes();
            InputEvents.ButtonPressed += (obj, e) => { if (e.Button == Config.toggleDisplayKey) { displayEnabled = !displayEnabled; } };
            GameEvents.QuarterSecondTick += QuaterSecondUpdate;
            GraphicsEvents.OnPostRenderEvent += DrawTooltip;
            helper.WriteConfig(Config);
            Monitor.Log("Init Complete");
        }

        /// <summary>
        /// Subscribes the stored Relationships to the relevant events
        /// </summary>
        private void InitRelationshipTypes()
        {
            //subscribe to events
            foreach(IRelationship r in RelationshipTypes)
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
                    SaveEvents.AfterSave += (obj, args) => { ((IPerSaveSerializable)r).SaveData(Helper); };
                    SaveEvents.AfterLoad += (obj, args) => { ((IPerSaveSerializable)r).LoadData(Helper); };
                }
            }
            Monitor.Log("Relationship Event Subscription Complete.");
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
            IEnumerable<Character> locationCharacters;
            if(Game1.currentLocation is AnimalHouse)
            {
                locationCharacters = (Game1.currentLocation as AnimalHouse).animals.Values.Select(c => (Character)c)
                    .Union(Game1.currentLocation.getCharacters())
                    .Union(Game1.currentLocation.getFarmers());
            }else
            {
                locationCharacters = Game1.currentLocation.getCharacters().Select(c => (Character)c)
                    .Union(Game1.currentLocation.getFarmers());
            }
            foreach (Character c in locationCharacters)
            {
                if (c == null)
                    continue;
                if (c == Game1.player)
                    return false;
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
        private bool displayEnabled;
        internal Character selectedCharacter;
        internal Item heldItem;
        internal Tooltip tooltip;
        /// <summary>
        /// Used for logging
        /// </summary>
        private bool isFirstTickForMouseHover;
        
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
                    foreach(IRelationship relationship in RelationshipTypes)
                    {
                        if (relationship.ConditionsMet(selectedCharacter, heldItem))
                        {
                            tooltip.header.text = relationship.GetHeaderText(selectedCharacter, heldItem);
                            tooltip.body.text = relationship.GetDisplayText(selectedCharacter, heldItem);
                            break;//Finds the FIRST match, ignores later matches -- may want to change this later
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
