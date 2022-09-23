using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500"); //reconfigure cpde to make requests to Dapr sidecar
const string PUBSUBNAME = "application";
const string TOPIC = "submissions";
Console.WriteLine($"Publishing to baseURL: {baseURL}, Pubsub Name: {PUBSUBNAME}, Topic: {TOPIC} ");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

for (int i = 1; i <= 10; i++) {
    var order = new LoanApplication
        { 
            ApplicationId = Guid.NewGuid().ToString(),
            Applicants = new List<Applicant>
            {
                new Applicant
                {
                    UserId = "blah@foo.com",
                    FirstName = "Blah",
                    LastName = "Blam"
                },
                new Applicant
                {
                    UserId = "foob@fbar.com",
                    FirstName = "Foo",
                    LastName = "Flam"
                }
            }
        };
    var orderJson = JsonSerializer.Serialize<LoanApplication>(order);
    var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

    // Publish an event/message using Dapr PubSub via HTTP Post
    var response = httpClient.PostAsync($"{baseURL}/v1.0/publish/{PUBSUBNAME}/{TOPIC}", content);
    Console.WriteLine("Published data: " + order);

    await Task.Delay(TimeSpan.FromSeconds(1));
}

// public record LoanApplication([property: JsonPropertyName("applicationId")] string ApplicationId);

public class LoanApplication
{
    [JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; }
    
    public List<Applicant> Applicants { get; set; } = new();
    //
    // public LoanApplicationStatus ApplicationStatus { get; set; }
}

public class Applicant
{
    public string UserId { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? MiddleName { get; set; }
    
}