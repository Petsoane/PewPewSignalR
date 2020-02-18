using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PewPewSignalR.Models
{
	public class User
	{
		[Required]
		[UnusedUsername]
		public string Username { get; set; }
		public string Status { get; set; }

	}

	// This Class Will be used to check if the given username is tied to any username currentlty
	public class UnusedUsername : ValidationAttribute
	{
		private  static List<string> LoggedUsers = new List<string>();

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string _checkedUserName = (string)value;
			if (LoggedUsers.Contains(_checkedUserName))
			{
				return new ValidationResult("The username is already taken please choose another one");
			}
			LoggedUsers.Add(_checkedUserName);
			return ValidationResult.Success;
		}
	}
}
