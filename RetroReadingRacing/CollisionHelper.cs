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
        CollisionRight,
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
                if (a.Right > b.Left) return CollisionDirection.CollisionLeft;
                else if (a.Left < b.Right) return CollisionDirection.CollisionRight;
                else if (a.Top < b.Bottom) return CollisionDirection.CollisionTop;
                else if (a.Bottom > b.Top) return CollisionDirection.CollisionBottom;
                else return CollisionDirection.NoCollision;
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
