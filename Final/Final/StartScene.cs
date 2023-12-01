using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class StartScene : GameScene
    {
        private MenuComponent menuComponent;
        private SpriteBatch startSceneSpriteBatch;
        private SpriteFont titleFont;
        private string titleText = "2023";
        

        public MenuComponent MenuComponent { get => menuComponent; set => menuComponent = value; }

        public StartScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            this.startSceneSpriteBatch = mainGame._spriteBatch;
            SpriteFont regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            SpriteFont highlightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont"); 
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");


            string[] menuItems = { "Start Game", "Help", "Leaderboard", "Credit", "Option", "Exit" };
            menuComponent = new MenuComponent(game, startSceneSpriteBatch, regularFont, highlightFont, menuItems);
            this.ComponentList.Add(menuComponent);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            startSceneSpriteBatch.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Game.GraphicsDevice.Viewport.Width - titleSize.X) / 2, 100); 

            startSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            startSceneSpriteBatch.End();
        }
    }
}
