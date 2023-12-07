using Final.GameComponents;
using Final.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class CollisionManager : GameComponent
    {
        private List<GameComponent> gameSceneGameComponents;
        private BossHelicopter bossHelicopter;
        private FighterAircraft fighterAircraft;

        public CollisionManager(Game game, PlayScene playScene, BossHelicopter bossHelicopter, FighterAircraft fighterAircraft) : base(game)
        {
            this.gameSceneGameComponents = playScene.ComponentList;
            this.bossHelicopter = bossHelicopter;
            this.fighterAircraft = fighterAircraft;
        }

        private double stayingbulletElapsedTime;
        public override void Update(GameTime gameTime)
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
                        bossHelicopter.IsGotHit = true;
                        bulletsToRemove.Add(eachAircraftBullet);
                    }
                    foreach (SmallHelicopter smallHelicopter in PlayScene.SmallHelicopterList)
                    {
                        Rectangle smallHelicopterHitBox = smallHelicopter.GetHitbox();
                        if (smallHelicopterHitBox.Intersects(aircraftBulletHitBox))
                        {
                            
                            bulletsToRemove.Add(eachAircraftBullet);
                            smallHelicopter.IsGotHit = true;
                        }
                    }
                }
            }
            stayingbulletElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (stayingbulletElapsedTime > 200)//Improve collision effect
            {
                foreach (AircraftBasicBullet bullet in bulletsToRemove)
                {
                    gameSceneGameComponents.Remove(bullet);
                    bullet.Dispose();
                }
                stayingbulletElapsedTime = 0;
            }

            base.Update(gameTime);
        }
    }
}
