// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Yammerly.Helpers
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

    private const string LoggedInKey = "logged_in";
    private static readonly bool LoggedInDefault = false;

    #endregion


    public static bool LoggedIn
    {
      get
      {
        return AppSettings.GetValueOrDefault<bool>(LoggedInKey, LoggedInDefault);
      }
      set
      {
        AppSettings.AddOrUpdateValue<bool>(LoggedInKey, value);
      }
    }

  }
}