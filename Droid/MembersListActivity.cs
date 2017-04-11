
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TeamManager.Droid
{
	[Activity(Label = "Members")]
	public class MembersListActivity : Activity
	{

		string[] members;

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.members_list_toolbar_menus, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "Login" layout resource
			SetContentView(Resource.Layout.MembersList);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			//ActionBar.Title = "My Toolbar";

			members = new string[] { "Faisal Ahmad", "Farheen Faisal", "Abdul Raheem", "Abdul Kareem", "Fatima Faisal", "Momina Faisal" };
			//this.ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, members);
		}

//		protected override void OnListItemClick(ListView l, View v, int position, long id)
//		{
//			var t = members[position];
//			Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();		
//		}
	}
}
