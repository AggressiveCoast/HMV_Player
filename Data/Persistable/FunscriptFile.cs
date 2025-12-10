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
        public double at { get; set; }
        public int pos { get; set; }
    }
}