﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer;

namespace BusinessLayer
{
	/// <summary>
	/// Represents a single speaker
	/// </summary>
	public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<BusinessLayer.Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>
		public int? Register(IRepository repository)
		{

			int? speakerId = null;
			
						//put list of employers in array
			var emps = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };

			bool good = ((Exp > 10 || HasBlog || Certifications.Count() > 3 || emps.Contains(Employer)));



			if (!good)
			{
				//need to get just the domain from the email
				string emailDomain = Email.Split('@').Last();
				var domains = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "CompuServe.com" };

				if (!domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
				{
					good = true;
				}
			}
			bool approved = false;
			if (good)
			{
				
				if (Sessions.Count() != 0)
				{
					foreach (var session in Sessions)
					{
						var oldTopics = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

						foreach (var tech in oldTopics)
						{
							if (session.Title.Contains(tech) || session.Description.Contains(tech))
							{
								session.Approved = false;
								break;
							}
							else
							{
								session.Approved = true;
								approved = true;
							}
						}
					}
				}
				else
				{
					throw new ArgumentException("Can't register speaker with no sessions to present.");
				}

				if (approved)
				{

					//if we got this far, the speaker is approved
					//let's go ahead and register him/her now.
					//First, let's calculate the registration fee. 
					//More experienced speakers pay a lower fee.
					if (Exp <= 1)
					{
						RegistrationFee = 500;
					}
					else if (Exp >= 2 && Exp <= 3)
					{
						RegistrationFee = 250;
					}
					else if (Exp >= 4 && Exp <= 5)
					{
						RegistrationFee = 100;
					}
					else if (Exp >= 6 && Exp <= 9)
					{
						RegistrationFee = 50;
					}
					else
					{
						RegistrationFee = 0;
					}



					//Now, save the speaker and sessions to the db.
					try
					{
						speakerId = repository.SaveSpeaker(this);
					}
					catch (Exception e)
					{
						//in case the db call fails 
					}
				}
				else
				{
					throw new NoSessionsApprovedException("No sessions approved.");
				}
			}
			else
			{
				throw new SpeakerDoesntMeetRequirementsException("Speaker doesn't meet our abitrary and capricious standards.");
			}



			//if we got this far, the speaker is registered.
			return speakerId;
		}
		private void ValidateData()
		{
			if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("First Name is required.");
			if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("Last Name is required.");
			if (string.IsNullOrEmpty(Email)) throw new ArgumentNullException("Email is required.");
			if (Sessions.Count() == 0) throw new ArgumentException("Can't register speaker with no sessions to present.");
		}

		#region Custom Exceptions
		public class SpeakerDoesntMeetRequirementsException : Exception
		{
			public SpeakerDoesntMeetRequirementsException(string message)
				: base(message)
			{
			}

			public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
				: base(string.Format(format, args)) { }
		}

		public class NoSessionsApprovedException : Exception
		{
			public NoSessionsApprovedException(string message)
				: base(message)
			{
			}
		}
		#endregion
	}
}