using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroReadingRacing
{
    public class Tilemap
    {
        /// <summary>
        /// The map width
        /// </summary>
        public int MapWidth { get; init; }

        /// <summary>
        /// The map height
        /// </summary>
        public int MapHeight { get; init; }

        /// <summary>
        /// The width of a tile in the map
        /// </summary>
        public int TileWidth { get; init; }

        /// <summary>
        /// The height of a tile in the map
        /// </summary>
        public int TileHeight { get; init; }

        /// <summary>
        /// The texture containing the tiles
        /// </summary>
        public Texture2D TilesetTexture { get; init; }

        /// <summary>
        /// An array of source rectangles corresponding to
        /// tile positions in the texture
        /// </summary>
        public Tile[] Tiles { get; init; }

        public Rectangle[] TileSet { get; init; }


        /// <summary>
        /// Checks to see if a sprite has collided with another sprite
        /// </summary>
        /// <param name="other">other sprite</param>
        /// <returns></returns>
        public CollisionDirection CollidesWith(Rectangle player, Rectangle tile)
        {
            return CollisionHelper.Collides(player, tile);
        }

        /// <summary>
        /// The map data - an array of indices to the Tile array
        /// </summary>
        public int[] TileIndices { get; init; }

        public int Update(GameTime gameTime, Car player)
        {
            Rectangle playerHitbox = player.HitBox;
            int result = 0;
            foreach (var tile in Tiles)
            {
                if (tile.Bounds != null)
                {
                    foreach (var bound in tile.Bounds)
                    {
                        CollisionDirection direction = CollidesWith(playerHitbox, bound);
                        int tempResult = 0;
                        if (tile.ID == 16 || tile.ID == 17 || tile.ID == 21 || tile.ID == 22) tempResult = 2;
                        else if (tile.ID == 18 || tile.ID == 19 || tile.ID == 23 || tile.ID == 24) tempResult = -1;
                        else if (tile.ID == 26 || tile.ID == 27 || tile.ID == 31 || tile.ID == 32) tempResult = 3;
                        else if (tile.ID == 28 || tile.ID == 29 || tile.ID == 33 || tile.ID == 34) tempResult = 4;
                        else if (tile.ID == 36 || tile.ID == 37 || tile.ID == 41 || tile.ID == 42) tempResult = 5;
                        else if (tile.ID == 38 || tile.ID == 39 || tile.ID == 43 || tile.ID == 44) tempResult = 6;
                        else if (tile.ID == 46 || tile.ID == 47 || tile.ID == 51 || tile.ID == 52) tempResult = 7;
                        else if (tile.ID == 48 || tile.ID == 49 || tile.ID == 53 || tile.ID == 54) tempResult = 11;
                        else if (tile.ID == 56 || tile.ID == 57 || tile.ID == 61 || tile.ID == 62) tempResult = 12;
                        //else if (tile.ID >= 6 && tile.ID <= 13) tempResult = 1;
                        if (direction == CollisionDirection.CollisionTop || direction == CollisionDirection.CollisionBottom)
                        {
                            if (result > 7 && result <= 10)
                            {
                                result = 10;
                            }
                            else if (tempResult == 0) result = 8;
                            else result = tempResult;
                        }
                        else if (direction == CollisionDirection.CollisionLeft || direction == CollisionDirection.CollisionRight)
                        {
                            if (result > 7 && result <= 10)
                            {
                                result = 10;
                            }
                            else if (tempResult == 0) result = 9;
                            else result = tempResult;
                        }
                        /*switch (direction)
                        {
                            case CollisionDirection.CollisionTop:
                                result = 7;
                                break;
                            case CollisionDirection.CollisionBottom:
                                result = 7;
                                break;
                            case CollisionDirection.CollisionLeft:
                                result = 8;
                                break;
                            case CollisionDirection.CollisionRight:
                                result = 8;
                                break;
                            default:
                                break;
                        }*/
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Draws the tilemap. Assumes that spriteBatch.Begin() has been called.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">a spritebatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for(int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    // Indices start at 1, so shift by 1 for array coordinates
                    int index = TileIndices[y * MapWidth + x] - 1;

                    // Index of -1 (shifted from 0) should not be drawn
                    if (index == -1) continue;

                    // Draw the current tile
                    spriteBatch.Draw(
                        TilesetTexture,
                        new Rectangle(
                            x * TileWidth,
                            y * TileHeight,
                            TileWidth,
                            TileHeight
                            ),
                        TileSet[index],
                        Color.White
                    );
                }
            }

        }
    }
}
