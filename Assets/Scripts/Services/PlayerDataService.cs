using System.Collections.Generic;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 玩家数据服务。所有玩家数据修改都从这里进入。
    /// </summary>
    public class PlayerDataService
    {
        public static PlayerDataService Shared { get; private set; }

        private readonly EventLibraryService eventLibraryService;
        private PlayerData playerData;

        public PlayerDataService(EventLibraryService eventLibraryService)
        {
            this.eventLibraryService = eventLibraryService;
            playerData = CreateMockPlayerData();
        }

        public static PlayerDataService GetShared(EventLibraryService eventLibraryService)
        {
            if (Shared == null)
            {
                Shared = new PlayerDataService(eventLibraryService);
            }

            return Shared;
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

        public List<EventDefinition> GetPersonalEvents()
        {
            return eventLibraryService.GetPlayerPersonalEvents(playerData);
        }

        public List<EventDefinition> GetPersonalEventsByType(EventType type)
        {
            List<EventDefinition> result = new List<EventDefinition>();
            foreach (EventDefinition eventDefinition in GetPersonalEvents())
            {
                if (eventDefinition.Type == type)
                {
                    result.Add(eventDefinition);
                }
            }

            return result;
        }

        public PlayerEventData GetPlayerEventData(string eventId)
        {
            PlayerEventData playerEvent = playerData.PersonalEvents.Find(item => item.EventId == eventId);
            if (playerEvent == null)
            {
                playerEvent = new PlayerEventData(eventId, false);
                playerData.PersonalEvents.Add(playerEvent);
            }

            return playerEvent;
        }

        public void AddEventToPersonalLibrary(string eventId)
        {
            if (eventLibraryService.GetEventById(eventId) == null)
            {
                return;
            }

            PlayerEventData playerEvent = GetPlayerEventData(eventId);
            playerEvent.IsInPersonalLibrary = true;
        }

        public void RemoveEventFromPersonalLibrary(string eventId)
        {
            PlayerEventData playerEvent = GetPlayerEventData(eventId);
            playerEvent.IsInPersonalLibrary = false;
        }

        public bool IsEventInPersonalLibrary(string eventId)
        {
            PlayerEventData playerEvent = playerData.PersonalEvents.Find(item => item.EventId == eventId);
            return playerEvent != null && playerEvent.IsInPersonalLibrary;
        }

        public void RecordEventOnce(string eventId)
        {
            EventDefinition eventDefinition = eventLibraryService.GetEventById(eventId);
            if (eventDefinition == null || eventDefinition.Type != EventType.Record || !IsEventInPersonalLibrary(eventId))
            {
                return;
            }

            PlayerEventData playerEvent = GetPlayerEventData(eventId);
            playerEvent.TodayCount += 1;
            playerEvent.TodayScore += eventDefinition.RewardScore;
            playerEvent.TotalCount += 1f;
            playerEvent.TotalScore += eventDefinition.RewardScore;
            playerEvent.TodayCompleted = true;

            playerData.TodayDimensions.AddValue(eventDefinition.Dimension, eventDefinition.RewardScore);
        }

        public void RecordContinuousEvent(string eventId, float elapsedMinutes)
        {
            EventDefinition eventDefinition = eventLibraryService.GetEventById(eventId);
            if (eventDefinition == null || eventDefinition.Type != EventType.Continuous || !IsEventInPersonalLibrary(eventId))
            {
                return;
            }

            float safeMinutes = elapsedMinutes < 0f ? 0f : elapsedMinutes;
            float score = eventDefinition.RequiredMinutes > 0f
                ? safeMinutes / eventDefinition.RequiredMinutes * eventDefinition.RewardScore
                : 0f;

            PlayerEventData playerEvent = GetPlayerEventData(eventId);
            playerEvent.TodayMinutes += safeMinutes;
            playerEvent.TodayScore += score;
            playerEvent.TotalMinutes += safeMinutes;
            playerEvent.TotalScore += score;
            playerEvent.TodayCompleted = score > 0f;

            playerData.TodayDimensions.AddValue(eventDefinition.Dimension, score);
        }

        public void ClearTodayProgress()
        {
            playerData.TodayDimensions.Clear();

            foreach (PlayerEventData playerEvent in playerData.PersonalEvents)
            {
                playerEvent.TodayCount = 0;
                playerEvent.TodayMinutes = 0f;
                playerEvent.TodayScore = 0f;
                playerEvent.TodayCompleted = false;
                playerEvent.IsActive = false;
            }
        }

        private PlayerData CreateMockPlayerData()
        {
            PlayerData data = new PlayerData
            {
                Nickname = "阿澈",
                PetId = "pet_doubao",
                TargetDimensions = new DimensionSet(10f, 6f, 8f, 4f, 10f, 0f),
                CurrentDimensions = new DimensionSet(10f, 6f, 8f, 4f, 10f, 0f),
                TodayDimensions = new DimensionSet(),
                LastSettlementDate = string.Empty,
                IsInitialized = true,
                SelectedEventId = "fruit"
            };

            data.PersonalEvents.Add(new PlayerEventData("run"));
            data.PersonalEvents.Add(new PlayerEventData("study"));
            data.PersonalEvents.Add(new PlayerEventData("fruit"));
            data.PersonalEvents.Add(new PlayerEventData("date"));

            data.UnlockedEquipmentIds.Add("equip_old_running_shoes");
            data.UnlockedEquipmentIds.Add("equip_round_glasses");
            data.EquippedEquipmentIds.Add("equip_old_running_shoes");

            return data;
        }
    }
}
