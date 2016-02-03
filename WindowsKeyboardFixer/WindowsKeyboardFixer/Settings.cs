using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsKeyboardFixer {

    /// <summary>
    ///  manges the windows settings (via the registry).
    /// </summary>
    public class Settings {


        #region property appName
        private string _appName;


        public string appName
        {
            get { return _appName; }
            set { _appName = value; }
        }
        #endregion

        #region constructors
        public Settings() {

        }
        #endregion

        #region public functions
        /// <summary>
        /// the entry in the register where to store / delete apps that are to start when booting. 
        /// </summary>
        private const string REGISTRY_RUN_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";


        /// <summary>
        /// registers the app for boot, if the paramter is true, otherwise deletes it from the boot process.
        /// </summary>
        /// <param name="register">true -> added to boot list, false -> removed</param>
        public void RegiterOrUnRegister(bool register) {
            Settings.setStartup(register, appName);
        }

        /// <summary>
        /// appends the app from the boot app list
        /// </summary>
        public void RegisterForBoot() {
            setStartup(true, appName);
        }
        /// <summary>
        /// removes the app from the boot app list
        /// </summary>
        public void UnRegisterForBoot() {
            setStartup(false, appName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public bool HaveRegistered() {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_KEY, true);
            return rk.GetValue(appName, null) != null;
        }
        #endregion

        #region internal functions. 
        /// <summary>
        /// helper function. 
        /// </summary>
        /// <param name="isAdd"></param>
        /// <param name="appName"></param>
        private static void setStartup(bool isAdd, string appName) {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_KEY, true);
            if (isAdd) {
                rk.SetValue(appName, Application.ExecutablePath.ToString()); //the path is the app to start :)
            } else {
                rk.DeleteValue(appName, false);
            }
        }
        #endregion
    }
}
