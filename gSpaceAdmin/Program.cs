using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using gSpaceAdmin.Protos;

namespace gSpaceAdmin;
class Program
{
  static async Task Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true});
    var client = new Gspace.GspaceClient(channel);

    using var callStream = client.MonitorSpace(new Empty());

    await foreach(var chatMessage in callStream.ResponseStream.ReadAllAsync())
    {
      Console.WriteLine($"{chatMessage.UserName}: {chatMessage.ChatItem}");
    }
  }
}