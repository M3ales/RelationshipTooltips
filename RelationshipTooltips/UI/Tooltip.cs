using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace M3ales.RelationshipTooltips.UI
{
    public class Tooltip : Frame
    {
        public Tooltip(int x, int y, Color color, FrameAnchor anchor = FrameAnchor.TopLeft, FrameAnchor parentAnchor = FrameAnchor.TopLeft, List<IFrameDrawable> components = null, Frame parent = null,string header = "", string text = "") : base(x, y, color, anchor, parentAnchor, components, parent)
        {
            this.header = new ShadowText(text, Game1.dialogueFont, Game1.textColor, 20, 20);
            this.body = new ShadowText(text, Game1.smallFont, Game1.textColor, 20, 20);
            this.components = new List<IFrameDrawable>()
            {
                new TextureBox(color),
                this.header,
                this.body
            };
        }
        public ShadowText header;
        public ShadowText body;
    }
}
