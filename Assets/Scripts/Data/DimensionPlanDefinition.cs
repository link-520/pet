using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 目标六维预设方案。
    /// </summary>
    [Serializable]
    public class DimensionPlanDefinition
    {
        public string Id;
        public string Name;
        public string Description;
        public DimensionSet TargetDimensions;

        public DimensionPlanDefinition(string id, string name, string description, DimensionSet targetDimensions)
        {
            Id = id;
            Name = name;
            Description = description;
            TargetDimensions = targetDimensions;
        }
    }
}
