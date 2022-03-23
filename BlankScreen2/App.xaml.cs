using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BlankScreen2.Model;
using BlankScreen2.ViewModel;

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