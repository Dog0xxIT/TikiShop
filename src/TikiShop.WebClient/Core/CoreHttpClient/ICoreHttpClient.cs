using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Core.CoreHttpClient;

public interface ICoreHttpClient
{
    Task<ResultObject<T>> GetAsync<T>(string clientName, string uri, object? queryObj = null) where T : class;
    Task<ResultObject<T>> PostAsync<T>(string clientName, string uri, object reqObj) where T : class;
    Task<ResultObject<T>> PutAsync<T>(string clientName, string uri, object reqObj) where T : class;
    Task<ResultObject<T>> PatchAsync<T>(string clientName, string uri, object reqObj) where T : class;
    Task<ResultObject<T>> DeleteAsync<T>(string clientName, string uri) where T : class;
    Task<ResultObject> PostAsync(string clientName, string uri, object reqObj);
    Task<ResultObject> PutAsync(string clientName, string uri, object reqObj);
    Task<ResultObject> PatchAsync(string clientName, string uri, object reqObj);
    Task<ResultObject> DeleteAsync(string clientName, string uri);
}