using LifeRPG.Data;
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

        private bool completed;
        private EventDefinition currentEvent;

        public bool IsCompleted => completed;
        public bool CanSubmitRecordEvent => currentEvent != null && currentEvent.Type == LifeRPG.Data.EventType.Record && completed;

        private void Awake()
        {
            if (completedButton != null)
            {
                completedButton.onClick.AddListener(ToggleCompleted);
            }
        }

        public void Refresh(EventDefinition eventDefinition, PlayerEventData playerEventData)
        {
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
                    : "持续性事件请使用“开始事件”入口计时。";
            }

            RefreshCompletedButton();
        }

        private void ToggleCompleted()
        {
            if (currentEvent == null || currentEvent.Type != LifeRPG.Data.EventType.Record)
            {
                return;
            }

            completed = !completed;
            RefreshCompletedButton();
        }

        private void RefreshCompletedButton()
        {
            if (completedButton != null)
            {
                completedButton.interactable = currentEvent != null && currentEvent.Type == LifeRPG.Data.EventType.Record;
            }

            if (completedButtonText != null)
            {
                completedButtonText.text = completed ? "已完成" : "未完成";
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
    }
}

