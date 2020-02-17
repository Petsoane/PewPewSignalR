using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PewPewSignalR.Models
{
	public class Message
	{
		[Key]
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Reciever { get; set; }
		public string Content { get; set; }
	}
}
