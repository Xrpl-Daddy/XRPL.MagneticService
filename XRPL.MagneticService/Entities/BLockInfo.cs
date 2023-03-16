namespace XRPL.MagneticService.Entities
{
    public class BLockInfo
    {
        public int BlockIndex { get; set; }
        public DateTime TimeNextBlock { get; set; }
        public double TotalBurn { get; set; }
        public double TotalMined { get; set; }
        public int TotalMiners { get; set; }
        public double MaximumToken { get; set; }
    }
}
