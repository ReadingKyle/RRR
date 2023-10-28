using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RetroReadingRacing.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using static System.TimeZoneInfo;
using System.Threading;
using SharpDX.Direct2D1;
using System.IO;
using ParticleSystemExample;

namespace RetroReadingRacing.Screens
{
    public class Track : GameScreen
    {
        private GameState _gameState;

        private ContentManager _content;

        string _mapName;
        private Tilemap _map;

        private Car _car;

        private bool _passedCheckpoint;

        private int _numLaps;

        private TimeSpan _timer;

        private ExplosionParticleSystem _explosions;

        public Track(string mapname)
        {
            _mapName = mapname;
        }

        public override void Activate()
        {
            base.Activate();

            _gameState = LoadGameState();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _map = _content.Load<Tilemap>(_mapName);
            _car = new Car(_content.Load<Texture2D>("tiles"), new Vector2(95, 410));

            _explosions = new ExplosionParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_explosions);
        }

        private void SaveGameState()
        {
            string serializedState = _gameState.Serialize();
            File.WriteAllText("savefile.json", serializedState);
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

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new MenuScreen());
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the menu.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start > 0)
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (B) to resume\npress (A) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }

            PlayerIndex playerIndex;

            if (_numLaps < 3)
            {
                _timer += gameTime.ElapsedGameTime;
            }

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
            int triggeredCollision = _map.Update(gameTime, _car);
            if (triggeredCollision == 1)
            {
                _explosions.PlaceExplosion(new Vector2(_car.Position.X, _car.Position.Y));
                _car.rotation = 0;
                _car.speed = 0;
                _car.acceleration = 0;
                _car.Position = new Vector2(95, 410);
                _passedCheckpoint = false;
            }

            if (_car.Position.X > 320 && _car.Position.Y > 320)
            {
                _passedCheckpoint = true;
            }

            if (_passedCheckpoint && _car.Position.Y <= 384 && _car.Position.Y >= 380 && _car.Position.X >= 64 && _car.Position.X <= 128)
            {
                _numLaps++;
                _passedCheckpoint = false;
            }

            if (_numLaps == 3)
            {
                if (_mapName == "track1" && (_timer < _gameState.TrackOneHighScore || _gameState.TrackOneHighScore == TimeSpan.Zero))
                {
                    _gameState.TrackOneHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track2" && (_timer < _gameState.TrackTwoHighScore || _gameState.TrackTwoHighScore == TimeSpan.Zero))
                {
                    _gameState.TrackTwoHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track3" && (_timer < _gameState.TrackThreeHighScore || _gameState.TrackThreeHighScore == TimeSpan.Zero))
                {
                    _gameState.TrackThreeHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track4" && (_timer < _gameState.TrackFourHighScore || _gameState.TrackFourHighScore == TimeSpan.Zero))
                {
                    _gameState.TrackFourHighScore = _timer;
                    SaveGameState();
                }
                LoadingScreen.Load(ScreenManager, false, new MenuScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            _map.Draw(gameTime, spriteBatch);
            _car.Draw(spriteBatch);
            spriteBatch.DrawString(ScreenManager.Font, $"{_timer.Minutes:D2}:{_timer.Seconds:D2}", new Vector2(544, 32), Color.Black);
            spriteBatch.DrawString(ScreenManager.Font, $"{_numLaps}/3", new Vector2(32, 32), Color.Black);
            spriteBatch.End();
        }
    }
}
