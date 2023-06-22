using gSpaceServer.Protos;

namespace gSpaceServer.Utils
{
  public class MessageQueue
  {
    private static Queue<LineMessage> _queue;
    static MessageQueue()
    {
      _queue= new Queue<LineMessage>();
    }

    public static void AddMessageToQueue(LineMessage msg)
    {
      _queue.Enqueue(msg);
    }

    public static LineMessage GetNextMessage() 
    { 
      return _queue.Dequeue(); 
    }

    public static int GetNewMessageCount()
    {
      return _queue.Count();
    }
  }
}
