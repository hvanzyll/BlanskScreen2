using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlankScreen2.Model;
using BlankScreen2.ViewModel;

namespace BlankScreen2.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public sealed partial class SettingsWnd : Window
	{
		private ScreenMgr _ScreenMgr;

		public SettingsWnd(ScreenMgr screenMgr)
		{
			_ScreenMgr = screenMgr;
			InitializeComponent();
			this.DataContext = _ScreenMgr.Settings;
		}

		private void RefreshDisplays_Click(object sender, RoutedEventArgs e)
		{
			_ScreenMgr.RefreshDisplays();
		}

		private void BlanksScreen_Click(object sender, RoutedEventArgs e)
		{
			_ScreenMgr.ShowBlankScreen();
			this.Close();
		}
	}
}