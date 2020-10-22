using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TJAPlayer3.ErrorReporting
{
    public static class ErrorReporter
    {
        private const string EnvironmentAlpha = "alpha";
        private const string EnvironmentBeta = "beta";
        private const string EnvironmentDevelopment = "development";
        private const string EnvironmentProduction = "production";

        public const string GetCurrentSkinNameOrFallbackFallbackForExceptionEncountered = "[GetCurrentSkinNameOrNull exception encountered]";

        public static void WithErrorReporting(Action action)
        {
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    NotifyUserOfError(e);
                }
            }
        }

        public static string ToSha256InBase64(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var utf8Bytes = Encoding.UTF8.GetBytes(value);
                var sha256Bytes = sha256.ComputeHash(utf8Bytes);
                return Convert.ToBase64String(sha256Bytes);
            }
        }

        public static string GetEnvironment(string informationalVersion)
        {
            switch (Regex.Match(informationalVersion, @"(?<=^.+?[+-])\w+").Value)
            {
                case "Branch":
                {
                    return EnvironmentProduction;
                }
                case "beta":
                {
                    return EnvironmentBeta;
                }
                case "alpha":
                {
                    return EnvironmentAlpha;
                }
                default:
                {
                    return EnvironmentDevelopment;
                }
            }
        }

        private static void NotifyUserOfError(Exception exception)
        {
            var messageBoxText =
                "An error has occurred and TJAPlayer3 now must close.\n\n" +
                "If you wish, you can report this issue or look for similar issues by visiting our GitHub Issues page.\n\n" +
                "Would you like the error details copied to the clipboard and your browser opened?\n\n" +
                exception;
            var dialogResult = MessageBox.Show(
                messageBoxText,
                $"{TJAPlayer3.AppDisplayNameWithThreePartVersion} Error",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);
            if (dialogResult == DialogResult.Yes)
            {
                Clipboard.SetText(exception.ToString());
                Process.Start("https://github.com/twopointzero/TJAPlayer3/issues");
            }
        }
    }
}