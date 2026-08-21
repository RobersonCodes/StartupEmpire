using System;

namespace StartupEmpire.Referrals
{
    [Serializable]
    public sealed class ReferralRedemptionResultDto
    {
        public bool Success;
        public string Status;
        public int InviterRewardGems;
        public int InviteeRewardGems;
    }
}
