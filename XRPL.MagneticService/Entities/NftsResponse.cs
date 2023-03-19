using Newtonsoft.Json;

namespace XRPL.MagneticService.Entities
{
    public class NftsResponse:List<NFT>
    {

    }
    public class NFT
    {
        [JsonProperty("nfTokenId")]
        public string NFTokenId { get; set; }
        [JsonProperty("rar")]
        public MagneticNFTsType Type { get; set; }
    }

    public enum MagneticNFTsType
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic
    }
}
