using gSpaceServer.Protos;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace gSpaceServer.Utils
{
  public class UserList
  {
    private static ConcurrentBag<User> _userList;
    static UserList()
    {
      _userList = new ConcurrentBag<User>();
    }

    public static ConcurrentBag<User> GetUserList() 
    {
      return _userList;
    }

    public static void AddNewUser(User user)
    {
      _userList.Add(user);
    }

    public static void AddMessageToChatQueue(ChatMessage chat) 
    {
      //News Message
      if (chat.UserName == "Bot")
      {
        foreach (var user in _userList)
        {
          user.AddMessageToQueue(chat);
        }
      }
      //Chat Message
      else
      {
        foreach (var user in _userList)
        {
          if (user.Space == chat.SpaceName)
          {
            user.AddMessageToQueue(chat);
          }
        }
        AddMessageToBotQueue(chat);
      }
    }

    public static void AddMessageToBotQueue(ChatMessage chat)
    {
      var admin = _userList.Where(u => u.Username == "Bot").FirstOrDefault();

      if (admin != null)
      {
        admin.AddMessageToQueue(chat);
      }
    }

    public static ChatMessage? GetMessageFromUserQueue(string username)
    {
      var userFromList = _userList.FirstOrDefault((u) => u.Username == username);
      if (userFromList != null)
      {
        if(userFromList.GetNewMessageCount() > 0)
        {
          return userFromList.GetNextMessage();
        }
      }

      return null;
    }
  }
}
