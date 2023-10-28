using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System;
using System.Diagnostics;

namespace PipelineExtension
{
    /// <summary>
    /// Processes a BasicTilemapContent object, building and linking the associated texture 
    /// and setting up the tile information.
    /// </summary>
    [ContentProcessor(DisplayName = "BasicTilemapProcessor")]
    public class BasicTilemapProcessor : ContentProcessor<BasicTilemapContent, BasicTilemapContent>
    {
        /// <summary>
        /// A scaling parameter to make the tilemap bigger
        /// </summary>
        public float Scale { get; set; } = 2.0f;

        public override BasicTilemapContent Process(BasicTilemapContent map, ContentProcessorContext context)
        {
            // We need to build the tileset texture associated with this tilemap
            // This will create the binary texture file and link it to this tilemap so 
            // they get loaded together by the ContentProcessor.  
            //map.TilesetTexture = context.BuildAsset<Texture2DContent, Texture2DContent>(map.TilesetTexture, "Texture2DProcessor");
            map.TilesetTexture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(new ExternalReference<TextureContent>(map.TilesetImageFilename), "TextureProcessor");

            // Determine the number of rows and columns of tiles in the tileset texture
            int tilesetColumns = map.TilesetTexture.Mipmaps[0].Width / map.TileWidth;
            int tilesetRows = map.TilesetTexture.Mipmaps[0].Height / map.TileHeight;

            // We need to create the bounds for each tile in the tileset image
            // These will be stored in the tile set array
            map.TileSet = new Rectangle[tilesetColumns * tilesetRows];
            context.Logger.LogMessage($"{map.TileSet.Length} Total tiles");
            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    // The Tiles array provides the source rectangle for a tile
                    // within the tileset texture
                    map.TileSet[y * tilesetColumns + x] = new Rectangle(
                        x * map.TileWidth,
                        y * map.TileHeight,
                        map.TileWidth,
                        map.TileHeight
                        );
                }
            }
            map.Tiles = new BasicTile[map.MapWidth * map.MapHeight];
            for (int y = 0; y < map.MapHeight; y++)
            {
                for (int x = 0; x < map.MapWidth; x++)
                {
                    int index = map.MapWidth * y + x;
                    map.Tiles[index] = new BasicTile
                    {
                        ID = map.TileIndices[index],
                        SourceRectangle = map.TileSet[map.TileIndices[index]],
                    };
                    if (map.Tiles[index].ID == 6)
                    {
                        map.Tiles[index].Bounds = new Rectangle[3]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)((y * map.TileHeight + 28) * Scale),
                                (int)(5 * Scale),
                                (int)(5 * Scale)
                            )
                        };
                    }
                    else if (map.Tiles[index].ID == 7)
                    {
                        map.Tiles[index].Bounds = new Rectangle[3]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth) * Scale),
                                (int)((y * map.TileHeight + 28) * Scale),
                                (int)(5 * Scale),
                                (int)(5 * Scale)
                            )
                        };
                    }
                    else if (map.Tiles[index].ID == 8)
                    {
                        map.Tiles[index].Bounds = new Rectangle[3]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)((y * map.TileHeight + 28) * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth) * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)((y * map.TileHeight) * Scale),
                                (int)(5 * Scale),
                                (int)(5 * Scale)
                            )
                        };
                    }
                    else if (map.Tiles[index].ID == 9)
                    {
                        map.Tiles[index].Bounds = new Rectangle[3]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)((y * map.TileHeight + 28) * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth) * Scale),
                                (int)((y * map.TileHeight) * Scale),
                                (int)(5 * Scale),
                                (int)(5 * Scale)
                            )
                        };
                    }
                    else if (map.Tiles[index].ID == 10 || map.Tiles[index].ID >= 16)
                    {
                        map.Tiles[index].Bounds = new Rectangle[1]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                        };
                    }
                    else if (map.Tiles[index].ID == 11)
                    {
                        map.Tiles[index].Bounds = new Rectangle[2]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                        };
                    }
                    else if (map.Tiles[index].ID == 12)
                    {
                        map.Tiles[index].Bounds = new Rectangle[2]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                            new Rectangle(
                                (int)((x * map.TileWidth + 28) * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(5 * Scale),
                                (int)(map.TileHeight * Scale)
                            ),
                        };
                    }
                    else if (map.Tiles[index].ID == 13)
                    {
                        map.Tiles[index].Bounds = new Rectangle[2]
                        {
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)(y * map.TileHeight * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                            new Rectangle(
                                (int)(x * map.TileWidth * Scale),
                                (int)((y * map.TileHeight + 28) * Scale),
                                (int)(map.TileWidth * Scale),
                                (int)(5 * Scale)
                            ),
                        };
                    }
                    context.Logger.LogMessage($"{index}");
                }
            }

            // Now that we've created our source rectangles, we can 
            // apply the scaling factor to the tile dimensions - this 
            // will have us draw tiles at a different size than their source
            map.TileWidth = (int)(map.TileWidth * Scale);
            map.TileHeight = (int)(map.TileHeight * Scale);

            // Return the fully processed tilemap
            return map;
        }
    }
}
