using SocialBar.EventArgs;
using SocialBar.Parameters;
using SocialBar.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace SocialBar.Wrappers
{
	public class CustomTwitterClient
	{
		public List<long> RecievedTweets { get; private set; } = new List<long>();
		private bool initRun = true;

		public CustomTwitterClient(string token, string tokenSecret)
		{
			if (token != "" && tokenSecret != "")
			{
				var appCredentials = new TwitterCredentials(ApiTokens.TWITTER_KEY, ApiTokens.TWITTER_SECRET, token, tokenSecret);
				Auth.SetCredentials(appCredentials);
			}
			else
			{
				// Create a new set of credentials for the application.
				var appCredentials = new TwitterCredentials(ApiTokens.TWITTER_KEY, ApiTokens.TWITTER_SECRET);

				// Init the authentication process and store the related `AuthenticationContext`.
				var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

				// Go to the URL so that Twitter authenticates the user and gives him a PIN code.
				Process.Start(authenticationContext.AuthorizationURL);

				// Ask the user to enter the pin code given by Twitter
				TwitterPinDialog inputDialog = new TwitterPinDialog();
				string pinCode = null;

				if (inputDialog.ShowDialog() == true)
					pinCode = inputDialog.PIN;

				// With this pin code it is now possible to get the credentials back from Twitter
				var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;

				//ConfigurationManager.AppSettings["TwitterToken"] = userCredentials.AccessToken;
				//ConfigurationManager.AppSettings["TwitterSecret"] = userCredentials.AccessTokenSecret;
				settings["TwitterToken"].Value = userCredentials.AccessToken;
				settings["TwitterSecret"].Value = userCredentials.AccessTokenSecret;

				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

				// Use the user credentials in your application
				Auth.SetCredentials(userCredentials);
			}
		}

		public void Refresh()
		{
			// Get more control over the request with a HomeTimelineParameters
			var homeTimelineParameter = new HomeTimelineParameters
			{
				MaximumNumberOfTweetsToRetrieve = 10
			};

			var tweetsHome = Timeline.GetHomeTimeline(homeTimelineParameter);

			if (tweetsHome == null)
			{
				Console.WriteLine(ExceptionHandler.GetLastException().TwitterDescription);
			}

			foreach (var item in tweetsHome)
			{
				if (!RecievedTweets.Contains(item.Id) && !initRun)
				{
					NewTweetRecieved(new OnNewTweetArgs { SenderUsername = item.CreatedBy.Name, Content = item.Text, MediaUrls = item.Media });
				}
				else
					RecievedTweets.Add(item.Id);
			}
			initRun = false;
		}

		#region Events

		public event EventHandler<OnNewTweetArgs> OnNewTweet;

		protected virtual void NewTweetRecieved(OnNewTweetArgs e)
		{
			EventHandler<OnNewTweetArgs> handler = OnNewTweet;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion Events
	}
}