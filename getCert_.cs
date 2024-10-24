using System;
using System.Net.Http;
using System.Threading.Tasks;

public class YourClass
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> MakeRequestsAsync(string uri, string creds)
    {
        string result = null;
        string firstLocation = null;
        string secondLocation = null;
        string thirdLocation = null;

        try
        {
            // First request
            var response = await MakeRequestAsync(uri, creds);
            firstLocation = response.Headers.Location?.ToString();
            Console.WriteLine($"Next location: {firstLocation}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"First request failed: {ex.Message}");
        }

        try
        {
            // Second request
            if (!string.IsNullOrEmpty(firstLocation))
            {
                var response = await MakeRequestAsync(firstLocation, creds);
                secondLocation = response.Headers.Location?.ToString();
                Console.WriteLine($"Third location: {secondLocation}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Second request failed: {ex.Message}");
        }

        try
        {
            // Third request
            if (!string.IsNullOrEmpty(secondLocation))
            {
                var response = await MakeRequestAsync(secondLocation, creds);
                thirdLocation = response.Headers.Location?.ToString();
                Console.WriteLine($"Third session: {thirdLocation}");
                result = await response.Content.ReadAsStringAsync(); // Assuming you're expecting content in the last call
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Third request failed: {ex.Message}");
        }

        return result;
    }

    private async Task<HttpResponseMessage> MakeRequestAsync(string uri, string creds)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        requestMessage.Headers.Add("User-Agent", "curl/123");
        requestMessage.Headers.Add("Accept", "*/*");
        // Add your authentication here (example with Basic auth)
        requestMessage.Headers.Add("Authorization", $"Basic {creds}");

        return await client.SendAsync(requestMessage);
    }
}
