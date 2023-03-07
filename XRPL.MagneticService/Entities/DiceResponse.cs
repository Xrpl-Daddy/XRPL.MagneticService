namespace XRPL.MagneticService.Entities
{
    public class DiceResponse
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string TX { get; set; }
        public DateTime gameTime { get; set; }
        public string Wallet { get; set; }
        public string Code { get; set; }
        public string CodeHEX { get; set; }
        public string Issuer { get; set; }
        public double Bet { get; set; }
        public double Than { get; set; }
        public bool Result { get; set; }
        public double Result_Number { get; set; }

    }
}
