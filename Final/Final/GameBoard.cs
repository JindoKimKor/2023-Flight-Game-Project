using Final.Scenes;
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


        public GameBoard(Game game, SpriteBatch playSceneSpirteBatch) : base(game)
        {
            gameBoardSpriteBatch = playSceneSpirteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            gameBoardSpriteBatch.Begin();
            float outline = 2.0f;
            Vector2 hittenCountPosition = new Vector2(30f, 780f);
            Vector2 enemyKillCountPosition = new Vector2(30f, 830f);
            Vector2 timeCountPosition = new Vector2(30f, 880f);

            for (float x = -outline; x <= outline; x += 1)
            {
                for (float y = -outline; y <= outline; y += 1)
                {

                    gameBoardSpriteBatch.DrawString(regularFont, $"Hitten: {PlayScene.NumberOfGotHit}", hittenCountPosition + new Vector2(x, y), Color.Black);
                    gameBoardSpriteBatch.DrawString(regularFont, $"Enemy Kill: {PlayScene.NumberOfDestoryedSmallHelicopter}", enemyKillCountPosition + new Vector2(x, y), Color.Black);
                    gameBoardSpriteBatch.DrawString(regularFont, $"Time: {PlayScene.TimeString}", timeCountPosition + new Vector2(x, y), Color.Black);
                }

            }

            gameBoardSpriteBatch.DrawString(regularFont, $"Hitten: {PlayScene.NumberOfGotHit}", hittenCountPosition, Color.White);
            gameBoardSpriteBatch.DrawString(regularFont, $"Enemy Kill: {PlayScene.NumberOfDestoryedSmallHelicopter}", enemyKillCountPosition, Color.White);
            gameBoardSpriteBatch.DrawString(regularFont, $"Time: {PlayScene.TimeString}", timeCountPosition, Color.White);
            gameBoardSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
