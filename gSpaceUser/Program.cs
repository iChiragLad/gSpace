using Grpc.Net.Client;
using gSpaceServer;

namespace gSpaceUser;
class Program
{
  static void Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true });
    var client = new Greeter.GreeterClient(channel);

    var reply = client.SayHello(new HelloRequest { Name = "World!"});

    Console.WriteLine(reply.Message);
  }
}
