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

    private const string AuthTokenKey = "authToken";
    private static readonly string AuthTokenDefault = string.Empty;

    private const string UserIdKey = "user_id";
    private static readonly string UserIdDefault = string.Empty;
        #endregion

        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(UserId);
    
    public static string AuthToken
    {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value);
            }
     }

        public static string UserId
        {
            get { return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault); }
            set { AppSettings.AddOrUpdateValue<string>(UserIdKey, value); }
        }
    }
}