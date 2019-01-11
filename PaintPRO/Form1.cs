using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PaintPRO
{
    public partial class Form1 : Form
    {
        public enum Shape
        {
            Free, Line, Ellipse, Rectangle, Triangle, Pipetka, Fill, Spray
        };

        Pen pen = new Pen(Color.Black, 2);
        Graphics g;
        GraphicsPath gp = new GraphicsPath();
        Point current, end, start;
        Bitmap bmp;
        Color originColor;
        Queue<Point> q = new Queue<Point>();
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
            if (currentShape == Shape.Pipetka)
            {
                pen.Color = bmp.GetPixel(e.Location.X, e.Location.Y);
                button3.BackColor = pen.Color;
            }
            if (currentShape == Shape.Fill)
            {
                originColor = bmp.GetPixel(e.Location.X, e.Location.Y);
                FillRegion(e.Location);
            }
        }
        private void Step(Point pp)
        {
            if(pp.X < 0 || pp.Y<0 ||pp.X >= pictureBox1.Width||pp.Y >= pictureBox1.Height)
            {
                return;
            }
            if (bmp.GetPixel(pp.X, pp.Y)!=originColor)
            {
                return;
            }else
            {
                bmp.SetPixel(pp.X, pp.Y, pen.Color);
                q.Enqueue(pp);
            }
        }
        private void FillRegion(Point pp)
        {
            q.Enqueue(pp);
            while (q.Count > 0)
            {
                Point cur = q.Dequeue();               
                Step(new Point(cur.X - 1, cur.Y));
                Step(new Point(cur.X + 1, cur.Y));
                Step(new Point(cur.X, cur.Y + 1));
                Step(new Point(cur.X, cur.Y - 1));
            }
            pictureBox1.Refresh();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (currentShape)
                {
                    case Shape.Free:
                        current = e.Location;
                        g.DrawLine(pen, start, current);
                        start = current;
                        break;
                    case Shape.Line:
                        current = e.Location;
                        gp.Reset();
                        gp.AddLine(start, current);
                        break;
                    case Shape.Rectangle:
                        current = e.Location;
                        if (current.X > start.X && current.Y > start.Y) {
                            Rectangle rec = new Rectangle(start.X, start.Y, current.X - start.X, current.Y - start.Y);
                            gp.Reset();
                            gp.AddRectangle(rec);
                        }
                        else if(current.X < start.X && current.Y < start.Y)
                        {
                            Rectangle rec = new Rectangle(current.X, current.Y, start.X-current.X  ,start.Y - current.Y );
                            gp.Reset();
                            gp.AddRectangle(rec);
                        }else if (current.X > start.X && current.Y < start.Y)
                        {
                            Rectangle rec = new Rectangle(start.X, current.Y, Math.Abs(current.X-start.X), Math.Abs(current.Y-start.Y));
                            gp.Reset();
                            gp.AddRectangle(rec);
                        }
                        else if (current.X < start.X && current.Y > start.Y)
                        {
                            Rectangle rec = new Rectangle(current.X, start.Y, Math.Abs(start.X-current.X), Math.Abs(start.Y- current.Y));
                            gp.Reset();
                            gp.AddRectangle(rec);
                        }
                        break;
                    case Shape.Ellipse:
                        current = e.Location;
                        if (current.X > start.X && current.Y > start.Y)
                        {
                            Rectangle rec = new Rectangle(start.X, start.Y, current.X - start.X, current.Y - start.Y);
                            gp.Reset();
                            gp.AddEllipse(rec);
                        }
                        else if (current.X < start.X && current.Y < start.Y)
                        {
                            Rectangle rec = new Rectangle(current.X, current.Y, start.X - current.X, start.Y - current.Y);
                            gp.Reset();
                            gp.AddEllipse(rec);
                        }
                        else if (current.X > start.X && current.Y < start.Y)
                        {
                            Rectangle rec = new Rectangle(start.X, current.Y, Math.Abs(current.X - start.X), Math.Abs(current.Y - start.Y));
                            gp.Reset();
                            gp.AddEllipse(rec);
                        }
                        else if (current.X < start.X && current.Y > start.Y)
                        {
                            Rectangle rec = new Rectangle(current.X, start.Y, Math.Abs(start.X - current.X), Math.Abs(start.Y - current.Y));
                            gp.Reset();
                            gp.AddEllipse(rec);
                        }
                        break;
                    case Shape.Triangle:
                        current = e.Location;
                        Point[] points ={
                            new Point((current.X+start.X)/2, start.Y),
                            new Point(start.X, current.Y),
                            new Point(current.X, current.Y),                              
                        };
                        gp.Reset();
                        gp.AddPolygon(points);
                        break;
                    case Shape.Spray:
                        Pen p = new Pen(button3.BackColor);                     
                        current = e.Location;
                        //gp.AddEllipse(new Rectangle(current.X, current.Y, (int)numericUpDown1.Value+1, (int)numericUpDown1.Value+1));
                        Random rnd = new Random();
                        int radius = (int)numericUpDown1.Value+1;
                        for (int i =0;i<=100; i++)
                        {                            
                            double theta = rnd.NextDouble() * (Math.PI * 2);
                            double r = rnd.NextDouble() * radius;
                            double x = e.X + Math.Cos(theta) * r;
                            double y = e.Y + Math.Sin(theta) * r;                                                    
                            g.DrawEllipse(p, new Rectangle((int)x, (int)y, 1, 1));
                        }
                        start = current;
                       // gp.Reset();
                        break;                                       
                }
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            g.DrawPath(pen, gp);
            //gp.Reset();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawPath(pen, gp);
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Free;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Line;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            gp.Reset();
            pen.Width = (float)numericUpDown1.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {        
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pen.Color = cd.Color;
                button3.BackColor = cd.Color;
            }
            gp.Reset();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Pen p = new Pen(Color.White);
            //pictureBox1.Image = null;
            g.FillRegion(p.Brush, new Region(new Rectangle(0,0, pictureBox1.Width, pictureBox1.Height)));
            gp.Reset();
            pictureBox1.Refresh();
            //g = Graphics.FromImage(pictureBox1.Image);
            //g = Graphics.FromImage(pictureBox1.Image);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            /*
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] ar = new byte[ms.Length];
            ms.Write(ar, 0, ar.Length);
            */
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {                
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                //pictureBox1.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.Refresh();
                gp.Reset();
                g = Graphics.FromImage(pictureBox1.Image);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Rectangle;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Ellipse;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Triangle;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Pipetka;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Fill;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            currentShape = Shape.Spray;
        }

        Shape currentShape = Shape.Free;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(pictureBox1.Image);
        }
    }
}
