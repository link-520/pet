using System;
using LifeRPG.Data;

namespace LifeRPG.Services
{
    /// <summary>
    /// 每日结算服务。跨天时把今日六维折算进个人目前六维。
    /// </summary>
    public class DailySettlementService
    {
        private const float PositiveInfluenceDays = 21f;
        private const float NegativeInfluenceDays = 7f;

        public bool SettleIfNeeded(PlayerData playerData, DateTime now)
        {
            if (playerData == null)
            {
                return false;
            }

            string today = now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(playerData.LastSettlementDate))
            {
                playerData.LastSettlementDate = today;
                return false;
            }

            if (playerData.LastSettlementDate == today)
            {
                return false;
            }

            SettleToday(playerData);
            playerData.LastSettlementDate = today;
            return true;
        }

        public void ForceSettleToday(PlayerData playerData)
        {
            if (playerData == null)
            {
                return;
            }

            SettleToday(playerData);
            playerData.LastSettlementDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void SettleToday(PlayerData playerData)
        {
            ApplyDimension(playerData, DimensionType.Body);
            ApplyDimension(playerData, DimensionType.Knowledge);
            ApplyDimension(playerData, DimensionType.Career);
            ApplyDimension(playerData, DimensionType.Relationship);
            ApplyDimension(playerData, DimensionType.Wealth);
            ApplyDimension(playerData, DimensionType.Happiness);

            ClearTodayProgress(playerData);
            playerData.CurrentDimensions.Clamp(0f, 10f);
        }

        private void ApplyDimension(PlayerData playerData, DimensionType dimension)
        {
            float todayScore = playerData.TodayDimensions.GetValue(dimension);
            float targetScore = playerData.TargetDimensions.GetValue(dimension);
            float delta = todayScore - targetScore;
            float influence = delta >= 0f ? delta / PositiveInfluenceDays : delta / NegativeInfluenceDays;
            playerData.CurrentDimensions.AddValue(dimension, influence);
        }

        private void ClearTodayProgress(PlayerData playerData)
        {
            playerData.TodayDimensions.Clear();

            foreach (PlayerEventData playerEvent in playerData.PersonalEvents)
            {
                playerEvent.TodayCount = 0;
                playerEvent.TodayMinutes = 0f;
                playerEvent.TodayScore = 0f;
                playerEvent.TodayCompleted = false;
                playerEvent.IsActive = false;
            }

            playerData.ActiveContinuousEventId = string.Empty;
            playerData.ActiveContinuousEventStartUnixSeconds = 0;
        }
    }
}
