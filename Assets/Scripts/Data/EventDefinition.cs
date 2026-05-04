using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 一个事件的静态定义。今天先用代码里的假数据创建。
    /// </summary>
    [Serializable]
    public class EventDefinition
    {
        public string Id;
        public string Name;
        public EventType Type;
        public DimensionType Dimension;
        public int RequiredCount;
        public int DurationMinutes;
        public int Score;

        public EventDefinition(string id, string name, EventType type, DimensionType dimension, int requiredCount, int durationMinutes, int score)
        {
            Id = id;
            Name = name;
            Type = type;
            Dimension = dimension;
            RequiredCount = requiredCount;
            DurationMinutes = durationMinutes;
            Score = score;
        }
    }
}
