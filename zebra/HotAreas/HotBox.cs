using System.Drawing;

namespace zebra
{
    public class HotBox
    {
        public HotBox(CORNER corner, int x, int y, int w, int h)
        {
            Corner = corner;
            Rect = new Rectangle(x, y, w, h);
        }

        public CORNER Corner { get; }

        public Rectangle Rect { get; }

        public bool Contains(Point point) => Rect.Contains(point);
    }
}
