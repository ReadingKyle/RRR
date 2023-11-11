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
using SharpDX.Direct2D1.Effects;

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
        private Tilemap _quickPlay;
        private Car _car1;
        private Car _car2;

        private ExplosionParticleSystem _explosions;

        bool gameStart;
        bool trackSelection;
        bool quickPlaySelection;

        bool _isTwoPlayer;

        public MenuScreen(bool isTwoPlayer)
        {
            _isTwoPlayer = isTwoPlayer;
        }

        public override void Activate()
        {
            base.Activate();

            _gameState = LoadGameState();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _tilemap = _content.Load<Tilemap>("menu");
            _trackselect = _content.Load<Tilemap>("trackselect");
            _quickPlay = _content.Load<Tilemap>("quickplay");
            _car1 = new Car(_content.Load<Texture2D>("tiles"), new Vector2(310, 450), PlayerIndex.One);
            _car2 = new Car(_content.Load<Texture2D>("tiles"), new Vector2(340, 450), PlayerIndex.Two);
            if (_isTwoPlayer) _car2.Exists = true;

            _explosions = new ExplosionParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_explosions);
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
            if (trackSelection)
            {
                triggeredCollision = _trackselect.Update(gameTime, car);
            }
            else if (quickPlaySelection)
            {
                triggeredCollision = _quickPlay.Update(gameTime, car);
            }
            else
            {
                triggeredCollision = _tilemap.Update(gameTime, car);
            }
            if (triggeredCollision != 0)
            {
                car.isColliding = true;
            }
            if (triggeredCollision == 1)
            {
                _explosions.PlaceExplosion(new Vector2(car.Position.X, car.Position.Y));
                car.Position = car.InitialPosition;
            }
            else if (triggeredCollision == 2)
            {
                trackSelection = true;
            }
            else if (triggeredCollision == 3)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track1", _car2.Exists, true, true));
            }
            else if (triggeredCollision == 4)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track2", _car2.Exists, true, true));
            }
            else if (triggeredCollision == 5)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track3", _car2.Exists, true, true));
            }
            else if (triggeredCollision == 6)
            {
                LoadingScreen.Load(ScreenManager, false, new Track("track4", _car2.Exists, true, true));
            }
            else if (triggeredCollision == -1)
            {
                if (trackSelection) trackSelection = false;
                else if (quickPlaySelection) quickPlaySelection = false;
                else ScreenManager.Game.Exit();
            }
            else if (triggeredCollision == 7)
            {
                quickPlaySelection = true;
            }
            else if (triggeredCollision == 8)
            {
                car.Position.Y = car.PreviousPosition.Y;
                car.speed = car.speed / 5;
                car.acceleration = car.acceleration / 5;
                _explosions.PlaceExplosion(new Vector2(car.Position.X, car.Position.Y));
                car.Position = car.InitialPosition;
            }
            else if (triggeredCollision == 9)
            {
                car.Position.X = car.PreviousPosition.X;
                car.speed = car.speed / 5;
                car.acceleration = car.acceleration / 5;
                _explosions.PlaceExplosion(new Vector2(car.Position.X, car.Position.Y));
                car.Position = car.InitialPosition;
            }
            else if (triggeredCollision == 10)
            {
                car.Position.X = car.PreviousPosition.X;
                car.Position.Y = car.PreviousPosition.Y;
                car.speed = car.speed / 5;
                car.acceleration = car.acceleration / 5;
                _explosions.PlaceExplosion(new Vector2(car.Position.X, car.Position.Y));
                car.Position = car.InitialPosition;
            }
            else if (triggeredCollision == 11)
            {
                Random random = new Random();
                int randomTrack = random.Next(1, 11);
                LoadingScreen.Load(ScreenManager, false, new Track("track" + randomTrack, _car2.Exists, false, true));
            }
            else if (triggeredCollision == 12)
            {
                Random random = new Random();
                int randomTrack = random.Next(1, 11);
                LoadingScreen.Load(ScreenManager, false, new Track("track" + randomTrack, _car2.Exists, false, false));
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
            UpdateCar(_car1, gameTime);
            if (_car2.Exists)
            {
                UpdateCar(_car2, gameTime);
            }

            if (_car2.Exists)
            {
                if (Math.Abs(_car1.speed) >= Math.Abs(_car2.speed)) HandleCarCollisions(_car1, _car2);
                else HandleCarCollisions(_car2, _car1);
            }

            if (!_car2.Exists && GamePad.GetState(PlayerIndex.Two).Buttons.A > 0)
            {
                _car2.Exists = true;
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
            if (!trackSelection && !quickPlaySelection)
            {
                _tilemap.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(ScreenManager.BigFont, "Retro Reading Racing", new Vector2(80, 160), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, "Use a controller\nDon't fall off the map\nHave fun!", new Vector2(80, 496), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, "Made by Kyle Reading", new Vector2(384, 496), Color.Black);
            }
            else if (trackSelection)
            {
                _trackselect.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackOneHighScore.Minutes:D2}:{_gameState.TrackOneHighScore.Seconds:D2}", new Vector2(80, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackTwoHighScore.Minutes:D2}:{_gameState.TrackTwoHighScore.Seconds:D2}", new Vector2(208, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackThreeHighScore.Minutes:D2}:{_gameState.TrackThreeHighScore.Seconds:D2}", new Vector2(336, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.SmallFont, $"Lowest Time:\n{_gameState.TrackFourHighScore.Minutes:D2}:{_gameState.TrackFourHighScore.Seconds:D2}", new Vector2(464, 208), Color.Black);
                spriteBatch.DrawString(ScreenManager.Font, "Track Selection", new Vector2(80, 496), Color.Black);
            }
            else
            {
                _quickPlay.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(ScreenManager.Font, "Multiplayer", new Vector2(80, 496), Color.Black);
            }
            if (_car2.Exists)
            {
                _car2.Draw(spriteBatch);
                spriteBatch.DrawString(ScreenManager.SmallFont, "Player 2", new Vector2(500, 600), Color.DodgerBlue);
            }
            else
            {
                spriteBatch.DrawString(ScreenManager.SmallFont, "Player 2: Press A to join", new Vector2(384, 600), Color.DodgerBlue);
            }
            spriteBatch.DrawString(ScreenManager.SmallFont, "Player 1", new Vector2(80, 600), Color.Red);
            _car1.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
