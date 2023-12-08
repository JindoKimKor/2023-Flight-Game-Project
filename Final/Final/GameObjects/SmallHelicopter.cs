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
    public delegate void RemovePassedOrExpolosedDelegate(SmallHelicopter smallHelicopter);

    public class SmallHelicopter : DrawableGameComponent
    {
        private MainGame mainGame;
        private SpriteBatch smallHelicopterSpriteBatch;
        private Texture2D smallHelicopterTexture;
        private Vector2 frameDimension;
        private List<Rectangle> animationFrames;
        private int currentFrameIndex;
        private Random random;
        private Vector2 currentPosition;
        private Vector2 textureOrigin;
        private const int SMALL_HELICOPTER_COLS = 5;

        private float movingSpeed;
        private int randomXPosition;
        private const int maxHealthCount = 1;
        private int hitCount = 0;

        private bool isGotHit = false;
        private bool isDestroyed = false;

        //destroyed
        private Texture2D destroyedTexture;
        private Vector2 destroyFrameDimension;
        private List<Rectangle> destroyAnimationFrames;
        private const int DESTROY_ANIMATION_COLS = 7;
        private int destroyedTextureIndex = 0;

        //generating bullet
        PlayScene playScene;
        
        public bool IsGotHit { get => isGotHit; set => isGotHit = value; }

        public RemovePassedOrExpolosedDelegate RemovePassedOrExpolosed { get; set; }
        public Vector2 CurrentPosition { get => currentPosition; set => currentPosition = value; }

        public SmallHelicopter(Game game, SpriteBatch playSceneSpriteBatch, PlayScene playScene) : base(game)
        {
            mainGame = (MainGame)game;
            smallHelicopterSpriteBatch = playSceneSpriteBatch;
            smallHelicopterTexture = mainGame.Content.Load<Texture2D>("images/smallHelicopter");
            frameDimension = new Vector2(smallHelicopterTexture.Width / SMALL_HELICOPTER_COLS, smallHelicopterTexture.Height);
            textureOrigin = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            animationFrames = new List<Rectangle>();
            random = new Random();
            randomXPosition = random.Next(0, 2);
            int randomYPosition = random.Next(170, 450);
            movingSpeed = random.Next(2, 4);
            currentPosition = new Vector2(randomXPosition == 0 ? 0 : Shared.stageSize.X, randomYPosition);

            for (int c = 0; c < SMALL_HELICOPTER_COLS; c++)
            {
                int x = c * (int)frameDimension.X;
                animationFrames.Add(new Rectangle(x, 0, (int)frameDimension.X, (int)frameDimension.Y));
            }

            PlayScene.SmallHelicopterList.Add(this);

            //destroying
            destroyedTexture = mainGame.Content.Load<Texture2D>("images/destroyAnimation");
            destroyFrameDimension = new Vector2(destroyedTexture.Width / DESTROY_ANIMATION_COLS, destroyedTexture.Height);
            destroyAnimationFrames = new List<Rectangle>();

            for (int c = 0; c < DESTROY_ANIMATION_COLS; c++)
            {
                int x = c * (int)destroyFrameDimension.X;

                destroyAnimationFrames.Add(new Rectangle(x, 0, (int)destroyFrameDimension.X, (int)destroyFrameDimension.Y));
            }

            this.playScene = playScene;

        }

        private float destroyeGeneratingElapsedTime = 0;

        public override void Update(GameTime gameTime)
        {
            destroyeGeneratingElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


            currentFrameIndex = currentFrameIndex == animationFrames.Count() - 1 ? 0 : ++currentFrameIndex;
            if (randomXPosition == 0)
            {
                if (!isDestroyed)
                {
                    currentPosition.X += movingSpeed;
                }
            }
            else
            {
                if (!isDestroyed)
                {
                    currentPosition.X -= movingSpeed;
                }
            }
            if ((randomXPosition == 0 && currentPosition.X == Shared.stageSize.X) ||
                (randomXPosition == 1 && currentPosition.X == 0)
                )
            {
                RemovePassedOrExpolosed?.Invoke(this);

            }
            if (hitCount >= maxHealthCount)
            {
                isDestroyed = true;
            }

            //if It got destroyed
            if (isDestroyed)
            {
                if (destroyeGeneratingElapsedTime >= 0.3f)
                {
                    destroyedTextureIndex++;
                    destroyeGeneratingElapsedTime = 0f;
                }
            }
            if (destroyedTextureIndex >= DESTROY_ANIMATION_COLS - 1)
            {
                RemovePassedOrExpolosed?.Invoke(this);
                GameBoard.NumberOfDestoryedSmallHelicopter++;
            }

            base.Update(gameTime);
        }

        private double elapsedTime = 0;
        private double frameInterval = 400;
        private double hitEffectTimer = 0.005;
        public override void Draw(GameTime gameTime)
        {
            smallHelicopterSpriteBatch.Begin();

            if (isDestroyed)
            {
                Vector2 adjustPosition = new Vector2(currentPosition.X + 40, currentPosition.Y + 50);
                smallHelicopterSpriteBatch.Draw(destroyedTexture, adjustPosition, destroyAnimationFrames[destroyedTextureIndex], Color.White, 0f, textureOrigin, 1.2f, SpriteEffects.None, 0f);
            }
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!isDestroyed)
            {
                if (elapsedTime >= frameInterval)
                {
                    SmallHelicopterBullet smallHelicopterBullet = new SmallHelicopterBullet(mainGame, smallHelicopterSpriteBatch, this);
                    playScene.ComponentList.Add(smallHelicopterBullet);
                    elapsedTime = 0;
                }
            }

            if (IsGotHit && !isDestroyed)
            {
                hitEffectTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectTimer <= 0)
                {
                    IsGotHit = false;
                    hitCount++;
                }
                smallHelicopterSpriteBatch.Draw(smallHelicopterTexture, currentPosition, animationFrames[currentFrameIndex], Color.Red, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }
            else if(!IsGotHit && !isDestroyed)
            {
                smallHelicopterSpriteBatch.Draw(smallHelicopterTexture, currentPosition, animationFrames[currentFrameIndex], Color.White, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);

            }


            smallHelicopterSpriteBatch.End();

            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameDimension.X * 0.2f);
            int scaledHeight = (int)(frameDimension.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
