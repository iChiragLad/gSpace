using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gSpaceServer.Protos;
using gSpaceServer.Utils;
using System;
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
      UserList.AddNewUser(new User(request.SpaceName, request.UserName));
      return Task.FromResult(new RegistrationResponse { Success = true });
    }

    public override async Task<NewsResponse> PublishNews(IAsyncStreamReader<NewsMessage> requestStream, ServerCallContext context)
    {
      await foreach (var news in requestStream.ReadAllAsync())
      {
        _logger.LogInformation($"At : {news.NewsTime}, received : {news.NewsItem}");
        UserList.AddMessageToChatQueue(new ChatMessage { ChatTime = news.NewsTime, ChatItem = news.NewsItem, UserName = "Bot", SpaceName = news.SpaceName });
      }

      return new NewsResponse { Delivered = true };
    }

    public override async Task MonitorSpace(Empty request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    {
      while (true)
      {
        if (MessageQueue.GetNewMessageCount() > 0)
        {
          await responseStream.WriteAsync(MessageQueue.GetNextMessage());
        }
        await Task.Delay(TimeSpan.FromMilliseconds(500));
      }
    }

    public override async Task StartChat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    {
      while(!await requestStream.MoveNext())
      {
        await Task.Delay(100);
      }

      var username = requestStream.Current.UserName;
      var space = requestStream.Current.SpaceName;
      UserList.AddMessageToChatQueue(requestStream.Current);

      var reqTask = Task.Run(async () =>
      {
        while (await requestStream.MoveNext())
        {
          _logger.LogInformation($"At : {requestStream.Current.ChatTime}, received : {requestStream.Current.ChatItem}");
          UserList.AddMessageToChatQueue(requestStream.Current);
        }
      });

      //Read messages from the queue of all users
      var task = Task.Run(async () =>
      {
        while (true)
        {
          var chatMessage = UserList.GetMessageFromUserQueue(username);

          if (chatMessage != null)
          {
            await responseStream.WriteAsync(chatMessage);
          }
          await Task.Delay(200);
        }
      });

      while (true)
      {
        await Task.Delay(10000);
      }
    }
  }
}