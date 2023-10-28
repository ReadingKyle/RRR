using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroReadingRacing
{
    public class Car
    {
        private Texture2D _texture;
        public Vector2 Position;
        public Vector2 PreviousPosition;
        public float angularSpeed;
        public float acceleration;
        public float rotation;
        public float speed;
        private float _topSpeed = 5f;

        private float _turnSpeed = 0.5f;

        public Rectangle HitBox;

        public Car(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            HitBox = new Rectangle((int)Position.X-12, (int)Position.Y-12, 16, 16);
        }

        public void Update(GameTime gameTime)
        {
            if (angularSpeed != 0 && speed != 0)
            {
                rotation += angularSpeed * _turnSpeed;
            }
            else
            {
                angularSpeed = 0;
            }

            if (Math.Abs(speed) < _topSpeed) speed += acceleration * 10;

            PreviousPosition = Position;

            Position.X += speed * (float)Math.Sin(rotation);
            Position.Y += -speed * (float)Math.Cos(rotation);
            HitBox.X = (int)Position.X - 12;
            HitBox.Y = (int)Position.Y - 12;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                new Rectangle(0, 0, 32, 32),
                Color.White,
                rotation,
                new Vector2(16, 24),
                1f,
                SpriteEffects.None,
                0
            );
/*            spriteBatch.Draw(
                _texture,
                new Vector2(HitBox.X, HitBox.Y),
                HitBox,
                Color.White,
                0,
                new Vector2(16, 24),
                2f,
                SpriteEffects.None,
                0
            );*/
        }
    }
}
