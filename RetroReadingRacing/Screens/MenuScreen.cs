using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ParticleSystemExample;
using RetroReadingRacing.StateManagement;

namespace RetroReadingRacing.Screens
{
    // Base class for screens that contain a menu of options. The user can
    // move up and down to select an entry, or cancel to back out of the screen.
    public class MenuScreen : GameScreen
    {
        private GameState _gameState;

        private ContentManager _content;

        private Tilemap _tilemap;
        private Tilemap _trackselect;
        private Car _car;

        private ExplosionParticleSystem _explosions;

        bool gameStart;
        bool trackSelection;

        public override void Activate()
        {
            base.Activate();

            _gameState = LoadGameState();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _tilemap = _content.Load<Tilemap>("map");
            _trackselect = _content.Load<Tilemap>("trackselect");
            _car = new Car(_content.Load<Texture2D>("tiles"), new Vector2(320, 320));

            _explosions = new ExplosionParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_explosions);
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the menu.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0 || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0)
            {
                _car.acceleration = (GamePad.GetState(PlayerIndex.One).Triggers.Right - GamePad.GetState(PlayerIndex.One).Triggers.Left) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_car.speed > 0 && GamePad.GetState(PlayerIndex.One).Triggers.Right == 0)
            {
                _car.acceleration = 0;
                _car.speed -= 0.05f;
                if (_car.speed < 0)
                {
                    _car.speed = 0; // Ensure speed doesn't go negative
                }
            }
            else if (_car.speed < 0 && GamePad.GetState(PlayerIndex.One).Triggers.Left == 0)
            {
                _car.acceleration = 0;
                _car.speed += 0.05f;
                if (_car.speed > 0)
                {
                    _car.speed = 0; // Ensure speed doesn't go positive (reverse)
                }
            }
            _car.angularSpeed = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            _car.Update(gameTime);
            int triggeredCollision = trackSelection ? _trackselect.Update(gameTime, _car) :_tilemap.Update(gameTime, _car);
            if (triggeredCollision == 1)
            {
                _explosions.PlaceExplosion(new Vector2(_car.Position.X, _car.Position.Y));
                _car.Position = new Vector2(320, 320);
            }
            else if (triggeredCollision == 2)
            {
                trackSelection = true;
            }
            else if (triggeredCollision == 3)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track1"));
            }
            else if (triggeredCollision == 4)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track2"));
            }
            else if (triggeredCollision == 5)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track3"));
            }
            else if (triggeredCollision == 6)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track4"));
            }
            else if (triggeredCollision == -1)
            {
                if (trackSelection) trackSelection = false;
                else ScreenManager.Game.Exit();
            }
        }

        private GameState LoadGameState()
        {
            if (File.Exists("savefile.json"))
            {
                string savedState = File.ReadAllText("savefile.json");
                return GameState.Deserialize(savedState);
            }
            return new GameState();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (!trackSelection)
            {
                _tilemap.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(ScreenManager.BigFont, "Retro Reading Racing", new Vector2(80, 160), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, "Use a controller\nDon't fall off the map\nHave fun!", new Vector2(80, 496), Color.Black);
            }
            else
            {
                _trackselect.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackOneHighScore.Minutes:D2}:{_gameState.TrackOneHighScore.Seconds:D2}", new Vector2(80, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackTwoHighScore.Minutes:D2}:{_gameState.TrackTwoHighScore.Seconds:D2}", new Vector2(208, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackThreeHighScore.Minutes:D2}:{_gameState.TrackThreeHighScore.Seconds:D2}", new Vector2(336, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackFourHighScore.Minutes:D2}:{_gameState.TrackFourHighScore.Seconds:D2}", new Vector2(464, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.Font, "Track Selection", new Vector2(80, 496), Color.Black);
            }
            _car.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
