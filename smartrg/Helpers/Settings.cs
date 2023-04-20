// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using Xamarin.Forms;

namespace smartrg.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "gwork_key";
		private static readonly string SettingsDefault = string.Empty;

		#endregion
		public static string GeneralSettings
		{
			get
			{
				return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(SettingsKey, value);
			}
		}
		public static DateTime LastUpdate
		{
			get
			{
				return AppSettings.GetValueOrDefault("lastupdate", new DateTime());
			}
			set
			{
				AppSettings.AddOrUpdateValue("lastupdate", value);
			}
		}
		public static string Loginname
		{
			get
			{
				return AppSettings.GetValueOrDefault("Loginname", "");
			}
			set
			{
				AppSettings.AddOrUpdateValue("Loginname", value);
			}
		}
		public static string ProfileImage
		{
			get
			{
				return AppSettings.GetValueOrDefault("ProfileImage", "");
			}
			set
			{
				AppSettings.AddOrUpdateValue("ProfileImage", value);
			}
		}
		public static string QRPayment
		{
			get
			{
				return AppSettings.GetValueOrDefault("QRPayment", "");
			}
			set
			{
				AppSettings.AddOrUpdateValue("QRPayment", value);
			}
		}

		public static int TeamID
		{
			get
			{
				return AppSettings.GetValueOrDefault("TeamID", 0);
			}
			set
			{
				AppSettings.AddOrUpdateValue("TeamID", value);
			}
		}

		public static int AnswerTime
		{
			get
			{
				return AppSettings.GetValueOrDefault("AnswerTime", 60);
			}
			set
			{
				AppSettings.AddOrUpdateValue("AnswerTime", value);
			}
		}
		public static int AnswerPeriod
		{
			get
			{
				return AppSettings.GetValueOrDefault("AnswerPeriod", 60);
			}
			set
			{
				AppSettings.AddOrUpdateValue("AnswerPeriod", value);
			}
		}

		public static string Lastversion
		{
			get
			{
				return AppSettings.GetValueOrDefault("Lastversion", "");
			}
			set
			{
				AppSettings.AddOrUpdateValue("Lastversion", value);
			}
		}
		public static string Printer
		{
			get
			{
				return AppSettings.GetValueOrDefault("Printer", "");
			}
			set
			{
				AppSettings.AddOrUpdateValue("Uid", value);
			}
		}

	}
}