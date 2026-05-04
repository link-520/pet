using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 玩家数据服务。MVP 阶段只负责创建假数据和提供少量读写方法。
    /// </summary>
    public class PlayerDataService
    {
        private readonly EventLibraryService eventLibraryService;
        private PlayerData playerData;

        public PlayerDataService(EventLibraryService eventLibraryService)
        {
            this.eventLibraryService = eventLibraryService;
            playerData = CreateMockPlayerData();
        }

        public PlayerData GetPlayerData()
        {
            return playerData;
        }

        public void SelectEvent(string eventId)
        {
            playerData.SelectedEventId = eventId;
        }

        public EventDefinition GetSelectedEvent()
        {
            if (string.IsNullOrEmpty(playerData.SelectedEventId))
            {
                return null;
            }

            return eventLibraryService.GetEventById(playerData.SelectedEventId);
        }

        /// <summary>
        /// MVP 版确认事件：只增加完成次数，并把事件分数加到对应六维。
        /// </summary>
        public void ConfirmSelectedEvent()
        {
            EventDefinition selectedEvent = GetSelectedEvent();
            if (selectedEvent == null)
            {
                return;
            }

            PlayerEventData playerEvent = playerData.Events.Find(item => item.EventId == selectedEvent.Id);
            if (playerEvent == null)
            {
                playerEvent = new PlayerEventData(selectedEvent.Id);
                playerData.Events.Add(playerEvent);
            }

            playerEvent.CompletedCount += selectedEvent.RequiredCount;
            playerData.Dimensions.AddValue(selectedEvent.Dimension, selectedEvent.Score);
        }

        private PlayerData CreateMockPlayerData()
        {
            PlayerData data = new PlayerData
            {
                PlayerName = "阿澈",
                PetName = "豆包",
                Dimensions = new DimensionSet(32, 18, 12, 24, 9, 28),
                TargetDimensions = new DimensionSet(50, 40, 35, 35, 25, 45)
            };

            foreach (EventDefinition eventDefinition in eventLibraryService.GetAllEvents())
            {
                data.Events.Add(new PlayerEventData(eventDefinition.Id));
            }

            data.UnlockedEquipments.Add("旧跑鞋：身体 +1");
            data.UnlockedEquipments.Add("便携笔记本：知识 +1");
            data.SelectedEventId = "run";

            return data;
        }
    }
}
