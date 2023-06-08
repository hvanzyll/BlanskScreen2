using System.Diagnostics;
using System.Management;

namespace WMIManagement
{
	public class MonitorBrightness
	{
		private int GetCurrentBrightness()
		{
			//create a management scope object
			ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\WMI");

			//create object query
			ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");

			//create object searcher
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
			{
				//get a collection of WMI objects
				using (ManagementObjectCollection queryCollection = searcher.Get())
				{
					//enumerate the collection.
					foreach (ManagementObject m in queryCollection)
					{
						// access properties of the WMI object
						if (Convert.ToBoolean(m["Active"]))
						{
							Debug.WriteLine("CurrentBrightness : {0}", m["CurrentBrightness"]);

							return Convert.ToInt32(m["CurrentBrightness"]);
						}
					}
				}
			}

			return -1;
		}
	}
}