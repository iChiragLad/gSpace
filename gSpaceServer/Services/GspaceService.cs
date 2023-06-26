using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gSpaceServer.Protos;
using gSpaceServer.Utils;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
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
      UserList.AddNewUser(new User { Username = request.UserName, Space = request.SpaceName});
      return Task.FromResult(new RegistrationResponse { Success = true });
    }

    public override async Task<NewsResponse> PublishNews(IAsyncStreamReader<NewsMessage> requestStream, ServerCallContext context)
    {
      await foreach (var message in requestStream.ReadAllAsync())
      {
        _logger.LogInformation($"At : {message.NewsTime}, received : {message.NewsItem}");
        MessageQueue.AddMessageToQueue(new ChatMessage { ChatTime = message.NewsTime, ChatItem = message.NewsItem, UserName = "Bot" });
      }

      return new NewsResponse { Delivered = true };
    }

    public override async Task MonitorSpace(Empty request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
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

    public override async Task StartChat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    {
      //Read messages from the queue of all users
      var task = Task.Run(() =>
      {
        while (true)
        {
          foreach(var registeredUser in UserList.GetUserList())
          {
            var chatMessage = UserList.GetMessageFromUserQueue(registeredUser);

            if(chatMessage != null)
            {
              responseStream.WriteAsync(chatMessage);
            }
          }
        }
      });

      await foreach (var chat in requestStream.ReadAllAsync())
      {
        _logger.LogInformation($"At : {chat.ChatTime}, received : {chat.ChatItem}");
        UserList.AddMessageToChatQueue(chat);
      }


    }
  }
}