using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    internal class LeaderBoardScene : GameScene
    {
        private SpriteBatch leaderBoardSceneSpriteBatch;
        private SpriteFont titleFont;
        private Texture2D backgroundTexture;
        private string titleText = "Top 5";
        private SpriteFont regularFont;
        private SpriteFont hilightFont;
        private Texture2D transparentBackground;

        private List<(string Initials, int Score)> topScores;

        public LeaderBoardScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            leaderBoardSceneSpriteBatch = mainGame._spriteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
            transparentBackground.SetData(new[] { Color.Black });


            topScores = new List<(string Initials, int Score)>();
            LoadTopScores();
        }

        private void LoadTopScores()
        {
            string filePath = "scores.txt";
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i += 2)
                {
                    string initials = lines[i];
                    int score = int.Parse(lines[i + 1]);
                    topScores.Add((initials, score));
                }

                topScores = topScores.OrderByDescending(s => s.Score).Take(5).ToList();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            leaderBoardSceneSpriteBatch.Begin();
            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
            leaderBoardSceneSpriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            leaderBoardSceneSpriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            leaderBoardSceneSpriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);


            int yPosition = 350; 
            int rank = 1;
            foreach (var score in topScores)
            {
                string scoreText = $"#{rank}: {score.Initials} - {score.Score}";
                Vector2 scorePosition = new Vector2((Shared.stageSize.X / 2) - (hilightFont.MeasureString(scoreText).X / 2), yPosition);
                leaderBoardSceneSpriteBatch.DrawString(hilightFont, scoreText, scorePosition, Color.White);

                yPosition += 80; 
                rank++;
            }

            leaderBoardSceneSpriteBatch.End();

        }
    }
}
