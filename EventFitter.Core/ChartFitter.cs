using System;
using System.Collections.Generic;
using System.Drawing; // For Point
using System.IO; // For Path, File
using System.Text; // For UTF8Encoding
using System.Threading.Tasks; // For Task
using Newtonsoft.Json; // For JsonConvert

namespace EventFitter.Core
{
    public class FittingProgressReport
    {
        public string? CurrentStepMessage { get; set; }
        public int LineIndex { get; set; } 
        public int EventType { get; set; } 
        public Point[]? RPEPoints { get; set; }
        public Point[]? CalcPoints { get; set; }
    }

    public class ChartFitter
    {
        public RPEChart? chart;
        public string filePath = "null";
        public const double PRECISION = 1.0;
        public int lineIndex; 
        
        private Point[] currentRPEPoints = { new Point(0,0), new Point(0, 0) };
        private Point[] currentCalcPoints = { new Point(0,0), new Point(0, 0) };

        public IProgress<FittingProgressReport>? ProgressReporter { get; set; }

        public ChartFitter() {} 

        public async Task startFitting() 
        {
            if (chart == null) throw new InvalidOperationException("Chart data is not loaded.");
            if (string.IsNullOrEmpty(filePath) || filePath == "null") throw new InvalidOperationException("File path is not set.");

            int lineCount = chart.judgeLineList.Count;

            for (lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                ProgressReporter?.Report(new FittingProgressReport { 
                    CurrentStepMessage = $"Starting Line: {lineIndex}",
                    LineIndex = this.lineIndex,
                    EventType = 0 // General event type for line processing start
                });

                chart.judgeLineList[lineIndex].father = -1;
                chart.judgeLineList[lineIndex].extended = new Extended();

                chart.judgeLineList[lineIndex].alphaControl = new List<AlphaControlItem> { new AlphaControlItem(0.0f), new AlphaControlItem(9999999.0f) };
                chart.judgeLineList[lineIndex].posControl = new List<PosControlItem> { new PosControlItem(0.0f), new PosControlItem(9999999.0f) };
                chart.judgeLineList[lineIndex].sizeControl = new List<SizeControlItem> { new SizeControlItem(0.0f), new SizeControlItem(9999999.0f) };
                chart.judgeLineList[lineIndex].skewControl = new List<SkewControlItem> { new SkewControlItem(0.0f), new SkewControlItem(9999999.0f) };
                chart.judgeLineList[lineIndex].yControl = new List<YControlItem> { new YControlItem(0.0f), new YControlItem(9999999.0f) };
                
                StartFitInLineAndType(1); // moveXEvents
                StartFitInLineAndType(2); // moveYEvents
                StartFitInLineAndType(3); // rotateEvents
                StartFitInLineAndType(4); // alphaEvents
            }
            
            UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
            string? directory = Path.GetDirectoryName(filePath);
            string originalFileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string newFileName = $"{originalFileName}_Fitted{extension}";
            string newFilePath = directory == null ? newFileName : Path.Combine(directory, newFileName);

            string jsonContent = JsonConvert.SerializeObject(chart, Formatting.Indented); 
            await File.WriteAllTextAsync(newFilePath, jsonContent, utf8);
            ProgressReporter?.Report(new FittingProgressReport { CurrentStepMessage = "Fitting complete. File saved.", LineIndex = this.lineIndex, EventType = 0 });
        }

        public double getTime(List<int> triple)
        {
            if (triple == null || triple.Count != 3) return 0; 
            return triple[2] == 0 ? 0 : (double)triple[0] + ((double)triple[1] / (double)triple[2]);
        }

