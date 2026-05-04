using System;
using System.Collections.Generic;
using LifeRPG.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 完整面板总视图。只负责接收数据并刷新 UI。
    /// </summary>
    public class MainPanelView : MonoBehaviour
    {
        [Header("宠物显示")]
        [SerializeField] private Image petImage;
        [SerializeField] private Sprite placeholderPetSprite;
        [SerializeField] private TMP_Text petNameText;

        [Header("装备占位")]
        [SerializeField] private TMP_Text equipmentText;

        [Header("六维显示")]
        [SerializeField] private Transform dimensionRoot;
        [SerializeField] private DimensionBarView dimensionBarPrefab;
        [SerializeField] private List<DimensionBarView> fixedDimensionBars = new List<DimensionBarView>();

        [Header("事件列表")]
        [SerializeField] private Transform eventListRoot;
        [SerializeField] private EventListItemView eventItemPrefab;
        [SerializeField] private TMP_Text eventFallbackText;

        [Header("当前状态")]
        [SerializeField] private TMP_Text currentEventText;
        [SerializeField] private Button confirmButton;

        private readonly List<DimensionBarView> runtimeDimensionBars = new List<DimensionBarView>();
        private readonly List<EventListItemView> runtimeEventItems = new List<EventListItemView>();

        public event Action<string> OnEventSelected;
        public event Action OnConfirmClicked;

        private void Awake()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(() => OnConfirmClicked?.Invoke());
            }

            HideSceneTemplates();
        }

        public void Refresh(PlayerData playerData, IReadOnlyList<EventDefinition> eventDefinitions)
        {
            if (playerData == null)
            {
                return;
            }

            RefreshPet(playerData);
            RefreshEquipment(playerData);
            RefreshDimensions(playerData.Dimensions, playerData.TargetDimensions);
            RefreshEvents(eventDefinitions, playerData);
            RefreshCurrentEvent(playerData, eventDefinitions);
        }

        private void RefreshPet(PlayerData playerData)
        {
            if (petImage != null && placeholderPetSprite != null)
            {
                petImage.sprite = placeholderPetSprite;
            }

            if (petNameText != null)
            {
                petNameText.text = $"{playerData.PlayerName} / {playerData.PetName}";
            }
        }

        private void RefreshEquipment(PlayerData playerData)
        {
            if (equipmentText != null)
            {
                equipmentText.text = playerData.UnlockedEquipments.Count > 0
                    ? "已解锁装备\n" + string.Join("\n", playerData.UnlockedEquipments)
                    : "暂无装备";
            }
        }

        private void RefreshDimensions(DimensionSet dimensions, DimensionSet targetDimensions)
        {
            if (dimensions == null)
            {
                return;
            }

            DimensionType[] types =
            {
                DimensionType.Body,
                DimensionType.Knowledge,
                DimensionType.Career,
                DimensionType.Relationship,
                DimensionType.Wealth,
                DimensionType.Happiness
            };

            if (dimensionBarPrefab != null && dimensionRoot != null)
            {
                ClearRuntimeDimensions();

                foreach (DimensionType type in types)
                {
                    DimensionBarView bar = Instantiate(dimensionBarPrefab, dimensionRoot);
                    bar.gameObject.SetActive(true);
                    bar.Refresh(type, dimensions.GetValue(type), targetDimensions.GetValue(type));
                    runtimeDimensionBars.Add(bar);
                }

                HideTemplateIfInRoot(dimensionBarPrefab.gameObject, dimensionRoot);

                return;
            }

            for (int i = 0; i < fixedDimensionBars.Count && i < types.Length; i++)
            {
                if (fixedDimensionBars[i] != null)
                {
                    DimensionType type = types[i];
                    fixedDimensionBars[i].Refresh(type, dimensions.GetValue(type), targetDimensions.GetValue(type));
                }
            }
        }

        private void RefreshEvents(IReadOnlyList<EventDefinition> eventDefinitions, PlayerData playerData)
        {
            if (eventDefinitions == null)
            {
                return;
            }

            if (eventItemPrefab != null && eventListRoot != null)
            {
                ClearRuntimeEvents();

                foreach (EventDefinition definition in eventDefinitions)
                {
                    EventListItemView item = Instantiate(eventItemPrefab, eventListRoot);
                    item.gameObject.SetActive(true);
                    item.Refresh(definition, FindPlayerEventData(playerData, definition.Id));
                    item.OnSelected += OnEventItemSelected;
                    runtimeEventItems.Add(item);
                }

                HideTemplateIfInRoot(eventItemPrefab.gameObject, eventListRoot);

                return;
            }

            if (eventFallbackText != null)
            {
                eventFallbackText.text = BuildEventFallbackText(eventDefinitions, playerData);
            }
        }

        private void RefreshCurrentEvent(PlayerData playerData, IReadOnlyList<EventDefinition> eventDefinitions)
        {
            if (currentEventText == null)
            {
                return;
            }

            EventDefinition selectedEvent = FindEventDefinition(eventDefinitions, playerData.SelectedEventId);
            currentEventText.text = selectedEvent == null ? "当前未选择事件" : $"当前选择：{selectedEvent.Name}";
        }

        private void OnEventItemSelected(string eventId)
        {
            OnEventSelected?.Invoke(eventId);
        }

        private PlayerEventData FindPlayerEventData(PlayerData playerData, string eventId)
        {
            return playerData.Events.Find(item => item.EventId == eventId);
        }

        private EventDefinition FindEventDefinition(IReadOnlyList<EventDefinition> eventDefinitions, string eventId)
        {
            if (string.IsNullOrEmpty(eventId) || eventDefinitions == null)
            {
                return null;
            }

            foreach (EventDefinition definition in eventDefinitions)
            {
                if (definition.Id == eventId)
                {
                    return definition;
                }
            }

            return null;
        }

        private string BuildEventFallbackText(IReadOnlyList<EventDefinition> eventDefinitions, PlayerData playerData)
        {
            List<string> lines = new List<string>();

            foreach (EventDefinition definition in eventDefinitions)
            {
                PlayerEventData progress = FindPlayerEventData(playerData, definition.Id);
                int completedCount = progress != null ? progress.CompletedCount : 0;
                string timeText = definition.DurationMinutes > 0 ? $"{definition.DurationMinutes}分钟" : "无分钟";
                lines.Add($"{definition.Name} / {definition.RequiredCount}次 / {timeText} / {definition.Score}分 / 已完成{completedCount}次");
            }

            return string.Join("\n", lines);
        }

        private void ClearRuntimeDimensions()
        {
            foreach (DimensionBarView bar in runtimeDimensionBars)
            {
                if (bar != null)
                {
                    Destroy(bar.gameObject);
                }
            }

            runtimeDimensionBars.Clear();
        }

        private void ClearRuntimeEvents()
        {
            foreach (EventListItemView item in runtimeEventItems)
            {
                if (item != null)
                {
                    item.OnSelected -= OnEventItemSelected;
                    Destroy(item.gameObject);
                }
            }

            runtimeEventItems.Clear();
        }

        private void HideSceneTemplates()
        {
            if (dimensionBarPrefab != null && dimensionRoot != null)
            {
                HideTemplateIfInRoot(dimensionBarPrefab.gameObject, dimensionRoot);
            }

            if (eventItemPrefab != null && eventListRoot != null)
            {
                HideTemplateIfInRoot(eventItemPrefab.gameObject, eventListRoot);
            }
        }

        private void HideTemplateIfInRoot(GameObject templateObject, Transform root)
        {
            if (templateObject == null || root == null)
            {
                return;
            }

            // 如果模板就是场景里放在列表容器下的对象，就隐藏它，只把它当复制模板。
            if (templateObject.transform.IsChildOf(root))
            {
                templateObject.SetActive(false);
            }
        }
    }
}
