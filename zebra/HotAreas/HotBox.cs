using System;
using System.Drawing;

namespace zebra
{
    public class HotBox
    {
        public HotBox(CORNER corner, int x, int y, int w, int h)
        {
            this.Corner = corner;
            this.Rect = new Rectangle(x, y, w, h);
        }

        public CORNER Corner { get; }

        public Rectangle Rect { get; }

        public Boolean Contains(Point point)
        {
            return Rect.Contains(point);
        }
    }
}
