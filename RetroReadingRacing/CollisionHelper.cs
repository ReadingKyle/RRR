using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroReadingRacing
{
    public enum CollisionDirection
    {
        NoCollision,
        CollisionTop,
        CollisionBottom,
        CollisionLeft,
        CollisionRight
    }
    public static class CollisionHelper
    {
        /// <summary>
        /// Detects a collision between two BoundingRectangles
        /// </summary>
        /// <param name="a">The first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns>true for collision, false otherwise</returns>
        public static CollisionDirection Collides(Rectangle a, Rectangle b)
        {
            if (!(a.Right < b.Left || a.Left > b.Right ||
                    a.Top > b.Bottom || a.Bottom < b.Top))
            {
                int topBottom = Math.Abs(a.Top - b.Bottom);
                int bottomTop = Math.Abs(a.Bottom - b.Top);
                int rightLeft = Math.Abs(a.Right - b.Left);
                int leftRight = Math.Abs(a.Left - b.Right);

                if (topBottom < bottomTop && topBottom < rightLeft && topBottom < leftRight)
                {
                    return CollisionDirection.CollisionTop;
                }
                else if (bottomTop < rightLeft && bottomTop < leftRight)
                {
                    return CollisionDirection.CollisionBottom;
                }
                else if (rightLeft < leftRight)
                {
                    return CollisionDirection.CollisionLeft;
                }
                else
                {
                    return CollisionDirection.CollisionRight;
                }
            }
            else return CollisionDirection.NoCollision;
        }

        /*public static bool Collides(Rectangle a, Rectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right ||
                    a.Top > b.Bottom || a.Bottom < b.Top);
        }*/
    }
}
