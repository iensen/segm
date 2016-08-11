using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace WindowsFormsApplication3
{
   public  class DistrubuteSelector:Selector
    {
        public override void Draw(bool DestroyFrame)
        {
         // if (!exist) return;
            Rectangle r = GetFrame();
            Graphics.FromHwnd(MainForm.MPicture).DrawRectangle(new Pen(Color.Blue, 2.0f), r);
  //          ControlPaint.DrawLockedFrame(Graphics.FromHwnd(MainForm.MPicture), r, true);
            if (DestroyFrame)
            {
                exist = false;
                done = true;
            }

        }
        public void DrawToPicture2(bool DestroyFrame)
        {
            Rectangle r = GetFrame();
            Graphics.FromHwnd(MainForm.SPicture).DrawRectangle(new Pen(Color.Blue, 2.0f), r);
            //          ControlPaint.DrawLockedFrame(Graphics.FromHwnd(MainForm.MPicture), r, true);
            if (DestroyFrame)
            {
                exist = false;
                done = true;
            }

        }
    }
}
