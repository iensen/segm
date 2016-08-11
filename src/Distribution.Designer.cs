namespace WindowsFormsApplication3
{
    partial class Distribution
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.zgc = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zgc
            // 
            this.zgc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zgc.IsShowPointValues = true;
            this.zgc.IsZoomOnMouseCenter = true;
            this.zgc.Location = new System.Drawing.Point(0, 0);
            this.zgc.Name = "zgc";
            this.zgc.ScrollGrace = 0;
            this.zgc.ScrollMaxX = 0;
            this.zgc.ScrollMaxY = 0;
            this.zgc.ScrollMaxY2 = 0;
            this.zgc.ScrollMinX = 0;
            this.zgc.ScrollMinY = 0;
            this.zgc.ScrollMinY2 = 0;
            this.zgc.Size = new System.Drawing.Size(644, 467);
            this.zgc.TabIndex = 0;
            this.zgc.Resize += new System.EventHandler(this.zedGraphControl1_Resize);
            // 
            // Distribution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 467);
            this.Controls.Add(this.zgc);
            this.Name = "Distribution";
            this.Text = "Distribution";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Distribution_FormClosing);
            this.Resize += new System.EventHandler(this.Distribution_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zgc;
    }
}