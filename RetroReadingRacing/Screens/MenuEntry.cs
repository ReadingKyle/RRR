using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RetroReadingRacing.StateManagement;

namespace RetroReadingRacing.Screens
{
    // Helper class represents a single entry in a MenuScreen. By default this
    // just draws the entry text string, but it can be customized to display menu
    // entries in different ways. This also provides an event that will be raised
    // when the menu entry is selected.
    public class MenuEntry
    {
        private string _text;
        private Texture2D _texture;
        private float _selectionFade;    // Entries transition out of the selection effect when they are deselected
        private Vector2 _position;    // This is set by the MenuScreen each frame in Update

        private readonly List<MenuEntry> _menuEntries = new List<MenuEntry>();
        protected IList<MenuEntry> MenuEntries => _menuEntries;

        public string Text
        {
            private get => _text;
            set => _text = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
        }

        public MenuEntry(Texture2D texture)
        {
             _texture = texture;
        }

        public MenuEntry(string text)
        {
            _text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            if (isSelected) return; // add selection
        }


        // This can be overridden to customize the appearance.
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;
            int selectedFrame = isSelected ? 64 : 0;

            if (_texture != null)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(_texture, new Vector2(960, 960), new Rectangle(0 + selectedFrame, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            if (Text != null)
            {
                return (int)screen.ScreenManager.Font.MeasureString(Text).X;
            }
            else
            {
                return -1;
            }
        }
    }
}
