using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{

    public class BossHelicopter : DrawableGameComponent
    {
        private SpriteBatch bossHelicopterSpriteBatch;
        private Texture2D aliveBossHelicopterTexture;
        private Vector2 aliveBossFrameDimension;
        private List<Rectangle> aliveBossAnimationFrame;
        private Vector2 currentPosition;
        private const int ALIVE_BOSS_HELICOPTER_ROWS = 4;
        private bool isStartSequence;
        private const float entirySpeed = 0.7f;
        private int currentFrameIndex = -1;


        public BossHelicopter(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            
            bossHelicopterSpriteBatch = playSceneSpriteBatch;

            aliveBossHelicopterTexture = game.Content.Load<Texture2D>("images/firstStageBossHelicopter");
            aliveBossFrameDimension = new Vector2(aliveBossHelicopterTexture.Width / ALIVE_BOSS_HELICOPTER_ROWS, aliveBossHelicopterTexture.Height);
            isStartSequence = true;
            aliveBossAnimationFrame = new List<Rectangle>();
            InitializeAnimationFrames();
            currentPosition = new Vector2(Shared.stageSize.X / 2, -aliveBossFrameDimension.Y);
        }

        public void InitializeAnimationFrames()
        {
            for (int r = 0; r < ALIVE_BOSS_HELICOPTER_ROWS; r++)
            {
                    int x = r * (int)aliveBossFrameDimension.X;

                    aliveBossAnimationFrame.Add(new Rectangle(x, 0, (int)aliveBossFrameDimension.X, (int)aliveBossFrameDimension.Y));
            }

        }

        public void Hide()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void Show()
        {
            this.Enabled = true;
            this.Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            int limitBoosYPosition = 30;

            Action<int> changeStartSequence = (x) => { if (currentPosition.Y >= x) isStartSequence = false; };

            currentFrameIndex = currentFrameIndex == aliveBossAnimationFrame.Count() - 1 ? 0 : ++currentFrameIndex;
            currentPosition.Y = isStartSequence == true ? currentPosition.Y + entirySpeed : currentPosition.Y;
            
            changeStartSequence(limitBoosYPosition);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            bossHelicopterSpriteBatch.Begin();
            bossHelicopterSpriteBatch.Draw(aliveBossHelicopterTexture, currentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.White);
            bossHelicopterSpriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
