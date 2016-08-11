using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;
namespace WindowsFormsApplication3
{
    public class MarkerRelativePosition
    {
        public MarkerRelativePosition(float x, float y)
        {
            dx = x;
            dy = y;
        }
        public float dx;
        public float dy;
    }
    public partial class MainForm : Form
    {
        List<MarkerRelativePosition> MarkerPositions = new List<MarkerRelativePosition>();
        List<Label> Labels= new List<Label>();
        private void RedrawAllMarkers()
        {
            int l = Labels.Count;
            for (int i = 0; i < l; i++)
            {
                Labels[i].BringToFront();
                Labels[i].BackColor = Color.FromArgb(100, 210, 10, 20);
                if(MarkerPositions[i].dx>0)
                {
                    int x=panel1.Location.X+MainPictureBox.Location.X+Convert.ToInt32(MarkerPositions[i].dx*MainPictureBox.Size.Width);
                    int y = panel1.Location.Y+MainPictureBox.Location.Y + Convert.ToInt32(MarkerPositions[i].dy * MainPictureBox.Size.Height);
                    Labels[i].Location = new Point(x, y);
                }
               
            }
        }
        private void CreateNewMarker(bool VisibleM)
        {
            Labels.Add(new Label());
            MarkerPositions.Add(new MarkerRelativePosition(-1.0f, -1.0f));
            int newLabel = Labels.Count - 1;
            Labels[newLabel].Name = "DL" + newLabel.ToString();
            int sz = Math.Max(this.Size.Width,this.Size.Height) / 160;
            Labels[newLabel].Location = new Point(0, 0);
            //Labels[newLabel].Location = new Point(0, 30);
            Labels[newLabel].Size = new System.Drawing.Size(sz, sz);
            Labels[newLabel].BackColor = Color.FromArgb(100, 210, 10, 20);
            Labels[newLabel].Invalidate();
            Labels[newLabel].Show();
            Labels[newLabel].BringToFront();
            Labels[newLabel].MouseUp += new MouseEventHandler(this.lblDragger_MouseUp);
            Labels[newLabel].MouseLeave += new EventHandler(lblDragger_MouseLeave);
            Labels[newLabel].MouseMove += new MouseEventHandler(this.lblDragger_MouseMove);
            Labels[newLabel].MouseDown += new MouseEventHandler(this.lblDragger_MouseDown);
            Labels[newLabel].PreviewKeyDown+=new PreviewKeyDownEventHandler(this.Marker_KeyDown);
            if (VisibleM)  this.Controls.Add(Labels[newLabel]);
            if (!VisibleM) Labels[newLabel].Visible = false;
            UpdateMarkersCount();
            Invalidate();
        }
        private void button8_Click(object sender, EventArgs e)
        {
          if(MainPictureBox.Image==null)return;

          Bitmap b=new Bitmap(MainPictureBox.Image);
          foreach(MarkerRelativePosition Pos in MarkerPositions)
          {
              int x=(int)((float)b.Width*Pos.dx);
              int y = (int)((float)b.Height * Pos.dy);
              if (Pos.dx == -1.0f) continue;
              for(int i=x-20;i<=x+20;i++)
                  for(int j=y-20;j<=y+20;j++)
                  {
                     
                      if (i < 0 || i >= b.Width) continue;
                      if (j < 0 || j >= b.Height) continue;
                      b.SetPixel(Math.Max(i,0),j,Color.FromArgb(255,255,0,0));
                  }
          }
          MainPictureBox.Image=b;
        }
        bool isDragging;
        int clickOffsetX, clickOffsetY;
        int GetLblDraggerNumber(object sender)
        {
            string name = ((Label)sender).Name;
            if (name.Length > 2 && name.Substring(0, 2) == "DL")
                return int.Parse(name.Substring(2));
            else
                return -1;
        }
        int ActiveMarker=-1;
        private void lblDragger_MouseDown(Object sender, MouseEventArgs e)
        {
           // RedrawAllMarkers();
           
            int Index = GetLblDraggerNumber(sender);
            if (Index < 0) return;
            Labels[Index].BackColor =  Color.FromArgb(255, 0, 255, 0);
            ActiveMarker = Index;
            isDragging = true;
            clickOffsetX = e.X;
            clickOffsetY = e.Y;
        }
        private void RemoveMarkerAt(int Index)
        {
            Labels[Index].Dispose();
            Labels.RemoveAt(Index);
            MarkerPositions.RemoveAt(Index);
            ActiveMarker = -1;
            for(int i=Index;i<Labels.Count;i++)
            {
                Labels[i].Name = "DL" + i.ToString();
            }
        }
        private void MouseUpLeave(Object sender)
        {
            isDragging = false;
            int Index = GetLblDraggerNumber(sender);
            int sz = Math.Max(this.Size.Width, this.Size.Height) / 160;
            Labels[Index].Size = new Size(sz, sz);
            int locX = Labels[Index].Location.X;
            int locY = Labels[Index].Location.Y;
            if (!(locX >= panel1.Location.X + pictureBox1.Location.X &&
                  locX <= panel1.Location.X + pictureBox1.Location.X + pictureBox1.Size.Width &&
                  locY >= panel1.Location.Y + pictureBox1.Location.Y &&
                  locY <= panel1.Location.Y + pictureBox1.Location.Y + pictureBox1.Size.Height))
            {
           
                if (MarkerPositions[Index].dx == -1.0f)
                CreateNewMarker(true);
                RemoveMarkerAt(Index);
            }
            else
            {
                int MarkerPosX = locX - (panel1.Location.X + pictureBox1.Location.X);
                int MarkerPosY = locY - (panel1.Location.Y + pictureBox1.Location.Y);
                if (MarkerPositions[Index].dx == -1.0f) CreateNewMarker(true);
                MarkerPositions[Index].dx = ((float)MarkerPosX) / (float)MainPictureBox.Size.Width;
                MarkerPositions[Index].dy = ((float)MarkerPosY) / (float)MainPictureBox.Size.Height;
             

            }
            
        }
        private void Marker_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MessageBox.Show(e.KeyCode.ToString());
        }
        private void lblDragger_MouseUp(Object sender, MouseEventArgs e)
        {

            MouseUpLeave(sender);
        }
        private void lblDragger_MouseLeave(Object sender, EventArgs e)
        {

            MouseUpLeave(sender);
        }

        private void lblDragger_MouseMove(Object sender,
        System.Windows.Forms.MouseEventArgs e)
        {
            int Index = GetLblDraggerNumber(sender);
            if (Index < 0) return;
            Labels[Index].Size = new Size(16, 16);
            if (isDragging == true)
            {
                
                Labels[Index].Left = e.X + Labels[Index].Left - clickOffsetX;
                Labels[Index].Top = e.Y + Labels[Index].Top - clickOffsetY;
                Labels[Index].BringToFront();
                Labels[Index].Invalidate();
            }
            Invalidate();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if(ActiveMarker>=0)
            RemoveMarkerAt(ActiveMarker);

        }
        private void DeleteAllMarkers()
        {
            while (Labels.Count > 0)
            {
                RemoveMarkerAt(0);
            }
            label14.Text = "0";
        
        }
        private void button11_Click(object sender, EventArgs e)
        {
            DeleteAllMarkers(); 
        }


    }
}