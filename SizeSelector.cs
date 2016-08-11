using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public class SizeSelector :  Selector
    {

        public override void Draw(bool DestroyFrame)
        {
            if (!exist) return;
            Rectangle r = GetFrame();

            Graphics.FromHwnd(MainForm.MPicture).DrawEllipse(new Pen(Color.Red,2.0f), r);

            if (DestroyFrame)
            {
                exist = false;
                done = true;
            }

        }

    }
}
