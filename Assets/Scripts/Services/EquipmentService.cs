using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 根据六维和事件累计分数刷新装备解锁状态。
    /// </summary>
    public class EquipmentService
    {
        private readonly EquipmentLibraryService equipmentLibraryService;

        public EquipmentService(EquipmentLibraryService equipmentLibraryService)
        {
            this.equipmentLibraryService = equipmentLibraryService;
        }

        public void RefreshUnlockedEquipment(PlayerData playerData)
        {
            if (playerData == null)
            {
                return;
            }

            foreach (EquipmentDefinition definition in equipmentLibraryService.GetAllEquipment())
            {
                bool shouldUnlock = IsRequirementMet(playerData, definition);
                bool unlocked = playerData.UnlockedEquipmentIds.Contains(definition.Id);

                if (shouldUnlock && !unlocked)
                {
                    playerData.UnlockedEquipmentIds.Add(definition.Id);
                }
                else if (!shouldUnlock && unlocked)
                {
                    playerData.UnlockedEquipmentIds.Remove(definition.Id);
                    playerData.EquippedEquipmentIds.Remove(definition.Id);
                }
            }
        }

        public bool Equip(PlayerData playerData, string equipmentId)
        {
            if (playerData == null || !playerData.UnlockedEquipmentIds.Contains(equipmentId))
            {
                return false;
            }

            EquipmentDefinition definition = equipmentLibraryService.GetEquipmentById(equipmentId);
            if (definition == null)
            {
                return false;
            }

            playerData.EquippedEquipmentIds.RemoveAll(item =>
            {
                EquipmentDefinition equippedDefinition = equipmentLibraryService.GetEquipmentById(item);
                return equippedDefinition != null && equippedDefinition.Type == definition.Type;
            });

            if (!playerData.EquippedEquipmentIds.Contains(equipmentId))
            {
                playerData.EquippedEquipmentIds.Add(equipmentId);
            }

            return true;
        }

        public void Unequip(PlayerData playerData, string equipmentId)
        {
            if (playerData != null)
            {
                playerData.EquippedEquipmentIds.Remove(equipmentId);
            }
        }

        private bool IsRequirementMet(PlayerData playerData, EquipmentDefinition definition)
        {
            if (playerData.CurrentDimensions.GetValue(definition.RequiredDimension) < definition.RequiredDimensionScore)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.RequiredEventId))
            {
                return true;
            }

            PlayerEventData eventData = playerData.PersonalEvents.Find(item => item.EventId == definition.RequiredEventId);
            return eventData != null && eventData.TotalScore >= definition.RequiredEventTotalScore;
        }
    }
}
