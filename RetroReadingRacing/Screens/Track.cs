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
using System.Reflection.Metadata;

namespace RetroReadingRacing.Screens
{
    public class Track : GameScreen
    {
        private GameState _gameState;

        private ContentManager _content;

        string _mapName;
        private Tilemap _map;

        private Car _car1;
        private Car _car2;

        private TimeSpan _timer;
        private double _endGameTimer;

        private ExplosionParticleSystem _explosions;

        private bool _isTwoPlayer;
        private bool _isCareer;
        private bool _isSplosive;

        private bool _player1Wins;
        private bool _player2Wins;

        private bool _newHighScore;

        private Texture2D _overlay;

        private Trophy _trophy;

        public Track(string mapname, bool isTwoPlayer, bool isCareer, bool isSplosive)
        {
            _mapName = mapname;
            _isTwoPlayer = isTwoPlayer;
            _isCareer = isCareer;
            _isSplosive = isSplosive;
        }

        public override void Activate()
        {
            base.Activate();

            _gameState = LoadGameState();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _map = _content.Load<Tilemap>(_mapName);
            _overlay = _content.Load<Texture2D>("darkscreen");

            _car1 = new Car(_content.Load<Texture2D>("tiles"), new Vector2(85, 410), PlayerIndex.One);
            _car2 = new Car(_content.Load<Texture2D>("tiles"), new Vector2(105, 410), PlayerIndex.Two);

            _trophy = new Trophy(_content.Load<Model>("trophy"));
            if (_isTwoPlayer)
            {
                _car2.Exists = true;
            }

            _explosions = new ExplosionParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_explosions);

            _endGameTimer = 5;
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
            LoadingScreen.Load(ScreenManager, false, new MenuScreen(_isTwoPlayer));
        }

