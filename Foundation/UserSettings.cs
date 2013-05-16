using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;

namespace Stampsy.Extensions.Foundation
{
    /// <summary>
    /// A friendlier wrapper around <see cref="NSUserDefaults"/>.
    /// </summary>
    /// <remarks>
    /// Call <see cref="RegisterDefaultsFromSettingsBundle"/> in <c>AppDelegate.FinishedLaunching</c> to use defaults specified in <c>Settings.bundle/Root.plist</c>.
    /// Call <see cref="Synchronize"/> any time you want to sync settings with disk (a good time might be in <c>AppDelegate.OnActivated</c> so you can react to changes in Settings.app).
    ///
    /// <see cref="Read"/>, <see cref="Write"/> and <see cref="Remove"/> presume you have an enumeration with keys named as preference keys, for example:
    /// <code>
    ///
    ///
    /// enum MySetting {
    ///     MuteNotifications,
    ///     GodMode,
    ///     UserId
    /// }
    ///
    /// using MySettings = Stampsy.Extensions.Foundation.UserSettings<MySetting>
    ///
    /// MySettings.RegisterDefaultsFromSettingsBundle ();
    /// bool god = MySettings.Read<bool> (MySetting.GodMode);
    /// </code>
    /// </remarks>
    /// <typeparam name="TSettingsEnum">Enum that contains preference keys.</typeparam>
    public static class UserSettings<TSettingsEnum>
        where TSettingsEnum : struct
    {
        static NSUserDefaults StandardDefaults {
            get { return NSUserDefaults.StandardUserDefaults; }
        }

        /// <summary>
        /// Reads defaults from Settings.bundle and registers them with <see cref="NSUserDefaults"/>.
        /// Call this method in your <c>AppDelegate</c>'s <c>FinishedLaunching</c> override.
        /// </summary>
        /// <remarks>
        /// Learn more about the problem and this solution:
        /// http://stackoverflow.com/a/510329/458193
        /// </remarks>
        public static void RegisterDefaultsFromSettingsBundle ()
        {
            var bundle = NSBundle.MainBundle.PathForResource ("Settings", "bundle");
            if (bundle == null) {
                // We can't use default values from Settings.bundle because it doesn't exist
                return;
            }

            var settings = NSDictionary.FromFile (Path.Combine (bundle, "Root.plist"));
            var preferences = (NSArray) settings.ValueForKey ((NSString) "PreferenceSpecifiers");
            var defaultsToRegister = new NSMutableDictionary ();

            for (int i = 0; i < preferences.Count; i++) {
                var pref = new NSDictionary (preferences.ValueAt ((uint) i));
                var key = (NSString) pref.ValueForKey ((NSString) "Key");
                var def = pref.ValueForKey ((NSString) "DefaultValue");

                if (key != null && def != null)
                    defaultsToRegister.SetValueForKey (def, key);
            }

            StandardDefaults.RegisterDefaults (defaultsToRegister);
        }

        /// <summary>
        /// Registers arbitrary defaults with <see cref="NSUserDefaults"/>.
        /// Call this method in your <c>AppDelegate</c>'s <c>FinishedLaunching</c> override.
        /// </summary>
        public static void RegisterDefaults (Dictionary<TSettingsEnum, object> defaults)
        {
            var keys = defaults.Keys.Select (s => (NSString) s.ToString ()).ToArray ();
            var values = defaults.Values.ToArray ();

            StandardDefaults.RegisterDefaults (NSDictionary.FromObjectsAndKeys (values, keys));
        }

        /// <summary>
        /// Synchronizes in-memory changes with settings on disk.
        /// Call this method in your <c>AppDelegate</c>'s <c>OnActivated</c> override to immediately react to changes in Settings.app.
        /// </summary>
        public static void Synchronize ()
        {
            StandardDefaults.Synchronize ();
        }

        /// <summary>
        /// Reads a value from preferences.
        /// </summary>
        /// <param name="setting">The name of enum value will be used as preference key.</param>
        /// <typeparam name="TPreference">The type of preference value.</typeparam>
        public static TPreference Read<TPreference> (TSettingsEnum setting)
        {
            var key = setting.ToString ();

            object val = null;

            switch (typeof (TPreference).Name) {
                case "Int32":
                val = (object) StandardDefaults.IntForKey (key);
                break;
                case "Single":
                val = (object) StandardDefaults.FloatForKey (key);
                break;
                case "Boolean":
                val = (object) StandardDefaults.BoolForKey (key);
                break;
                case "String":
                val = (object) StandardDefaults.StringForKey (key);
                break;
                case "Int64":
                val = (object) Convert.ToInt64 (StandardDefaults.StringForKey (key));
                break;
                default:
                throw new NotImplementedException (string.Format ("Reading preference as '{0}' is not supported yet.", typeof (TPreference).Name));
            }

            return (TPreference) val;
        }

        /// <summary>
        /// Writes a value to user preferences.
        /// </summary>
        /// <param name="setting">The name of enum value will be used as preference key.</param>
        /// <param name="value">The value to write</param>
        /// <param name="synchronize">If set to <c>true<c/>, the change will immediately get flushed to the disk.</param>
        /// <typeparam name="TPreference">The type of preference value.</typeparam>
        public static void Write<TPreference> (TSettingsEnum setting, TPreference value, bool synchronize = true)
        {
            var key = setting.ToString ();

            switch (typeof (TPreference).Name) {
                case "Int32":
                StandardDefaults.SetInt ((int) (object) value, key);
                break;
                case "Single":
                StandardDefaults.SetFloat ((float) (object) value, key);
                break;
                case "Boolean":
                StandardDefaults.SetBool ((bool) (object) value, key);
                break;
                case "String":
                StandardDefaults.SetString (value.ToString (), key);
                break;
                case "Int64":
                StandardDefaults.SetString (value.ToString (), key);
                break;
                default:
                throw new NotImplementedException (string.Format ("Writing preference as '{0}' is not supported yet.", typeof (TPreference).Name));
            }

            if (synchronize)
                Synchronize ();
        }

        /// <summary>
        /// Removes the specified preference.
        /// </summary>
        /// <param name="setting">The name of enum value will be used as preference key.</param>
        /// <param name="synchronize">If set to <c>true<c/>, the change will immediately get flushed to the disk.</param>
        public static void Remove (TSettingsEnum setting, bool synchronize = true)
        {
            Remove (new [] { setting }, synchronize);
        }

        /// <summary>
        /// Removes the specified preferences and synchronizes settings.
        /// </summary>
        /// <remarks>To control whether to synchronize settings, user other overloads.</remarks>
        /// <param name="settings">The name of enum value will be used as preference key.</param>
        /// <typeparam name="TSettingsEnum">Enum that contains preference keys.</typeparam>
        public static void Remove (params TSettingsEnum [] settings)
        {
            Remove (settings, true);
        }

        /// <summary>
        /// Removes the specified preference.
        /// </summary>
        /// <param name="setting">The name of enum value will be used as preference key.</param>
        /// <typeparam name="TSettingsEnum">Enum that contains preference keys.</typeparam>
        public static void Remove (TSettingsEnum [] settings, bool synchronize = true)
        {
            foreach (var setting in settings) {
                StandardDefaults.RemoveObject (setting.ToString ());
            }

            if (synchronize)
                Synchronize ();
        }
    }
}