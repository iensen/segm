using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    class PatternSelector:Selector
    {

        public override void Draw(bool DestroyFrame)
        {
            if (!exist) return;
            Rectangle r = GetFrame();
            if (this.FinishY == r.Bottom && this.FinishX==r.Right )
            { 

                Graphics.FromHwnd(MainForm.MPicture).DrawLine(new Pen(Color.Red, 2.0f), r.Left, r.Top, r.Right, r.Bottom);
            }
            else
            {
                Graphics.FromHwnd(MainForm.MPicture).DrawLine(new Pen(Color.Red, 2.0f), r.Left, r.Bottom, r.Right, r.Top);
            }
            if (DestroyFrame)
            {
                exist = false;
                done = true;
            }

        }
    }
}
