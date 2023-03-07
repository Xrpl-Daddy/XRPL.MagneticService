namespace XRPL.MagneticService.Entities
{
    public class DiceSettingsResponse
    {
        public double LessThan { get; set; }
        public double MoreThan { get; set; }
        public double Common { get; set; }
        public double Rare { get; set; }
        public double Epic { get; set; }
        public double Legendary { get; set; }
        public double Mythic { get; set; }
        public double MaxNFTBaff { get; set; }
        public AwaibleToken[] AwaibleTokens { get; set; }
    }
    public class AwaibleToken
    {
        public string Code { get; set; }
        public string CodeHEX { get; set; }
        public string Issuer { get; set; }
        public double MaxBet { get; set; }
        public double MinBet { get; set; }
    }

}
