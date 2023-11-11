using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PipelineExtension;
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
        public CollisionDirection LastCollision;
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

        public PlayerIndex PlayerNum;

        public bool isColliding;

        public Vector2 InitialPosition;

        public bool Exists;

        public bool checkpoint1;
        public bool checkpoint2;
        public bool checkpoint3;

        public int NumLaps;

        public Car(Texture2D texture, Vector2 position, PlayerIndex playerNum)
        {
            _texture = texture;
            Position = position;
            HitBox = new Rectangle((int)Position.X-11, (int)Position.Y-8, 16, 16);
            PlayerNum = playerNum;
            InitialPosition = Position;
            NumLaps = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (angularSpeed != 0 && speed != 0)
            {
                rotation += angularSpeed * _turnSpeed;
                if (rotation > MathHelper.TwoPi)
                {
                    rotation -= MathHelper.TwoPi;
                }
            }
            else
            {
                angularSpeed = 0;
            }

            if (Math.Abs(speed) < _topSpeed) speed += acceleration * 10;

            PreviousPosition = Position;

            Position.X += speed * (float)Math.Sin(rotation);
            Position.Y += -speed * (float)Math.Cos(rotation);
            HitBox.X = (int)Position.X - 8;
            HitBox.Y = (int)Position.Y - 8;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                new Rectangle(PlayerNum == 0 ? 0 : 32, 0, 32, 31),
                Color.White,
                rotation,
                new Vector2(16, 16),
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
