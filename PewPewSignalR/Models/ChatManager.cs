using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PewPewSignalR.Models
{
	public class ChatManager
	{
		public List<dynamic> Messages { get; set; } = new List<dynamic>();
		public Dictionary<string, string> MappedUserId { get; set; } = new Dictionary<string, string>();
	}

	/**
	 * This class is user to represent online users context.
	 **/
	public class UserMessageContext
	{
		// A Store for all the users online.
		public  Dictionary<string, dynamic> UsersOnline = new Dictionary<string, dynamic>();

		private  void InitUser(string username, string connectionId)
		{
			UsersOnline.Add(username, new ExpandoObject());
			UsersOnline[username].Reciever = "group";
			UsersOnline[username].Messages = new List<dynamic>();
			UsersOnline[username].ConnectionId = connectionId;
			UsersOnline[username].Username = username;
			System.Console.WriteLine(UsersOnline);
		}

		public void LogUser(string username, string connectionId) { InitUser(username, connectionId); }

		public void AddMessages(string username, string message)
		{

			if (UsersOnline.ContainsKey(username))
			{
				dynamic dm = new ExpandoObject();
				dm.UserId = username;
				dm.Message = message;

				UsersOnline[username].Messages.Add(dm);
			}
		}

		public dynamic GetUserContext(string username)
		{
			if (UsersOnline.ContainsKey(username))
				return UsersOnline[username];
			return null;
		}

		public List<dynamic> GetMessages(string username)
		{
			if (UsersOnline.ContainsKey(username))
				return  (List<dynamic>) UsersOnline[username].Messages;
			return new List<dynamic>();
		}
		
		public void ChangeReciever(string username,string newReciever)
		{
			if ((UsersOnline.ContainsKey(newReciever) || newReciever == "group") && UsersOnline.ContainsKey(username))
			{
				UsersOnline[username].Reciever = newReciever;
				UsersOnline[username].Messages = new List<dynamic>();
			}
		}

		public string GetReciverId(string username)
		{
			if (UsersOnline.ContainsKey(username) && UsersOnline[username].Reciever != "group")
			{
				return UsersOnline[UsersOnline[username].Reciever].ConnectionId;
				
			}
			return null;
		}

		public List<string> GetAllUserNames()
		{
			var output = new List<string>();
			foreach (var username in UsersOnline.Keys)
				output.Add(username);
			return output;
		}
	}
}
