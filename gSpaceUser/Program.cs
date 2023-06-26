using Grpc.Net.Client;
using gSpaceUser.Protos;

namespace gSpaceUser;
class Program
{
  static void Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true });
    var client = new Gspace.GspaceClient(channel);

    Console.WriteLine("Weclome to gSpace app!!!\n");
    Console.Write("Enter your Username: ");
    var username = Console.ReadLine();
    Console.Write("Enter the Space you want to join: ");
    var spaceName = Console.ReadLine();


    try
    {
      Console.WriteLine($"Connecting you to the chat room ({spaceName})...");
      var reply = client.RegisterToSpace(new RegistrationRequest { UserName = username, SpaceName = spaceName });
      Console.WriteLine(reply.SpaceId);
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Couldn't connect you to the chat room at this moment.. Please try again");
      Console.ForegroundColor = ConsoleColor.White;
      return;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Successfuly connected to {spaceName}!!!");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Press ENTER to start");
    Console.ReadKey();

    var inputText = "Type your message: ";

    Console.Clear();
    Console.Write(inputText);
    var message = Console.ReadLine();

    // Send message to Server

    //Receive message from server
    int row = 2;
    PrintMessage(message, row);
    ResetCursor(inputText.Length);

    Console.ReadKey();

  }

  static void ResetCursor(int inputTextLength)
  {
    Console.SetCursorPosition(inputTextLength, 0);
    Console.Write(new string(' ', Console.BufferWidth));
    Console.SetCursorPosition(inputTextLength, 0);
  }

  static void PrintMessage(string message, int row)
  {
    Console.SetCursorPosition(0, row++);
    Console.Write($"Chirag: {message}");
  }
}