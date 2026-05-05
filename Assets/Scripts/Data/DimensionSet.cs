using System;
using UnityEngine;

namespace LifeRPG.Data
{
    /// <summary>
    /// 统一表示目标六维、今日六维、个人当前六维。
    /// </summary>
    [Serializable]
    public class DimensionSet
    {
        public float Body;
        public float Knowledge;
        public float Career;
        public float Relationship;
        public float Wealth;
        public float Happiness;

        public DimensionSet()
        {
        }

        public DimensionSet(float body, float knowledge, float career, float relationship, float wealth, float happiness)
        {
            Body = body;
            Knowledge = knowledge;
            Career = career;
            Relationship = relationship;
            Wealth = wealth;
            Happiness = happiness;
        }

        public float GetValue(DimensionType type)
        {
            switch (type)
            {
                case DimensionType.Body:
                    return Body;
                case DimensionType.Knowledge:
                    return Knowledge;
                case DimensionType.Career:
                    return Career;
                case DimensionType.Relationship:
                    return Relationship;
                case DimensionType.Wealth:
                    return Wealth;
                case DimensionType.Happiness:
                    return Happiness;
                default:
                    return 0f;
            }
        }

        public void SetValue(DimensionType type, float value)
        {
            switch (type)
            {
                case DimensionType.Body:
                    Body = value;
                    break;
                case DimensionType.Knowledge:
                    Knowledge = value;
                    break;
                case DimensionType.Career:
                    Career = value;
                    break;
                case DimensionType.Relationship:
                    Relationship = value;
                    break;
                case DimensionType.Wealth:
                    Wealth = value;
                    break;
                case DimensionType.Happiness:
                    Happiness = value;
                    break;
            }
        }

        public void AddValue(DimensionType type, float amount)
        {
            SetValue(type, GetValue(type) + amount);
        }

        public void Clear()
        {
            Body = 0f;
            Knowledge = 0f;
            Career = 0f;
            Relationship = 0f;
            Wealth = 0f;
            Happiness = 0f;
        }

        public DimensionSet Clone()
        {
            return new DimensionSet(Body, Knowledge, Career, Relationship, Wealth, Happiness);
        }

        public void Clamp(float minValue, float maxValue)
        {
            Body = Mathf.Clamp(Body, minValue, maxValue);
            Knowledge = Mathf.Clamp(Knowledge, minValue, maxValue);
            Career = Mathf.Clamp(Career, minValue, maxValue);
            Relationship = Mathf.Clamp(Relationship, minValue, maxValue);
            Wealth = Mathf.Clamp(Wealth, minValue, maxValue);
            Happiness = Mathf.Clamp(Happiness, minValue, maxValue);
        }
    }
}
