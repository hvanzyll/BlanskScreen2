using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BlankScreen2.Helpers
{
	public static class SettingPath
	{
		private const string c_Extention = ".json";

		private static string Company
		{
			get
			{
				try
				{
					Assembly? oAssembly = Assembly.GetEntryAssembly();
					if (oAssembly != null)
					{
						bool inherit = false;
						if (Attribute.GetCustomAttribute(
												oAssembly, typeof(AssemblyCompanyAttribute),
												inherit) is AssemblyCompanyAttribute oAttrib)
						{
							string strVal = oAttrib.Company;
							return strVal;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
				return "Company";
			}
		}

		private static string Title
		{
			get
			{
				try
				{
					Assembly? oAssembly = Assembly.GetEntryAssembly();
					if (oAssembly != null)
					{
						bool inherit = false;

						if (Attribute.GetCustomAttribute(
												oAssembly, typeof(AssemblyTitleAttribute),
												inherit) is AssemblyTitleAttribute oAttrib)

						{
							string strVal = oAttrib.Title;
							return strVal;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
				return "Title";
			}
		}

		public static string GetSettingPath()
		{
			string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string strPath = Path.Combine(commonAppData, Company);
			strPath = Path.Combine(strPath, Title);
			Directory.CreateDirectory(strPath);

			return strPath;
		}

		private static string GetSettingFilePath(string fileNameNoExt)
		{
			string strPath = GetSettingPath();
			string fileName = fileNameNoExt + c_Extention;
			string strFilePath = Path.Combine(strPath, fileName);

			return strFilePath;
		}

		public static T? LoadSettings<T>(string fileNameNoExt)
		{
			try
			{
				string settingsPath = SettingPath.GetSettingFilePath(fileNameNoExt);
				string json = File.ReadAllText(settingsPath);
				T? settings = JsonConvert.DeserializeObject<T>(json);
				return settings;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				T? settings = (T?)Activator.CreateInstance(typeof(T), new object[] { });
				SaveSettings<T>(fileNameNoExt, settings);
				return settings;
			}
		}

		public static void SaveSettings<T>(string fileNameNoExt, T? settings)
		{
			if (settings == null)
				return;

			string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
			string settingsPath = SettingPath.GetSettingFilePath(fileNameNoExt);
			File.WriteAllText(settingsPath, json);
		}
	}
}