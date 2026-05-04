using System;
using System.Collections.Generic;

namespace LifeRPG.Data
{
    /// <summary>
    /// 玩家数据总入口。MVP 阶段不接存档，运行时直接使用假数据。
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string PlayerName;
        public string PetName;
        public DimensionSet Dimensions;
        public DimensionSet TargetDimensions;
        public List<string> UnlockedEquipments;
        public List<PlayerEventData> Events;
        public string SelectedEventId;

        public PlayerData()
        {
            PlayerName = "Player";
            PetName = "Pet";
            Dimensions = new DimensionSet();
            TargetDimensions = new DimensionSet();
            UnlockedEquipments = new List<string>();
            Events = new List<PlayerEventData>();
            SelectedEventId = string.Empty;
        }
    }
}
