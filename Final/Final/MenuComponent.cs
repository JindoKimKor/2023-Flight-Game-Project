using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class MenuComponent : DrawableGameComponent
    {
        private SpriteBatch sb;
        private SpriteFont regularFont, hilightFont;
        private List<string> menuItems;

        public int SelectedIndex { get; set; }
        private Vector2 position;
        private Color regularColor = Color.PaleVioletRed;
        private Color hilightColor = Color.DarkViolet;

        private KeyboardState oldState;
        public MenuComponent(Game game, SpriteBatch sb, SpriteFont regularFont, SpriteFont hilightFont, string[] menus) : base(game)
        {
            this.sb = sb;
            this.regularFont = regularFont;
            this.hilightFont = hilightFont;
            menuItems = menus.ToList();
            position = new Vector2(Shared.stage.X/2, Shared.stage.Y/2);
        }
        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyUp(Keys.Down) && oldState.IsKeyDown(Keys.Down))
            {
                SelectedIndex++;
                if (SelectedIndex == menuItems.Count)
                {
                    SelectedIndex = 0;
                }
            }

            if (ks.IsKeyUp(Keys.Up) && oldState.IsKeyDown(Keys.Up))
            {
                SelectedIndex--;
                if (SelectedIndex == -1)
                {
                    SelectedIndex = menuItems.Count - 1;
                }
            }
            oldState = ks;
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Vector2 tempPos = position;


            sb.Begin();
            for (int i = 0; i < menuItems.Count; i++)
            {
                float itemWidth = regularFont.MeasureString(menuItems[i]).X;
                tempPos.X = (Shared.stage.X - itemWidth) / 2;

                if (i == SelectedIndex)
                {
                    itemWidth = hilightFont.MeasureString(menuItems[i]).X;
                    tempPos.X = (Shared.stage.X - itemWidth) / 2;
                    sb.DrawString(hilightFont, menuItems[i], tempPos, hilightColor);
                    tempPos.Y += hilightFont.LineSpacing;

                }
                else
                {
                    sb.DrawString(regularFont, menuItems[i], tempPos, regularColor);
                    tempPos.Y += regularFont.LineSpacing;
                }
            }
            sb.End();

            base.Draw(gameTime);
        }
    }
}
