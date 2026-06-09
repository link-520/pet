using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 一件装备的静态定义。
    /// </summary>
    [Serializable]
    public class EquipmentDefinition
    {
        public string Id;
        public string Name;
        public EquipmentType Type;
        public string IconId;
        public DimensionType RequiredDimension;
        public float RequiredDimensionScore;
        public string RequiredEventId;
        public float RequiredEventTotalScore;

        public EquipmentDefinition(
            string id,
            string name,
            EquipmentType type,
            string iconId,
            DimensionType requiredDimension,
            float requiredDimensionScore,
            string requiredEventId = "",
            float requiredEventTotalScore = 0f)
        {
            Id = id;
            Name = name;
            Type = type;
            IconId = iconId;
            RequiredDimension = requiredDimension;
            RequiredDimensionScore = requiredDimensionScore;
            RequiredEventId = requiredEventId;
            RequiredEventTotalScore = requiredEventTotalScore;
        }
    }
}
