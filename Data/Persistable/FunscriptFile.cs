using System;
using System.Collections.Generic;

namespace HMV_Player.Data;

public class FunscriptFile {

    public MetaData metaData { get; set; }
    public Action[] actions { get; set; }

    public class MetaData {
        public string title { get; set; }
        public string description { get; set; }
        public string[] performers { get; set; }
        public string video_Url { get; set; }
        public string[] tags { get; set; }
        public long duration { get; set; }
        public long average_Speed { get; set; }
        public string creator { get; set; }
    }

    public class Action {
        public double at { get; set; } // milli
        public int pos { get; set; }
    }
    
    public class ActionTimeComparer : IComparer<Action>
    {
        public int Compare(Action? x, Action? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return x.at.CompareTo(y.at);
        }
    }
}