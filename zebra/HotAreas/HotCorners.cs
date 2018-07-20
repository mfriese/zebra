using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace zebra
{
    public class HotCorners
    {
        public HotCorners()
        {
            Mapping = new Dictionary<CORNER, Action>();
            CornerBoxes = new List<HotBox>();
            BoxLock = false;

            foreach (var screen in Screen.AllScreens)
            {
                CornerBoxes.Add(new HotBox(CORNER.UPPER_LEFT, screen.Bounds.Left - 8, screen.Bounds.Top - 8, 16, 16));
                CornerBoxes.Add(new HotBox(CORNER.UPPER_RIGHT, screen.Bounds.Right - 8, screen.Bounds.Top - 8, 16, 16));
                CornerBoxes.Add(new HotBox(CORNER.LOWER_RIGHT, screen.Bounds.Right - 8, screen.Bounds.Bottom - 8, 16, 16));
                CornerBoxes.Add(new HotBox(CORNER.LOWER_LEFT, screen.Bounds.Left - 8, screen.Bounds.Bottom - 8, 16, 16));
            }
        }

        protected Boolean BoxLock { get; set; }

        protected IList<HotBox> CornerBoxes { get; }

        protected Dictionary<CORNER, Action> Mapping { get; }

        public void RegisterHandler(CORNER corner, Action handler)
        {
            Mapping.Add(corner, handler);
        }

        public void TriggerOnHit(Point point)
        {
            Boolean hit = false;
            Action hitAction = null;

            foreach (HotBox box in CornerBoxes)
            {
                if (box.Contains(point))
                {
                    hitAction = Mapping.ContainsKey(box.Corner) ? Mapping[box.Corner] : null;
                    hit = true;
                    break;
                }
            }

            // No hit. Unlock and exit.
            if (!hit)
            {
                BoxLock = false;
                return;
            }

            // Do not act if commandos are blocked.
            if (hit && BoxLock)
                return;

            // It's unlocked, so lock it first and then trigger the action.
            BoxLock = true;

            // Invoke if not null
            hitAction?.Invoke();
        }
    }
}
