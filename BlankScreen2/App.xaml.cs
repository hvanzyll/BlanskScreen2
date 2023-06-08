using BlankScreen2.Helpers;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows;

namespace BlankScreen2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private ScreenMgr _ScreenMgr = new ScreenMgr();

		public App()
		{
			try
			{
				//create a management scope object
				ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\WMI");

				//create object query
				ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");

				//create object searcher
				ManagementObjectSearcher searcher =
										new ManagementObjectSearcher(scope, query);

				//get a collection of WMI objects
				ManagementObjectCollection queryCollection = searcher.Get();

				//enumerate the collection.
				foreach (ManagementObject m in queryCollection)
				{
					// access properties of the WMI object
					Debug.WriteLine("CurrentBrightness : {0}", m["CurrentBrightness"]);
				}

				_ScreenMgr.ShowWindow();
			}
			catch (Exception ex)
			{
				ShowException(ex);
			}
		}

		private static void ShowException(Exception ex)
		{
			string msg = "An unhandled exception occured.\n" +
				ex.ToString();
			MessageBox.Show(msg);
		}
	}
}