using BlankScreen2.View;

namespace BlankScreen2.Model
{
	internal class WndEntry
	{
		public BlankScreenWnd BlankScreenWnd { get; }
		public DisplayEntry DisplayEntry { get; }

		public WndEntry(BlankScreenWnd blankScreenWnd, DisplayEntry displayEntry)
		{
			BlankScreenWnd = blankScreenWnd;
			DisplayEntry = displayEntry;
		}
	}
}