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
		private string _connectionId;

		public ChatHub(IConfiguration configuration, ChatManager chatManager)
		{
			// Get the configuration of the wholes app.
			_configaration = configuration;
			// Create a new Context that is bound to the chathandler instance.
			_options = new DbContextOptionsBuilder<ApplicationDbContext>();
			// This will get a string that is used to define the database.
			_connectionString = _configaration.GetConnectionString("Connect");
			_options.UseSqlServer(_connectionString);

			_chatManager = chatManager;
		}

		public override Task OnConnectedAsync()
		{
			base.OnConnectedAsync();
			_connectionId = Context.ConnectionId;
			Clients.All.SendAsync("connected", Context.ConnectionId);
			return Task.CompletedTask;
		}

		public async Task SendMessage(string user, string message)
		{
			//user = HttpContext.Session.GetString("Username");
			dynamic dynamicMessage = new ExpandoObject();
			dynamicMessage.UserId = user;
			dynamicMessage.Message = message;

			// Create a new db context.
			ApplicationDbContext db = new ApplicationDbContext(_options.Options);
			// Create a new message obeject.
			Message _message = new Message
			{
				Content = message,
				// Verona please help add this to the client side.... i Need to get the username of the sender...
				// Also in private chat mode, i will also need to get the username of the specific username the message is going.
				Sender = "This should be added from the client side.."
			};

			await db.Messages.AddAsync(_message);
			await db.SaveChangesAsync();

			_chatManager.Messages.Add(dynamicMessage);

			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		public async Task PrivateMessage(string user, string message)
		{
			//FIND CONNECTION ID BY USER TABLE MAPPING = connectionId
			var connectionId = "Bob"; // This needs to come from the user mapping table by matching the connectionId or vice versa
			await Clients.Client(connectionId).SendAsync("pvtMessage", message, user);
		}
	}
}
