using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Final.Scenes
{
    /// <summary>
    /// Leader Board Scene
    /// </summary>
    public class LeaderBoardScene : GameScene
    {
        // Rendering components
        private SpriteBatch spriteBatch;

        // Fonts for text rendering
        private SpriteFont titleFont;
        private SpriteFont highlightFont;

        // Textures for scene visuals
        private Texture2D backgroundTexture;
        private Texture2D transparentBackground;

        // Scene specific fields
        private string titleText = "Top 5";
        private List<(string Initials, int Score)> topScores;
        private string scoresFilePath = "scores.txt";
        private Vector2 titlePosition;
        private Vector2 titleSize;

        // Constants for layout
        private const int ScoreStartYPosition = 350;
        private const int ScoreYPositionIncrement = 80;

        public LeaderBoardScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            spriteBatch = mainGame.SpriteBatch;
            LoadContent();
            void LoadContent()
            {
                highlightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
                titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
                backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            }
            InitializeTransparentBackground();
            void InitializeTransparentBackground()
            {
                transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
                transparentBackground.SetData(new[] { Color.Black });
            }

        }

        /// <summary>
        /// Load current data
        /// </summary>
        public void LoadTopScores()
        {
            topScores = new List<(string Initials, int Score)>();

            if (File.Exists(scoresFilePath))
            {
                var lines = File.ReadAllLines(scoresFilePath);
                for (int i = 0; i < lines.Length; i += 2)
                {
                    string initials = lines[i];
                    int score = int.Parse(lines[i + 1]);
                    topScores.Add((initials, score));
                    topScores = topScores.OrderByDescending(s => s.Score).Take(5).ToList();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            DrawBackground();
            void DrawBackground()
            {
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
                spriteBatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);
            }
            DrawTitle();
            void DrawTitle()
            {
                titleSize = titleFont.MeasureString(titleText);
                titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);
                spriteBatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);
            }
            DrawScores();
            void DrawScores()
            {
                int yPosition = ScoreStartYPosition;
                int rank = 1;
                foreach (var score in topScores)
                {
                    string scoreText = $"#{rank}: {score.Initials} - {score.Score}";
                    Vector2 scorePosition = new Vector2((Shared.stageSize.X / 2) - (highlightFont.MeasureString(scoreText).X / 2), yPosition);
                    spriteBatch.DrawString(highlightFont, scoreText, scorePosition, Color.White);

                    yPosition += ScoreYPositionIncrement;
                    rank++;
                }
            }
            spriteBatch.End();
        }
    }
}
