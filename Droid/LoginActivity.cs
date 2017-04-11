using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Threading;

namespace TeamManager.Droid
{
	[Activity(Label = "Team Manager", MainLauncher = true, Icon = "@mipmap/icon")]
	public class LoginActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Get an instance of the PersistenceManager to ensure that the database is initialized if it
			// has not already been initialized. Do this on a background thread.
			ThreadPool.QueueUserWorkItem(o => PersistenceManager.getInstance());

			// Set our view from the "Login" layout resource
			SetContentView(Resource.Layout.Login);

			// Get our button from the layout resource,
			// and attach an event to it
			Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
			loginButton.Click += delegate {

				var intent = new Intent(this, typeof(MembersListActivity));
				StartActivity(intent);
			};
		}
	}
}