        public void UpdateCar(Car car, GameTime gameTime)
        {
            if (GamePad.GetState(car.PlayerNum).Triggers.Right > 0)
            {
                car.acceleration = (GamePad.GetState(car.PlayerNum).Triggers.Right * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (GamePad.GetState(car.PlayerNum).Triggers.Left > 0)
            {
                car.acceleration = -(GamePad.GetState(car.PlayerNum).Triggers.Left * (float)gameTime.ElapsedGameTime.TotalSeconds) / 5;
            }
            if (car.speed > 0 && GamePad.GetState(car.PlayerNum).Buttons.X > 0)
            {
                car.speed -= 0.075f;
            }
            if (car.speed < 0 && GamePad.GetState(car.PlayerNum).Buttons.X > 0)
            {
                car.speed += 0.075f;
            }
            if (car.speed > 0 && GamePad.GetState(car.PlayerNum).Triggers.Left > 0)
            {
                car.speed -= 0.1f;
            }
            if (car.speed < 0 && GamePad.GetState(car.PlayerNum).Triggers.Right > 0)
            {
                car.speed += 0.1f;
            }
            if (car.speed > 0 && GamePad.GetState(car.PlayerNum).Triggers.Right == 0)
            {
                car.acceleration = 0;
                car.speed -= 0.05f;
                if (car.speed < 0)
                {
                    car.speed = 0; // Ensure speed doesn't go negative
                }
            }
            if (car.speed < 0 && GamePad.GetState(car.PlayerNum).Triggers.Left == 0)
            {
                car.acceleration = 0;
                car.speed += 0.05f;
                if (car.speed > 0)
                {
                    car.speed = 0; // Ensure speed doesn't go positive (reverse)
                }
            }

            car.angularSpeed = GamePad.GetState(car.PlayerNum).ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            car.Update(gameTime);
            int triggeredCollision;
            triggeredCollision = _map.Update(gameTime, car);
            if (triggeredCollision != 0)
            {
                car.isColliding = true;
            }
            if (triggeredCollision >= 8 && triggeredCollision <= 10 && _isSplosive)
            {
                _explosions.PlaceExplosion(new Vector2(car.Position.X, car.Position.Y));
                car.checkpoint1 = false;
                car.checkpoint2 = false;
                car.checkpoint3 = false;
                car.Position = car.InitialPosition;
                car.rotation = 0;
                car.speed = 0;
                car.acceleration = 0;
            }
            else if (triggeredCollision == 8)
            {
                car.Position.Y = car.PreviousPosition.Y;
                car.speed = car.speed / 5;
            }
            else if (triggeredCollision == 9)
            {
                car.Position.X = car.PreviousPosition.X;
                car.speed = car.speed / 5;
                car.acceleration = car.acceleration / 5;
            }
            else if (triggeredCollision == 10)
            {
                car.Position.X = car.PreviousPosition.X;
                car.Position.Y = car.PreviousPosition.Y;
                car.speed = car.speed / 5;
                car.acceleration = car.acceleration / 5;
            }
            if (car.Position.X > 320 && !car.checkpoint1)
            {
                car.checkpoint1 = true;
            }
            if (car.Position.Y > 320 && car.checkpoint1 && !car.checkpoint2)
            {
                car.checkpoint2 = true;
            }
            if (car.Position.X < 320 && car.checkpoint2 && !car.checkpoint3)
            {
                car.checkpoint3 = true;
            }

            if (car.checkpoint1 && car.checkpoint2 && car.checkpoint3 && car.Position.Y <= 384 && car.Position.Y >= 380 && car.Position.X >= 64 && car.Position.X <= 128)
            {
                car.NumLaps++;
                car.checkpoint1 = false;
                car.checkpoint2 = false;
                car.checkpoint3 = false;
            }

            if (car.NumLaps == 3)
            {
                if (_car1.NumLaps == 3) _player1Wins = true;
                if (_car2.NumLaps == 3) _player2Wins = true;
                if (_mapName == "track1" && (_timer < _gameState.TrackOneHighScore || _gameState.TrackOneHighScore == TimeSpan.Zero) && _isCareer)
                {
                    _newHighScore = true;
                    _gameState.TrackOneHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track2" && (_timer < _gameState.TrackTwoHighScore || _gameState.TrackTwoHighScore == TimeSpan.Zero) && _isCareer)
                {
                    _newHighScore = true;
                    _gameState.TrackTwoHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track3" && (_timer < _gameState.TrackThreeHighScore || _gameState.TrackThreeHighScore == TimeSpan.Zero) && _isCareer)
                {
                    _newHighScore = true;
                    _gameState.TrackThreeHighScore = _timer;
                    SaveGameState();
                }
                else if (_mapName == "track4" && (_timer < _gameState.TrackFourHighScore || _gameState.TrackFourHighScore == TimeSpan.Zero) && _isCareer)
                {
                    _newHighScore = true;
                    _gameState.TrackFourHighScore = _timer;
                    SaveGameState();
                }
            }
        }

        public void HandleCarCollisions(Car car1, Car car2)
        {
            CollisionDirection direction = CollisionHelper.Collides(_car1.HitBox, _car2.HitBox);
            if (direction != CollisionDirection.NoCollision)
            {
                if (direction == CollisionDirection.CollisionLeft)
                {
                    car1.Position.X = car1.PreviousPosition.X;
                    car1.speed -= car1.speed * 1.75f;
                    car1.acceleration = 0;
                }
                if (direction == CollisionDirection.CollisionRight)
                {
                    car1.Position.X = car1.PreviousPosition.X;
                    car1.speed -= car1.speed * 1.75f;
                    car1.acceleration = 0;
                }
                if (direction == CollisionDirection.CollisionTop)
                {
                    car1.Position.Y = car1.PreviousPosition.Y;
                    car1.speed -= car1.speed * 1.75f;
                    car1.acceleration = 0;
                }
                if (direction == CollisionDirection.CollisionBottom)
                {
                    car1.Position.Y = car1.PreviousPosition.Y;
                    car1.speed -= car1.speed * 1.75f;
                    car1.acceleration = 0;
                }
            }
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the menu.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start > 0 || GamePad.GetState(PlayerIndex.Two).Buttons.Start > 0)
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (B) to resume\npress (A) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }

            if (_car1.NumLaps < 3 && _car2.NumLaps < 3)
            {
                _timer += gameTime.ElapsedGameTime;

                UpdateCar(_car1, gameTime);
                if (_car2.Exists) UpdateCar(_car2, gameTime);
            }
            else
            {
                _endGameTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                _trophy.Update(gameTime);
                if (_endGameTimer < 0)
                {
                    LoadingScreen.Load(ScreenManager, false, new MenuScreen(_isTwoPlayer));
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

            _map.Draw(gameTime, spriteBatch);
            if (_car2.Exists) _car2.Draw(spriteBatch);
            _car1.Draw(spriteBatch);
            spriteBatch.DrawString(ScreenManager.Font, $"{_timer.Minutes:D2}:{_timer.Seconds:D2}", new Vector2(544, 32), Color.Black);
            if (_car2.Exists)
            {
                spriteBatch.DrawString(ScreenManager.Font, $"{_car1.NumLaps}/3", new Vector2(32, 32), Color.Red);
                spriteBatch.DrawString(ScreenManager.Font, $"{_car2.NumLaps}/3", new Vector2(128, 32), Color.DodgerBlue);
            }
            else
            {
                spriteBatch.DrawString(ScreenManager.Font, $"{_car1.NumLaps}/3", new Vector2(32, 32), Color.Black);
            }
            if (_isTwoPlayer && _player1Wins)
            {
                spriteBatch.Draw(
                _overlay,
                new Vector2(0, 0),
                new Rectangle(0, 0, 640, 640),
                new Color(Color.White, 128), // 128 is half of 255 (0.5 opacity)
                0,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0
            );
                spriteBatch.DrawString(ScreenManager.Font, "Player 1 Wins!", new Vector2(220, 150), Color.White);
            }
            else if (_isTwoPlayer && _player2Wins)
            {
                spriteBatch.Draw(
                _overlay,
                new Vector2(0, 0),
                new Rectangle(0, 0, 640, 640),
                new Color(Color.White, 128), // 128 is half of 255 (0.5 opacity)
                0,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0
            );
                spriteBatch.DrawString(ScreenManager.Font, "Player 2 Wins!", new Vector2(220, 150), Color.White);
            }
            else if (!_isTwoPlayer && _player1Wins)
            {
                spriteBatch.Draw(
                _overlay,
                new Vector2(0, 0),
                new Rectangle(0, 0, 640, 640),
                new Color(Color.White, 128), // 128 is half of 255 (0.5 opacity)
                0,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0
            );
                spriteBatch.DrawString(ScreenManager.Font, "Race Complete!", new Vector2(220, 150), Color.White);
            }
            if (_isCareer && _newHighScore && (_player1Wins || _player2Wins))
            {
                spriteBatch.DrawString(ScreenManager.Font, "New Best Time!", new Vector2(220, 350), Color.White);
                spriteBatch.DrawString(ScreenManager.Font, $"{_timer.Minutes:D2}:{_timer.Seconds:D2}", new Vector2(290, 400), Color.White);
            }
            spriteBatch.End();

            if (_newHighScore)
            {
                ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                ScreenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                _trophy.Draw(gameTime);

                ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            }
        }
    }
}
