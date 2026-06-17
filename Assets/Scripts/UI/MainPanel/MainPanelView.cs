using System;
using System.Collections.Generic;
using LifeRPG.Data;
using LifeRPG.Services;
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

        [Header("装备占位")]
        [SerializeField] private TMP_Text equipmentText;
        [SerializeField] private Transform equipmentRoot;
        [SerializeField] private List<Transform> fixedEquipmentSlots = new List<Transform>();

        [Header("六维显示")]
        [SerializeField] private Transform dimensionRoot;
        [SerializeField] private List<DimensionBarView> fixedDimensionBars = new List<DimensionBarView>();
        [SerializeField] private DimensionRadarChart dimensionRadarChart;

        [Header("事件列表")]
        [SerializeField] private Transform eventListRoot;
        [SerializeField] private EventListItemView eventItemPrefab;

        [Header("事件填写")]
        [SerializeField] private TMP_Text currentEventText;
        [SerializeField] private EventFillPanelView eventFillPanelView;
        [SerializeField] private Button confirmButton;

        [Header("窗口控制")]
        [SerializeField] private Button closeButton;

        private readonly List<DimensionBarView> runtimeDimensionBars = new List<DimensionBarView>();
        private readonly List<EventListItemView> runtimeEventItems = new List<EventListItemView>();
        private readonly Dictionary<string, Sprite> equipmentIconCache = new Dictionary<string, Sprite>();
        private EquipmentLibraryService equipmentLibraryService;
        private EventDefinition selectedEvent;
        private RectTransform rectTransform;
        private bool fittingWindow;

        public event Action<string> OnEventSelected;
        public event Action OnConfirmClicked;
        public event Action OnCloseClicked;
        public event Action OnOpenEquipmentWarehouseClicked;
        public event Action OnOpenEventWarehouseClicked;
        public event Action OnOpenDimensionWarehouseClicked;

        public bool CanSubmitSelectedEvent => eventFillPanelView == null || eventFillPanelView.CanSubmitSelectedEvent;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            equipmentLibraryService = EquipmentLibraryService.GetShared();

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

        private void OnEnable()
        {
        }

        public void Refresh(PlayerData playerData, IReadOnlyList<EventDefinition> eventDefinitions)
        {

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
        }
        private void RefreshEquipment(PlayerData playerData)
        {
            EquipmentType[] slotTypes =
            {
                EquipmentType.Hat,
                EquipmentType.Glasses,
                EquipmentType.Tie,
                EquipmentType.Gloves,
                EquipmentType.Clothes,
                EquipmentType.Pet
            };

            if (fixedEquipmentSlots.Count > 0)
            {
                for (int i = 0; i < fixedEquipmentSlots.Count && i < slotTypes.Length; i++)
                {
                    RefreshEquipmentSlot(fixedEquipmentSlots[i], slotTypes[i], playerData);
                }
            }

        }

        private void RefreshEquipmentSlot(Transform slot, EquipmentType type, PlayerData playerData)
        {
            if (slot == null)
            {
                return;
            }

            EquipmentDefinition equipped = FindEquippedEquipment(type, playerData);
            bool hasEquipment = equipped != null;

            TMP_Text label = EnsureEquipmentLabel(slot);
            if (label != null)
            {
                label.text = hasEquipment ? equipped.Name : GetEquipmentTypeName(type);
            }

            HideLegacyEquipmentIcons(slot);
            Image icon = EnsureEquipmentIcon(slot);
            if (icon != null)
            {
                Sprite sprite = LoadEquipmentIcon(hasEquipment ? equipped.IconId : GetDefaultEquipmentIconId(type));
                if (sprite != null)
                {
                    icon.sprite = sprite;
                    icon.preserveAspect = true;
                }

                icon.color = hasEquipment
                    ? Color.white
                    : new Color(1f, 1f, 1f, 0.38f);
            }

            Button button = slot.GetComponentInChildren<Button>(true);
            Image background = button != null ? button.targetGraphic as Image : null;
            if (background != null)
            {
                background.color = hasEquipment
                    ? new Color(1f, 0.965f, 0.91f, 1f)
                    : new Color(1f, 0.965f, 0.91f, 0.65f);
            }
        }

        private EquipmentDefinition FindEquippedEquipment(EquipmentType type, PlayerData playerData)
        {
            if (playerData == null || playerData.EquippedEquipmentIds == null || equipmentLibraryService == null)
            {
                return null;
            }

            foreach (string equipmentId in playerData.EquippedEquipmentIds)
            {
                EquipmentDefinition equipment = equipmentLibraryService.GetEquipmentById(equipmentId);
                if (equipment != null && equipment.Type == type)
                {
                    return equipment;
                }
            }

            return null;
        }

        private void HideLegacyEquipmentIcons(Transform slot)
        {
            Image[] images = slot.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.transform == slot
                    || image.GetComponent<Button>() != null
                    || image.transform.name == "RuntimeEquipmentIcon")
                {
                    continue;
                }

                image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            }
        }

        private Image EnsureEquipmentIcon(Transform slot)
        {
            Transform existing = slot.Find("RuntimeEquipmentIcon");
            if (existing != null)
            {
                return existing.GetComponent<Image>();
            }

            GameObject iconObject = new GameObject("RuntimeEquipmentIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.transform.SetParent(slot, false);
            iconObject.transform.SetAsLastSibling();

            RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.18f, 0.36f);
            rectTransform.anchorMax = new Vector2(0.82f, 0.94f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image image = iconObject.GetComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            return image;
        }

        private TMP_Text EnsureEquipmentLabel(Transform slot)
        {
            TMP_Text label = slot.GetComponentInChildren<TMP_Text>(true);
            if (label == null)
            {
                return null;
            }

            RectTransform rectTransform = label.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0.32f);
                rectTransform.offsetMin = new Vector2(2f, 2f);
                rectTransform.offsetMax = new Vector2(-2f, -2f);
            }

            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 12f;
            label.fontSizeMax = 19f;
            label.raycastTarget = false;
            return label;
        }

        private Sprite LoadEquipmentIcon(string iconId)
        {
            if (string.IsNullOrEmpty(iconId))
            {
                return null;
            }

            if (equipmentIconCache.TryGetValue(iconId, out Sprite cached))
            {
                return cached;
            }

            Sprite sprite = Resources.Load<Sprite>($"LifeRPG_UI_Images/{iconId}");
            equipmentIconCache[iconId] = sprite;
            return sprite;
        }

        private string GetDefaultEquipmentIconId(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Hat:
                    return "equip_hat";
                case EquipmentType.Glasses:
                    return "equip_glasses";
                case EquipmentType.Tie:
                    return "equip_tie";
                case EquipmentType.Gloves:
                    return "equip_gloves";
                case EquipmentType.Clothes:
                    return "equip_clothes";
                case EquipmentType.Pet:
                    return "equip_pet";
                default:
                    return string.Empty;
            }
        }

        private string GetEquipmentTypeName(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Hat:
                    return "帽子";
                case EquipmentType.Glasses:
                    return "眼镜";
                case EquipmentType.Tie:
                    return "领带";
                case EquipmentType.Gloves:
                    return "手套";
                case EquipmentType.Clothes:
                    return "衣服";
                case EquipmentType.Pet:
                    return "宠物";
                default:
                    return type.ToString();
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

            for (int i = 0; i < fixedDimensionBars.Count && i < types.Length; i++)
            {
                if (fixedDimensionBars[i] != null)
                {
                    DimensionType type = types[i];
                    fixedDimensionBars[i].Refresh(type, currentDimensions.GetValue(type), targetDimensions.GetValue(type), todayDimensions.GetValue(type));
                }
            }

            RefreshDimensionRadar(todayDimensions, targetDimensions);
        }

        private void RefreshDimensionRadar(DimensionSet currentDimensions, DimensionSet targetDimensions)
        {
            if (dimensionRadarChart != null)
            {
                dimensionRadarChart.Refresh(currentDimensions, targetDimensions);
            }
        }

        private void RefreshEvents(IReadOnlyList<EventDefinition> eventDefinitions, PlayerData playerData)
        {
            if (eventDefinitions == null || eventItemPrefab == null || eventListRoot == null)
            {
                return;
            }

            ClearRuntimeEvents();

            foreach (EventDefinition definition in eventDefinitions)
            {
                EventListItemView item = Instantiate(eventItemPrefab, eventListRoot);

                item.gameObject.SetActive(true);
                item.Refresh(definition, FindPlayerEventData(playerData, definition.Id));
                item.SetSelected(playerData != null && definition.Id == playerData.SelectedEventId);
                item.OnSelected += OnEventItemSelected;

                runtimeEventItems.Add(item);
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

        /// <summary>
        /// 根据当前事件类型和填写完成度更新确认按钮可交互状态。
        /// </summary>
        private void RefreshConfirmButton()
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = selectedEvent != null && CanSubmitSelectedEvent;
            }
        }

        /// <summary>
        /// 转发事件条目的选中回调给外部控制器。
        /// </summary>
        private void OnEventItemSelected(string eventId)
        {
            OnEventSelected?.Invoke(eventId);
        }

        /// <summary>
        /// 查找玩家在指定事件上的进度数据。
        /// </summary>
        private PlayerEventData FindPlayerEventData(PlayerData playerData, string eventId)
        {
            return playerData.PersonalEvents.Find(item => item.EventId == eventId);
        }

        /// <summary>
        /// 从事件配置列表中查找指定事件定义。
        /// </summary>
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

        /// <summary>
        /// 构建没有事件条目组件时使用的纯文本事件列表。
        /// </summary>
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

        /// <summary>
        /// 清理运行时创建的六维条实例。
        /// </summary>
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

        /// <summary>
        /// 清理运行时创建的事件条目并解绑选择事件。
        /// </summary>
        private void ClearRuntimeEvents()
        {
            foreach (EventListItemView item in runtimeEventItems)
            {
                if (item == null)
                {
                    continue;
                }

                item.OnSelected -= OnEventItemSelected;
                Destroy(item.gameObject);
            }
            runtimeEventItems.Clear();
        }


        private void HideSceneTemplates()
        {
            if (eventItemPrefab != null && eventListRoot != null)
            {
                HideTemplateIfInRoot(null, eventListRoot);
            }
        }

        private void EnsureTitleButton(string objectName, TMP_Text titleText, Vector2 offset, string labelText, Action onClick)
        {
            if (titleText == null || titleText.transform.parent == null)
            {
                return;
            }

            Transform parent = titleText.transform.parent;
            Transform existing = parent.Find(objectName);
            Button button = existing != null ? existing.GetComponent<Button>() : null;

            if (button == null)
            {
                GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                buttonObject.transform.SetParent(parent, false);
                buttonObject.transform.SetAsLastSibling();

                RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
                RectTransform titleRect = titleText.transform as RectTransform;
                if (titleRect != null)
                {
                    buttonRect.anchorMin = titleRect.anchorMin;
                    buttonRect.anchorMax = titleRect.anchorMax;
                    buttonRect.pivot = titleRect.pivot;
                    buttonRect.anchoredPosition = titleRect.anchoredPosition + offset;
                }

                buttonRect.sizeDelta = new Vector2(84f, 38f);

                Image image = buttonObject.GetComponent<Image>();
                image.color = new Color(1f, 0.965f, 0.9f, 0.96f);

                button = buttonObject.GetComponent<Button>();
                button.targetGraphic = image;
                CreateButtonLabel(buttonObject.transform, titleText, labelText);
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }

        private void CreateButtonLabel(Transform parent, TMP_Text template, string text)
        {
            GameObject labelObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            TMP_Text label = labelObject.GetComponent<TMP_Text>();
            label.text = text;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 14f;
            label.fontSizeMax = 22f;
            label.color = new Color(0.55f, 0.31f, 0.1f, 1f);
            label.raycastTarget = false;

            if (template != null)
            {
                label.font = template.font;
            }
        }

        private TMP_Text FindTextByName(string objectName)
        {
            Transform root = FindChildByName(transform, objectName);
            return root != null ? root.GetComponent<TMP_Text>() : null;
        }

        private TMP_Text FindTextByText(string text)
        {
            TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text label in labels)
            {
                if (label != null && label.text == text)
                {
                    return label;
                }
            }

            return null;
        }

        /// <summary>
        /// 删除 root 下面所有子对象，避免场景模板继续参与显示或布局。
        /// </summary>
        private void HideTemplateIfInRoot(GameObject templateObject, Transform root)
        {
            if (root == null)
            {
                return;
            }

            for (int i = root.childCount - 1; i >= 0; i--)
            {
                GameObject child = root.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
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
