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
        private SpriteBatch menuComponentSpriteBatch;
        private SpriteFont regularFont, highlightFont;
        private List<string> menuItemList;

        public int SelectedIndex { get; set; }
        private Vector2 menuItemsStartPosition;
        private Color regularColor = Color.PaleVioletRed;
        private Color highlightColor = Color.DarkViolet;

        private KeyboardState oldKeyboardState;
        public MenuComponent(Game game, SpriteBatch startSceneSpriteBatch, SpriteFont regularFont, SpriteFont highlightFont, string[] menuArray) : base(game)
        {
            this.menuComponentSpriteBatch = startSceneSpriteBatch;
            this.regularFont = regularFont;
            this.highlightFont = highlightFont;
            menuItemList = menuArray.ToList();
            menuItemsStartPosition = new Vector2(Shared.stageSize.X/2, Shared.stageSize.Y/2);
        }
        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyUp(Keys.Down) && oldKeyboardState.IsKeyDown(Keys.Down))
            {
                SelectedIndex++;
                if (SelectedIndex == menuItemList.Count)
                {
                    SelectedIndex = 0;
                }
            }

            if (currentKeyboardState.IsKeyUp(Keys.Up) && oldKeyboardState.IsKeyDown(Keys.Up))
            {
                SelectedIndex--;
                if (SelectedIndex == -1)
                {
                    SelectedIndex = menuItemList.Count - 1;
                }
            }
            oldKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Vector2 drawingPosition = menuItemsStartPosition;

            menuComponentSpriteBatch.Begin();

            for (int i = 0; i < menuItemList.Count; i++)
            {
                float itemWidth = regularFont.MeasureString(menuItemList[i]).X;
                drawingPosition.X = (Shared.stageSize.X - itemWidth) / 2;

                if (i == SelectedIndex)
                {
                    itemWidth = highlightFont.MeasureString(menuItemList[i]).X;
                    drawingPosition.X = (Shared.stageSize.X - itemWidth) / 2;
                    menuComponentSpriteBatch.DrawString(highlightFont, menuItemList[i], drawingPosition, highlightColor);
                    drawingPosition.Y += highlightFont.LineSpacing;

                }
                else
                {
                    menuComponentSpriteBatch.DrawString(regularFont, menuItemList[i], drawingPosition, regularColor);
                    drawingPosition.Y += regularFont.LineSpacing;
                }
            }
            menuComponentSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
