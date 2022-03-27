using System.Windows;
using BlankScreen2.Helpers;

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
			_ScreenMgr.ShowWindow();
		}
	}
}