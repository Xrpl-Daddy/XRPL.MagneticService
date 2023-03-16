namespace XRPL.MagneticService.Entities
{
    public class MagneticPools : List<PoolInfo>
    {


    }
    public class PoolInfo
    {
        public Pool pool { get; set; }
        public bool isParticipant { get; set; }
        public double tradedQuantity { get; set; }
        public int dice { get; set; }
    }

    public class Pool
    {
        public int poolIndex { get; set; }
        public double PoolSumm { get; set; }
        public double StartPoolSumm { get; set; }
        public string Issuer { get; set; }
        public string Currency { get; set; }
        public string Sponsor_Issuer { get; set; }
        public string Sponsor_Currency { get; set; }
        public string Memo { get; set; }
        public double Burn { get; set; }
        public double Complexity { get; set; }
        public double MinRewardForOneBlock { get; set; }
        public double LastRewardForOneBlock { get; set; }
        public double StartReward { get; set; }
        public int CountMinersInLastBlock { get; set; }
        public double RewardForLastBlock { get; set; }
        public double BurnForLastBlock { get; set; }
        public string NeedPair { get; set; }
        public string NeedIssuer { get; set; }
        public double MinVolume { get; set; }
        public int TraideType { get; set; }
        public Reward[] Rewards { get; set; }
        public string GuarantetReward { get; set; }
        public int Dice { get; set; }
        public string DiceToken { get; set; }
        public string DiceIssuer { get; set; }
        public string Common_NFT { get; set; }
        public string Rare_NFT { get; set; }
        public string Epic_NFT { get; set; }
        public string Legendary_NFT { get; set; }
        public string Mythic_NFT { get; set; }
    }

    public class Reward
    {
        public double NeedHolding_MAG { get; set; }
        public double NeedHolding_SPONSOR { get; set; }
        public string rew { get; set; }
        public bool isParticipant { get; set; }
    }

}
