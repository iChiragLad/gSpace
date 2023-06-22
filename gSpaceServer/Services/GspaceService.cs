using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gSpaceServer.Protos;
using gSpaceServer.Utils;
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
        MessageQueue.AddMessageToQueue(new LineMessage { LinemessageTime = message.NewsTime, LinemessageItem = message.NewsItem, UserName = "Bot" });
      }

      return new NewsResponse { Delivered = true };
    }

    public override async Task MonitorSpace(Empty request, IServerStreamWriter<LineMessage> responseStream, ServerCallContext context)
    {
      while(true)
      {
        if(MessageQueue.GetNewMessageCount() > 0)
        {
          await responseStream.WriteAsync(MessageQueue.GetNextMessage());
        }
        await Task.Delay(TimeSpan.FromMilliseconds(500));
      }
    }
  }
}