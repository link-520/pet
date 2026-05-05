using System;
using System.Collections.Generic;

namespace LifeRPG.Data
{
    /// <summary>
    /// 玩家数据总入口。所有 UI 通过 PlayerDataService 修改它。
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string Nickname;
        public string PetId;
        public DimensionSet TargetDimensions;
        public DimensionSet CurrentDimensions;
        public DimensionSet TodayDimensions;
        public List<PlayerEventData> PersonalEvents;
        public List<string> UnlockedEquipmentIds;
        public List<string> EquippedEquipmentIds;
        public string LastSettlementDate;
        public bool IsInitialized;

        // 当前 UI 选中事件，后续可以移到 Controller 状态里。
        public string SelectedEventId;

        public PlayerData()
        {
            Nickname = "Player";
            PetId = "pet_default";
            TargetDimensions = new DimensionSet();
            CurrentDimensions = new DimensionSet();
            TodayDimensions = new DimensionSet();
            PersonalEvents = new List<PlayerEventData>();
            UnlockedEquipmentIds = new List<string>();
            EquippedEquipmentIds = new List<string>();
            LastSettlementDate = string.Empty;
            IsInitialized = false;
            SelectedEventId = string.Empty;
        }
    }
}
