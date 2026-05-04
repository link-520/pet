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
        public int CompletedCount;
        public bool IsActive;

        public PlayerEventData(string eventId)
        {
            EventId = eventId;
            CompletedCount = 0;
            IsActive = false;
        }
    }
}
