using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {app.UseDeveloperExceptionPage();}

// Register Dapr pub/sub subscriptions
app.MapGet("/dapr/subscribe", () => {
    var sub = new DaprSubscription(PubsubName: "application", Topic: "submissions", Route: "orders");
    Console.WriteLine("Dapr pub/sub is subscribed to: " + sub);
    return Results.Json(new DaprSubscription[]{sub});
});

// Dapr subscription in /dapr/subscribe sets up this route
app.MapPost("/orders", (DaprData<LoanApplication> requestData) => {
    Console.WriteLine("Subscriber received : " + requestData.Data.ApplicationId);
    Console.WriteLine("Subscriber received : " + requestData.Data.Applicants[0].FirstName);
    return Results.Ok(requestData.Data);
});

await app.RunAsync();

public record DaprData<T> ([property: JsonPropertyName("data")] T Data); 
// public record LoanApplication([property: JsonPropertyName("applicationId")] string ApplicationId);
public record DaprSubscription(
  [property: JsonPropertyName("pubsubname")] string PubsubName, 
  [property: JsonPropertyName("topic")] string Topic, 
  [property: JsonPropertyName("route")] string Route);


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