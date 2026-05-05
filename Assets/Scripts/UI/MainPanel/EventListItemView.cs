using System;
using LifeRPG.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 事件列表中的单个条目。只负责显示事件信息，并把点击事件抛给外层。
    /// </summary>
    public class EventListItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private Button selectButton;

        private string eventId;

        public event Action<string> OnSelected;

        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(NotifySelected);
            }
        }

        public void Refresh(EventDefinition definition, PlayerEventData playerEventData)
        {
            eventId = definition.Id;

            if (titleText != null)
            {
                titleText.text = definition.Name;
            }

            if (detailText != null)
            {
                string timeText = definition.RequiredMinutes > 0f ? $"{definition.RequiredMinutes:0.#}min" : "instant";
                float todayScore = playerEventData != null ? playerEventData.TodayScore : 0f;
                detailText.text = $"{GetEventTypeName(definition.Type)} / {GetDimensionName(definition.Dimension)} / +{definition.RewardScore:0.#} / {timeText} / 今日 {todayScore:0.#}";
            }
        }

        private void NotifySelected()
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return;
            }

            OnSelected?.Invoke(eventId);
        }

        private string GetEventTypeName(LifeRPG.Data.EventType type)
        {
            return type == LifeRPG.Data.EventType.Continuous ? "持续" : "记录";
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
