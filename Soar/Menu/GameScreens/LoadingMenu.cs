/*This code is managed under the Apache v2 license. 
To see an overview: 
http://www.tldrlegal.com/license/apache-license-2.0-(apache-2.0)

Author: Robert Gawdzik
www.github.com/rgawdzik/

THIS CODE HAS NO FORM OF ANY WARRANTY, AND IS CONSIDERED AS-IS.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Paradox.Menu.GameScreens
{
    public class LoadingMenu : GameScreen
    {
        public static event EventHandler Selected;
        public event EventHandler StartLoading;
        private GameVideo _loadingVideo;
        /// <summary>
        /// Constructor
        /// </summary>
        public LoadingMenu(Texture2D[] panels, Texture2D[] backgroundAssets, GameVideo loadingVideo)
            : base(panels, backgroundAssets)
        {
            _loadingVideo = loadingVideo;
            LoadingMenu.Selected += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Shown;
                    this._delay = 0;
                    StartLoadingEvent();
                    _loadingVideo.StartVideo();
                };
        }

        public void UpdateMenu(bool doneloading)
        {
            if (CurrentState == State.Shown)
            {
                _loadingVideo.UpdateVideo();
                if (doneloading && _loadingVideo.RequestLeave)
                {
                    _loadingVideo.StopVideo();
                    this.CurrentState = State.Hidden;
                }
            }
        }

        public bool IsDone()
        {
            return _loadingVideo.VideoDone();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Graphics.SpriteFont font)
        {
            _loadingVideo.Draw(device, spriteBatch);
        }

        public static void SelectedEvent()
        {
            if (Selected != null)
            {
                Selected(null, EventArgs.Empty); //Since we do not need to know what menu called us, we just pass null.
            }
        }

        public void StartLoadingEvent()
        {
            if (StartLoading != null)
                StartLoading(this, EventArgs.Empty);
        }
    }
}
