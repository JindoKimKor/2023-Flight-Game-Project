using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    internal class CreditScene : GameScene
    {
        private SpriteBatch creditSceneSpritebatch;
        private SpriteFont titleFont;
        private Texture2D backgroundTexture;
        private string titleText = "2023";
        string createdBy = "Created By";
        string creator1 = "Jindo Kim";
        string creator2 = "Sangkwon Kim";
        SpriteFont regularFont;
        SpriteFont hilightFont;
        private Texture2D transparentBackground;


        public CreditScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            creditSceneSpritebatch = mainGame._spriteBatch;
            regularFont = game.Content.Load<SpriteFont>("fonts/RegularFont");
            hilightFont = game.Content.Load<SpriteFont>("fonts/HighlightFont");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");

            transparentBackground = new Texture2D(GraphicsDevice, 1, 1);
            transparentBackground.SetData(new[] { Color.Black });
        }
        
        public override void Draw(GameTime gameTime)
        {
            creditSceneSpritebatch.Begin();

            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((Shared.stageSize.X - titleSize.X) / 2, 100);

            Vector2 subTitleSize = hilightFont.MeasureString(createdBy);
            Vector2 SubtitlePosition = new Vector2((Shared.stageSize.X - subTitleSize.X) / 2, 300);

            Vector2 creator1Size = regularFont.MeasureString(creator1);
            Vector2 creator2Size = regularFont.MeasureString(creator2);
            Vector2 creator1Position = new Vector2((Shared.stageSize.X - creator1Size.X) / 2, 400);
            Vector2 creator2Position = new Vector2((Shared.stageSize.X - creator2Size.X) / 2, 450);



            creditSceneSpritebatch.Draw(backgroundTexture, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White);
            creditSceneSpritebatch.Draw(transparentBackground, new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y), Color.White * 0.6f);

            creditSceneSpritebatch.DrawString(titleFont, titleText, titlePosition, Color.BlueViolet);

            creditSceneSpritebatch.DrawString(hilightFont, createdBy, SubtitlePosition, Color.DarkViolet);

            creditSceneSpritebatch.DrawString(regularFont, creator1, creator1Position, Color.PaleVioletRed);
            creditSceneSpritebatch.DrawString(regularFont, creator2, creator2Position, Color.PaleVioletRed);

            creditSceneSpritebatch.End();
            base.Draw(gameTime);
        }
    }
}
