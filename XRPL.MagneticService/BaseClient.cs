using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

using Newtonsoft.Json;

namespace XRPL.MagneticService;

public abstract class BaseClient
{
    #region Base

    protected DateTime LastRequestDateTime { get; private set; }

    /// <summary> Http клиент </summary>
    protected readonly HttpClient _Client;
    JsonSerializerSettings serializerSettings;

    /// <summary> Get </summary>
    /// <typeparam name="TEntity">Тип нужных данных</typeparam>
    /// <param name="url">адрес</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns></returns>
    protected async Task<BaseServerResponse<TEntity>> GetAsync<TEntity>(string url, CancellationToken Cancel = default) where TEntity : /*IResponse,*/ new()
    {
        Remaining -= 1;

        LastRequestDateTime = DateTime.Now;
        var response = await _Client.GetAsync(url, Cancel);

        if (response.Headers.TryGetValues("x-rate-limit-reset", out var reset))
        {
            DateTime.TryParse(reset.FirstOrDefault(), out var reset_sec);
            SetReset(reset_sec);
        }
        if (response.Headers.TryGetValues("x-rate-limit-remaining", out var remaining))
        {
            int.TryParse(remaining.FirstOrDefault(), out var remaining_count);
            Remaining = remaining_count;

        }
        if (response.Headers.TryGetValues("x-rate-limit-limit", out var limit))
        {
            Limit = limit.FirstOrDefault();
        }

        if (Remaining == 0 && response.StatusCode.ToString() == "TooManyRequests")
        {
            if (!await CheckLimit(Cancel))
                return new BaseServerResponse<TEntity>() { Response = response };

            return new BaseServerResponse<TEntity>() { Response = response };
        }

        if (response.StatusCode.ToString() == "TooManyRequests")
        {
            if (!await CheckLimit(Cancel))
                return new BaseServerResponse<TEntity>() { Response = response };

            return await GetAsync<TEntity>(url, Cancel);
        }
        if (response.StatusCode == HttpStatusCode.NotFound || !response.IsSuccessStatusCode) return new BaseServerResponse<TEntity>() { Response = response };

        var data = await response.Content.ReadAsStringAsync(Cancel);
        var result = string.IsNullOrWhiteSpace(data) ? new TEntity() : JsonConvert.DeserializeObject<TEntity>(data, serializerSettings);
        return new BaseServerResponse<TEntity>() { Response = response, Data = result };
    }
    protected async Task<bool> CheckLimit(CancellationToken Cancel)
    {
        if (Remaining is {} and 0)
        {
            if (WaitWhenLimit && Reset is { } time && time > DateTime.Now)
            {
                OnWaitAction?.Invoke($"Available number of requests: {Remaining};{Environment.NewLine}"
                                     + $"Reset through {Reset} sec.");
                Debug.WriteLine("Wait");
                var wait = time - DateTime.Now;
                wait += TimeSpan.FromSeconds(3);
                if (wait.TotalSeconds > 0)
                    await Task.Delay(wait, Cancel);
                await CheckLimit(Cancel);
            }
            else
                return false;
        }

        return true;
    }

    /// <summary> Post </summary>
    /// <typeparam name="TItem">Тип нужных данных</typeparam>
    /// <typeparam name="TEntity">тип данных ответа</typeparam>
    /// <param name="url">адрес</param>
    /// <param name="item">данные</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns></returns>
    protected async Task<BaseServerResponse<TEntity>> PostAsync<TItem, TEntity>(string url, TItem item, CancellationToken Cancel = default) where TEntity :/* IResponse, */new()
    {
        Remaining -= 1;

        LastRequestDateTime = DateTime.Now;
        var response = await _Client.PostAsJsonAsync(url, item, Cancel);
        if (response.Headers.TryGetValues("x-rate-limit-reset", out var reset))
        {
            DateTime.TryParse(reset.FirstOrDefault(), out var reset_sec);
            SetReset(reset_sec);
        }
        if (response.Headers.TryGetValues("x-rate-limit-remaining", out var remaining))
        {
            int.TryParse(remaining.FirstOrDefault(), out var remaining_count);
            Remaining = remaining_count;

        }
        if (response.Headers.TryGetValues("x-rate-limit-limit", out var limit))
        {
            Limit = limit.FirstOrDefault();
        }

        if (Remaining == 0 && response.StatusCode.ToString() == "TooManyRequests")
        {
            if (!await CheckLimit(Cancel))
                return new BaseServerResponse<TEntity>() { Response = response };

            return new BaseServerResponse<TEntity>() { Response = response };
        }
        if (response.StatusCode.ToString() == "TooManyRequests")
        {
            if (!await CheckLimit(Cancel))
                return new BaseServerResponse<TEntity>() { Response = response };

            return await PostAsync<TItem, TEntity>(url, item, Cancel);
        }
        if (response.StatusCode == HttpStatusCode.NotFound || !response.IsSuccessStatusCode) return new BaseServerResponse<TEntity>() { Response = response };

        var data = await response.Content.ReadAsStringAsync(Cancel);
        var result = string.IsNullOrWhiteSpace(data) ? new TEntity() : JsonConvert.DeserializeObject<TEntity>(data, serializerSettings);
        return new BaseServerResponse<TEntity>() { Response = response, Data = result };
    }


    #endregion

    #region Limit

    public Action<string> OnWaitAction;
    /// <summary>
    /// requests limit period.
    /// </summary>
    public string? Limit { get; private set; }

    /// <summary>
    /// Number of remaining requests per minute.
    /// </summary>
    public int? Remaining { get; private set; }
    /// <summary>
    /// Number of seconds until the current rate limit window resets.
    /// </summary>
    public DateTime? Reset { get; private set; }

    protected void SetReset(DateTime val)
    {
        Reset = val;
        if (Reset == DateTime.MinValue)
        {
            timer = null;
            Reset = null;
            Remaining = null;
            Limit = null;
            return;
        }
        timer?.Dispose();
        timer = null;
        timer = new Timer(
            State =>
            {
                Debug.WriteLine($"{Reset}");

                if (Reset is { } r && r <= DateTime.Now)
                {
                    timer = null;
                    Reset = null;
                    Remaining = null;
                    Limit = null;

                }
            }, val, 0, 1000);
    }


    private static Timer timer;
    /// <summary>
    /// If the maximum number of requests per minute has been exceeded, either waits for the limit to be reset or returns null
    /// </summary>
    public bool WaitWhenLimit { get; set; }

    #endregion

    public readonly string ApiServerAddress;
    /// <summary>
    /// api key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// https://api.xmagnetic.org
    /// Api Client
    /// </summary>
    /// <param name="waitWhenLimit">If the maximum number of requests per minute has been exceeded, either waits for the limit to be reset or returns null</param>
    /// <param name="apiKey">api key</param>
    /// <param name="BaseServiceAddress">server address</param>
    protected BaseClient(bool waitWhenLimit, string apiKey, string BaseServiceAddress = "https://api.xmagnetic.org")
    {
        ApiServerAddress = BaseServiceAddress;
        WaitWhenLimit = waitWhenLimit;
        _Client = new HttpClient
        {
            BaseAddress = new Uri(ApiServerAddress)
        };

        _Client.DefaultRequestHeaders.Accept.Clear();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _Client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            ApiKey = apiKey;
        }

        serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

    }

}