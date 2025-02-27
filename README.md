Application should automatically seed the database with a default API key: 12345, it should be included in the X-API-KEY header.

### Notes:

- email should be unique for active users. From experience, users forget the username and sites offer username lookup by email
- when creating a user, we assume that the domain logic in the database is valid (no duplicates)
- validate password could be a method in the user class but was put in the service layer to avoid fetching the entire user at the expense of the domain model being anemic
- authentication was implemented with a simple filter (minimal APIs support bearer tokens and OIDC)
- instead of deleting the user, we mark it as deleted. Since this is the last operation on a user, UpdatedAt tells us the deletion date (no Delete datetime)
- no repository pattern - EF Core already implements Unit of Work pattern, opinions are split on whether it's needed. A repository pattern would enable to separate EF testing of the data layer though and we would add another set of Dtos.
- in real world use, I would separate the password update from the general user update
- move more configuration to appsettings

### Validation:

- needs to be added. Out of the box minimal API has no validation and will simply bind a null to a value
- Fluent Validation can be used ([link](https://docs.fluentvalidation.net/en/latest/aspnet.html#minimal-apis) [link2](https://benfoster.io/blog/minimal-api-validation-endpoint-filters/)). Will add this as an exercise
- Microsoft is however planning ([source](https://www.reddit.com/r/dotnet/comments/1invk34/comment/mceay12/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button)) on adding it

### Testing

- added unit tests for the service layer. InMemory did not support all EF Core operations, here the value of splitting DBContext to a separate Repository which I could mock would be useful
- to-do: integration tests with WebApplicationFactory ([sample](https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/minimal-apis/samples/MinApiTestsSample/IntegrationTests/TodoEndpointsV2Tests.cs))
- in real world scenarios we would be using MS SQL or something similar, so we would likely be using Test Contains. I do know it's importat to test with the actual database engine ([talk](https://www.youtube.com/watch?v=td9HE0vxsf4) [video](https://www.youtube.com/watch?v=m7r2qyUabTs)) to check for possible runtime errors

I used the official doumentation ([source](https://learn.microsoft.com/en-us/ef/core/testing/) [source2](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0#iresult-implementation-types)) to educate myself as at my current workplace, we did not use testing.


### Logging

- rolling files are requested, this is supported out of the box by the Serilog file sink
- need to go over settings as the logs are too verbose
- on my to-do is to make a more generic request logging filter that can be applied to the entire group
- password should be masked, am looking into the sensitive enricher ([link](https://github.com/serilog-contrib/Serilog.Enrichers.Sensitive)), Microsoft also has their own libraries for masking sensitive data ([link](https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/))
- for some of the requested data I could probably have used enrichers ([environment](https://github.com/serilog/serilog-enrichers-environment) [clientInfo](https://github.com/serilog-contrib/serilog-enrichers-clientinfo)). I assume that file logs still have an important part in on-prem deploymens, so I will have to look in to modifying the message template to include the enriched data.

Again I am somewhat handicapped from coming from a legacy codebase which used an inhouse developed logger and monitoring tools which were not structured but just plain text.
