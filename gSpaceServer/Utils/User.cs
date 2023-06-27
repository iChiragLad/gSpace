using gSpaceServer.Protos;

namespace gSpaceServer.Utils
{
  public class User
  {

    private Queue<ChatMessage> _queue { get; }
    public string Username { get; }
    public string Space { get; }

    public User(string space, string username)
    {
      Username = username;
      Space = space;
      this._queue = new Queue<ChatMessage>();
    }

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