        public void StartFitInLineAndType(object typeObj)
        {
            if (chart == null) return;
            int eventType = Convert.ToInt32(typeObj); 

            ProgressReporter?.Report(new FittingProgressReport { 
                CurrentStepMessage = $"Line: {this.lineIndex}, Event Type: {eventType}",
                LineIndex = this.lineIndex,
                EventType = eventType
            });

            List<RPEEvent> Events;
            switch (eventType) 
            {
                case 1: Events = chart.judgeLineList[this.lineIndex].eventLayers[0].moveXEvents; break;
                case 2: Events = chart.judgeLineList[this.lineIndex].eventLayers[0].moveYEvents; break;
                case 3: Events = chart.judgeLineList[this.lineIndex].eventLayers[0].rotateEvents; break;
                case 4: Events = chart.judgeLineList[this.lineIndex].eventLayers[0].alphaEvents; break;
                default: return;
            }

            for (int i = 1; i < Events.Count - 1; i++)
            {
                if (Events[i].GetMontonicity() == 0 && (Events[i - 1].end == Events[i].start))
                {
                    Events.RemoveAt(i); 
                    i--;
                }
            }

            if (Events.Count <= 5) return;

            List<int[]> cuttedEventsIndex = new List<int[]>();
            int baseLoopIndex = 0; 
            for (int Length = 1; baseLoopIndex + Length < Events.Count; Length++)
            {
                int lastMontonicity = Events[baseLoopIndex + Length - 1].GetMontonicity();
                if (Events[baseLoopIndex + Length].GetMontonicity() != lastMontonicity || Math.Abs(getTime(Events[baseLoopIndex + Length].startTime) - getTime(Events[baseLoopIndex + Length - 1].endTime)) > 0.1)
                {
                    ProgressReporter?.Report(new FittingProgressReport { CurrentStepMessage = $"Segmenting: {baseLoopIndex} to {baseLoopIndex + Length -1}", LineIndex = this.lineIndex, EventType = eventType });
                    cuttedEventsIndex.Add(new int[] { baseLoopIndex, baseLoopIndex + Length - 1 });
                    baseLoopIndex += Length;
                    Length = 0; 
                }
            }
            cuttedEventsIndex.Add(new int[] { baseLoopIndex, Events.Count - 1 });

            for (int ii = 0; ii < cuttedEventsIndex.Count; ii++)
            {
                int[] indexes = cuttedEventsIndex[ii];
                int lastVelocityChange = 0;
                int curVelocityChange = 0;
                for (int idx = indexes[0] + 1; idx < indexes[1] && idx + 1 < Events.Count; idx++)
                {
                    double velDiff1 = Events[idx].getVelocity() - Events[idx - 1].getVelocity();
                    if (Math.Abs(velDiff1) >= 0.01) lastVelocityChange = velDiff1 > 0 ? 1 : -1; else lastVelocityChange = 0;
                    
                    double velDiff2 = Events[idx + 1].getVelocity() - Events[idx].getVelocity();
                    if (Math.Abs(velDiff2) >= 0.01) curVelocityChange = velDiff2 > 0 ? 1 : -1; else curVelocityChange = 0;

                    if (lastVelocityChange != curVelocityChange && Math.Abs(lastVelocityChange) >= 0.05) 
                    {
                        cuttedEventsIndex.Insert(ii, new int[] { indexes[0], idx });
                        cuttedEventsIndex.Insert(ii + 1, new int[] { idx + 1, indexes[1] });
                        cuttedEventsIndex.RemoveAt(ii + 2);
                        break; 
                    }
                }
            }

            for (int ie = cuttedEventsIndex.Count - 1; ie >= 0; ie--)
            {
                int[] evtIdxs = cuttedEventsIndex[ie];
                ProgressReporter?.Report(new FittingProgressReport { CurrentStepMessage = $"Fitting segment: {evtIdxs[0]} to {evtIdxs[1]}", LineIndex = this.lineIndex, EventType = eventType});
                int currentLocalBaseIndex = evtIdxs[0]; 
                NiheInRange(ref currentLocalBaseIndex, evtIdxs[1] - evtIdxs[0] + 1, ref Events, eventType);
            }
        }

        public void NiheInRange(ref int localBaseIndex, int Length, ref List<RPEEvent> Events, int eventTypeArg) 
        {
            if (chart == null || Events == null) return;
            if (localBaseIndex < 0) localBaseIndex = 0;
            if (localBaseIndex + Length > Events.Count) Length = Events.Count - localBaseIndex; 
            if (Length <= 0) return;

            double[] values;
            double[] beatTimes;
            double beatTimeRange;

            GetEventInRange(ref Events, localBaseIndex, Length, out values, out beatTimes, out beatTimeRange);
            int EaseIndex = CalcEaseWithValues(values, beatTimes, beatTimeRange, PRECISION, eventTypeArg);

            if (EaseIndex != -1)
            {
                RPEEvent fittedEvent = new RPEEvent {
                    start = Events[localBaseIndex].start,
                    end = Events[localBaseIndex + Length - 1].end,
                    startTime = Events[localBaseIndex].startTime,
                    endTime = Events[localBaseIndex + Length - 1].endTime,
                    easingType = EaseIndex
                };
                ReplaceEvent(fittedEvent, localBaseIndex, Length, ref Events);
            }
        }

