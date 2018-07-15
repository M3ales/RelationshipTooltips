using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3ales.RelationshipTooltips.UI
{
    public interface IFrameDrawable
    {
        void Draw(SpriteBatch b, int x, int y, Frame parentFrame);
        int SizeX { get; }
        int SizeY { get; }
    }
}
