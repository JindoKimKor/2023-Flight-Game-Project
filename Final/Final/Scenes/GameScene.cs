using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Final.Scenes
{
    /// <summary>
    /// Abstract Class for all game scenes
    /// </summary>
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
        /// <summary>
        /// Constructor, initialize Scene's Game Components List
        /// </summary>
        /// <param name="game"></param>
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
