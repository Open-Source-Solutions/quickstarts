using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500"); //reconfigure cpde to make requests to Dapr sidecar
const string PUBSUBNAME = "applications";
const string TOPIC = "submissions";
Console.WriteLine($"Publishing to baseURL: {baseURL}, Pubsub Name: {PUBSUBNAME}, Topic: {TOPIC} ");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

for (int i = 1; i <= 10; i++) {
    var order = new Order(i);
    var orderJson = JsonSerializer.Serialize<Order>(order);
    var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

    // Publish an event/message using Dapr PubSub via HTTP Post
    var response = httpClient.PostAsync($"{baseURL}/v1.0/publish/{PUBSUBNAME}/{TOPIC}", content);
    Console.WriteLine("Published data: " + order);

    await Task.Delay(TimeSpan.FromSeconds(1));
}

public record Order([property: JsonPropertyName("orderId")] int OrderId);

// var loanApplication = new LoanApplication
// {
//     Id = "632b430f435eb3c03b7a804e",
//     Applicants = new List<Applicant>()
//     {
//         new Applicant
//         {
//             UserId = "aaron.scribner@gmf.com",
//             FirstName = "Aaron",
//             LastName = "Scribner"
//         }
//     },
// };
//
// var loanApplicationEvent = new LoanApplicationSubmittedEvent(loanApplication);
//
// var loanApplicationEventJson = JsonSerializer.Serialize((loanApplicationEvent));
// var content = new StringContent(loanApplicationEventJson, Encoding.UTF8, "application/json");
// var response = httpClient.PostAsync($"{baseURL}/v1.0/publish/{PUBSUBNAME}/{TOPIC}", content);
// Console.WriteLine("Published data: " + content);


public class LoanApplicationSubmittedEvent 
{
    public LoanApplicationSubmittedEvent(LoanApplication loanApplication)
    {
        LoanApplication = loanApplication;
    }
    
    public LoanApplication LoanApplication { get; }
}

public class LoanApplication
{
    [property: JsonPropertyName("id")] 
    public string? Id { get; set; }

    [property: JsonPropertyName("applicants")] 
    public List<Applicant> Applicants { get; set; } = new();
}

public enum LoanApplicationStatus
{
    New,
    Submitted,
    Approved,
    Declined,
    Pending,
    Underwriting,
    Abandoned
}

public class Applicant
{
    [property: JsonPropertyName("id")] 
    public string? Id { get; set; }
    
    [property: JsonPropertyName("userId")] 
    public string UserId { get; set; }
    
    [property: JsonPropertyName("firstName")] 
    public string FirstName { get; set; }
    
    [property: JsonPropertyName("lastName")] 
    public string LastName { get; set; }
    
    [property: JsonPropertyName("middleName")] 
    public string? MiddleName { get; set; }
}