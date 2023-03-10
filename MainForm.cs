using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text;

namespace EventFitter
{
    public partial class MainForm : Form
    {
        int lineIndex;

        RPEChart? chart;

        Point[] RPEPoints =  { new Point(0,0), new Point(0, 0) };
        Point[] calcPoints = { new Point(0,0), new Point(0, 0) };

        public MainForm()
        {
            InitializeComponent();

            


            OpenFileDialog ofd = new OpenFileDialog();
             ofd.InitialDirectory = Application.StartupPath;
             ofd.Title = "Please choose the chart to be fitted";
             ofd.Multiselect = true;
             ofd.Filter = "RPE Chart|*.json";
             ofd.FilterIndex = 2;
             ofd.RestoreDirectory = true;
             if (ofd.ShowDialog() == DialogResult.OK)
             { 
                string ChartContent = File.ReadAllText(ofd.FileName);
                chart = JsonConvert.DeserializeObject<RPEChart>(ChartContent);

                //startFitting();
            }
        }

        private void FitButton_Click(object sender, EventArgs e)
        {
            if(chart != null)
            {
                startFitting();
            }
        }

        

        public void startFitting()
        {
            int lineCount = chart.judgeLineList.Count;

            for (lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                StartFitInLineAndType(1);
                StartFitInLineAndType(2);
                StartFitInLineAndType(3);
                StartFitInLineAndType(4);
            }
            Graphics g = CreateGraphics();
            g.Clear(Color.White);
            g.Dispose();
            UTF8Encoding utf8 = new UTF8Encoding(false);
            StreamWriter sw = new StreamWriter("C:\\Users\\23369\\Desktop\\114514.json", false, utf8);
            sw.Write(JsonConvert.SerializeObject(chart));
            sw.Close();
        }


        public double getTime(List<int> triple)
        {
            return triple[2] == 0 ? 0 : (double)triple[0] + ((double)triple[1] / (double)triple[2]);
        }

        private void StartFitInLineAndType(object type)
        {
            lineIndexLabel.Text = "LineIndex: " + lineIndex;
            eventTypeLabel.Text = "EventType: " + type;
            List<RPEEvent> Events = new List<RPEEvent>();
            switch (type)
            {
                case 1:
                    Events = chart.judgeLineList[lineIndex].eventLayers[0].moveXEvents;
                    break;
                case 2:
                    Events = chart.judgeLineList[lineIndex].eventLayers[0].moveYEvents;
                    break;
                case 3:
                    Events = chart.judgeLineList[lineIndex].eventLayers[0].rotateEvents;
                    break;
                case 4:
                    Events = chart.judgeLineList[lineIndex].eventLayers[0].alphaEvents;
                    break;
            }
            
            int baseIndex = 0;

            double precision = 1;


            int indexStepLength = 1;
            int maxWindowLength = 128;
            int originWindowLength = 3;



            int fitWindowLength = originWindowLength;




            while (baseIndex < Events.Count)
            {
                int i;
                for (i = 1; i + baseIndex < Events.Count - 1; i++)
                {
                    if (Events[baseIndex + i].GetMontonicity() == 0 && (Events[baseIndex + i - 1].end == Events[baseIndex + i].start))
                    {
                        Events.RemoveRange(baseIndex + i, 1);
                    }
                }


                double[] values;
                double[] beatTimes;
                double beatTimeRange;
                GetEventInRange(Events, baseIndex, fitWindowLength, out values, out beatTimes, out beatTimeRange);

                int EaseIndex = TryFitInRange(values, beatTimes, beatTimeRange, precision);
                if (EaseIndex != -1)
                {
                    RPEEvent fittedEvent = new RPEEvent();
                    fittedEvent.start = Events[baseIndex].start;
                    fittedEvent.end = Events[baseIndex + fitWindowLength - 1].end;
                    fittedEvent.startTime = Events[baseIndex].startTime;
                    fittedEvent.endTime = Events[baseIndex + fitWindowLength - 1].endTime;
                    fittedEvent.easingType = EaseIndex;

                    ReplaceEvent(fittedEvent, baseIndex, fitWindowLength, ref Events);
                    baseIndex += 1;
                    fitWindowLength = originWindowLength;

                }
                else if (fitWindowLength < maxWindowLength)
                {
                    fitWindowLength += 4;
                }
                else
                {
                    baseIndex += indexStepLength;
                    fitWindowLength = originWindowLength;
                }

            }
        }

