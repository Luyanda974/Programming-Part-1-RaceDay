using System.Net.Http.Json;

namespace RaceDay.Web.Services
{//start of namespace
    public class RaceDayApiService
    {//start of class
        private readonly HttpClient _httpClient;

        // Receive the configured HTTP client through dependency injection.
        public RaceDayApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: Retrieve information from the API.
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        // POST: Send information to the API.
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // PUT: Update information through the API.
        public async Task PutAsync<TRequest>(string endpoint, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);

            response.EnsureSuccessStatusCode();
        }

        // DELETE: Remove information through the API.
        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }
        // GET: Retrieve protected information using the logged-in user's JWT.
        public async Task<T?> GetWithTokenAsync<T>(string endpoint, string token)
        {
            // Create an individual request to the API.
            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            // Attach the JWT token to authenticate the request.
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            // Send the request to the API.
            var response = await _httpClient.SendAsync(request);

            // Check whether the API request was successful.
            response.EnsureSuccessStatusCode();

            // Convert the API response into the requested model.
            return await response.Content.ReadFromJsonAsync<T>();
        }

    }//end of class
}//end of namespace
