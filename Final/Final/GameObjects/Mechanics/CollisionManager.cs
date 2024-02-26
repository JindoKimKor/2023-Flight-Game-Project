using Final.GameObjects.Characters;
using Final.GameObjects.Weapons;
using Final.Scenes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Final.GameObjects.Mechanics
{
    /// <summary>
    /// Collision Managing Class
    /// </summary>
    public class CollisionManager : GameComponent
    {
        private List<GameComponent> gameSceneGameComponents;
        private BossHelicopter bossHelicopter;
        private FighterAircraft fighterAircraft;

        /// <summary>
        /// Collision Manager Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="playScene"></param>
        /// <param name="bossHelicopter">Boss Helicopter Object on the play scene</param>
        /// <param name="fighterAircraft">Aircraft Object on the play scene</param>
        public CollisionManager(Game game, PlayScene playScene, BossHelicopter bossHelicopter, FighterAircraft fighterAircraft) : base(game)
        {
            gameSceneGameComponents = playScene.ComponentList;
            this.bossHelicopter = bossHelicopter;
            this.fighterAircraft = fighterAircraft;
        }

        private double stayingbulletElapsedTime;
        public override void Update(GameTime gameTime)
        {

            RemoveAircraftBullets();
            RemoveEnemysBullets();
            base.Update(gameTime);

            void RemoveAircraftBullets()
            {
                //Remove aircraft bullet
                List<AircraftBasicBullet> bulletsToRemove = new List<AircraftBasicBullet>();

                foreach (GameComponent item in gameSceneGameComponents)
                {
                    if (item is AircraftBasicBullet eachAircraftBullet)
                    {
                        Rectangle aircraftBulletHitBox = eachAircraftBullet.GetHitbox();
                        Rectangle bossHitBox = bossHelicopter.GetHitbox();

                        if (bossHitBox.Intersects(aircraftBulletHitBox))
                        {
                            bossHelicopter.IsHit = true;
                            bulletsToRemove.Add(eachAircraftBullet);
                        }
                        foreach (SmallHelicopter smallHelicopter in PlayScene.SmallHelicopterList)
                        {
                            Rectangle smallHelicopterHitBox = smallHelicopter.GetHitbox();
                            if (smallHelicopterHitBox.Intersects(aircraftBulletHitBox))
                            {
                                bulletsToRemove.Add(eachAircraftBullet);
                                smallHelicopter.IsHit = true;
                            }
                        }
                    }
                }
                stayingbulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (stayingbulletElapsedTime > 15)//Improve collision effect
                {
                    foreach (AircraftBasicBullet bullet in bulletsToRemove)
                    {
                        gameSceneGameComponents.Remove(bullet);
                        bullet.Dispose();
                    }
                    stayingbulletElapsedTime = 0;
                }
            }

            void RemoveEnemysBullets()
            {
                List<BossHelicopterBasicBullet> bossBulletsToRemove = new List<BossHelicopterBasicBullet>();
                List<SmallHelicopterBullet> smallBulletsToRemove = new List<SmallHelicopterBullet>();

                foreach (GameComponent item in gameSceneGameComponents)
                {
                    Rectangle fighterAircraftHixBox = fighterAircraft.GetHitbox();

                    if (item is BossHelicopterBasicBullet eachBossBullet)
                    {
                        Rectangle bossBulletHitBox = eachBossBullet.GetHitbox();
                        if (fighterAircraftHixBox.Intersects(bossBulletHitBox))
                        {
                            fighterAircraft.IsGotHit = true;
                            PlayScene.NumberOfGotHit++;
                            bossBulletsToRemove.Add(eachBossBullet);
                        }
                    }
                    if (item is SmallHelicopterBullet eachSmallBullet)
                    {
                        Rectangle smallBulletHitBox = eachSmallBullet.GetHitbox();
                        if (fighterAircraftHixBox.Intersects(smallBulletHitBox))
                        {
                            fighterAircraft.IsGotHit = true;
                            PlayScene.NumberOfGotHit++;
                            smallBulletsToRemove.Add(eachSmallBullet);
                        }
                    }
                }
                stayingbulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (stayingbulletElapsedTime > 15)//Improve collision effect
                {
                    foreach (BossHelicopterBasicBullet bullet in bossBulletsToRemove)
                    {
                        gameSceneGameComponents.Remove(bullet);
                        bullet.Dispose();
                    }
                    foreach (SmallHelicopterBullet bullet in smallBulletsToRemove)
                    {
                        gameSceneGameComponents.Remove(bullet);
                        bullet.Dispose();
                    }
                    stayingbulletElapsedTime = 0;
                }
            }
        }
    }
}
