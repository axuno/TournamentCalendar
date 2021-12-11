﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailMergeLib;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendar.Models.Contact;
using TournamentCalendar.Models.Shared;
using SmartFormat;

namespace TournamentCalendar
{
	///<summary>
	///Mailer class
	///</summary>
	public class Mailer
	{
		// coming from appsettings.json:
	    private readonly string _domainName; 

		private readonly IMailMergeService _mailMergeService;
		private readonly MailMergeAddress _contactFormFrom = new MailMergeAddress(MailAddressType.From, "Volleyball-Turnier.de", "info@volleyball-turnier.de");

		public Mailer(IMailMergeService mailMergeService, string domainName)
		{
		    _mailMergeService = mailMergeService;
		    _domainName = domainName;
        }

		public async Task<ContactModel> ContactForm(ContactModel model, string contactFormUrl)
		{
            var mmm = _mailMergeService.MessageStore.ScanForMessages().First(m => m.Category == "ContactForm").LoadMessage();

			var so = (model,
                new Dictionary<string, string>
                {
                    {"DomainName", _domainName},
                    {"Protocol", "https"},
                    {"ContactFormUrl", contactFormUrl}
                });

            model.EmailSuccessFul = false;
			try
			{
			    if (!model.CarbonCopyToSender)
			    {
			        mmm.MailMergeAddresses.Remove(mmm.MailMergeAddresses.Get(MailAddressType.CC).First());
			    }
                await _mailMergeService.Sender.SendAsync(mmm, (object) so);
				model.EmailSuccessFul = true;
			}
			catch (Exception ex)
			{
				model.Exception = ex;
			    throw;
			}

			return model;
		}
		
		public async Task<ConfirmModel<CalendarEntity>> MailTournamentCalendarForm(ConfirmModel<CalendarEntity> model, string approveUrl, string editUrl, string showUrl)
		{
		    var so = 
		    (
		        model.Entity,
		        new Dictionary<string, string>
		        {
		            {"DomainName", _domainName},
		            {"Protocol", "https"},
		            {"ApproveUrl", approveUrl},
		            {"EditUrl", editUrl},
		            {"ShowUrl", showUrl}
		        }
		    );

		    model.EmailSuccessful = false;
			try
			{
			    var mmm = _mailMergeService.MessageStore.ScanForMessages().First(m => m.Category == "CalenderEntry").LoadMessage();
                await _mailMergeService.Sender.SendAsync(mmm, (object) so);
				model.EmailSuccessful = true;
			}
			catch (Exception ex)
			{
				model.Exception = ex;
			    throw;
			}

			return model;
		}

		public async Task<ConfirmModel<InfoServiceEntity>> MailInfoServiceRegistrationForm(ConfirmModel<InfoServiceEntity> model, string approveUrl = "", string editUrl = "")
		{
		    var so = 
		    (
		        model.Entity,
		        new Dictionary<string, string>
		        {
		            {"DomainName", _domainName},
		            {"Protocol", "https"},
		            {"ApproveUrl", approveUrl},
		            {"EditUrl", editUrl}
		        }
		    );
            
            model.EmailSuccessful = false;
			try
			{
			    var mmm = _mailMergeService.MessageStore.ScanForMessages().First(m => m.Category == "InfoServiceEntry").LoadMessage();
                await _mailMergeService.Sender.SendAsync(mmm, (object) so);
				model.EmailSuccessful = true;
			}
			catch (Exception ex)
			{
				model.Exception = ex;
			    throw;
			}

			return model;
		}
	}
}