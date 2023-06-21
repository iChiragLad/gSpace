using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gSpaceServer.Protos;
using System.Collections.ObjectModel;
using static gSpaceServer.Protos.Gspace;

namespace gSpaceServer.Services
{
  public class GspaceService : GspaceBase
  {
    private readonly ILogger _logger;

    public GspaceService(ILogger<GspaceService> logger)
    {
      _logger = logger;
    }

    public override Task<RegistrationResponse> RegisterToSpace(RegistrationRequest request, ServerCallContext context)
    {
      _logger.LogInformation("Registring the user to the space...");
      return Task.FromResult(new RegistrationResponse { SpaceId = Guid.NewGuid().ToString() });
    }

    public override async Task<NewsResponse> PublishNews(IAsyncStreamReader<News> requestStream, ServerCallContext context)
    {
      await foreach (var message in requestStream.ReadAllAsync())
      {
        _logger.LogInformation($"At : {message.NewsTime}, received : {message.NewsItem}");
      }

      return new NewsResponse { Delivered = true };
    }

    public override async Task MonitorSpace(Empty request, IServerStreamWriter<LineMessage> responseStream, ServerCallContext context)
    {
      var NewsList = new ReadOnlyCollection<string>(
        new[]
        {
          "Yo Yo Yo - A state visit with high expectations",
          "Yo Yo Yo - Election not a licence for violence",
        });
      
      foreach(var news in NewsList)
      {
        await responseStream.WriteAsync(new LineMessage() { LinemessageItem = news, LinemessageTime = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())});
        await Task.Delay(TimeSpan.FromSeconds(2));
      }
    }
  }
}