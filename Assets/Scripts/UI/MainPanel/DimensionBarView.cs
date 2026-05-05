using LifeRPG.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 单个六维显示条。显示当前六维、目标六维和今日累计值。
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
            if (nameText != null)
            {
                nameText.text = GetDimensionName(type);
            }

            if (valueText != null)
            {
                string baseText = targetValue > 0f ? $"当前 {FormatValue(currentValue)} / 目标 {FormatValue(targetValue)}" : $"当前 {FormatValue(currentValue)}";
                valueText.text = $"{baseText}  今日 +{FormatValue(todayValue)}";
            }

            if (valueSlider != null)
            {
                valueSlider.minValue = 0f;
                valueSlider.maxValue = targetValue > 0f ? targetValue : 100f;
                valueSlider.value = currentValue;
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
