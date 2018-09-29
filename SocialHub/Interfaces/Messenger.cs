using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBar.Interfaces
{
	public interface Messenger
	{
		String Name { get; set; }
		String Message { get; set; }
		String Title { get; set; }
		String Sender { get; set; }
		MessengerHandler Handler { get; set; }
		void Run();
	}
}
