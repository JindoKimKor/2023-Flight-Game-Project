using Final.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    public class StartScene : GameScene
    {
        private MenuComponent menuComponent;
        private SpriteBatch startSceneSpriteBatch;
        private SpriteFont titleFont;
        private Texture2D backgroundTexture;
        private string titleText = "2023";


        public MenuComponent MenuComponent { get => menuComponent; set => menuComponent = value; }

        public StartScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            startSceneSpriteBatch = mainGame._spriteBatch;
            SpriteFont regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            SpriteFont highlightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");

            string[] menuItems = { "Start Game", "Help", "Leaderboard", "Credit", "Option", "Exit" };
            menuComponent = new MenuComponent(game, startSceneSpriteBatch, regularFont, highlightFont, menuItems);
            ComponentList.Add(menuComponent);
        }
        public override void Draw(GameTime gameTime)
        {
            startSceneSpriteBatch.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Game.GraphicsDevice.Viewport.Width - titleSize.X) / 2, 100);
            startSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            startSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            startSceneSpriteBatch.End();
            base.Draw(gameTime);

        }
    }
}
