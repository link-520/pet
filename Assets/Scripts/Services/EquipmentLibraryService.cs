using System.Collections.Generic;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 装备库服务。负责提供官方装备定义。
    /// </summary>
    public class EquipmentLibraryService
    {
        public static EquipmentLibraryService Shared { get; private set; }

        private readonly List<EquipmentDefinition> equipment = new List<EquipmentDefinition>();

        public EquipmentLibraryService()
        {
            InitializeDefaultEquipment();
        }

        public static EquipmentLibraryService GetShared()
        {
            if (Shared == null)
            {
                Shared = new EquipmentLibraryService();
            }

            return Shared;
        }

        public IReadOnlyList<EquipmentDefinition> GetAllEquipment()
        {
            return equipment;
        }

        public EquipmentDefinition GetEquipmentById(string equipmentId)
        {
            return equipment.Find(item => item.Id == equipmentId);
        }

        private void InitializeDefaultEquipment()
        {
            equipment.Clear();

            equipment.Add(new EquipmentDefinition("equip_old_running_shoes", "旧跑鞋", EquipmentType.Clothes, "equip_clothes", DimensionType.Body, 6f, "run", 10f));
            equipment.Add(new EquipmentDefinition("equip_round_glasses", "圆框眼镜", EquipmentType.Glasses, "equip_glasses", DimensionType.Knowledge, 6f, "study", 12f));
            equipment.Add(new EquipmentDefinition("equip_straw_hat", "草帽", EquipmentType.Hat, "equip_hat", DimensionType.Happiness, 5f, "fruit", 8f));
            equipment.Add(new EquipmentDefinition("equip_blue_tie", "蓝领带", EquipmentType.Tie, "equip_tie", DimensionType.Career, 6f, "work", 12f));
            equipment.Add(new EquipmentDefinition("equip_warm_gloves", "暖手套", EquipmentType.Gloves, "equip_gloves", DimensionType.Relationship, 5f, "date", 8f));
            equipment.Add(new EquipmentDefinition("equip_pet_badge", "宠物徽章", EquipmentType.Pet, "equip_pet", DimensionType.Wealth, 5f, "bookkeeping", 6f));
        }
    }
}
