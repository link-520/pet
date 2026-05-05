using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 一个事件的静态定义。
    /// </summary>
    [Serializable]
    public class EventDefinition
    {
        public string Id;
        public string Name;
        public DimensionType Dimension;
        public EventType Type;
        public int RequiredCount;
        public float RequiredMinutes;
        public float RewardScore;
        public string Description;
        public string IconId;

        public EventDefinition(string id, string name, DimensionType dimension, EventType type, int requiredCount, float requiredMinutes, float rewardScore, string description = "", string iconId = "")
        {
            Id = id;
            Name = name;
            Dimension = dimension;
            Type = type;
            RequiredCount = requiredCount;
            RequiredMinutes = requiredMinutes;
            RewardScore = rewardScore;
            Description = description;
            IconId = iconId;
        }
    }
}
