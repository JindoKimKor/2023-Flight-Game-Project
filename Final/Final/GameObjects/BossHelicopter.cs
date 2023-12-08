using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Final.GameComponents
{

    public class BossHelicopter : DrawableGameComponent
    {
        public enum BossStage
        {
            firstStage,
            secondStage,
            destroyed
        }

        private SpriteBatch bossHelicopterSpriteBatch;

        //Boss Helicopter
        private Texture2D firstStageBossHelicopterTexture;
        private Texture2D secondStageBossHelicopterTexture;
        private Texture2D deadStageBossHelicopterTexture;
        
        private Vector2 aliveBossFrameDimension;
        private List<Rectangle> aliveBossAnimationFrame;
        private static Vector2 bossHelicopterCurrentPosition;
        private Vector2 textureOrigin;
        private const int ALIVE_BOSS_HELICOPTER_COLS = 4;
        private bool isStartSequence;
        private const float entirySpeed = 0.7f;
        private int currentFrameIndex = -1;
        private int finalYPosition = 100;
        private bool isGotHit;
        private int maxHealth = 10;
        private int secondStageHealth = 5;
        private int hitCount;
        public BossStage bossStage;

        public bool IsStartSequence { get => isStartSequence; set => isStartSequence = value; }
        public static Vector2 BossHelicopterCurrentPosition { get => bossHelicopterCurrentPosition; set => bossHelicopterCurrentPosition = value; }

        public bool IsGotHit { get => isGotHit; set => isGotHit = value; }


        public BossHelicopter(Game game, SpriteBatch playSceneSpriteBatch) : base(game)
        {

            bossHelicopterSpriteBatch = playSceneSpriteBatch;

            BossHelicopterCurrentPosition = new Vector2(Shared.stageSize.X / 2, -aliveBossFrameDimension.Y);

            firstStageBossHelicopterTexture = game.Content.Load<Texture2D>("images/firstStageBossHelicopter");
            secondStageBossHelicopterTexture = game.Content.Load<Texture2D>("images/secondStageBossHelicopter");
            deadStageBossHelicopterTexture = game.Content.Load<Texture2D>("images/deadBossHelicopter");
            
            aliveBossFrameDimension = new Vector2(firstStageBossHelicopterTexture.Width / ALIVE_BOSS_HELICOPTER_COLS, firstStageBossHelicopterTexture.Height);
            textureOrigin = new Vector2(aliveBossFrameDimension.X / 2, aliveBossFrameDimension.Y / 2);
            IsStartSequence = true;
            aliveBossAnimationFrame = new List<Rectangle>();

            for (int c = 0; c < ALIVE_BOSS_HELICOPTER_COLS; c++)
            {
                int x = c * (int)aliveBossFrameDimension.X;

                aliveBossAnimationFrame.Add(new Rectangle(x, 0, (int)aliveBossFrameDimension.X, (int)aliveBossFrameDimension.Y));
            }
            bossStage = BossStage.firstStage;


            
        }

        private double gettingNewXCoordinateElapsedTime = 0;
        private double frameInterval = 1000;
        private float newXCoordinate;
        //
        private Random random = new Random();
        private double shakeElapsedTime = 0;
        private double shakeInterval = 50;


        public override void Update(GameTime gameTime)
        {
            Action<int> changeStartSequence = (x) => { if (BossHelicopterCurrentPosition.Y >= x) IsStartSequence = false; };

            currentFrameIndex = currentFrameIndex == aliveBossAnimationFrame.Count() - 1 ? 0 : ++currentFrameIndex;
            bossHelicopterCurrentPosition.Y = IsStartSequence == true ? BossHelicopterCurrentPosition.Y + entirySpeed : BossHelicopterCurrentPosition.Y;

            changeStartSequence(finalYPosition);
            //Adjust X coordinate according to fighter aircraft X coordinate

            gettingNewXCoordinateElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (gettingNewXCoordinateElapsedTime >= frameInterval && (bossStage == BossStage.firstStage || bossStage == BossStage.secondStage))
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
            else if (bossStage == BossStage.destroyed)
            {
                shakeElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (shakeElapsedTime >= shakeInterval)
                {
                    bossHelicopterCurrentPosition.X += random.Next(-4, 6);
                    bossHelicopterCurrentPosition.Y += random.Next(-4, 6);

                    shakeElapsedTime = 0;
                }
            }

            //change boss stage
            if (hitCount >= secondStageHealth && hitCount < maxHealth)
            {
                bossStage = BossStage.secondStage;
            }
            else if (hitCount >= maxHealth)
            {
                bossStage = BossStage.destroyed;
            }

            base.Update(gameTime);
        }

        private double hitEffectTimer = 0.005;
        public override void Draw(GameTime gameTime)
        {

            bossHelicopterSpriteBatch.Begin();
            if (IsGotHit)
            {
                hitEffectTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectTimer <= 0)
                {
                    IsGotHit = false;
                    hitCount++;
                }
                if (hitCount >= secondStageHealth && hitCount < maxHealth)
                {
                    bossHelicopterSpriteBatch.Draw(secondStageBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.Red, 0f, textureOrigin, 0.9f, SpriteEffects.None, 0f);
                }

                else if (hitCount >= 0 && hitCount < secondStageHealth)
                {
                    bossHelicopterSpriteBatch.Draw(firstStageBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.Red, 0f, textureOrigin, 0.91f, SpriteEffects.None, 0f);
                }

            }
            else
            {
                if (hitCount >= secondStageHealth && hitCount < maxHealth)
                {
                    bossHelicopterSpriteBatch.Draw(secondStageBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.White, 0f, textureOrigin, 0.9f, SpriteEffects.None, 0f);
                }
                else if (hitCount >= 0 && hitCount < secondStageHealth)
                {
                    bossHelicopterSpriteBatch.Draw(firstStageBossHelicopterTexture, BossHelicopterCurrentPosition, aliveBossAnimationFrame[currentFrameIndex], Color.White, 0f, textureOrigin, 0.9f, SpriteEffects.None, 0f);
                }
                else if (hitCount >= maxHealth)
                {
                    IsGotHit = !IsGotHit;//flash effect
                    bossHelicopterSpriteBatch.Draw(deadStageBossHelicopterTexture, BossHelicopterCurrentPosition, new Rectangle(0, 0, deadStageBossHelicopterTexture.Width, deadStageBossHelicopterTexture.Height), Color.White, 0f, textureOrigin, 0.9f, SpriteEffects.None, 0f);
                }

            }

            bossHelicopterSpriteBatch.End();
            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(aliveBossFrameDimension.X * 0.9f);
            int scaledHeight = (int)(aliveBossFrameDimension.Y * 0.9f);

            return new Rectangle((int)BossHelicopterCurrentPosition.X, (int)BossHelicopterCurrentPosition.Y, scaledWidth, scaledHeight);
        }

    }
}
