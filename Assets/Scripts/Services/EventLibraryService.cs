using System.Collections.Generic;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 事件库服务。今天先用硬编码假数据，之后可以替换成配置表或 ScriptableObject。
    /// </summary>
    public class EventLibraryService
    {
        private readonly List<EventDefinition> events = new List<EventDefinition>();

        public EventLibraryService()
        {
            InitializeMockEvents();
        }

        public IReadOnlyList<EventDefinition> GetAllEvents()
        {
            return events;
        }

        public EventDefinition GetEventById(string eventId)
        {
            return events.Find(item => item.Id == eventId);
        }

        private void InitializeMockEvents()
        {
            events.Clear();

            events.Add(new EventDefinition("run", "晨跑", EventType.Continuous, DimensionType.Body, 1, 10, 2));
            events.Add(new EventDefinition("read", "读书", EventType.Continuous, DimensionType.Knowledge, 1, 20, 3));
            events.Add(new EventDefinition("date", "约朋友吃饭", EventType.Record, DimensionType.Relationship, 1, 0, 2));
            events.Add(new EventDefinition("movie", "看一部电影", EventType.Record, DimensionType.Happiness, 1, 0, 2));
        }
    }
}
