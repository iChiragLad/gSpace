using gSpaceServer.Protos;

namespace gSpaceServer.Utils
{
  public class MessageQueue
  {
    private static Queue<ChatMessage> _queue;
    static MessageQueue()
    {
      _queue= new Queue<ChatMessage>();
    }

    public static void AddMessageToQueue(ChatMessage msg)
    {
      _queue.Enqueue(msg);
    }

    public static ChatMessage GetNextMessage() 
    { 
      return _queue.Dequeue(); 
    }

    public static int GetNewMessageCount()
    {
      return _queue.Count();
    }
  }
}
