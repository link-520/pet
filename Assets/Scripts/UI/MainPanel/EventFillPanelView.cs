using LifeRPG.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 主面板右侧事件填写区。只负责展示当前选择事件和完成按钮状态。
    /// </summary>
    public class EventFillPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject emptyRoot;
        [SerializeField] private GameObject contentRoot;
        [SerializeField] private TMP_Text eventNameText;
        [SerializeField] private TMP_Text dimensionText;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private Button completedButton;
        [SerializeField] private TMP_Text completedButtonText;
        [SerializeField] private Button incompleteButton;
        [SerializeField] private TMP_Text timeInputText;

        private bool completed;
        private EventDefinition currentEvent;

        public event Action OnCompletionChanged;

        public bool IsCompleted => completed;
        public bool CanSubmitRecordEvent => currentEvent != null
            && currentEvent.Type == LifeRPG.Data.EventType.Record
            && (completedButton == null || completed);

        public bool CanSubmitSelectedEvent => currentEvent != null
            && (currentEvent.Type == LifeRPG.Data.EventType.Continuous || CanSubmitRecordEvent);

        private void Awake()
        {
            AutoBindReferences();

            if (completedButton != null)
            {
                completedButton.onClick.AddListener(MarkCompleted);
            }

            if (incompleteButton != null)
            {
                incompleteButton.onClick.AddListener(MarkIncomplete);
            }
        }

        public void Refresh(EventDefinition eventDefinition, PlayerEventData playerEventData)
        {
            AutoBindReferences();
            currentEvent = eventDefinition;
            completed = false;

            bool hasEvent = currentEvent != null;
            SetVisible(emptyRoot, !hasEvent);
            SetVisible(contentRoot, hasEvent);

            if (!hasEvent)
            {
                RefreshCompletedButton();
                return;
            }

            if (eventNameText != null)
            {
                eventNameText.text = currentEvent.Name;
            }

            if (dimensionText != null)
            {
                dimensionText.text = GetDimensionName(currentEvent.Dimension);
            }

            if (rewardText != null)
            {
                rewardText.text = $"每次 +{currentEvent.RewardScore:0.#}";
            }

            if (timeInputText != null)
            {
                timeInputText.text = currentEvent.RequiredMinutes > 0f
                    ? $"{currentEvent.RequiredMinutes:0.#}min"
                    : "-";
            }

            if (progressText != null)
            {
                int todayCount = playerEventData != null ? playerEventData.TodayCount : 0;
                float todayScore = playerEventData != null ? playerEventData.TodayScore : 0f;
                progressText.text = $"今日 {todayCount} 次 / {todayScore:0.#} 分";
            }

            if (hintText != null)
            {
                hintText.text = currentEvent.Type == LifeRPG.Data.EventType.Record
                    ? "点击完成后确认，本次记录会进入今日六维。"
                    : "确认后按计划时长记录，本次持续事件会进入今日六维。";
            }

            RefreshCompletedButton();
        }

        private void MarkCompleted()
        {
            if (currentEvent == null || currentEvent.Type != LifeRPG.Data.EventType.Record)
            {
                return;
            }

            completed = true;
            RefreshCompletedButton();
            OnCompletionChanged?.Invoke();
        }

        private void MarkIncomplete()
        {
            completed = false;
            RefreshCompletedButton();
            OnCompletionChanged?.Invoke();
        }

        private void RefreshCompletedButton()
        {
            if (completedButton != null)
            {
                completedButton.gameObject.SetActive(currentEvent == null || currentEvent.Type == LifeRPG.Data.EventType.Record);
                completedButton.interactable = currentEvent != null && currentEvent.Type == LifeRPG.Data.EventType.Record;
            }

            if (completedButtonText != null)
            {
                completedButtonText.text = completed ? "已完成" : "未完成";
            }

            if (completedButton != null)
            {
                Image completedImage = completedButton.GetComponent<Image>();
                if (completedImage != null)
                {
                    completedImage.color = completed ? new Color(0.82f, 0.94f, 0.78f, 1f) : Color.white;
                }
            }

            if (incompleteButton != null)
            {
                incompleteButton.gameObject.SetActive(currentEvent == null || currentEvent.Type == LifeRPG.Data.EventType.Record);

                Image incompleteImage = incompleteButton.GetComponent<Image>();
                if (incompleteImage != null)
                {
                    incompleteImage.color = completed ? Color.white : new Color(0.9f, 0.9f, 0.9f, 1f);
                }
            }
        }

        private void SetVisible(GameObject target, bool visible)
        {
            if (target != null)
            {
                target.SetActive(visible);
            }
        }

        private string GetDimensionName(DimensionType type)
        {
            switch (type)
            {
                case DimensionType.Body:
                    return "身体";
                case DimensionType.Knowledge:
                    return "知识";
                case DimensionType.Career:
                    return "事业";
                case DimensionType.Relationship:
                    return "关系";
                case DimensionType.Wealth:
                    return "财富";
                case DimensionType.Happiness:
                    return "快乐";
                default:
                    return type.ToString();
            }
        }

        private void AutoBindReferences()
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                string name = text.gameObject.name.ToLowerInvariant();
                string value = text.text;

                if (eventNameText == null && (name.Contains("event") || value.Contains("事件") || value.Contains("吃水果")))
                {
                    eventNameText = text;
                    continue;
                }

                if (timeInputText == null && (name.Contains("time") || value.Contains("min") || value.Contains("时间")))
                {
                    timeInputText = text;
                    continue;
                }

                if (completedButtonText == null && value.Contains("已完成"))
                {
                    completedButtonText = text;
                    completedButton = text.GetComponentInParent<Button>();
                    continue;
                }

                if (value.Contains("未完成"))
                {
                    incompleteButton = text.GetComponentInParent<Button>();
                }
            }

            if (eventNameText == null && texts.Length > 0)
            {
                eventNameText = texts[0];
            }

            if (completedButton == null)
            {
                Button[] buttons = GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(true);
                    if (buttonText != null && buttonText.text.Contains("已完成"))
                    {
                        completedButton = button;
                        completedButtonText = buttonText;
                    }
                    else if (buttonText != null && buttonText.text.Contains("未完成"))
                    {
                        incompleteButton = button;
                    }
                }
            }
        }
    }
}
