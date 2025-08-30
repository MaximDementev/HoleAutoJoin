using System;

namespace HoleAutoJoin.Core
{
    [Serializable]
    public class HoleJoinSettings
    {
        public bool IsAutoJoinEnabled { get; set; } = true;
        public double JoinTolerance { get; set; } = 0.1;
        public bool ShowNotifications { get; set; } = true;
        public DateTime LastModified { get; set; } = DateTime.Now;

        public HoleJoinSettings()
        {
        }

        public HoleJoinSettings Clone()
        {
            return new HoleJoinSettings
            {
                IsAutoJoinEnabled = this.IsAutoJoinEnabled,
                JoinTolerance = this.JoinTolerance,
                ShowNotifications = this.ShowNotifications,
                LastModified = DateTime.Now
            };
        }
    }
}
