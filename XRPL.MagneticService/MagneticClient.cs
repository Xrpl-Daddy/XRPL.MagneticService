using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XRPL.MagneticService.Entities;

namespace XRPL.MagneticService
{
    public class MagneticClient : BaseClient
    {
        public MagneticClient(bool waitWhenLimit, string apiKey, string BaseServiceAddress = "https://api.xmagnetic.org") : base(waitWhenLimit, apiKey, BaseServiceAddress)
        {
        }
        /// <summary>
        /// check one of the previous dice
        /// </summary>
        /// <param name="tokenId">dice tokenId</param>
        /// <param name="wallet">account number - owner of this dice</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<DiceResponse>> CheckDice(string tokenId, string wallet, CancellationToken Cancel = default)
        {
            if (!await CheckLimit(Cancel))
                return null;
            var req = $"?wallet={wallet}";
            if (!string.IsNullOrWhiteSpace(tokenId))
                req += $"&token={tokenId}";
            var response = await GetAsync<DiceResponse>($"MagneticApi/CheckDice{req}", Cancel);
            return response;
        }
        /// <summary>
        /// get dice history
        /// </summary>
        /// <param name="limit">limit count for response, maximum 400</param>
        /// <param name="wallet">account number for history, can be null for all players history</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<DiceHistoryResponse>> GetDiceHistory(int limit = 400, string wallet = null, CancellationToken Cancel = default)
        {
            if (limit > 400)
                limit = 400;
            if (limit < 10) 
                limit = 10;

            if (!await CheckLimit(Cancel))
                return null;
            var req = $"?limit={limit}";
            if (!string.IsNullOrWhiteSpace(wallet))
                req += $"&wallet={wallet}";
            var response = await GetAsync<DiceHistoryResponse>($"MagneticApi/GetDiceHistory{req}", Cancel);
            return response;
        }
        /// <summary>
        /// Get dice settings
        /// </summary>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<DiceSettingsResponse>> GetDiceSettings(CancellationToken Cancel = default)
        {
            if (!await CheckLimit(Cancel))
                return null;
            var response = await GetAsync<DiceSettingsResponse>($"MagneticApi/GetDiceSettings", Cancel);
            return response;
        }
        /// <summary>
        /// Get account nfts
        /// </summary>
        /// <param name="wallet">account</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<BaseServerResponse<NftsResponse>> GetNFTs(string wallet, CancellationToken Cancel = default)
        {
            if (string.IsNullOrWhiteSpace(wallet))
                throw new ArgumentNullException(nameof(wallet));

            if (!await CheckLimit(Cancel))
                return null;
            var response = await GetAsync<NftsResponse>($"GetNFTs?wallet={wallet}", Cancel);
            return response;
        }
        /// <summary>
        /// Get Account nfts power for dice
        /// </summary>
        /// <param name="nftsResponse">nfts server response</param>
        /// <param name="settingsResponse">dice settings server response</param>
        /// <returns></returns>
        public async Task<double> GetDiceNFTsPower(BaseServerResponse<NftsResponse> nftsResponse, BaseServerResponse<DiceSettingsResponse> settingsResponse)
        {
            if (!nftsResponse.Response.IsSuccessStatusCode || nftsResponse.Data is not { Count: > 0 } nfts)
                return 0;
            if (!settingsResponse.Response.IsSuccessStatusCode || settingsResponse.Data is not { } settings)
                return 0;
            var power = 0d;
            foreach (var nft in nfts)
                power += nft.Type switch
                {
                    MagneticNFTsType.Common => settings.Common,
                    MagneticNFTsType.Rare => settings.Rare,
                    MagneticNFTsType.Epic => settings.Epic,
                    MagneticNFTsType.Legendary => settings.Legendary,
                    MagneticNFTsType.Mythic => settings.Mythic,
                    _ => 0
                };

            if (power > settings.MaxNFTBaff)
                return settings.MaxNFTBaff;
            return power;

        }
    }
}
