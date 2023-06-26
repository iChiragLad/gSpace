using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using gSpaceUser.Protos;

namespace gSpaceUser;
class Program
{
  static void Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true });
    var client = new Gspace.GspaceClient(channel);

    Console.WriteLine("Weclome to the gSpace app!!!\n");
    Console.Write("Enter your Username: ");
    var username = Console.ReadLine();
    Console.Write("Enter the Space you want to join: ");
    var spaceName = Console.ReadLine();

    Console.WriteLine($"Connecting you to the {spaceName} space...");

    try
    {
      var reply = client.RegisterToSpace(new RegistrationRequest { UserName = username, SpaceName = spaceName });
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine($"Successfuly connected to {spaceName}!!!");
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Couldn't connect you to the chat room at this moment.. Please try again");
      Console.ForegroundColor = ConsoleColor.White;
      return;
    }

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Press ENTER to start.");
    Console.ReadKey();
    Console.Clear();

    var promptText = "Type your message: ";
    string inputMessage = string.Empty;
    int row = 2;
    using var call = client.StartChat();

    //Receive message from server
    var task = Task.Run(async () =>
    {
      await foreach (var chat in call.ResponseStream.ReadAllAsync())
      {
        PrintMessage(chat, row, promptText.Length);
        row++;
      }
    });

    Console.Write(promptText);
    // Send message to Server
    while (true)
    {
      inputMessage = Console.ReadLine();
      ResetCursor(promptText.Length);

      call.RequestStream.WriteAsync(new ChatMessage { ChatTime = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()), ChatItem = inputMessage, UserName = username, SpaceName = spaceName });
    }

    call.RequestStream.CompleteAsync();
    Console.ReadKey();
  }

  static void ResetCursor(int promptTextLength)
  {
    Console.SetCursorPosition(promptTextLength, 0);
    Console.Write(new string(' ', Console.BufferWidth));
    Console.SetCursorPosition(promptTextLength, 0);
  }

  static void PrintMessage(ChatMessage chat, int row, int promptTextLength)
  {
    Console.SetCursorPosition(0, row);
    Console.Write($"{chat.UserName}: {chat.ChatItem}");
    ResetCursor(promptTextLength);

  }
}