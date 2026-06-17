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
        [SerializeField] private TMP_Text eventNameText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private TMP_Text dimensionText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button selectButton;

        private string eventId;
        private Image backgroundImage;
        private Color normalColor = Color.white;
        private Color selectedColor = new Color(0.82f, 0.94f, 0.78f, 1f);

        public event Action<string> OnSelected;

        private void Awake()
        {
            AutoBindReferences();

            if (selectButton != null)
            {
                selectButton.onClick.AddListener(NotifySelected);
            }
        }

        public void Refresh(EventDefinition definition, PlayerEventData playerEventData)
        {
            AutoBindReferences();
            eventId = definition.Id;

            if (eventNameText != null)
            {
                eventNameText.text = definition.Name;
            }

            if (detailText != null)
            {
                string durationText = definition.RequiredMinutes > 0f ? $"{definition.RequiredMinutes:0.#}min" : "instant";
                float todayScore = playerEventData != null ? playerEventData.TodayScore : 0f;
                detailText.text = $"{GetEventTypeName(definition.Type)} / {GetDimensionName(definition.Dimension)} / +{definition.RewardScore:0.#} / {durationText} / 今日 {todayScore:0.#}";
            }

            if (dimensionText != null)
            {
                dimensionText.text = GetDimensionName(definition.Dimension);
            }

            if (timeText != null)
            {
                timeText.text = definition.RequiredMinutes > 0f ? $"{definition.RequiredMinutes:0.#}min" : "-";
            }

            if (scoreText != null)
            {
                scoreText.text = $"{definition.RewardScore:0.#}";
            }
        }

        public void SetSelected(bool selected)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = selected ? selectedColor : normalColor;
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

        private void AutoBindReferences()
        {
            if (selectButton == null)
            {
                selectButton = GetComponent<Button>();
                if (selectButton == null)
                {
                    selectButton = gameObject.AddComponent<Button>();
                }
            }

            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
                if (backgroundImage == null)
                {
                    Transform bg = transform.Find("BG");
                    backgroundImage = bg != null ? bg.GetComponent<Image>() : null;
                }

                if (backgroundImage != null)
                {
                    normalColor = backgroundImage.color;
                    selectButton.targetGraphic = backgroundImage;
                }
            }

            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                string lowerName = text.gameObject.name.ToLowerInvariant();
                if (eventNameText == null && (lowerName == "event" || lowerName.Contains("name")))
                {
                    eventNameText = text;
                    continue;
                }

                if (detailText == null && (lowerName == "six" || lowerName == "time" || lowerName == "score"))
                {
                    detailText = text;
                }

                if (dimensionText == null && lowerName == "six")
                {
                    dimensionText = text;
                }

                if (timeText == null && lowerName == "time")
                {
                    timeText = text;
                }

                if (scoreText == null && lowerName == "score")
                {
                    scoreText = text;
                }
            }

            if (eventNameText == null && texts.Length > 0)
            {
                eventNameText = texts[0];
            }
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
