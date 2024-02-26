using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Final.GameObjects.UIs
{
    /// <summary>
    /// Menu Class
    /// </summary>
    public class MenuComponent : DrawableGameComponent
    {
        // Rendering components
        private SpriteBatch spriteBatch;

        // Fonts
        private SpriteFont regularFont, highlightFont;

        // Menu items and styling
        private List<string> menuItemList;
        public int SelectedIndex { get; set; }
        private Vector2 menuItemsStartPosition;
        private Color regularColor = Color.PaleVioletRed;
        private Color highlightColor = Color.DarkViolet;

        // Input handling
        private KeyboardState oldKeyboardState;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="startSceneSpriteBatch"></param>
        /// <param name="regularFont">regular style font</param>
        /// <param name="highlightfont">highlight style font</param>
        /// <param name="menuArray">menu items</param>
        public MenuComponent(Game game, SpriteBatch startSceneSpriteBatch, SpriteFont regularFont, SpriteFont highlightfont, string[] menuArray) : base(game)
        {
            spriteBatch = startSceneSpriteBatch;
            this.regularFont = regularFont;
            highlightFont = highlightfont;
            menuItemList = menuArray.ToList();
            menuItemsStartPosition = new Vector2(Shared.stageSize.X / 2, Shared.stageSize.Y / 2);
        }
        public override void Update(GameTime gameTime)
        {

            HandleKeyboardInput();
            base.Update(gameTime);

            void HandleKeyboardInput()
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();

                // Handle Down key
                if (currentKeyboardState.IsKeyUp(Keys.Down) && oldKeyboardState.IsKeyDown(Keys.Down))
                {
                    SelectedIndex = (SelectedIndex + 1) % menuItemList.Count;
                }

                // Handle Up key
                if (currentKeyboardState.IsKeyUp(Keys.Up) && oldKeyboardState.IsKeyDown(Keys.Up))
                {
                    SelectedIndex = (SelectedIndex - 1 + menuItemList.Count) % menuItemList.Count;
                }
                oldKeyboardState = currentKeyboardState;

            }
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            DrawMenuItems();
            spriteBatch.End();
            base.Draw(gameTime);

            void DrawMenuItems()
            {
                Vector2 drawingPosition = menuItemsStartPosition;

                for (int i = 0; i < menuItemList.Count; i++)
                {
                    SpriteFont usedFont = i == SelectedIndex ? highlightFont : regularFont;
                    Color usedColor = i == SelectedIndex ? highlightColor : regularColor;

                    float itemWidth = usedFont.MeasureString(menuItemList[i]).X;
                    drawingPosition.X = (Shared.stageSize.X - itemWidth) / 2;
                    spriteBatch.DrawString(usedFont, menuItemList[i], drawingPosition, usedColor);
                    drawingPosition.Y += usedFont.LineSpacing;
                }
            }
        }



    }
}
