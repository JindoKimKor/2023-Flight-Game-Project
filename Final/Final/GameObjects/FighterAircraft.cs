using Final.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.GameComponents
{
    public enum AircraftFrames
    {
        Idle = 12,
        MoveLeftSlow = 11,
        MoveLeftFast = 10,
        MoveRightSlow = 13,
        MoveRightFast = 14,
        MoveUpSlow = 7,
        MoveUpFast = 2,
        MoveDownSlow = 17,
        MoveDownFast = 22,
        MoveNorthwestSlow = 6,
        MoveNorthwestFast = 0,
        MoveNortheastSlow = 8,
        MoveNortheastFast = 4,
        MoveSouthwestSlow = 16,
        MoveSouthwestFast = 20,
        MoveSoutheastSlow = 19,
        MoveSoutheastFast = 24,
    }

    public class FighterAircraft : DrawableGameComponent
    {
        private SpriteBatch fighterAircraftSpriteBatch;
        private Texture2D fighterAircraftTexture;
        private Vector2 frameDimension;
        private List<Rectangle> animationFrames;
        private AircraftFrames currentFrame;
        private Vector2 textureOrigin;

        private Texture2D fireTexture;
        private Vector2 fireFrameDimension;
        private List<Rectangle> fireAnimationFrames;
        private Vector2 firePosition;
        private int fireFrameIndex;

        //frame index
        private const int AIRCRAFT_TEXTURE_ROWS = 5;
        private const int AIRCRAFT_TEXTURE_COLS = 5;

        private const int FIRE_TEXTURE_ROWS = 5;


        private MainGame mainGame;

        private bool isGotHit;
        private int hitCount;
        public bool IsGotHit { get => isGotHit; set => isGotHit = value; }
        public int HitCount { get => hitCount; set => hitCount = value; }

        public FighterAircraft(Game game, SpriteBatch playSceneSpriteBatch, Texture2D fighterAircraftTexture, Vector2 startingPosition) : base(game)
        {
            mainGame = (MainGame)game;
            fighterAircraftSpriteBatch = playSceneSpriteBatch;
            this.fighterAircraftTexture = fighterAircraftTexture;
            frameDimension = new Vector2(fighterAircraftTexture.Width / AIRCRAFT_TEXTURE_ROWS, fighterAircraftTexture.Height / AIRCRAFT_TEXTURE_COLS);
            textureOrigin = new Vector2(frameDimension.X / 2, frameDimension.Y / 2);
            PlayScene.FighterAircraftCurrentPosition = startingPosition;
            currentFrame = AircraftFrames.Idle;

            //fire animation
            fireTexture = mainGame.Content.Load<Texture2D>("images/aircraftAttackAnimation");
            fireFrameDimension = new Vector2(fireTexture.Width, fireTexture.Height / FIRE_TEXTURE_ROWS);
            fireFrameIndex = (FIRE_TEXTURE_ROWS - 1);
            InitializeAnimationFrames();

        }

        private void InitializeAnimationFrames()
        {
            animationFrames = new List<Rectangle>();
            for (int r = 0; r < AIRCRAFT_TEXTURE_ROWS; r++)
            {
                for (int c = 0; c < AIRCRAFT_TEXTURE_COLS; c++)
                {
                    int x = c * (int)frameDimension.X;
                    int y = r * (int)frameDimension.Y;

                    Rectangle frameRectangle = new Rectangle(x, y, (int)frameDimension.X, (int)frameDimension.Y);

                    animationFrames.Add(frameRectangle);
                }
            }

            fireAnimationFrames = new List<Rectangle>();
            for (int r = 0; r < FIRE_TEXTURE_ROWS; r++)
            {
                int y = r * (int)fireFrameDimension.Y;

                fireAnimationFrames.Add(new Rectangle(0, y, (int)fireFrameDimension.X, (int)fireFrameDimension.Y));
            }
        }

        public void ChangeAirCraftPositionAndAnimationWithSpeed(AircraftFrames newFrame, Vector2 newPosition)
        {
            float screenEdgeMinimumX = (frameDimension.X / 2);
            float screenEdgeMinimumY = (frameDimension.Y / 2);
            float screenEdgeMaxX = Shared.stageSize.X - (frameDimension.X / 2);
            float screenEdgeMaxY = Shared.stageSize.Y - (frameDimension.Y / 2);


            currentFrame = newFrame;
            //To keep the aircraft within the screen
            if (!PlayScene.IsStartingSequence)
            {
                PlayScene.FighterAircraftCurrentPosition = new Vector2(
                    newPosition.X <= screenEdgeMinimumX ? screenEdgeMinimumX : newPosition.X >= screenEdgeMaxX ? screenEdgeMaxX : newPosition.X,
                    newPosition.Y <= screenEdgeMinimumY ? screenEdgeMinimumY : newPosition.Y >= screenEdgeMaxY ? screenEdgeMaxY : newPosition.Y
                );
            }
        }

        private double elapsedTime = 0;
        private double frameInterval = 100;

        public override void Update(GameTime gameTime)
        {

            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= frameInterval)
            {
                fireFrameIndex--;
                if (fireFrameIndex <= 1)
                {
                    fireFrameIndex = (FIRE_TEXTURE_ROWS - 1);
                }
                elapsedTime = 0;
            }

            firePosition = PlayScene.FighterAircraftCurrentPosition;
            //adjust position according to its presentation
            firePosition.X = firePosition.X - 22f;
            firePosition.Y = firePosition.Y - 50f;

            base.Update(gameTime);

        }

        private double hitEffectTimer = 0.005;
        public override void Draw(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            fighterAircraftSpriteBatch.Begin();

            if (IsGotHit)
            {
                hitEffectTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitEffectTimer <= 0)
                {
                    IsGotHit = false;
                    hitCount++;
                }
                fighterAircraftSpriteBatch.Draw(fighterAircraftTexture, PlayScene.FighterAircraftCurrentPosition, animationFrames[(int)currentFrame], Color.Red, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }
            else
            {
                fighterAircraftSpriteBatch.Draw(fighterAircraftTexture, PlayScene.FighterAircraftCurrentPosition, animationFrames[(int)currentFrame], Color.White, 0f, textureOrigin, 0.8f, SpriteEffects.None, 0f);
            }


            if (keyboardState.IsKeyDown(Keys.Space) && !PlayScene.IsStartingSequence)
            {
                fighterAircraftSpriteBatch.Draw(fireTexture, firePosition, fireAnimationFrames[fireFrameIndex], Color.White);
            }
            fighterAircraftSpriteBatch.End();



            base.Draw(gameTime);
        }

        public Rectangle GetHitbox()
        {
            int scaledWidth = (int)(frameDimension.X * 0.8f);
            int scaledHeight = (int)(frameDimension.Y * 0.8f);

            return new Rectangle((int)PlayScene.FighterAircraftCurrentPosition.X, (int)PlayScene.FighterAircraftCurrentPosition.Y, scaledWidth, scaledHeight);
        }
    }
}
