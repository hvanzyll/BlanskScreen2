using System.Collections.ObjectModel;
using System.Linq;

namespace BlankScreen2.Model
{
	public sealed class DisplayEntries : ObservableCollection<DisplayEntry>
	{
		public DisplayEntry? FindByDisplayName(string deviceName)
		{
			return this.FirstOrDefault(de => de.DeviceName == deviceName);
		}

		public int GetDisplayIndex(DisplayEntry displayEntry)
		{
			for (int displayIndex = 0; displayIndex < this.Count; displayIndex++)
			{
				if (this[displayIndex] == displayEntry)
					return displayIndex;
			}
			return -1;
		}

		public void ClearAllNonRefreshed()
		{
			DisplayEntry? deFound = this.FirstOrDefault(de => !de.Refreshed);
			if (deFound != null)
			{
				this.Remove(deFound);
				ClearAllNonRefreshed();
			}
		}

		public void ResetRefreshed()
		{
			foreach (DisplayEntry displayEntry in this)
				displayEntry.Refreshed = false;
		}
	}
}