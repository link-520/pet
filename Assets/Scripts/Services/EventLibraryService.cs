using System.Collections.Generic;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 事件库服务。负责提供官方事件模板。
    /// </summary>
    public class EventLibraryService
    {
        public static EventLibraryService Shared { get; private set; }

        private readonly List<EventDefinition> events = new List<EventDefinition>();

        public EventLibraryService()
        {
            InitializeMockEvents();
        }

        public static EventLibraryService GetShared()
        {
            if (Shared == null)
            {
                Shared = new EventLibraryService();
            }

            return Shared;
        }

        public IReadOnlyList<EventDefinition> GetAllEvents()
        {
            return events;
        }

        public List<EventDefinition> GetEventsByType(EventType type)
        {
            List<EventDefinition> result = new List<EventDefinition>();
            foreach (EventDefinition eventDefinition in events)
            {
                if (eventDefinition.Type == type)
                {
                    result.Add(eventDefinition);
                }
            }

            return result;
        }

        public List<EventDefinition> GetEventsByDimension(DimensionType dimension)
        {
            List<EventDefinition> result = new List<EventDefinition>();
            foreach (EventDefinition eventDefinition in events)
            {
                if (eventDefinition.Dimension == dimension)
                {
                    result.Add(eventDefinition);
                }
            }

            return result;
        }

        public EventDefinition GetEventById(string eventId)
        {
            return events.Find(item => item.Id == eventId);
        }

        public List<EventDefinition> GetPlayerPersonalEvents(PlayerData player)
        {
            List<EventDefinition> result = new List<EventDefinition>();
            if (player == null)
            {
                return result;
            }

            foreach (PlayerEventData playerEvent in player.PersonalEvents)
            {
                if (!playerEvent.IsInPersonalLibrary)
                {
                    continue;
                }

                EventDefinition eventDefinition = GetEventById(playerEvent.EventId);
                if (eventDefinition != null)
                {
                    result.Add(eventDefinition);
                }
            }

            return result;
        }

        private void InitializeMockEvents()
        {
            events.Clear();

            events.Add(new EventDefinition("run", "跑步", DimensionType.Body, EventType.Continuous, 1, 10f, 2f, "跑步 10 分钟。", "event_run"));
            events.Add(new EventDefinition("study", "学习", DimensionType.Knowledge, EventType.Continuous, 1, 30f, 3f, "专注学习 30 分钟。", "event_study"));
            events.Add(new EventDefinition("read", "阅读", DimensionType.Knowledge, EventType.Continuous, 1, 20f, 2f, "阅读 20 分钟。", "event_read"));
            events.Add(new EventDefinition("work", "推进项目", DimensionType.Career, EventType.Continuous, 1, 30f, 3f, "推进工作或项目 30 分钟。", "event_work"));
            events.Add(new EventDefinition("meditation", "冥想", DimensionType.Happiness, EventType.Continuous, 1, 10f, 1f, "冥想 10 分钟。", "event_meditation"));
            events.Add(new EventDefinition("fruit", "吃水果", DimensionType.Happiness, EventType.Record, 1, 0f, 2f, "记录一次健康的小快乐。", "event_fruit"));
            events.Add(new EventDefinition("date", "约会", DimensionType.Relationship, EventType.Record, 1, 0f, 2f, "记录一次亲密关系投入。", "event_date"));
            events.Add(new EventDefinition("bookkeeping", "记账", DimensionType.Wealth, EventType.Record, 1, 0f, 1f, "记录一次财务整理。", "event_bookkeeping"));
            events.Add(new EventDefinition("water", "喝水", DimensionType.Body, EventType.Record, 1, 0f, 1f, "记录一次喝水。", "event_water"));
        }
    }
}
