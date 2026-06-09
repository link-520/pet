using System.Collections.Generic;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 目标六维方案库。
    /// </summary>
    public class DimensionPlanLibraryService
    {
        public static DimensionPlanLibraryService Shared { get; private set; }

        private readonly List<DimensionPlanDefinition> plans = new List<DimensionPlanDefinition>();

        public DimensionPlanLibraryService()
        {
            InitializeDefaultPlans();
        }

        public static DimensionPlanLibraryService GetShared()
        {
            if (Shared == null)
            {
                Shared = new DimensionPlanLibraryService();
            }

            return Shared;
        }

        public IReadOnlyList<DimensionPlanDefinition> GetAllPlans()
        {
            return plans;
        }

        public DimensionPlanDefinition GetPlanById(string planId)
        {
            return plans.Find(item => item.Id == planId);
        }

        private void InitializeDefaultPlans()
        {
            plans.Clear();

            plans.Add(new DimensionPlanDefinition("balanced", "均衡成长", "六个维度均衡发展，适合稳定推进。", new DimensionSet(6f, 6f, 6f, 6f, 6f, 6f)));
            plans.Add(new DimensionPlanDefinition("study_sprint", "学习冲刺", "重点提升知识，同时保持身体和快乐。", new DimensionSet(4f, 9f, 6f, 5f, 4f, 6f)));
            plans.Add(new DimensionPlanDefinition("career_first", "事业优先", "优先推进事业和财富。", new DimensionSet(5f, 5f, 9f, 4f, 7f, 5f)));
            plans.Add(new DimensionPlanDefinition("life_repair", "关系修复", "补足关系与快乐，恢复生活状态。", new DimensionSet(5f, 4f, 5f, 9f, 5f, 7f)));
            plans.Add(new DimensionPlanDefinition("wealth_advance", "财富进阶", "专注财富管理，同时兼顾事业。", new DimensionSet(4f, 5f, 6f, 4f, 9f, 5f)));
            plans.Add(new DimensionPlanDefinition("body_reset", "身体重启", "通过身体习惯重建日常节奏。", new DimensionSet(9f, 5f, 5f, 5f, 4f, 6f)));
        }
    }
}
