using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace M3ales.RelationshipTooltips.UI
{
    class TextureBox : IFrameDrawable
    {
        Color color;
        public int SizeX => 0;
        public int SizeY => 0;
        public TextureBox(Color c)
        {
            color = c;
        }
        public void Draw(SpriteBatch b, int x, int y, Frame parentFrame)
        {
            IClickableMenu.drawTextureBox(b, x, y, parentFrame.Width, parentFrame.Height, color);
        }
    }
}
