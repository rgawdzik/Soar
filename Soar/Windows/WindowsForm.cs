/*This code is managed under the Apache v2 license. 
To see an overview: 
http://www.tldrlegal.com/license/apache-license-2.0-(apache-2.0)

Author: Robert Gawdzik
www.github.com/rgawdzik/

THIS CODE HAS NO FORM OF ANY WARRANTY, AND IS CONSIDERED AS-IS.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradox
{
    public partial class ParadoxForm : Form
    {
        int ScreenWidth = 1920;
        int ScreenHeight = 1080;
        bool FullScreen = false;

        public ParadoxForm()
        {
            InitializeComponent();
        }

        private void PlayButtonClick(object sender, EventArgs args)
        {
            object[] parameters = { ScreenWidth, ScreenHeight, FullScreen };
            Thread ParadoxThread = new Thread(new ParameterizedThreadStart(Paradox.RunGame));
            ParadoxThread.Start((object)parameters);
            this.Hide();
        }

        private void FullScreen_CheckedChanged(object sender, EventArgs e)
        {
            //Reverses the FullScreen value;
            FullScreen = !FullScreen;
        }

        private void ScreenResolutionSelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string value = comboBox.GetItemText(comboBox.Items[comboBox.SelectedIndex]);
            string[] valueArray = value.Split('x');
            ScreenWidth = Int32.Parse(valueArray[0]);
            ScreenHeight = Int32.Parse(valueArray[1]);
        }

        
    }
}
