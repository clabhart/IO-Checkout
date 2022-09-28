using System;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    internal static class Program
    {
        #region Private Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            IOCheckout checkout;
            Application.Run(checkout = new IOCheckout());
            checkout.Dispose();
        }

        #endregion Private Methods
    }
}