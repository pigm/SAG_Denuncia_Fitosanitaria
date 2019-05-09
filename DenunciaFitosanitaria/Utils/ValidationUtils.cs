using System.Text.RegularExpressions;
using Plugin.Connectivity;
namespace DenunciaFitosanitaria.Utils
{
    /// <summary>
    /// Validation utils.
    /// </summary>
    public static class ValidationUtils
    {
        /// <summary>
        /// Emails the is valid.
        /// </summary>
        /// <returns><c>true</c>, if is valid was emailed, <c>false</c> otherwise.</returns>
        /// <param name="email">Email.</param>
		public static bool EmailIsValid(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// Gets the network status.
        /// </summary>
        /// <returns><c>true</c>, if network status was gotten, <c>false</c> otherwise.</returns>
        public static bool GetNetworkStatus()
        {
            return CrossConnectivity.Current.IsConnected;
        }

    }
}
