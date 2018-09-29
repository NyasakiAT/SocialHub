using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBar.Interfaces
{
	public interface MessengerHandler
	{
		List<Messenger> Messengers { get; set; }
		void TriggerAction(String messenger, String message, String sender, String title = "");
	}
}
