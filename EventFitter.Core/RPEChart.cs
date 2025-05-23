﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//generated automatically

namespace EventFitter.Core
{

    public class BPMListItem
    {
        public float bpm { get; set; }
        public List<int> startTime { get; set; }
    }
    public class META
    {
        public int RPEVersion { get; set; }
        public string background { get; set; }
        public string charter { get; set; }
        public string composer { get; set; }
        public string id { get; set; }
        public string level { get; set; }
        public string name { get; set; }
        public int offset { get; set; }
        public string song { get; set; }
    }
    public class AlphaControlItem
    {
        public AlphaControlItem(float xx)
        {
            alpha = 1.0f;
            easing = 1;
            x = xx;
        }
        public float alpha { get; set; }
        public int easing { get; set; }
        public float x { get; set; }
    }
    public class RPEEvent
    {
        public RPEEvent()
        {
            linkgroup = 0;
            easingLeft = 0;
            easingRight = 1;
        }


        public int GetMontonicity()
        {
            if(end > start)
            {
                return 1;
            }
            else if(end < start)
            {
                return -1;
            }
            return 0;
        }

        private double getTime(List<int> triple)
        {
            return triple[2] == 0 ? 0 : (double)triple[0] + ((double)triple[1] / (double)triple[2]);
        }
        public double getDuration()
        {
            return getTime(endTime)-getTime(startTime);
        }
        public double getVelocity()
        {
            return (end-start)/getDuration();
        }

        public float easingLeft { get; set; }
        public float easingRight { get; set; }
        public int easingType { get; set; }
        public float end { get; set; }
        public List<int> endTime { get; set; }
        public int linkgroup { get; set; }
        public float start { get; set; }
        public List<int> startTime { get; set; }
    }
    public class SpeedEventsItem
    {
        public float end { get; set; }
        public List<int> endTime { get; set; }
        public int linkgroup { get; set; }
        public float start { get; set; }
        public List<int> startTime { get; set; }
    }
    public class EventLayersItem
    {
        public List<RPEEvent> alphaEvents { get; set; }
        public List<RPEEvent> moveXEvents { get; set; }
        public List<RPEEvent> moveYEvents { get; set; }
        public List<RPEEvent> rotateEvents { get; set; }
        public List<SpeedEventsItem> speedEvents { get; set; }
    }
    public class Extended
    {
        public Extended() {
            RPEEvent defaultInc = new RPEEvent();
            defaultInc.start = 0.0f;
            defaultInc.end = 0.0f;
            defaultInc.easingType = 1;
            defaultInc.startTime = new List<int> { 0, 0, 1 };
            defaultInc.endTime = new List<int> { 1, 0, 1 };
            inclineEvents = new List<RPEEvent>
            {
                defaultInc
            };
        }
        public List<RPEEvent> inclineEvents { get; set; }
    }
    public class NotesItem
    {
        public int above { get; set; }
        public int alpha { get; set; }
        public List<int> endTime { get; set; }
        public int isFake { get; set; }
        public float positionX { get; set; }
        public float size { get; set; }
        public float speed { get; set; }
        public List<int> startTime { get; set; }
        public int type { get; set; }
        public float visibleTime { get; set; }
        public float yOffset { get; set; }
    }
    public class PosControlItem
    {
        public PosControlItem(float xx)
        {
            pos = 1.0f;
            easing = 1;
            x = xx;
        }
        public int easing { get; set; }
        public float pos { get; set; }
        public float x { get; set; }
    }
    public class SizeControlItem
    {
        public SizeControlItem(float xx)
        {
            size = 1.0f;
            easing = 1;
            x = xx;
        }
        public int easing { get; set; }
        public float size { get; set; }
        public float x { get; set; }
    }
    public class SkewControlItem
    {
        public SkewControlItem(float xx)
        {
            skew = 0.0f;
            easing = 1;
            x = xx;
        }
        public int easing { get; set; }
        public float skew { get; set; }
        public float x { get; set; }
    }
    public class YControlItem
    {
        public YControlItem(float xx)
        {
            y = 1.0f;
            easing = 1;
            x = xx;
        }
        public int easing { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }
    public class JudgeLineListItem
    {
        public int @Group { get; set; }
        public string Name { get; set; }
        public string Texture { get; set; }
        public List<AlphaControlItem> alphaControl { get; set; }
        public float bpmfactor { get; set; }
        public List<EventLayersItem> eventLayers { get; set; }
        public Extended extended { get; set; }
        public int father { get; set; }
        public int isCover { get; set; }
        public List<NotesItem> notes { get; set; }
        public int numOfNotes { get; set; }
        public List<PosControlItem> posControl { get; set; }
        public List<SizeControlItem> sizeControl { get; set; }
        public List<SkewControlItem> skewControl { get; set; }
        public List<YControlItem> yControl { get; set; }
        public int zOrder { get; set; }
    }
    public class RPEChart
    {
        public List<BPMListItem> BPMList { get; set; }
        public META META { get; set; }
        public List<string> judgeLineGroup { get; set; }
        public List<JudgeLineListItem> judgeLineList { get; set; }
    }
}
