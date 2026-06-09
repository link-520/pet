using System.Collections.Generic;
using System;
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
        private readonly SaveDataService saveDataService;
        private readonly DailySettlementService dailySettlementService;
        private readonly EquipmentService equipmentService;
        private PlayerData playerData;

        public PlayerDataService(EventLibraryService eventLibraryService)
        {
            this.eventLibraryService = eventLibraryService;
            saveDataService = new SaveDataService();
            dailySettlementService = new DailySettlementService();
            equipmentService = new EquipmentService(EquipmentLibraryService.GetShared());
            playerData = saveDataService.Load() ?? CreateDefaultPlayerData();
            RepairPlayerData();
            dailySettlementService.SettleIfNeeded(playerData, DateTime.Now);
            equipmentService.RefreshUnlockedEquipment(playerData);
            SaveNow();
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
            SaveNow();
        }

        public void InitializePlayer(string nickname, string petId, string dimensionPlanId, List<string> personalEventIds)
        {
            playerData.Nickname = string.IsNullOrEmpty(nickname) ? "Player" : nickname;
            playerData.PetId = string.IsNullOrEmpty(petId) ? "pet_default" : petId;
            ApplyDimensionPlan(dimensionPlanId, true);

            playerData.PersonalEvents.Clear();
            if (personalEventIds != null)
            {
                foreach (string eventId in personalEventIds)
                {
                    AddEventToPersonalLibrary(eventId);
                }
            }

            if (playerData.PersonalEvents.Count == 0)
            {
                AddDefaultPersonalEvents(playerData);
            }

            playerData.IsInitialized = true;
            SaveNow();
        }

        public bool ApplyDimensionPlan(string dimensionPlanId, bool alsoResetCurrentDimensions = false)
        {
            DimensionPlanDefinition plan = DimensionPlanLibraryService.GetShared().GetPlanById(dimensionPlanId);
            if (plan == null)
            {
                return false;
            }

            playerData.TargetDimensions = plan.TargetDimensions.Clone();
            if (alsoResetCurrentDimensions)
            {
                playerData.CurrentDimensions = plan.TargetDimensions.Clone();
            }

            SaveNow();
            return true;
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
            SaveNow();
        }

        public void RemoveEventFromPersonalLibrary(string eventId)
        {
            PlayerEventData playerEvent = GetPlayerEventData(eventId);
            playerEvent.IsInPersonalLibrary = false;
            SaveNow();
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
            equipmentService.RefreshUnlockedEquipment(playerData);
            SaveNow();
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
            equipmentService.RefreshUnlockedEquipment(playerData);
            SaveNow();
        }

        public bool StartContinuousEvent(string eventId)
        {
            EventDefinition eventDefinition = eventLibraryService.GetEventById(eventId);
            if (eventDefinition == null || eventDefinition.Type != EventType.Continuous || !IsEventInPersonalLibrary(eventId))
            {
                return false;
            }

            playerData.ActiveContinuousEventId = eventId;
            playerData.ActiveContinuousEventStartUnixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            foreach (PlayerEventData playerEvent in playerData.PersonalEvents)
            {
                playerEvent.IsActive = playerEvent.EventId == eventId;
            }

            SaveNow();
            return true;
        }

        public bool FinishActiveContinuousEvent()
        {
            if (string.IsNullOrEmpty(playerData.ActiveContinuousEventId) || playerData.ActiveContinuousEventStartUnixSeconds <= 0)
            {
                return false;
            }

            long elapsedSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() - playerData.ActiveContinuousEventStartUnixSeconds;
            float elapsedMinutes = Math.Max(0f, elapsedSeconds / 60f);
            string eventId = playerData.ActiveContinuousEventId;

            playerData.ActiveContinuousEventId = string.Empty;
            playerData.ActiveContinuousEventStartUnixSeconds = 0;

            foreach (PlayerEventData playerEvent in playerData.PersonalEvents)
            {
                playerEvent.IsActive = false;
            }

            RecordContinuousEvent(eventId, elapsedMinutes);
            return true;
        }

        public float GetActiveContinuousElapsedMinutes()
        {
            if (string.IsNullOrEmpty(playerData.ActiveContinuousEventId) || playerData.ActiveContinuousEventStartUnixSeconds <= 0)
            {
                return 0f;
            }

            long elapsedSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() - playerData.ActiveContinuousEventStartUnixSeconds;
            return Math.Max(0f, elapsedSeconds / 60f);
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

            SaveNow();
        }

        public void ForceSettleToday()
        {
            dailySettlementService.ForceSettleToday(playerData);
            equipmentService.RefreshUnlockedEquipment(playerData);
            SaveNow();
        }

        public bool Equip(string equipmentId)
        {
            bool changed = equipmentService.Equip(playerData, equipmentId);
            if (changed)
            {
                SaveNow();
            }

            return changed;
        }

        public void Unequip(string equipmentId)
        {
            equipmentService.Unequip(playerData, equipmentId);
            SaveNow();
        }

        public void SaveNow()
        {
            saveDataService.Save(playerData);
        }

        public void ResetSaveData()
        {
            saveDataService.DeleteSave();
            playerData = CreateDefaultPlayerData();
            SaveNow();
        }

        private PlayerData CreateDefaultPlayerData()
        {
            PlayerData data = new PlayerData
            {
                Nickname = "阿澈",
                PetId = "pet_doubao",
                TargetDimensions = new DimensionSet(10f, 6f, 8f, 4f, 10f, 0f),
                CurrentDimensions = new DimensionSet(10f, 6f, 8f, 4f, 10f, 0f),
                TodayDimensions = new DimensionSet(),
                LastSettlementDate = DateTime.Now.ToString("yyyy-MM-dd"),
                IsInitialized = true,
                ActiveContinuousEventId = string.Empty,
                ActiveContinuousEventStartUnixSeconds = 0,
                SelectedEventId = "fruit"
            };

            AddDefaultPersonalEvents(data);

            data.UnlockedEquipmentIds.Add("equip_old_running_shoes");
            data.UnlockedEquipmentIds.Add("equip_round_glasses");
            data.EquippedEquipmentIds.Add("equip_old_running_shoes");

            return data;
        }

        private void RepairPlayerData()
        {
            if (playerData == null)
            {
                playerData = CreateDefaultPlayerData();
                return;
            }

            if (playerData.TargetDimensions == null)
            {
                playerData.TargetDimensions = new DimensionSet(10f, 6f, 8f, 4f, 10f, 0f);
            }

            if (playerData.CurrentDimensions == null)
            {
                playerData.CurrentDimensions = playerData.TargetDimensions.Clone();
            }

            if (playerData.TodayDimensions == null)
            {
                playerData.TodayDimensions = new DimensionSet();
            }

            if (playerData.ActiveContinuousEventId == null)
            {
                playerData.ActiveContinuousEventId = string.Empty;
            }

            bool shouldCreateDefaultPersonalEvents = playerData.PersonalEvents == null || playerData.PersonalEvents.Count == 0;
            if (playerData.PersonalEvents == null)
            {
                playerData.PersonalEvents = new List<PlayerEventData>();
            }

            if (playerData.UnlockedEquipmentIds == null)
            {
                playerData.UnlockedEquipmentIds = new List<string>();
            }

            if (playerData.EquippedEquipmentIds == null)
            {
                playerData.EquippedEquipmentIds = new List<string>();
            }

            if (string.IsNullOrEmpty(playerData.SelectedEventId))
            {
                playerData.SelectedEventId = "fruit";
            }

            if (shouldCreateDefaultPersonalEvents)
            {
                AddDefaultPersonalEvents(playerData);
            }
        }

        private void AddDefaultPersonalEvents(PlayerData data)
        {
            AddDefaultPersonalEvent(data, "run");
            AddDefaultPersonalEvent(data, "study");
            AddDefaultPersonalEvent(data, "work");
            AddDefaultPersonalEvent(data, "fruit");
            AddDefaultPersonalEvent(data, "date");
            AddDefaultPersonalEvent(data, "water");
            AddDefaultPersonalEvent(data, "bookkeeping");
        }

        private void AddDefaultPersonalEvent(PlayerData data, string eventId)
        {
            if (eventLibraryService.GetEventById(eventId) == null)
            {
                return;
            }

            PlayerEventData playerEvent = data.PersonalEvents.Find(item => item.EventId == eventId);
            if (playerEvent == null)
            {
                data.PersonalEvents.Add(new PlayerEventData(eventId));
                return;
            }

            playerEvent.IsInPersonalLibrary = true;
        }
    }
}
