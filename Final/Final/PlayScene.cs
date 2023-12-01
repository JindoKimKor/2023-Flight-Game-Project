using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class PlayScene : GameScene
    {
        private SpriteBatch playSceneSpriteBatch;
        private Texture2D backgroundTexture;
        private Rectangle screenRectangleOne;
        private Rectangle screenRectangleTwo;
        private int textureScrollSpeed = 2;


        public PlayScene(Game game) : base(game)
        {
            MainGame mainGame = (MainGame)game;
            playSceneSpriteBatch = mainGame._spriteBatch;
            backgroundTexture = mainGame.Content.Load<Texture2D>("images/background");
            screenRectangleOne = new Rectangle(0, 0, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
            screenRectangleTwo = new Rectangle(0, -(int)Shared.stageSize.Y, (int)Shared.stageSize.X, (int)Shared.stageSize.Y);
        }


        public override void Update(GameTime gameTime)
        {
            screenRectangleOne.Y += textureScrollSpeed;
            screenRectangleTwo.Y += textureScrollSpeed;
            if (screenRectangleOne.Y >= (int)Shared.stageSize.Y)
            {
                screenRectangleOne.Y = 0;
            }
            if (screenRectangleTwo.Y >= 0)
            {
                screenRectangleTwo.Y = -(int)Shared.stageSize.Y;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            playSceneSpriteBatch.Begin();
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleOne, Color.White);
            playSceneSpriteBatch.Draw(backgroundTexture, screenRectangleTwo, Color.White);

            playSceneSpriteBatch.End();
            base.Draw(gameTime);
        }



    }
}
