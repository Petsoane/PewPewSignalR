using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PewPewSignalR.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PewPewSignalR.Hubs
{
	public class ChatHub : Hub
	{
		private readonly ChatManager _chatManager;

		private readonly IConfiguration _configaration;
		private string _connectionString;
		private DbContextOptionsBuilder<ApplicationDbContext> _options;
		private UserMessageContext _userMessageContext;

		public ChatHub(IConfiguration configuration, ChatManager chatManager, UserMessageContext userMessageContext)
		{
			// Get the configuration of the wholes app.
			_configaration = configuration;
			// Create a new Context that is bound to the chathandler instance.
			_options = new DbContextOptionsBuilder<ApplicationDbContext>();
			// This will get a string that is used to define the database.
			_connectionString = _configaration.GetConnectionString("Connect");
			_options.UseSqlServer(_connectionString);

			_chatManager = chatManager;
			_userMessageContext = userMessageContext;
		}

		public void AddUser(string username)
		{
			//throws execption when you refresh the page
			_chatManager.MappedUserId.Add(username, Context.ConnectionId);
			_userMessageContext.LogUser(username, Context.ConnectionId);
		}


		public void ChangeReciever(string username, string newReciever)
		{
			Console.WriteLine("Changing the user reciver to " + newReciever);
			_userMessageContext.ChangeMessagesTo(username, newReciever);
		}
		public async Task TestSend(string username, string message)
		{
			_userMessageContext.AddMessages(username, message);
			dynamic userContext = _userMessageContext.GetUserContext(username);
			if (userContext.MessagesTo == "group")
			{

				await SendMessage(username, message);
			}
			else
			{
				Console.WriteLine("The new reciever is '" + userContext.MessagesTo);
				await SendPrivate(username, message);
			}
		}

		public async Task SendMessage(string user, string message)
		{
			//user = HttpContext.Session.GetString("Username");
			string timestamp = DateTime.Now.ToString("HH:mm:ss (dd.MM.yyyy)");
			dynamic dynamicMessage = new ExpandoObject();
			dynamicMessage.UserId = user;
			dynamicMessage.Message = message;
			dynamicMessage.timestamp = timestamp;

			// Create a new db context.
			ApplicationDbContext db = new ApplicationDbContext(_options.Options);
			// Create a new message obeject.
			Message _message = new Message
			{
				Content = message,
				// Verona please help add this to the client side.... i Need to get the username of the sender...
				// Also in private chat mode, i will also need to get the username of the specific username the message is going.
				Sender = user
			};

			await db.Messages.AddAsync(_message);
			await db.SaveChangesAsync();

//			_chatManager.Messages.Add(dynamicMessage);

			await Clients.All.SendAsync("ReceiveMessage", user, message, timestamp);
		}

	
		public async Task SendPrivate(string username, string message)
		{
			string reciever = _userMessageContext.GetReciverId(username);

			if (reciever != null)
			{
				Console.WriteLine("The reciever user id is " + reciever);
				await Clients.Client(reciever).SendAsync("PrivateReciever", username, message);
			}
		}
	}
}
