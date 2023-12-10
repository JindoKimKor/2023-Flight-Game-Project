using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final.GameObjects
{
    /// <summary>
    /// GameBoard Class in the play
    /// </summary>
    public class GameBoard : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteFont regularFont;

        /// <summary>
        /// GameBoard Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playSceneSpirteBatch"></param>
        public GameBoard(Game game, SpriteBatch playSceneSpirteBatch) : base(game)
        {
            spriteBatch = playSceneSpirteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont"); ;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            DrawTextOutline();

            DrawText();

            spriteBatch.End();
            base.Draw(gameTime);


            void DrawTextOutline()
            {
                float outline = 2.0f;
                Vector2 hittenCountPosition = new Vector2(30f, 780f);
                Vector2 enemyKillCountPosition = new Vector2(30f, 830f);
                Vector2 timeCountPosition = new Vector2(30f, 880f);

                for (float x = -outline; x <= outline; x += 1)
                {
                    for (float y = -outline; y <= outline; y += 1)
                    {
                        DrawTextWithOutline(hittenCountPosition + new Vector2(x, y), $"Hitten: {PlayScene.NumberOfGotHit}", Color.Black);
                        DrawTextWithOutline(enemyKillCountPosition + new Vector2(x, y), $"Enemy Kill: {PlayScene.NumberOfDestoryedSmallHelicopter}", Color.Black);
                        DrawTextWithOutline(timeCountPosition + new Vector2(x, y), $"Time: {PlayScene.TimeString}", Color.Black);
                    }
                }

                void DrawTextWithOutline(Vector2 position, string text, Color color)
                {
                    spriteBatch.DrawString(regularFont, text, position, color);
                }
            }

            void DrawText()
            {
                Vector2 hittenCountPosition = new Vector2(30f, 780f);
                Vector2 enemyKillCountPosition = new Vector2(30f, 830f);
                Vector2 timeCountPosition = new Vector2(30f, 880f);

                spriteBatch.DrawString(regularFont, $"Hitten: {PlayScene.NumberOfGotHit}", hittenCountPosition, Color.White);
                spriteBatch.DrawString(regularFont, $"Enemy Kill: {PlayScene.NumberOfDestoryedSmallHelicopter}", enemyKillCountPosition, Color.White);
                spriteBatch.DrawString(regularFont, $"Time: {PlayScene.TimeString}", timeCountPosition, Color.White);
            }

        }

    }
}

