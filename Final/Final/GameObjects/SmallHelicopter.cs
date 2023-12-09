using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{
    public delegate void RemovePassedOrExpolosedDelegate(SmallHelicopter smallHelicopter);

    public class SmallHelicopter : DrawableGameComponent
    {
        private MainGame mainGame;

        // SpriteBatch and texture for rendering
        private SpriteBatch spriteBatch;
        private Texture2D helicopterTexture;

        // Animation frames and dimensions
        private Vector2 frameSize;
        private List<Rectangle> animationFrames;
        private const int HELICOPTER_COLS = 5;
        private int helicopterFrameIndex;

        // Helicopter position and movement properties
        private Vector2 currentPosition;
        private Vector2 textureOrigin;
        private float movingSpeed;
        private int startPositionX;
        private const int MAX_HEALTH = 1;
        private Random randomizeXAndYStartPosition;

        // Helicopter state tracking
        private int hitCount = 0;
        private bool isHit = false;
        private bool isBeingDestroyed = false;
        private bool isDestructionInitiated = false;


        // Destruction animation properties
        private Texture2D destructionTexture;
        private Vector2 destructionFrameSize;
        private List<Rectangle> destroyAnimationFrames;
        private const int DESTROY_ANIMATION_COLS = 7;
        private int destructionFrameIndex = 0;
        private float destructionElapsedTime = 0;
        private SoundEffect destructionSound;

        // Bullet generation properties
        PlayScene playScene;
        
        public bool IsHit { get => isHit; set => isHit = value; }

        public RemovePassedOrExpolosedDelegate RemovePassedOrExpolosed { get; set; }
        public Vector2 CurrentPosition { get => currentPosition; set => currentPosition = value; }

        public SmallHelicopter(Game game, SpriteBatch playSceneSpriteBatch, PlayScene playScene) : base(game)
        {
            mainGame = (MainGame)game;
            spriteBatch = playSceneSpriteBatch;
            this.playScene = playScene;
            destructionSound = mainGame.Content.Load<SoundEffect>("sounds/destroyedSound");
            InitializeTextures();
            InitializeAnimationFrames();
            InitializePositionAndSpeed();
            AddToScene();
        }

        private void InitializeTextures()
        {
            helicopterTexture = mainGame.Content.Load<Texture2D>("images/smallHelicopter");
            destructionTexture = mainGame.Content.Load<Texture2D>("images/destroyAnimation");
        }

        private void InitializeAnimationFrames()
        {
            frameSize = new Vector2(helicopterTexture.Width / HELICOPTER_COLS, helicopterTexture.Height);
            textureOrigin = new Vector2(frameSize.X / 2, frameSize.Y / 2);
            animationFrames = GenerateAnimationFrames(helicopterTexture, HELICOPTER_COLS);

            destructionFrameSize = new Vector2(destructionTexture.Width / DESTROY_ANIMATION_COLS, destructionTexture.Height);
            destroyAnimationFrames = GenerateAnimationFrames(destructionTexture, DESTROY_ANIMATION_COLS);
        }

        private void InitializePositionAndSpeed()
        {
            randomizeXAndYStartPosition = new Random();
            startPositionX = randomizeXAndYStartPosition.Next(0, 2);
            int randomYPosition = randomizeXAndYStartPosition.Next(170, 450);
            movingSpeed = randomizeXAndYStartPosition.Next(2, 4);
            currentPosition = new Vector2(startPositionX == 0 ? 0 : Shared.stageSize.X, randomYPosition);
        }

        private void AddToScene()
        {
            PlayScene.SmallHelicopterList.Add(this);
        }

        private List<Rectangle> GenerateAnimationFrames(Texture2D texture, int columns)
        {
            List<Rectangle> frames = new List<Rectangle>();
            Vector2 frameSize = new Vector2(texture.Width / columns, texture.Height);

            for (int c = 0; c < columns; c++)
            {
                int x = c * (int)frameSize.X;
                frames.Add(new Rectangle(x, 0, (int)frameSize.X, (int)frameSize.Y));
            }

            return frames;
        }


        public override void Update(GameTime gameTime)
        {

            UpdateHelicopterMovement();
            UpdateDestructionState(gameTime);

            base.Update(gameTime);
        }

        private double elapsedTime = 0;
        private double frameInterval = 400;
        private double hitEffectTimer = 0.005;
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (isBeingDestroyed)
            {
                Vector2 adjustPosition = new Vector2(currentPosition.X + 40, currentPosition.Y + 50);
                spriteBatch.Draw(destructionTexture, adjustPosition, destroyAnimationFrames[destructionFrameIndex], Color.White, 0f, textureOrigin, 1.2f, SpriteEffects.None, 0f);
            }
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!isBeingDestroyed)
            {
                if (elapsedTime >= frameInterval)
                {
                    SmallHelicopterBullet smallHelicopterBullet = new SmallHelicopterBullet(mainGame, spriteBatch, this);
                    playScene.ComponentList.Add(smallHelicopterBullet);
                    elapsedTime = 0;
                }
            }

            if (IsHit && !isBeingDestroyed)
            {
                hitEffectTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectTimer <= 0)
                {
                    IsHit = false;
                    hitCount++;
                }
                spriteBatch.Draw(helicopterTexture, currentPosition, animationFrames[helicopterFrameIndex], Color.Red, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }
            else if(!IsHit && !isBeingDestroyed)
            {
                spriteBatch.Draw(helicopterTexture, currentPosition, animationFrames[helicopterFrameIndex], Color.White, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);

            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateHelicopterMovement()
        {
            helicopterFrameIndex = helicopterFrameIndex == animationFrames.Count() - 1 ? 0 : ++helicopterFrameIndex;

            if (startPositionX == 0)
            {
                if (!isBeingDestroyed)
                {
                    currentPosition.X += movingSpeed;
                }
            }
            else
            {
                if (!isBeingDestroyed)
                {
                    currentPosition.X -= movingSpeed;
                }
            }
            if ((startPositionX == 0 && currentPosition.X == Shared.stageSize.X) ||
                (startPositionX == 1 && currentPosition.X == 0)
                )
            {
                RemovePassedOrExpolosed?.Invoke(this);

            }
        }
        private void UpdateDestructionState(GameTime gameTime)
        {
            destructionElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (hitCount >= MAX_HEALTH && !isDestructionInitiated)
            {
                isBeingDestroyed = true;
                isDestructionInitiated = true;
                destructionSound.Play();
                GameBoard.NumberOfDestoryedSmallHelicopter++;
            }

            //if It got destroyed
            if (isBeingDestroyed)
            {
                if (destructionElapsedTime >= 0.3f)
                {
                    destructionFrameIndex++;
                    destructionElapsedTime = 0f;
                }
            }
            if (destructionFrameIndex >= DESTROY_ANIMATION_COLS - 1)
            {
                RemovePassedOrExpolosed?.Invoke(this);
            }

        }
        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameSize.X * 0.2f);
            int scaledHeight = (int)(frameSize.Y * 0.2f);

            return new Rectangle((int)currentPosition.X, (int)currentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
