using FC.CodeFlix.Catalog.API.Configurations.Policies;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class APIClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _defaultSerializeOptions;

        public APIClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _defaultSerializeOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new JsonSnakeCasePolicy()
            };
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload) where TOutput : class
        {
            var payloadInfo = JsonSerializer.Serialize(payload, _defaultSerializeOptions);
            var response = await _httpClient.PostAsync(
                route,
                new StringContent(payloadInfo, Encoding.UTF8, "application/json")
            );

            return (response, await GetOutput<TOutput>(response));
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route, object? queryStringParametersObject = null) where TOutput : class
        {
            var url = PrepareGetRoute(route, queryStringParametersObject);
            var response = await _httpClient.GetAsync(url);
            return (response, await GetOutput<TOutput>(response));
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route) where TOutput : class
        {
            var response = await _httpClient.DeleteAsync(route);

            return (response, await GetOutput<TOutput>(response));
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload) where TOutput : class
        {
            var response = await _httpClient.PutAsync(
                route,
                new StringContent(JsonSerializer.Serialize(payload, _defaultSerializeOptions), Encoding.UTF8, "application/json")
            );

            return (response, await GetOutput<TOutput>(response));
        }

        private async Task<TOutput> GetOutput<TOutput>(HttpResponseMessage response) where TOutput : class
        {
            var outputString = await response.Content.ReadAsStringAsync();
            TOutput? output = null;
            if (!string.IsNullOrWhiteSpace(outputString))
                output = JsonSerializer.Deserialize<TOutput>(outputString, _defaultSerializeOptions);
            return output!;
        }

        private string PrepareGetRoute(string route, object? queryStringParametersObject)
        {
            if (queryStringParametersObject is null) return route;
            var jsonParameters = JsonSerializer.Serialize(queryStringParametersObject, _defaultSerializeOptions);
            var parametersDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonParameters);
            var url = QueryHelpers.AddQueryString(route, parametersDictionary!);
            return url;
        }
    }
}
