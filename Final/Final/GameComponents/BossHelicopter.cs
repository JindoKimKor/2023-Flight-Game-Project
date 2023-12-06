using Final.Scenes;
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

        //Boss Helicopter
        private Texture2D aliveBossHelicopterTexture;
        private Vector2 aliveBossFrameDimension;
        private List<Rectangle> aliveBossAnimationFrame;
        private static Vector2 bossHelicopterCurrentPosition;
        private Vector2 textureOrigin;
        private const int ALIVE_BOSS_HELICOPTER_COLS = 4;
        private bool isStartSequence;
        private const float entirySpeed = 0.7f;
        private int currentFrameIndex = -1;
        private int finalYPosition = 100;

        public bool IsStartSequence { get => isStartSequence; set => isStartSequence = value; }
        public static Vector2 BossHelicopterCurrentPosition { get => bossHelicopterCurrentPosition; set => bossHelicopterCurrentPosition = value; }

        public bool IsGotHit { get => isGotHit; set => isGotHit = value; }

        public BossHelicopter(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {
            
            bossHelicopterSpriteBatch = playSceneSpriteBatch;

            BossHelicopterCurrentPosition = new Vector2(Shared.stageSize.X / 2, -aliveBossFrameDimension.Y);

            aliveBossHelicopterTexture = game.Content.Load<Texture2D>("images/firstStageBossHelicopter");

            aliveBossFrameDimension = new Vector2(aliveBossHelicopterTexture.Width / ALIVE_BOSS_HELICOPTER_COLS, aliveBossHelicopterTexture.Height);
            textureOrigin = new Vector2(aliveBossFrameDimension.X / 2, aliveBossFrameDimension.Y / 2);
            IsStartSequence = true;
            aliveBossAnimationFrame = new List<Rectangle>();

            for (int c = 0; c < ALIVE_BOSS_HELICOPTER_COLS; c++)
            {
                int x = c * (int)aliveBossFrameDimension.X;

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

        private double gettingNewXCoordinateElapsedTime = 0;
        private double frameInterval = 1000;
        private float newXCoordinate;
        private bool isGotHit;

        public override void Update(GameTime gameTime)
        {
            Action<int> changeStartSequence = (x) => { if (BossHelicopterCurrentPosition.Y >= x) IsStartSequence = false; };

            currentFrameIndex = currentFrameIndex == aliveBossAnimationFrame.Count() - 1 ? 0 : ++currentFrameIndex;
            bossHelicopterCurrentPosition.Y = IsStartSequence == true ? BossHelicopterCurrentPosition.Y + entirySpeed : BossHelicopterCurrentPosition.Y;
            
            changeStartSequence(finalYPosition);
            //Adjust X coordinate according to fighter aircraft X coordinate

            gettingNewXCoordinateElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (gettingNewXCoordinateElapsedTime >= frameInterval)
            {
                newXCoordinate = PlayScene.FighterAircraftCurrentPosition.X;
                gettingNewXCoordinateElapsedTime = 0;
            }
            if (bossHelicopterCurrentPosition.X > newXCoordinate)
            {
                bossHelicopterCurrentPosition.X--;
            }
            else if (bossHelicopterCurrentPosition.X < newXCoordinate)
            {
                bossHelicopterCurrentPosition.X++;
            }
            
            base.Update(gameTime);
        }

        private double hitEffectDuration = 500;
        private double hitEffectTimer = 0;
        public override void Draw(GameTime gameTime)
        {

            bossHelicopterSpriteBatch.Begin();

            if (IsGotHit)
            {
                hitEffectTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectTimer <= 0)
                {
                    IsGotHit = false;
                }
                bossHelicopterSpriteBatch.Draw(aliveBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.Red, 0f, textureOrigin, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                bossHelicopterSpriteBatch.Draw(aliveBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.White, 0f, textureOrigin, 1f, SpriteEffects.None, 0f);
            }
            bossHelicopterSpriteBatch.End();
            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int margin = 50;

            return new Rectangle((int)BossHelicopterCurrentPosition.X + (margin / 2), (int)BossHelicopterCurrentPosition.Y, (int)aliveBossFrameDimension.X - (margin * 2), (int)aliveBossFrameDimension.Y);
        }

    }
}
