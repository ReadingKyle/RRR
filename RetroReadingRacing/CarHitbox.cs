using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetroReadingRacing.StateManagement;

namespace PipelineExtension
{
    public class CarHitbox
    {
        public Vector2 TopLeft;
        public Vector2 TopRight;
        public Vector2 BottomLeft;
        public Vector2 BottomRight;

        public Vector2 Origin;

        Texture2D _texture;

        private ContentManager _content;

        public CarHitbox(Vector2 origin, Texture2D texture)
        {
            TopLeft = new Vector2(origin.X - 0.06f, origin.Y - .12f);
            TopRight = new Vector2(origin.X + 6f, origin.Y - 12f);
            BottomLeft = new Vector2(origin.X - 6f, origin.Y + 12f);
            BottomRight = new Vector2(origin.X + 6f, origin.Y + 12f);

            _texture = texture;
        }

        public void Update(float rotationAngle, Vector2 origin, float angularSpeed, float turnSpeed)
        {
            Matrix rotationMatrix = Matrix.CreateRotationZ(rotationAngle / 100);

            TopLeft = Vector2.Transform(TopLeft - Origin, rotationMatrix);
            TopLeft += Origin;

            TopRight = Vector2.Transform(TopRight - Origin, rotationMatrix);
            TopRight += Origin;

            BottomLeft = Vector2.Transform(BottomLeft - Origin, rotationMatrix);
            BottomLeft += Origin;

            BottomRight = Vector2.Transform(BottomRight - Origin, rotationMatrix);
            BottomRight += Origin;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(
                    _texture,
                    TopLeft,
                    new Rectangle(0, 0, 100, 100),
                    Color.White,
                    0,
                    Origin,
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.Draw(
                    _texture,
                    TopRight,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.Draw(
                    _texture,
                    BottomLeft,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.Draw(
                    _texture,
                    BottomRight,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    1f,
                    SpriteEffects.None,
                    0
                );
            }
    }
}
