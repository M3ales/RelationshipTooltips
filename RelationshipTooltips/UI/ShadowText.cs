using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace M3ales.RelationshipTooltips.UI
{
    /// <summary>
    /// Text with a dropshadow
    /// </summary>
    public class ShadowText : IFrameDrawable
    {
        public ShadowText(string text, SpriteFont font, Color color, int xPadding = 0, int yPadding = 0)
        {
            this.text = text;
            this.font = font;
            this.color = color;
            this.xPadding = xPadding;
            this.yPadding = yPadding;
        }
        public SpriteFont font;
        public Color color;
        public string text;
        public int xPadding;
        public int yPadding;

        public int SizeX
        {
            get
            {
                if (font != null)
                    return (int)(font == Game1.dialogueFont ? 1f* font.MeasureString(text).X : font.MeasureString(text).X) + (xPadding*2);
                return 0;
            }
        }

        public int SizeY
        {
            get
            {
                if (font != null)
                    return (int)(font == Game1.dialogueFont ? 0.25f* font.MeasureString(text).Y : font.MeasureString(text).Y) + (yPadding*2);
                return 0;
            }
        }

        public void Draw(SpriteBatch b, int x, int y, Frame parentFrame)
        {
            Utility.drawTextWithShadow(b, text, font, new Vector2(x + xPadding, y + yPadding), this.color);
        }
    }
}
