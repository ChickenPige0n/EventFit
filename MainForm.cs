using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text;
using System.Text.Unicode;

namespace EventFitter
{
    public partial class MainForm : Form
    {
        const double PRECISION = 1.0;

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
                chart.judgeLineList[lineIndex].father = -1;


                chart.judgeLineList[lineIndex].extended = new Extended();

                chart.judgeLineList[lineIndex].alphaControl = new List<AlphaControlItem>();
                chart.judgeLineList[lineIndex].alphaControl.Add(new AlphaControlItem(0.0f));
                chart.judgeLineList[lineIndex].alphaControl.Add(new AlphaControlItem(9999999.0f));

                chart.judgeLineList[lineIndex].posControl = new List<PosControlItem>();
                chart.judgeLineList[lineIndex].posControl.Add(new PosControlItem(0.0f));
                chart.judgeLineList[lineIndex].posControl.Add(new PosControlItem(9999999.0f));

                chart.judgeLineList[lineIndex].sizeControl = new List<SizeControlItem>();
                chart.judgeLineList[lineIndex].sizeControl.Add(new SizeControlItem(0.0f));
                chart.judgeLineList[lineIndex].sizeControl.Add(new SizeControlItem(9999999.0f));

                chart.judgeLineList[lineIndex].skewControl = new List<SkewControlItem>();
                chart.judgeLineList[lineIndex].skewControl.Add(new SkewControlItem(0.0f));
                chart.judgeLineList[lineIndex].skewControl.Add(new SkewControlItem(9999999.0f));

                chart.judgeLineList[lineIndex].yControl = new List<YControlItem>();
                chart.judgeLineList[lineIndex].yControl.Add(new YControlItem(0.0f));
                chart.judgeLineList[lineIndex].yControl.Add(new YControlItem(9999999.0f));

                StartFitInLineAndType(1);
                StartFitInLineAndType(2);
                StartFitInLineAndType(3);
                StartFitInLineAndType(4);
            }
            Graphics g = CreateGraphics();
            g.Clear(Color.White);
            g.Dispose();
            UTF8Encoding utf8 = new UTF8Encoding(false);
            StreamWriter sw = new StreamWriter("C:\\Users\\23369\\Desktop\\86022134.json", false, utf8);
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


            


            //RemoveRange 包括index上的事件

            //移除静态事件
            int i;
            for (i = 1; i < Events.Count - 1; i++)
            {
                if (Events[i].GetMontonicity() == 0 && (Events[i - 1].end == Events[i].start))
                {
                    Events.RemoveRange(i, 1);
                    i -= 1;
                }
            }


            if (Events.Count <= 5)
            {
                return;
            }



            List<int[]> cuttedEventsIndex = new List<int[]> { };

            int baseIndex = 0;
            int Length;

            //选出单调性突变的事件组
            
            int lastMontonicity;
            for (Length = 1;baseIndex+Length<Events.Count; Length++)
            {
                lastMontonicity = Events[baseIndex+Length-1].GetMontonicity();
                if (Events[baseIndex + Length].GetMontonicity()!=lastMontonicity||Math.Abs(getTime(Events[baseIndex + Length].startTime) - getTime(Events[baseIndex + Length-1].endTime))>0.1)
                {
                    eventTypeLabel.Text = baseIndex + "/" + baseIndex+Length;
                    cuttedEventsIndex.Add(new int[2] {baseIndex, baseIndex+Length-1});

                    baseIndex += Length;
                    Length = 0;
                }
            }
            cuttedEventsIndex.Add(new int[2] { baseIndex, Events.Count - 1 });

            


            for (int ii = 0;ii < cuttedEventsIndex.Count;ii++)
            {
                int[] indexes = cuttedEventsIndex[ii];


                double velDiff;
                int lastVelocityChange;
                int curVelocityChange;
                for (int idx = indexes[0]+1; idx < indexes[1]&& idx + 1 < Events.Count; idx++)
                {
                    velDiff = Events[idx].getVelocity() - Events[idx - 1].getVelocity();
                    //与idx-1相比idx事件在加速
                    if(velDiff>0)
                    {
                        lastVelocityChange = 1;
                    }
                    //在减速
                    else if (velDiff < 0)
                    {
                        lastVelocityChange = -1;
                    }
                    else
                    {
                        lastVelocityChange = 0;
                    }
                    velDiff = Events[idx+1].getVelocity() - Events[idx].getVelocity();
                    //与idx相比idx+1事件在加速
                    if (velDiff > 0)
                    {
                        curVelocityChange = 1;
                    }
                    //在减速
                    else if (velDiff < 0)
                    {
                        curVelocityChange = -1;
                    }
                    else
                    {
                        curVelocityChange = 0;
                    }


                    if (lastVelocityChange != curVelocityChange && Math.Abs(lastVelocityChange)>=0.05)
                    {
                        // 0   1 2 3 4 5
                        //   ↑
                        //insert 1:
                        //分割
                        
                        cuttedEventsIndex.Insert(ii, new int[2] { indexes[0], idx });
                        cuttedEventsIndex.Insert(ii + 1, new int[2] { idx + 1, indexes[1] });
                        cuttedEventsIndex.RemoveAt(ii+2);
                        //ii += 1;
                        break;
                    }
                }
            }


