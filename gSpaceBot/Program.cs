using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using gSpaceUser.Protos;
using System.Collections.ObjectModel;

namespace gSpaceBot;
class Program
{
  static readonly IReadOnlyCollection<string> NewsList = new ReadOnlyCollection<string>(new[]
  {
    "A state visit with high expectations",
    "Election not a licence for violence",
    "More tribal seats for Assam?",
    "A Manipur court order that’s in the eye of the storm",
    "Identify this popular social media personality",
    "An action plan to check heat-stroke deaths",
    "Why rice is a boiling issue between Karnataka and Centre",
    "Titan in search of Titanic goes missing",
    "Law catches up with Biden’s son, but…",
    "PM Modi to lead a yoga session at UN headquarters on International Yoga Day"
  });

  static async Task Main(string[] args)
  {
    var channel = GrpcChannel.ForAddress("http://localhost:5078", new GrpcChannelOptions { UnsafeUseInsecureChannelCallCredentials = true });
    var client = new Gspace.GspaceClient(channel);

    using var callStream = client.PublishNews();

    Console.WriteLine("Starting news stream...");

    foreach (var news in NewsList)
    {
      await callStream.RequestStream.WriteAsync(new News { NewsItem = news, NewsTime = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()) });
      Thread.Sleep(2000);
    }

    Console.WriteLine("Completing news stream...");

    await callStream.RequestStream.CompleteAsync();

    var response = await callStream;

    Console.WriteLine($"Delivery status : {response.Delivered}");
  }
}
