using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RetroReadingRacing.StateManagement;
using Microsoft.Xna.Framework;

namespace RetroReadingRacing.Screens
{
    public class SplashScreen : GameScreen
    {
        ContentManager _content;
        Texture2D _backgroundRed;
        Texture2D _backgroundYellow;
        Texture2D _backgroundGreen;
        TimeSpan _displayTime;

        public override void Activate()
        {
            base.Activate();

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundRed = _content.Load<Texture2D>("splash");
            _backgroundYellow = _content.Load<Texture2D>("splash_yellow");
            _backgroundGreen = _content.Load<Texture2D>("splash_green");
            _displayTime = TimeSpan.FromSeconds(2);
         
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            _displayTime -= gameTime.ElapsedGameTime;
            if (_displayTime <= TimeSpan.Zero) ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (_displayTime.TotalSeconds > 1.3) ScreenManager.SpriteBatch.Draw(_backgroundRed, new Vector2(0, 0), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(0, 0), 10.0f, SpriteEffects.None, 0);
            else if (_displayTime.TotalSeconds > 0.6) ScreenManager.SpriteBatch.Draw(_backgroundYellow, new Vector2(0, 0), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(0, 0), 10.0f, SpriteEffects.None, 0);
            else ScreenManager.SpriteBatch.Draw(_backgroundGreen, new Vector2(0, 0), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(0, 0), 10.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }
    }
}
