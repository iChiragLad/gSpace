using Grpc.Net.Client;
using gSpaceUser.Protos;

namespace gSpaceUser;
class Program
{
  static void Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true });
    var client = new Gspace.GspaceClient(channel);

    Console.Write("Enter the Space you want to join: ");
    var spaceName = Console.ReadLine();
    var reply = client.RegisterToSpace(new RegistrationRequest { UserName = "chirag", SpaceName = spaceName });

    Console.WriteLine(reply.SpaceId);
    Console.ReadKey();
  }
}
