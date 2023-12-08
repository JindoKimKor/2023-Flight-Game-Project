using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class GameBoard : DrawableGameComponent
    {
        private SpriteBatch gameBoardSpriteBatch;
        private SpriteFont regularFont;
        private static int numberOfDestoryedSmallHelicopter;
        private static int numberOfGotHit;

        public static int NumberOfDestoryedSmallHelicopter { get => numberOfDestoryedSmallHelicopter; set => numberOfDestoryedSmallHelicopter = value; }
        public static int NumberOfGotHit { get => numberOfGotHit; set => numberOfGotHit = value; }

        public GameBoard(Game game, SpriteBatch playSceneSpirteBatch) : base(game)
        {
            gameBoardSpriteBatch = playSceneSpirteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            gameBoardSpriteBatch.Begin();

            gameBoardSpriteBatch.DrawString(regularFont, $"The Number of Hitten: {numberOfGotHit}", new Vector2(50f, 50f), Color.White);
            gameBoardSpriteBatch.DrawString(regularFont, $"The Number of Small Helicopter: {numberOfDestoryedSmallHelicopter}", new Vector2(70f, 70f), Color.White);
            gameBoardSpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
