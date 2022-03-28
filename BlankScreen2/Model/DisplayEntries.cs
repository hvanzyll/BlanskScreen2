using System.Collections.ObjectModel;
using System.Linq;

namespace BlankScreen2.Model
{
	public sealed class DisplayEntries : ObservableCollection<DisplayEntry>
	{
		public DisplayEntry? FindByDisplayName(string displayName)
		{
			return this.FirstOrDefault(de => de.DisplayName == displayName);
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
	}
}