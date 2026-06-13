using LifeRPG.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 单个六维显示条。显示今日获得值与每日目标值。
    /// </summary>
    public class DimensionBarView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Slider valueSlider;

        public void Refresh(DimensionType type, float value)
        {
            Refresh(type, value, 0f, 0f);
        }

        public void Refresh(DimensionType type, float currentValue, float targetValue)
        {
            Refresh(type, currentValue, targetValue, 0f);
        }

        public void Refresh(DimensionType type, float currentValue, float targetValue, float todayValue)
        {
            float safeTargetValue = Mathf.Max(0f, targetValue);
            float safeTodayValue = Mathf.Max(0f, todayValue);

            if (nameText != null)
            {
                nameText.text = GetDimensionName(type);
            }

            if (valueText != null)
            {
                valueText.text = $"{FormatValue(safeTodayValue)}/{FormatValue(safeTargetValue)}";
            }

            if (valueSlider != null)
            {
                valueSlider.minValue = 0f;
                valueSlider.maxValue = safeTargetValue > 0f ? safeTargetValue : 1f;
                valueSlider.value = Mathf.Clamp(safeTodayValue, 0f, valueSlider.maxValue);
            }
        }

        private string FormatValue(float value)
        {
            return value.ToString("0.#");
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
