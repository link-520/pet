using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 玩家针对某个事件的运行时数据。
    /// </summary>
    [Serializable]
    public class PlayerEventData
    {
        public string EventId;
        public bool IsInPersonalLibrary;
        public int TodayCount;
        public float TodayMinutes;
        public float TodayScore;
        public float TotalCount;
        public float TotalMinutes;
        public float TotalScore;
        public bool TodayCompleted;
        public bool IsActive;

        public PlayerEventData(string eventId, bool isInPersonalLibrary = true)
        {
            EventId = eventId;
            IsInPersonalLibrary = isInPersonalLibrary;
            TodayCount = 0;
            TodayMinutes = 0f;
            TodayScore = 0f;
            TotalCount = 0f;
            TotalMinutes = 0f;
            TotalScore = 0f;
            TodayCompleted = false;
            IsActive = false;
        }
    }
}
