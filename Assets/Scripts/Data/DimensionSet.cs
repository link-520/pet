using System;

namespace LifeRPG.Data
{
    /// <summary>
    /// 保存玩家六维分数的简单数据结构。
    /// </summary>
    [Serializable]
    public class DimensionSet
    {
        public int Body;
        public int Knowledge;
        public int Career;
        public int Relationship;
        public int Wealth;
        public int Happiness;

        public DimensionSet()
        {
        }

        public DimensionSet(int body, int knowledge, int career, int relationship, int wealth, int happiness)
        {
            Body = body;
            Knowledge = knowledge;
            Career = career;
            Relationship = relationship;
            Wealth = wealth;
            Happiness = happiness;
        }

        /// <summary>
        /// 按六维类型读取分数。
        /// </summary>
        public int GetValue(DimensionType type)
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
                    return 0;
            }
        }

        /// <summary>
        /// 按六维类型设置分数。
        /// </summary>
        public void SetValue(DimensionType type, int value)
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

        /// <summary>
        /// 给某个维度增加分数。
        /// </summary>
        public void AddValue(DimensionType type, int amount)
        {
            SetValue(type, GetValue(type) + amount);
        }
    }
}