        private void GetEventInRange(List<RPEEvent> Events, int baseIndex, int eventCount, out double[] values, out double[] beatTimes, out double beatTimeRange)
        {
            double endTime;
            double startTime;
            values = new double[eventCount];
            beatTimes = new double[eventCount];
            beatTimeRange = -getTime(Events[baseIndex].startTime)
                +
                getTime(Events[baseIndex + eventCount - 1].endTime);
            for (int outIndex = baseIndex; outIndex < baseIndex + eventCount; outIndex++)
            {
                values[outIndex - baseIndex] = Events[outIndex].start;
                endTime = getTime(Events[outIndex].endTime);
                startTime = getTime(Events[outIndex].startTime);
                beatTimes[outIndex - baseIndex] = endTime - startTime;
            }

            values[eventCount - 1] = Events[baseIndex + eventCount - 1].end;
        }

        //<summary>
        //1:moveX 2:moveY 3:rotation 4:alpha 5:speed
        //</summary>
        private void ReplaceEvent(RPEEvent eventToAdd, int baseIndex, int removeCount,ref List<RPEEvent> eventsToBeReplaced)
        {
            eventsToBeReplaced.RemoveRange(baseIndex, removeCount);
            eventsToBeReplaced.Insert(baseIndex==0?0:baseIndex-1, eventToAdd);
        }





        public int TryFitInRange(double[] values, double[] beatTimes, double beatTimeRange,double precision)
        {

            RPEPoints = new Point[values.Length];
            calcPoints = new Point[values.Length];

            double valueRange = values[^1] - values[0];
            bool successFlag;
            int successIndex = -1;

            for(int easingIndex = 1; easingIndex <= 28; easingIndex++)
            {
                double curBeat = 0;
                double calcValue = 0;
                successFlag = true;
                int i;
                for (i = 0; i < values.Length; i++)
                {
                    calcValue = values[0] + (Easings.easeFuncs[easingIndex - 1](curBeat / beatTimeRange) * valueRange);


                    RPEPoints[i] = new Point
                        (
                            25 + (int)Math.Abs((curBeat / beatTimeRange)*250),
                            25 + (int)Math.Abs(values[i] / valueRange*250)
                        );
                    calcPoints[i] = new Point
                        (
                            25 + (int)Math.Abs((curBeat / beatTimeRange) * 250),
                            25 + (int)Math.Abs(calcValue / valueRange * 250)
                        );
                    drawCurve();


                    Update();
                    
                    if (Math.Abs(calcValue - values[i]) > precision)
                    {
                        successFlag = false;
                        break;
                    }
                    if (i < (values.Length - 1))
                    {
                        curBeat += beatTimes[i];
                    }
                    
                }
                if (successFlag)
                {

                    Thread.Sleep(1000);
                    return easingIndex;
                }
            }

            return successIndex;
        }

        void Main_Paint(object sender,PaintEventArgs e)
        {
            //drawCurve();
        }

        
        private void drawCurve()
        {
            Update();
            Graphics g = pictureBox1.CreateGraphics();
            g.SmoothingMode = SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
            g.PixelOffsetMode= PixelOffsetMode.HighSpeed;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.CompositingQuality = CompositingQuality.HighQuality;
            
            g.Clear(Color.White);

            Pen p = new(Color.White, 5);
            //from 255 237 70 to 255 126 199
            //from 143 255 133 to 57 160 255
            
            int i = 0;
            foreach (var b in RPEPoints)
            {
                try
                {
                    if (RPEPoints[i + 1].X != 0 && RPEPoints[i + 1].Y != 0)
                    {
                        p.Color = Color.FromArgb(255, 237 - (int)(111 * i / RPEPoints.Length - 1), 70 + (int)(129 * i / RPEPoints.Length - 1));
                        g.DrawLine(p, b, RPEPoints[i + 1]);
                    }
                    if (calcPoints[i + 1].X != 0 && calcPoints[i + 1].Y != 0)
                    {
                        p.Color = Color.FromArgb(143 - (int)(86 * i / RPEPoints.Length - 1), 253 - (int)(95 * i / RPEPoints.Length - 1), 133 + (int)(122 * i / RPEPoints.Length - 1));
                        g.DrawLine(p, calcPoints[i], calcPoints[i + 1]);
                    }
                }catch (OverflowException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                if (i < RPEPoints.Length - 2)
                {
                    i++;
                }
            }

            g.Dispose();
            p.Dispose();
        }
        


        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void baseIndexLabel_Click(object sender, EventArgs e)
        {

        }
    }
}