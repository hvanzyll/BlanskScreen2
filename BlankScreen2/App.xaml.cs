using BlankScreen2.Helpers;
using System;
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