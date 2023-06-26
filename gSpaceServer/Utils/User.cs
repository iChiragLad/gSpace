using gSpaceServer.Protos;

namespace gSpaceServer.Utils
{
  public class User
  {

    private Queue<ChatMessage> _queue;

    public User()
    {
      _queue = new Queue<ChatMessage>();
    }

    public string Username { get; set; }
    public string Space { get; set; }

    public void AddMessageToQueue(ChatMessage msg)
    {
      _queue.Enqueue(msg);
    }

    public ChatMessage GetNextMessage()
    {
      return _queue.Dequeue();
    }

    public int GetNewMessageCount()
    {
      return _queue.Count();
    }

  }
}
