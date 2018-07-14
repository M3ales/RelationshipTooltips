using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips
{
    public class NPCUtils //Will be replaced by Bookcase dependancy at somepoint
    {        
        /// <summary>
        /// Cache for NPC tileLocation - reduces double call of NPC location calculation to a single call. Be careful about forgetting to assign this!
        /// </summary>
        private static Vector2 npcTileLoc;
        /// <summary>
        /// Gets the NPC currently on the tiles under the mouse cursor
        /// </summary>
        /// <returns>The first NPC below the mouse, or null</returns>
        public static NPC GetNPCUnderCursor()
        {
            foreach (NPC n in Game1.player.currentLocation.getCharacters())
            {
                npcTileLoc = n.getTileLocation();
                if (IsNPCOnMouseTile(true))
                {
                    return n;
                }
            }
            return null;
        }
        /// <summary>
        /// Checks if an NPC is on the same tile as the mouse, with the option of checking the tile above aswell. (Most NPCs fill 2 tiles, visually)
        /// </summary>
        /// <param name="checkOneTileAbove"></param>
        /// <returns></returns>
        public static bool IsNPCOnMouseTile(bool checkOneTileAbove = false)
        {
            return Game1.currentCursorTile == npcTileLoc || (checkOneTileAbove && Game1.currentCursorTile == (npcTileLoc - Vector2.UnitY));
        }
        /// <summary>
        /// Tries to get an NPC which is on the tile under the mouse cursor.
        /// </summary>
        /// <param name="npc">The NPC under the cursor, or null if none is found.</param>
        /// <returns>True if there is an NPC present, False if there is not.</returns>
        public static bool TryGetNPCUnderCursor(out NPC npc)
        {
            npc = GetNPCUnderCursor();
            return npc != null;
        }
    }
}