            //根据加速度划分完毕 开始拟合
            for(int ie = cuttedEventsIndex.Count-1;ie>=0;ie--)
            {
                int[] evtIdxs = cuttedEventsIndex[ie];
                eventTypeLabel.Text = evtIdxs[0] + "/" + evtIdxs[1];
                NiheInRange(ref evtIdxs[0], evtIdxs[1] - evtIdxs[0] + 1,ref Events);
            }
        }


        private void NiheInRange(ref int baseIndex,int Length,ref List<RPEEvent> Events)
        {
            if (baseIndex < 0)
            {
                baseIndex = 0;
            }
            double[] values;
            double[] beatTimes;
            double beatTimeRange;

            GetEventInRange(ref Events, baseIndex, Length, out values, out beatTimes, out beatTimeRange);

            int EaseIndex = CalcEaseWithValues(values, beatTimes, beatTimeRange, PRECISION);

            if (EaseIndex != -1)
            {
                RPEEvent fittedEvent = new RPEEvent();
                fittedEvent.start = Events[baseIndex].start;
                fittedEvent.end = Events[baseIndex + Length - 1].end;
                fittedEvent.startTime = Events[baseIndex].startTime;
                fittedEvent.endTime = Events[baseIndex + Length - 1].endTime;
                fittedEvent.easingType = EaseIndex;

                ReplaceEvent(fittedEvent, baseIndex, Length, ref Events);

            }
        }





        private void GetEventInRange(ref List<RPEEvent> Events, int baseIndex, int eventCount, out double[] values, out double[] beatTimes, out double beatTimeRange)
        {
            values = new double[eventCount + 1];
            beatTimes = new double[eventCount];

            for (int outIndex = baseIndex; outIndex < baseIndex + eventCount; outIndex++)
            {
                // 获取当前事件的start值和前一个事件到当前事件的duration
                values[outIndex - baseIndex] = Events[outIndex].start;
                beatTimes[outIndex - baseIndex] = Events[outIndex].getDuration();
            }

            // 最后一个事件的end值存储到values数组的最后一个位置
            values[^1] = Events[baseIndex + eventCount - 1].end;

            // 计算beatTimeRange
            beatTimeRange = getTime(Events[baseIndex + eventCount - 1].endTime) - getTime(Events[baseIndex].startTime);
        }

        //<summary>
        //1:moveX 2:moveY 3:rotation 4:alpha 5:speed
        //</summary>
        private void ReplaceEvent(RPEEvent eventToAdd, int baseIndex, int removeCount,ref List<RPEEvent> eventsToBeReplaced)
        {
            eventsToBeReplaced.RemoveRange(baseIndex, removeCount);
            eventsToBeReplaced.Insert(baseIndex, eventToAdd);
        }





        public int CalcEaseWithValues(double[] values, double[] beatTimes, double beatTimeRange,double precision)
        {

            RPEPoints = new Point[values.Length];
            calcPoints = new Point[values.Length];

            double valueRange = values[^1] - values[0];
            bool successFlag;
            int successIndex = -1;

            for(int easingIndex = 1; easingIndex <= 28; easingIndex++)
            {
                double curBeat = beatTimes[0];
                double calcValue = 0;
                successFlag = true;
                int i;
                for (i = 1; i < beatTimes.Length; i++)
                {

                    calcValue = values[0] + (Easings.easeFuncs[easingIndex - 1](curBeat / beatTimeRange) * valueRange);

                    if (Math.Abs(calcValue - values[i]) > precision)
                    {
                        successFlag = false;
                        break;
                    }

                    if (i < (beatTimes.Length - 1))
                    {
                        curBeat += beatTimes[i];
                    }

                    RPEPoints[i-1] = new Point
                        (
                            25 + (int)((curBeat / beatTimeRange)*250),
                            275 - (int)((values[i] - values[0]) / valueRange * 250)
                        );
                    calcPoints[i-1] = new Point
                        (
                            25 + (int)((curBeat / beatTimeRange) * 250),
                            275 - (int)((calcValue - values[0]) / valueRange * 250)
                        );
                    drawCurve();


                    Update();
                }
                if (successFlag)
                {
                    return easingIndex;
                }
            }

            return successIndex;//-1
        }

        void Main_Paint(object sender,PaintEventArgs e)
        {
            //drawCurve();
        }



        private void drawCurve()
        {
            // 为控件创建绘图缓存区位图
            Bitmap backBuffer = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);

            // 双缓冲技术
            using (Graphics g = Graphics.FromImage(backBuffer))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                // 绘制前，先用背景刷刷新图像
                g.Clear(Color.White);

                Pen p = new Pen(Color.White, 5);
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
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    if (i < RPEPoints.Length - 2)
                    {
                        i++;
                    }
                }

                // 注意释放 Pen 对象
                p.Dispose();
                // 使用 Graphics 对象的 DrawImage 将缓存中的位图复制到屏幕上
                pictureBox1.Image = backBuffer;
            }

        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void baseIndexLabel_Click(object sender, EventArgs e)
        {

        }

        private void lineIndexLabel_Click(object sender, EventArgs e)
        {

        }

        private void eventTypeLabel_Click(object sender, EventArgs e)
        {

        }
    }
}