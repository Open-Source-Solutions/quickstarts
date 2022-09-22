using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {app.UseDeveloperExceptionPage();}

// Register Dapr pub/sub subscriptions
app.MapGet("/dapr/subscribe", () => {
    var sub = new DaprSubscription(PubsubName: "applications", Topic: "submissions", Route: "submissions");
    Console.WriteLine("Dapr pub/sub is subscribed to: " + sub);
    return Results.Json(new DaprSubscription[]{sub});
});

// Dapr subscription in /dapr/subscribe sets up this route
app.MapPost("/submissions", (DaprData<Order> requestData) => {
    Console.WriteLine("Subscriber received : " + requestData.Data.OrderId);
    return Results.Ok(requestData.Data);
});

await app.RunAsync();

public record DaprData<T> ([property: JsonPropertyName("data")] T Data); 
public record Order([property: JsonPropertyName("orderId")] int OrderId);
public record DaprSubscription(
  [property: JsonPropertyName("pubsubname")] string PubsubName, 
  [property: JsonPropertyName("topic")] string Topic, 
  [property: JsonPropertyName("route")] string Route);

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
