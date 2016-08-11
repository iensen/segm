using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace WindowsFormsApplication3
{
    public abstract class Selector
    {
    /*
    * this class is for selection a region from the image
    */

        protected Graphics g;
        protected bool exist;
        abstract public void Draw(bool DestroyFrame);
        public bool Exist//is it now drawing?
        {
            get
            {
                return exist;
            }
            set
            {
               
                exist = value;
            }
        }
        protected bool done;
        public bool Done// have we a drawn frame?
        {
            get
            {
                return done;
            }
            set
            {
                done = value;
            }
        }
        protected int startX, startY, FinishX, FinishY;
        protected double dx, dy;
        public void ChangeScale(double newDx, double newDy)
        {
            dx = newDx;
            dy = newDy;
        }
        public void SetStart(int x, int y)
        {
            startX = x;
            startY = y;



        }
        public void SetFinish(int x, int y)
        {
            FinishX = x;
            FinishY = y;

            GC.Collect();
        }
        public void ChangeScale(PictureBox p)
        {
            if (p.Image == null) return;
            dx = p.Image.Width * 1.0 / p.Size.Width;
            dy = p.Image.Height * 1.0 / p.Size.Height;

        }
        public bool isExist()
        {
            return exist;
        }
        public Rectangle GetSelectedRectangle()
        {
            int stX = Math.Min((int)(startX * dx), (int)(FinishX * dx));
            int stY = Math.Min((int)(startY * dy), (int)(FinishY * dy));
            int fnX = Math.Max((int)(startX * dx), (int)(FinishX * dx));
            int fnY = Math.Max((int)(startY * dy), (int)(FinishY * dy));
            int Width = fnX - stX;
            int Height = fnY - stY;
            return new Rectangle(stX, stY, Width, Height);
        }
        protected Rectangle GetFrame()
        {
            int stX = Math.Min((startX), (FinishX));
            int stY = Math.Min((startY), (FinishY));
            int fnX = Math.Max((startX), (FinishX));
            int fnY = Math.Max((startY), (FinishY));
            int Width = fnX - stX;
            int Height = fnY - stY;
            return new Rectangle(stX, stY, Width, Height);
        }
    }
}
