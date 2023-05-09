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


        #region Base

        /// <summary>
        /// Current block info
        /// </summary>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<BLockInfo>> GetBlockInfo(CancellationToken Cancel = default)
        {
            if (!await CheckLimit(Cancel))
                return null;
            var response = await GetAsync<BLockInfo>($"MagneticApi/GetBlockChainInfo", Cancel);
            return response;
        }

        #endregion

        #region Pools
        /// <summary>
        /// Get MAG pools for mining
        /// </summary>
        /// <param name="account">account (not null)<br/>
        /// default - 0<br/>
        /// if not null - contains current account pool statistic</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<MagneticPools>> GetMagPools(string account = "0", CancellationToken Cancel = default)
        {

            if (!await CheckLimit(Cancel))
                return null;
            var parameter = string.Empty;
            if (!string.IsNullOrWhiteSpace(account))
                parameter = $"?wallet={account}";
            var response = await GetAsync<MagneticPools>($"MagneticApi/GetMagPools{parameter}", Cancel);
            return response;
        }
        /// <summary>
        /// Get magnetic sponsor pools for mining
        /// </summary>
        /// <param name="account">account (not null)<br/>
        /// default - 0<br/>
        /// if not null - contains current account pool statistic</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<MagneticPools>> GetSponsorPools(string account = "0", CancellationToken Cancel = default)
        {

            if (!await CheckLimit(Cancel))
                return null;
            var parameter = string.Empty;
            if (!string.IsNullOrWhiteSpace(account))
                parameter = $"?wallet={account}";
            var response = await GetAsync<MagneticPools>($"MagneticApi/GetSponsorPools{parameter}", Cancel);
            return response;
        }


        #endregion


        #region Dice

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
        /// check one of the previous dice
        /// </summary>
        /// <param name="wallet">account number</param>
        /// <param name="currencyCode">currency code - not a hex</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<DiceStatistic>> GetDiceStats(string wallet, string currencyCode, CancellationToken Cancel = default)
        {
            if (string.IsNullOrWhiteSpace(wallet) || string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentNullException(nameof(wallet));
            if (!await CheckLimit(Cancel))
                return null;
            var req = $"?wallet={wallet}";
            if (!string.IsNullOrWhiteSpace(currencyCode))
                req += $"&currency={currencyCode}";
            var response = await GetAsync<DiceStatistic>($"v1/GetDiceStats{req}", Cancel);
            return response;
        }

        /// <summary>
        /// get dice history
        /// </summary>
        /// <param name="currencyCode">currency code, not a hex</param>
        /// <param name="wallet">account number for history, can be null for all players history</param>
        /// <param name="startId">start game number</param>
        /// <param name="endId">end game number</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<BaseServerResponse<DiceHistoryResponse>> GetDiceHistory(long? startId = null, long? endId = null, string currencyCode = null, string wallet = null, CancellationToken Cancel = default)
        {
            if (endId < startId)
                (endId, startId) = (startId, endId);




            if (!await CheckLimit(Cancel))
                return null;

            var req = string.Empty;
            if (startId is not null)
                req += $"&startId={startId}";
            if (endId is not null)
                req += $"&endId={endId}";
            if (!string.IsNullOrWhiteSpace(wallet))
                req += $"&wallet={wallet}";
            if (!string.IsNullOrWhiteSpace(currencyCode))
                req += $"&currency={currencyCode}";

            var response = await GetAsync<DiceHistoryResponse>($"v1/GetDiceHistory?{req}", Cancel);
            return response;
        }
        /// <summary>
        /// get full dice history
        /// </summary>
        /// <param name="currencyCode">currency code, not a hex</param>
        /// <param name="wallet">account number for history, can be null for all players history</param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<List<DiceResponse>> GetFullDiceHistory(string currencyCode = null, string wallet = null, CancellationToken Cancel = default)
        {
            var all_game = new List<DiceResponse>();
            var dices2 = await GetDiceHistory(null, null, currencyCode, wallet, Cancel);
            while (dices2.Response.IsSuccessStatusCode && dices2.Data is List<DiceResponse> { Count: > 0 } data)
            {
                data = data.OrderBy(c => c.Id).ToList();
                all_game.InsertRange(0, data);

                var id = data.Min(c => c.Id);
                var start = id - 1000;
                var end = id - 1;
                if (start < 1) start = 1;
                if (end < 1) end = 1;

                if (start == end)
                    break;

                dices2 = await GetDiceHistory(start, end, currencyCode, wallet, Cancel);
            }

            return all_game;
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

        #endregion

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
        public double GetDiceNFTsPower(BaseServerResponse<NftsResponse> nftsResponse, BaseServerResponse<DiceSettingsResponse> settingsResponse)
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
