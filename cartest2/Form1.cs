using Syncfusion.Windows.Forms.Gauge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace cartest2
{


    public partial class Form1 : Form
    {

        long maksm = 30, mimm = 0;

     
        public int[] sayilar = new int[20];
        public string veriler;
        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

   

        public Form1()
        {
            InitializeComponent();

          
           


            //TODO Radial Guage

            RadialGauge radialGauge1 = new RadialGauge();

            this.radialGauge1.MinorDifference = 5;
            this.Controls.Add(radialGauge1);
            this.radialGauge1.VisualStyle = Syncfusion.Windows.Forms.Gauge.ThemeStyle.Black;
            this.radialGauge1.MajorDifference = 10F;
            this.radialGauge1.MaximumValue = 150F;
            this.radialGauge1.MinimumValue = 0F;
            this.radialGauge1.MinorDifference = 30F;
            this.radialGauge1.LabelPlacement = Syncfusion.Windows.Forms.Gauge.LabelPlacement.Outside;
            this.radialGauge1.TextOrientation = Syncfusion.Windows.Forms.Gauge.TextOrientation.SlideOver;
            this.radialGauge1.TickPlacement = Syncfusion.Windows.Forms.Gauge.TickPlacement.OutSide;
            this.radialGauge1.MajorTickMarkColor = System.Drawing.Color.LightYellow;
            this.radialGauge1.MinorTickMarkColor = System.Drawing.Color.Red;
            this.radialGauge1.GaugeArcColor = ColorTranslator.FromHtml("#00a0d1");
            this.radialGauge1.GaugeLableColor = ColorTranslator.FromHtml("#00a0d1");
            this.radialGauge1.InterLinesColor = System.Drawing.Color.Red;
            this.radialGauge1.MinorTickMarkHeight = 55;
            this.radialGauge1.MajorTickMarkHeight = 6;
            this.radialGauge1.MinorInnerLinesHeight = 60;

            CustomRenderer custom1 = new CustomRenderer(this.linearGauge1);
            linearGauge1.Renderer = custom1;
        }
        class CustomRenderer : ILinearGaugeRenderer
        {

            /// </summary>
            /// Gets the Linear gauge
            /// </summary>
            private LinearGauge m_LinearGauge;
            /// <summary>
            /// Gets/Sets the Tick Distance of the Linear gauge.
            /// </summary>
            private float majorTicksDistance;
            /// <summary>
            /// Calculates the Minor Ticks Pixels.
            /// </summary>
            private float m_minorTicksPixels;
            /// <summary>
            /// Start point of the frame
            /// </summary>
            private int start;
            /// <summary>
            /// Counts the Major ticks count for the given range.
            /// </summary>
            private int majorTicksCount;

            /// <summary>
            ///  Gets the Radial gauge
            /// </summary>
            internal LinearGauge LinearGauge
            {
                get
                {
                    return m_LinearGauge;
                }
            }

            public CustomRenderer(LinearGauge linearGauge)
            {
                m_LinearGauge = linearGauge;
                majorTicksDistance = 0;
                m_minorTicksPixels = 0;
                start = 25;

            }

            public void DrawFrame(System.Drawing.Graphics Graphics)
            {
            }

            [Obsolete]
            public void DrawLines(System.Drawing.Graphics Graphics)
            {
                Pen majorTickPen = new Pen(LinearGauge.MajorTickMarkColor);
                Pen minorTickPen = new Pen(LinearGauge.MinorTickMarkColor);
                Brush brush = new SolidBrush(LinearGauge.ForeColor);
                StringFormat sf = new StringFormat();
                Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                majorTicksDistance = ((LinearGauge.MaximumValue - LinearGauge.MinimumValue) / LinearGauge.MajorDifference);
                majorTicksCount = ((int)(LinearGauge.MaximumValue - LinearGauge.MinimumValue) / (LinearGauge.MajorDifference)) + 1;
                double majorTickValue = LinearGauge.MinimumValue;
                float tickPosition = 25f;
                float temp1 = 0;
                float s = (LinearGauge.MaximumValue - LinearGauge.MinimumValue) % LinearGauge.MajorDifference;
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                float minortickValue = 0;
                float tickPositionMinor = 0;
                GraphicsPath path = new GraphicsPath();
                int minor = LinearGauge.MinorTickCount;
                m_minorTicksPixels = ((this.LinearGauge.Height - 50) / majorTicksDistance);
                int x = this.LinearGauge.Width / 2;
                temp1 = 0;
                for (int L = 1; L <= majorTicksCount; L++)
                {
                    Graphics.DrawLine(majorTickPen, x, this.LinearGauge.Height - tickPosition, x - LinearGauge.MajorTicksHeight, this.LinearGauge.Height - tickPosition);
                    Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    if (LinearGauge.ShowScaleLabel)
                        Graphics.DrawString(Math.Round(majorTickValue, 2).ToString(),
                                 LinearGauge.Font, brush, new PointF(x - LinearGauge.MajorTicksHeight - 25, this.LinearGauge.Height - tickPosition), sf);
                    if (L == majorTicksCount)
                        minor = (LinearGauge.MinorTickCount * (int)Math.Ceiling(s)) / LinearGauge.MajorDifference;
                    if (majorTickValue < LinearGauge.MaximumValue)
                    {
                        for (int S = 1; S <= minor; S++)
                        {
                            minortickValue = (m_minorTicksPixels / (LinearGauge.MinorTickCount + 1)) * S;
                            tickPositionMinor = this.LinearGauge.Height - (minortickValue + temp1 + 25);
                            Graphics.DrawLine(minorTickPen, x, (float)tickPositionMinor, x - LinearGauge.MinorTickHeight, (float)tickPositionMinor);
                        }
                        temp1 = m_minorTicksPixels * L;
                    }

                    majorTickValue += LinearGauge.MajorDifference;
                    tickPosition += m_minorTicksPixels;
                }
                Graphics.FillRectangle(new SolidBrush(LinearGauge.GaugeBaseColor), this.LinearGauge.Width / 2, start - 1, 1, (((this.majorTicksDistance)) * m_minorTicksPixels) + 2);
                if (this.LinearGauge.MinimumValue > 0)
                    Graphics.FillRectangle(new SolidBrush(LinearGauge.ValueIndicatorColor), this.LinearGauge.Width / 2 + 10, start + (majorTicksDistance * m_minorTicksPixels) - (((LinearGauge.Value / LinearGauge.MajorDifference)) * m_minorTicksPixels), 5, (((LinearGauge.Value / LinearGauge.MajorDifference)) * m_minorTicksPixels) + 2);
                else
                    Graphics.FillRectangle(new SolidBrush(LinearGauge.ValueIndicatorColor), this.LinearGauge.Width / 2 + 10, start + (majorTicksDistance * m_minorTicksPixels) - ((((Math.Abs(this.LinearGauge.MinimumValue) + LinearGauge.Value) / LinearGauge.MajorDifference)) * m_minorTicksPixels), 5, ((((Math.Abs(this.LinearGauge.MinimumValue) + LinearGauge.Value) / LinearGauge.MajorDifference)) * m_minorTicksPixels) + 2);
                brush.Dispose();
                minorTickPen.Dispose();
            }

            public void DrawRanges(System.Drawing.Graphics Graphics)
            {
                Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (LinearRange ptrRange in this.LinearGauge.Ranges)
                {
                    int value = (int)Math.Ceiling(LinearGauge.MaximumValue - ptrRange.EndValue) / LinearGauge.MajorDifference;
                    if (ptrRange.EndValue > ptrRange.StartValue && ptrRange.EndValue <= this.LinearGauge.MaximumValue)
                    {
                        if (this.LinearGauge.MinimumValue >= 0 && ptrRange.StartValue < 0)
                        {
                            return;
                        }
                        float startValue = (float)ptrRange.StartValue;
                        float end = (float)ptrRange.EndValue;
                        if (this.LinearGauge.MinimumValue < 0)
                        {
                            startValue = this.LinearGauge.MinimumValue + Math.Abs(ptrRange.StartValue);
                        }
                        if (this.LinearGauge.MinimumValue < 0 && ptrRange.StartValue > 0)
                        {
                            startValue = Math.Abs(this.LinearGauge.MinimumValue) + Math.Abs(ptrRange.StartValue);
                        }
                        if (this.LinearGauge.MinimumValue < 0 && ptrRange.StartValue == 0)
                        {
                            startValue = Math.Abs(this.LinearGauge.MinimumValue) + Math.Abs(ptrRange.StartValue);
                            startValue = (((startValue / LinearGauge.MajorDifference)) * m_minorTicksPixels);
                        }

                        float height = (ptrRange.EndValue / LinearGauge.MajorDifference) * m_minorTicksPixels;
                        float endValueRangeHeight = 0f;
                        if (this.LinearGauge.MinimumValue < 0)
                        {
                            height = ((Math.Abs(this.LinearGauge.MinimumValue) + ptrRange.EndValue) / LinearGauge.MajorDifference) * m_minorTicksPixels;
                        }
                        endValueRangeHeight = height;
                        if (this.LinearGauge.MinimumValue < 0 && ptrRange.StartValue == 0)
                        {
                            endValueRangeHeight = (((ptrRange.EndValue - ptrRange.StartValue) / LinearGauge.MajorDifference) * m_minorTicksPixels);
                        }
                        if (ptrRange.StartValue == 0)
                            Graphics.FillRectangle(new SolidBrush(ptrRange.Color), this.LinearGauge.Width / 2 + 10, start + (majorTicksDistance * m_minorTicksPixels) - height, 8, endValueRangeHeight);
                        else if (ptrRange.StartValue > 0)
                            Graphics.FillRectangle(new SolidBrush(ptrRange.Color), this.LinearGauge.Width / 2 + 10, start + (majorTicksDistance * m_minorTicksPixels) - height, 8, (((ptrRange.EndValue - ptrRange.StartValue) / LinearGauge.MajorDifference) * m_minorTicksPixels));
                        else if (ptrRange.StartValue < 0)
                        {
                            Graphics.FillRectangle(new SolidBrush(ptrRange.Color), this.LinearGauge.Width / 2 + 10, start + (majorTicksDistance * m_minorTicksPixels) - height, 8, (((ptrRange.EndValue - ptrRange.StartValue) / LinearGauge.MajorDifference) * m_minorTicksPixels));
                        }
                    }
                }
            }

            public void DrawPointer(System.Drawing.Graphics Graphics)
            {
                Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                GraphicsPath path = new GraphicsPath();
                int a = 0;
                if (this.LinearGauge.MinimumValue < 0)
                    a = (int)Math.Ceiling((((Math.Abs(this.LinearGauge.MinimumValue) + LinearGauge.Value) / (float)LinearGauge.MajorDifference) * m_minorTicksPixels));
                else
                    a = (int)Math.Ceiling(((LinearGauge.Value / (float)LinearGauge.MajorDifference) * m_minorTicksPixels));
                int y = (this.LinearGauge.Height / 2 + 5 + LinearGauge.MajorTicksHeight) - LinearGauge.MajorTicksHeight;
                a = 10 + (int)Math.Ceiling((majorTicksDistance * m_minorTicksPixels)) - a;
                Rectangle rect = new Rectangle(new Point(this.LinearGauge.Width / 2 + 28, a), new Size(32, 32));
                SizeF sf = Graphics.MeasureString(this.LinearGauge.Value.ToString(), this.LinearGauge.GaugelabelFont);
                PointF point = new PointF(rect.X + rect.Width / 2 - sf.Width / 2, rect.Y + rect.Height / 2 - sf.Height / 2);
                Graphics.FillEllipse(new SolidBrush(LinearGauge.NeedleColor), rect);
                Graphics.DrawEllipse(new Pen(ColorTranslator.FromHtml("#00a0d1")), rect);
                Graphics.DrawLine(new Pen(ColorTranslator.FromHtml("#00a0d1")), rect.X, rect.Y + rect.Height / 2, rect.X - 18, rect.Y + rect.Height / 2);
                Graphics.DrawString(Math.Round(LinearGauge.Value, 2).ToString(), this.LinearGauge.GaugelabelFont, new SolidBrush(ColorTranslator.FromHtml("#ffffff")), point);
            }

            [Obsolete]
            public void UpdateRenderer(System.Windows.Forms.PaintEventArgs PaintEventArgs)
            {
                DrawLines(PaintEventArgs.Graphics);
                DrawRanges(PaintEventArgs.Graphics);
                DrawPointer(PaintEventArgs.Graphics);
            }
        }        

        private void Panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }       

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            ReadArduino();
        }

        public  void ReadArduino()
        {
            try
            {
                

                bunifuFlatButton1.Text = "Başla";
                label1.Text = "Bağlanıyor..";
                SerialPortManager.SerialPortManager.ReturnValue = string.Empty;
                SerialPortManager.SerialPortManager.IsFirstSending = true;               
                SerialPortManager.SerialPortManager.Open();
                label1.Text = "Bağlandı.";
                bunifuCircleProgressbar3.Value = 100;         
                timer1.Start();
                timer2.Start();
                timer3.Start();

            }
            catch (Exception)
            {
                label1.Text = "Bağlantı yok.";
                bunifuCircleProgressbar3.Value = 30;
                bunifuFlatButton1.Text = "Tekrar Dene";
                SerialPortManager.SerialPortManager.Close();
            }
        }

        private  void Timer1_Tick(object sender, EventArgs e)
        {            
            try
            {
                
                string[] pots = veriler.Split('|');
                radialGauge1.Value = float.Parse(pots[0]) ;
                bunifuCustomLabel2.Text = pots[0];        
                              
            }
            catch (Exception)
            {
             
            }
        }
     
        public void Spm_OnMessageReceived(string message, bool isCompleted, bool isSuccess)
        {
            veriler = message;
            if (isCompleted)
            {
                SerialPortManager.SerialPortManager.Close();
                if (isSuccess)
                {
                    // Do somethings here
                    MessageBox.Show("Operation succeeded!");
                }
                else
                {
                    // Do somethings here
                    MessageBox.Show("Operation failed");
                }
                // Finish process here or restart
            }
        }

        private  void Timer2_Tick(object sender, EventArgs e)
        { 
            try
            {               
                string[] pots = veriler.Split('|');
                bunifuCircleProgressbar1.Value = int.Parse(pots[1]);
                bunifuCircleProgressbar2.Value = int.Parse(pots[1]);
                bunifuCircleProgressbar4.Value = int.Parse(pots[1]);
                linearGauge1.Value = float.Parse(pots[1]);
                bunifuCustomLabel5.Text = pots[1];
            }
            catch (Exception)
            {                           
                bunifuCircleProgressbar3.Value = 30;        
            }
        }    
                

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
           
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();           
            SerialPortManager.SerialPortManager.Close();           
            Application.Exit();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            SerialPortManager.SerialPortManager.OnMessageReceived += Spm_OnMessageReceived;
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
                       
            string[] pots = veriler.Split('|');
            chart1.ChartAreas[0].AxisX.Minimum = mimm;
            chart1.ChartAreas[0].AxisX.Maximum = maksm;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 200;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(mimm, maksm);
            this.chart1.Series[0].Points.AddXY((maksm), pots[0]);
            this.chart1.Series[1].Points.AddXY((maksm), pots[1]);

            maksm++;
                mimm++;

            

        }
       
    }
    }
