using Final.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Scenes
{
    public abstract class GameScene : DrawableGameComponent
    {
        public List<GameComponent> ComponentList { get; set; }

        public virtual void Hide()
        {
            Enabled = false;
            Visible = false;
        }
        public virtual void Show()
        {
            Enabled = true;
            Visible = true;
        }
        protected GameScene(Game game) : base(game)
        {
            ComponentList = new List<GameComponent>();
            Hide();
        }
        public override void Update(GameTime gameTime)
        {
            for (int i = ComponentList.Count - 1; i >= 0; i--)
            {
                GameComponent gameComponent = ComponentList[i];
                if (gameComponent.Enabled)
                {
                    gameComponent.Update(gameTime);
                }
            }


            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            List<DrawableGameComponent> drawableComponentsToDraw = new List<DrawableGameComponent>();

            foreach (GameComponent gameComponent in ComponentList)
            {
                if (gameComponent is DrawableGameComponent drawableGameComponent && drawableGameComponent.Visible)
                {
                    drawableComponentsToDraw.Add(drawableGameComponent);
                }
            }
            foreach (DrawableGameComponent drawable in drawableComponentsToDraw)
            {
                drawable.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
