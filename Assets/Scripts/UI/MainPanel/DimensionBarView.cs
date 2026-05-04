using LifeRPG.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 单个六维显示条。MVP 阶段只显示名称、分数和可选 Slider。
    /// </summary>
    public class DimensionBarView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Slider valueSlider;

        public void Refresh(DimensionType type, int value)
        {
            Refresh(type, value, 0);
        }

        public void Refresh(DimensionType type, int value, int targetValue)
        {
            if (nameText != null)
            {
                nameText.text = GetDimensionName(type);
            }

            if (valueText != null)
            {
                valueText.text = targetValue > 0 ? $"{value}/{targetValue}" : value.ToString();
            }

            if (valueSlider != null)
            {
                valueSlider.minValue = 0;
                valueSlider.maxValue = targetValue > 0 ? targetValue : 100;
                valueSlider.value = value;
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
