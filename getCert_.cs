using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class YourClass
{
    public async Task<string> MakeRequestsAsync(string uri, string username, string password)
    {
        string result = null;
        string firstLocation = null;
        string secondLocation = null;
        string thirdLocation = null;

        // Set up HttpClient inside the method
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // First request
                var response = await MakeRequestAsync(client, uri, username, password);
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
                    var response = await MakeRequestAsync(client, firstLocation, username, password);
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
                    var response = await MakeRequestAsync(client, secondLocation, username, password);
                    thirdLocation = response.Headers.Location?.ToString();
                    Console.WriteLine($"Third session: {thirdLocation}");
                    result = await response.Content.ReadAsStringAsync(); // Assuming you're expecting content in the last call
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Third request failed: {ex.Message}");
            }
        }

        return result;
    }

    private async Task<HttpResponseMessage> MakeRequestAsync(HttpClient client, string uri, string username, string password)
    {
        // Create a basic authentication header from username and password
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        requestMessage.Headers.Add("User-Agent", "curl/123");
        requestMessage.Headers.Add("Accept", "*/*");
        requestMessage.Headers.Add("Authorization", $"Basic {authToken}");

        return await client.SendAsync(requestMessage);
    }
}