        public void GetEventInRange(ref List<RPEEvent> Events, int localBaseIndex, int eventCount, out double[] values, out double[] beatTimes, out double beatTimeRange) 
        {
            values = new double[eventCount + 1];
            beatTimes = new double[eventCount]; 

            for (int i = 0; i < eventCount; i++)
            {
                values[i] = Events[localBaseIndex + i].start;
                beatTimes[i] = Events[localBaseIndex + i].getDuration();
            }
            values[eventCount] = Events[localBaseIndex + eventCount - 1].end;
            beatTimeRange = getTime(Events[localBaseIndex + eventCount - 1].endTime) - getTime(Events[localBaseIndex].startTime);
        }

        public void ReplaceEvent(RPEEvent eventToAdd, int localBaseIndex, int removeCount, ref List<RPEEvent> eventsToBeReplaced)
        {
            if (eventsToBeReplaced == null) return;
            eventsToBeReplaced.RemoveRange(localBaseIndex, removeCount);
            eventsToBeReplaced.Insert(localBaseIndex, eventToAdd);
        }

        public int CalcEaseWithValues(double[] values, double[] beatTimes, double beatTimeRange, double precision, int eventTypeArg)
        {
            if (values.Length < 2) return -1;
            if (values.Length == 2 && beatTimeRange > 0) return 1;
            if (values.Length < 4 && values.Length > 2) return -1; 

            this.currentRPEPoints = new Point[values.Length]; 
            this.currentCalcPoints = new Point[values.Length];

            double valueRange = values[^1] - values[0];
            if (Math.Abs(valueRange) < 1e-6 && values.Length > 1) {
                bool isConstant = true;
                for(int k=1; k<values.Length; ++k) if(Math.Abs(values[k] - values[0]) > precision) { isConstant = false; break; }
                if(isConstant) return 1; 
            }

            for (int easingIndex = 1; easingIndex <= Easings.easeFuncs.Length; easingIndex++)
            {
                bool successFlag = true;
                double accumulatedTime = 0;

                if (this.currentRPEPoints.Length > 0) this.currentRPEPoints[0] = new Point(25, 275);
                if (this.currentCalcPoints.Length > 0) this.currentCalcPoints[0] = new Point(25, 275);

                for (int i = 1; i < values.Length; i++) 
                {
                    accumulatedTime += beatTimes[i-1]; 
                    double timeRatio = beatTimeRange == 0 ? (i == values.Length -1 ? 1.0 : 0.0) : Math.Min(1.0, accumulatedTime / beatTimeRange);
                    double calcValue = values[0] + (Easings.easeFuncs[easingIndex - 1](timeRatio) * valueRange);

                    if (Math.Abs(calcValue - values[i]) > precision) { successFlag = false; break; }
                    
                    if (i < this.currentRPEPoints.Length) this.currentRPEPoints[i] = new Point(25 + (int)(timeRatio * 250), 275 - (int)(((values[i] - values[0]) / (valueRange == 0 ? 1 : valueRange)) * 250));
                    if (i < this.currentCalcPoints.Length) this.currentCalcPoints[i] = new Point(25 + (int)(timeRatio * 250), 275 - (int)(((calcValue - values[0]) / (valueRange == 0 ? 1 : valueRange)) * 250));
                }

                if (successFlag)
                {
                    ProgressReporter?.Report(new FittingProgressReport { 
                        RPEPoints = (Point[])this.currentRPEPoints.Clone(), 
                        CalcPoints = (Point[])this.currentCalcPoints.Clone(),
                        LineIndex = this.lineIndex,
                        EventType = eventTypeArg, 
                        CurrentStepMessage = $"Calculated ease for segment, easing index: {easingIndex}"
                    });
                    return easingIndex;
                }
            }
            ProgressReporter?.Report(new FittingProgressReport { 
                RPEPoints = (Point[])this.currentRPEPoints.Clone(), 
                CalcPoints = (Point[])this.currentCalcPoints.Clone(),
                LineIndex = this.lineIndex,
                EventType = eventTypeArg, 
                CurrentStepMessage = "No suitable easing found for segment."
            });
            return -1;
        }
    }
}
