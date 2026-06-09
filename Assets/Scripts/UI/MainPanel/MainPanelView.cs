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

        [Header("事件填写")]
        [SerializeField] private TMP_Text currentEventText;
        [SerializeField] private EventFillPanelView eventFillPanelView;
        [SerializeField] private Button confirmButton;

        [Header("窗口控制")]
        [SerializeField] private Button closeButton;

        private readonly List<DimensionBarView> runtimeDimensionBars = new List<DimensionBarView>();
        private readonly List<EventListItemView> runtimeEventItems = new List<EventListItemView>();
        private readonly List<EventListItemView> fixedEventItems = new List<EventListItemView>();
        private EventDefinition selectedEvent;

        public event Action<string> OnEventSelected;
        public event Action OnConfirmClicked;
        public event Action OnCloseClicked;

        public bool CanSubmitSelectedRecordEvent => eventFillPanelView == null || eventFillPanelView.CanSubmitRecordEvent;

        private void Awake()
        {
            AutoBindReferences();

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(() => OnConfirmClicked?.Invoke());
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
            }

            if (eventFillPanelView != null)
            {
                eventFillPanelView.OnCompletionChanged += RefreshConfirmButton;
            }

            HideSceneTemplates();
        }

        public void Refresh(PlayerData playerData, IReadOnlyList<EventDefinition> eventDefinitions)
        {
            AutoBindReferences();

            if (playerData == null)
            {
                return;
            }

            RefreshPet(playerData);
            RefreshEquipment(playerData);
            RefreshDimensions(playerData.CurrentDimensions, playerData.TargetDimensions, playerData.TodayDimensions);
            RefreshEvents(eventDefinitions, playerData);
            RefreshEventFill(playerData, eventDefinitions);
        }

        private void RefreshPet(PlayerData playerData)
        {
            if (petImage != null && placeholderPetSprite != null)
            {
                petImage.sprite = placeholderPetSprite;
            }

            if (petNameText != null)
            {
                petNameText.text = $"{playerData.Nickname} / {playerData.PetId}";
            }
        }

        private void RefreshEquipment(PlayerData playerData)
        {
            if (equipmentText != null)
            {
                equipmentText.text = playerData.UnlockedEquipmentIds.Count > 0
                    ? "已解锁装备\n" + string.Join("\n", playerData.UnlockedEquipmentIds)
                    : "暂无装备";
            }
        }

        private void RefreshDimensions(DimensionSet currentDimensions, DimensionSet targetDimensions, DimensionSet todayDimensions)
        {
            if (currentDimensions == null || targetDimensions == null || todayDimensions == null)
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
                    bar.Refresh(type, currentDimensions.GetValue(type), targetDimensions.GetValue(type), todayDimensions.GetValue(type));
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
                    fixedDimensionBars[i].Refresh(type, currentDimensions.GetValue(type), targetDimensions.GetValue(type), todayDimensions.GetValue(type));
                }
            }
        }

        private void RefreshEvents(IReadOnlyList<EventDefinition> eventDefinitions, PlayerData playerData)
        {
            if (eventDefinitions == null)
            {
                return;
            }

            fixedEventItems.RemoveAll(item => item == null);
            if ((eventItemPrefab == null || eventListRoot == null) && fixedEventItems.Count > 0)
            {
                for (int i = 0; i < fixedEventItems.Count; i++)
                {
                    bool hasDefinition = i < eventDefinitions.Count;
                    fixedEventItems[i].gameObject.SetActive(hasDefinition);
                    fixedEventItems[i].OnSelected -= OnEventItemSelected;

                    if (!hasDefinition)
                    {
                        continue;
                    }

                    EventDefinition definition = eventDefinitions[i];
                    fixedEventItems[i].Refresh(definition, FindPlayerEventData(playerData, definition.Id));
                    fixedEventItems[i].SetSelected(definition.Id == playerData.SelectedEventId);
                    fixedEventItems[i].OnSelected += OnEventItemSelected;
                }

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
                    item.SetSelected(definition.Id == playerData.SelectedEventId);
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

        private void RefreshEventFill(PlayerData playerData, IReadOnlyList<EventDefinition> eventDefinitions)
        {
            selectedEvent = FindEventDefinition(eventDefinitions, playerData.SelectedEventId);
            PlayerEventData selectedProgress = selectedEvent != null ? FindPlayerEventData(playerData, selectedEvent.Id) : null;

            if (currentEventText != null)
            {
                currentEventText.text = selectedEvent == null ? "当前未选择事件" : $"当前选择：{selectedEvent.Name}";
            }

            if (eventFillPanelView != null)
            {
                eventFillPanelView.Refresh(selectedEvent, selectedProgress);
            }

            RefreshConfirmButton();
        }

        private void RefreshConfirmButton()
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = selectedEvent != null
                    && selectedEvent.Type == LifeRPG.Data.EventType.Record
                    && CanSubmitSelectedRecordEvent;
            }
        }

        private void OnEventItemSelected(string eventId)
        {
            OnEventSelected?.Invoke(eventId);
        }

        private PlayerEventData FindPlayerEventData(PlayerData playerData, string eventId)
        {
            return playerData.PersonalEvents.Find(item => item.EventId == eventId);
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
                float todayScore = progress != null ? progress.TodayScore : 0f;
                string timeText = definition.RequiredMinutes > 0f ? $"{definition.RequiredMinutes:0.#}分钟" : "无分钟";
                lines.Add($"{definition.Name} / {definition.RequiredCount}次 / {timeText} / {definition.RewardScore:0.#}分 / 今日{todayScore:0.#}分");
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

            if (templateObject.transform.IsChildOf(root))
            {
                templateObject.SetActive(false);
            }
        }

        private void AutoBindReferences()
        {
            if (eventFillPanelView == null)
            {
                Transform fillRoot = FindChildByName(transform, "CurrentEventPerfab");
                if (fillRoot == null)
                {
                    fillRoot = FindChildByName(transform, "CurrentEventPrefab");
                }

                if (fillRoot != null)
                {
                    eventFillPanelView = fillRoot.GetComponent<EventFillPanelView>();
                    if (eventFillPanelView == null)
                    {
                        eventFillPanelView = fillRoot.gameObject.AddComponent<EventFillPanelView>();
                    }
                }
            }

            if (confirmButton == null)
            {
                Transform confirmRoot = FindChildByName(transform, "ConfirmButton");
                confirmButton = confirmRoot != null ? confirmRoot.GetComponent<Button>() : null;
            }

            if (fixedEventItems.Count == 0)
            {
                EventListItemView[] existingItems = GetComponentsInChildren<EventListItemView>(true);
                foreach (EventListItemView item in existingItems)
                {
                    if (item != null && !fixedEventItems.Contains(item))
                    {
                        fixedEventItems.Add(item);
                    }
                }

                if (fixedEventItems.Count == 0)
                {
                    Transform[] children = GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in children)
                    {
                        if (!child.name.StartsWith("EventElement", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        EventListItemView item = child.GetComponent<EventListItemView>();
                        if (item == null)
                        {
                            item = child.gameObject.AddComponent<EventListItemView>();
                        }

                        fixedEventItems.Add(item);
                    }
                }
            }
        }

        private Transform FindChildByName(Transform root, string targetName)
        {
            if (root == null)
            {
                return null;
            }

            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == targetName)
                {
                    return child;
                }
            }

            return null;
        }
    }
}

