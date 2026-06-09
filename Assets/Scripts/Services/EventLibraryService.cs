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
            events.Add(new EventDefinition("fitness", "健身", DimensionType.Body, EventType.Continuous, 1, 45f, 3f, "力量或有氧训练 45 分钟。", "event_fitness"));
            events.Add(new EventDefinition("walk", "散步", DimensionType.Body, EventType.Continuous, 1, 30f, 1f, "轻松散步 30 分钟。", "event_walk"));
            events.Add(new EventDefinition("deep_work", "深度工作", DimensionType.Career, EventType.Continuous, 1, 60f, 4f, "深度推进一项重要任务 60 分钟。", "event_deep_work"));
            events.Add(new EventDefinition("budget_review", "财务复盘", DimensionType.Wealth, EventType.Continuous, 1, 20f, 2f, "整理预算、账单或投资记录 20 分钟。", "event_budget"));
            events.Add(new EventDefinition("family_time", "陪伴家人", DimensionType.Relationship, EventType.Continuous, 1, 30f, 2f, "高质量陪伴家人或朋友 30 分钟。", "event_family"));
            events.Add(new EventDefinition("fruit", "吃水果", DimensionType.Happiness, EventType.Record, 1, 0f, 2f, "记录一次健康的小快乐。", "event_fruit"));
            events.Add(new EventDefinition("date", "约会", DimensionType.Relationship, EventType.Record, 1, 0f, 2f, "记录一次亲密关系投入。", "event_date"));
            events.Add(new EventDefinition("bookkeeping", "记账", DimensionType.Wealth, EventType.Record, 1, 0f, 1f, "记录一次财务整理。", "event_bookkeeping"));
            events.Add(new EventDefinition("water", "喝水", DimensionType.Body, EventType.Record, 1, 0f, 1f, "记录一次喝水。", "event_water"));
            events.Add(new EventDefinition("early_sleep", "早睡", DimensionType.Body, EventType.Record, 1, 0f, 2f, "记录一次按计划早睡。", "event_sleep"));
            events.Add(new EventDefinition("healthy_meal", "健康饮食", DimensionType.Body, EventType.Record, 1, 0f, 2f, "记录一次健康饮食。", "event_meal"));
            events.Add(new EventDefinition("share_chat", "主动交流", DimensionType.Relationship, EventType.Record, 1, 0f, 1f, "记录一次主动联系或认真交流。", "event_chat"));
            events.Add(new EventDefinition("small_win", "今日小胜利", DimensionType.Happiness, EventType.Record, 1, 0f, 1f, "记录一次让自己开心的小事。", "event_win"));
        }
    }
}
