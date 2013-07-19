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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using Paradox.Game.Classes.Inputs;
using Paradox.Game.Classes;

namespace Paradox.Menu
{
    public class GameVideo
    {
         private Video _video;
         private VideoPlayer _videoPlayer;
         private Texture2D _videoTexture;
         private KeyboardInput kinput;
         private GamePadInput gInput;
         public bool RequestLeave;

         public GameVideo(Video video)
         {
             _video = video;
             _videoPlayer = new VideoPlayer();
             kinput = new KeyboardInput();
             gInput = new GamePadInput();
         }

        public void StartVideo()
        {
            _videoPlayer.Play(_video);
        }

        public void StopVideo()
        {
            _videoPlayer.Stop();
        }

        public void UpdateVideo()
        {
            RequestLeave = false;
#if XBOX
            if (kinput.GetKeyboardMenuActions().Count > 0)
                RequestLeave = true;
#else
            if (gInput.GetGamePadMenuActions().Count > 0)
                RequestLeave = true;
#endif
        }

        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (_videoPlayer.State != MediaState.Stopped)
            {
                
                _videoTexture = _videoPlayer.GetTexture();
                Rectangle screen = new Rectangle(device.Viewport.X, device.Viewport.Y, device.Viewport.Width, device.Viewport.Height);

                if (_videoTexture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(_videoTexture, screen, Color.White);
                    spriteBatch.End();
                }
            }
        }

        public bool VideoDone()
        {
            if (_videoPlayer.State == MediaState.Paused || _videoPlayer.State == MediaState.Stopped)
            {
                return true;
            }

            return false;
        }
    }
}
