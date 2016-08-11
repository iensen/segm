using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/*
 * this class is for selection a region from the image
 */

namespace WindowsFormsApplication3
{
    public class SelectionFrame:Selector
    {
        
        public override void Draw(bool DestroyFrame)
        {
            if (!exist) return;
            Rectangle r = GetFrame();

            ControlPaint.DrawLockedFrame(Graphics.FromHwnd(MainForm.MPicture), r, true);
            if (DestroyFrame)
            {
                exist = false;
                done = true;
            }

        }
       
    }
}
