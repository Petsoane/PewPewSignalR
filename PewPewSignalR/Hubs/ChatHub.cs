using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PewPewSignalR.Models;
using System.Dynamic;
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
			_configaration = configuration;
			_options = new DbContextOptionsBuilder<ApplicationDbContext>();
			_connectionString = _configaration.GetConnectionString("Connect");
			_options.UseSqlServer(_connectionString);

			_chatManager = chatManager;
			_userMessageContext = userMessageContext;
		}

		public async Task AddUser(string username)
		{
			_chatManager.MappedUserId.Add(username, Context.ConnectionId);
			_userMessageContext.LogUser(username, Context.ConnectionId);
			await Clients.All.SendAsync("NewOnlineUser", username);
		}

		
		public void ChangeReciever(string username, string newReciever)
		{
			_userMessageContext.ChangeReciever(username, newReciever);
		}

		public async Task Send(string username, string message)
		{
			_userMessageContext.AddMessages(username, message);
			dynamic userContext = _userMessageContext.GetUserContext(username);
			if (userContext.Reciever == "group")
			{

				await SendMessage(username, message);
			}
			else
			{
				await SendPrivate(username, message);
			}
		}

		public async Task SendMessage(string user, string message)
		{
			dynamic dynamicMessage = new ExpandoObject();
			dynamicMessage.UserId = user;
			dynamicMessage.Message = message;

			ApplicationDbContext db = new ApplicationDbContext(_options.Options);
			Message _message = new Message
			{
				Content = message,
				Sender = user
			};

			await db.Messages.AddAsync(_message);
			await db.SaveChangesAsync();

			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

	
		public async Task SendPrivate(string username, string message)
		{
			string reciever = _userMessageContext.GetReciverId(username);
			

			if (reciever != null)
			{
				await Clients.Client(reciever).SendAsync("PrivateReciever", username, message);
			}
		}
	}
}
