using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Data;
using System.Resources;
using System.Net.Mail;
using System.Web;
using System.Configuration;
using System.Net.Mime;
using System.Diagnostics;

namespace Grikwa.Models
{
    /// <summary>
    /// Implements some functions to support password manipulation or generation
    /// </summary>
    public class Password
    {
        /// <summary>
        /// Takes a string and generates a hash value of 16 bytes.
        /// </summary>
        /// <param name="str">The string to be hashed</param>
        /// <param name="passwordFormat">Selects the hashing algorithm used. Accepted values are "sha1" and "md5".</param>
        /// <returns>A hex string of the hashed password.</returns>
        public static string EncodeString(string str, string passwordFormat)
        {
            if (str == null)
                return null;

            ASCIIEncoding AE = new ASCIIEncoding();
            byte[] result;
            switch (passwordFormat)
            {
                case "sha1":
                    SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                    result = sha1.ComputeHash(AE.GetBytes(str));
                    break;
                case "md5":
                    MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    result = md5.ComputeHash(AE.GetBytes(str));
                    break;
                default:
                    throw new ArgumentException("Invalid format value. Accepted values are 'sha1' and 'md5'.", "passwordFormat");
            }

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }


            return sb.ToString();
        }

        /// <summary>
        /// Takes a string and generates a hash value of 16 bytes.  Uses "md5" by default.
        /// </summary>
        /// <param name="str">The string to be hashed</param>
        /// <returns>A hex string of the hashed password.</returns>
        public static string EncodeString(string str)
        {
            return EncodeString(str, "md5");
        }



        /// <summary>
        /// Takes a string and generates a hash value of 16 bytes.
        /// </summary>
        /// <param name="str">The string to be hashed</param>
        /// <param name="passwordFormat">Selects the hashing algorithm used. Accepted values are "sha1" and "md5".</param>
        /// <returns>A string of the hashed password.</returns>
        public static string EncodeBinary(byte[] buffer, string passwordFormat)
        {
            if (buffer == null)
                return null;

            byte[] result;
            switch (passwordFormat)
            {
                case "sha1":
                    SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                    result = sha1.ComputeHash(buffer);
                    break;
                case "md5":
                    MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    result = md5.ComputeHash(buffer);
                    break;
                default:
                    throw new ArgumentException("Invalid format value. Accepted values are 'sha1' and 'md5'.", "passwordFormat");
            }


            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }


            return sb.ToString();
        }

        /// <summary>
        /// Encodes the buffer using the default cryptographic provider.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public static string EncodeBinary(byte[] buffer)
        {
            return EncodeBinary(buffer, "md5");
        }

        /// <summary>
        /// Creates a random alphanumeric password.
        /// </summary>
        /// <returns>A default length character string with the new password.</returns>
        /// <remarks>The default length of the password is eight (8) characters.</remarks>
        public static string CreateRandomPassword()
        {
            //Default length is 8 characters
            return CreateRandomPassword(8);
        }

        /// <summary>
        /// Creates a random alphanumeric password on dimension (Length).
        /// </summary>
        /// <param name="Length">The number of characters in the password</param>
        /// <returns>The generated password</returns>
        public static string CreateRandomPassword(int Length)
        {
            Random rnd = new Random(Convert.ToInt32(DateTime.Now.Millisecond));  //Creates the seed from the time
            string Password = "";
            while (Password.Length < Length)
            {
                char newChar = Convert.ToChar((int)((122 - 48 + 1) * rnd.NextDouble() + 48));
                if ((((int)newChar) >= ((int)'A')) & (((int)newChar) <= ((int)'Z')) | (((int)newChar) >= ((int)'a')) & (((int)newChar) <= ((int)'z')) | (((int)newChar) >= ((int)'0')) & (((int)newChar) <= ((int)'9')))
                    Password += newChar;
            }
            return Password;
        }

        /// <summary>
        /// Takes a text message and encrypts it using a password as a key.
        /// </summary>
        /// <param name="plainMessage">A text to encrypt.</param>
        /// <param name="password">The password to encrypt the message with.</param>
        /// <returns>Encrypted string.</returns>
        /// <remarks>This method uses TripleDES symmmectric encryption.</remarks>
        public static string EncodeMessageWithPassword(string plainMessage, string password)
        {
            if (plainMessage == null)
                throw new ArgumentNullException("encryptedMessage", "The message cannot be null");

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];

            //Creates the key based on the password and stores it in a byte array.
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);

            MemoryStream ms = new MemoryStream(plainMessage.Length * 2);
            CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainMessage);
            encStream.Write(plainBytes, 0, plainBytes.Length);
            encStream.FlushFinalBlock();
            byte[] encryptedBytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(encryptedBytes, 0, (int)ms.Length);
            encStream.Close();

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Takes an encrypted message using TripleDES and a password as a key and converts it to the original text message.
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message to decode.</param>
        /// <param name="password">The password to decode the message.</param>
        /// <returns>The Decrypted message</returns>
        /// <remarks>This method uses TripleDES symmmectric encryption.</remarks>
        public static string DecodeMessageWithPassword(string encryptedMessage, string password)
        {
            if (encryptedMessage == null)
                throw new ArgumentNullException("encryptedMessage", "The encrypted message cannot be null");

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];

            //Creates the key based on the password and stores it in a byte array.
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);

            //This line protects the + signs that get replaced by spaces when the parameter is not urlencoded when sent.
            encryptedMessage = encryptedMessage.Replace(" ", "+");
            MemoryStream ms = new MemoryStream(encryptedMessage.Length * 2);
            CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] plainBytes;
            try
            {
                byte[] encBytes = Convert.FromBase64String(Convert.ToString(encryptedMessage));
                decStream.Write(encBytes, 0, encBytes.Length);
                decStream.FlushFinalBlock();
                plainBytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(plainBytes, 0, (int)ms.Length);
                decStream.Close();
            }
            catch (CryptographicException e)
            {
                throw new ApplicationException("Cannot decrypt message.  Possibly, the password is wrong", e);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }

    public class AccountHelper
    {

        #region seed
        private const string SEED = "3F18EB95DD73CD7FADDE7377868136D7FB502E99E73C0D197BFACBB9D1615052510BAC4CF4071020895465F02C4EF7312BB0390B1113A1F6141FDA03C74072F7";
        #endregion

        /// <summary>
        /// Gets the token for invitation.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string GetTokenForInvitation(string email)
        {
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("The email cannot be null");

            string token = Password.EncodeMessageWithPassword(String.Format("{0}#{1}", email, DateTime.Now), SEED);

            return token;
        }


        /// <summary>
        /// Gets the email from token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool GetEmailFromToken(string token, out string email)
        {
            email = String.Empty;


            string message = Password.DecodeMessageWithPassword(token, SEED);
            string[] messageParts = message.Split('#');

            if (messageParts.Count() != 2)
            {
                return false;
                // the token was not generated correctly.
            }
            else
            {
                email = messageParts[0];
                return true;
            }
        }

        /// <summary>
        /// Helper function used to generate a token to be used in the message sent to users when registered the first time to confirm their email address.
        /// </summary>
        /// <param name="email">The email address to encode.</param>
        /// <returns>The token generated from the email address, timestamp, and SEED value.</returns>
        public static string GetTokenForValidation(string email)
        {
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("The email cannot be null");

            string token = Password.EncodeMessageWithPassword(String.Format("{0}#{1}", email, DateTime.Now), SEED);

            return token;
        }


        /// <summary>
        /// Validates whether a given token is valid for a determined email address.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <param name="email">The email address to use in the validation.</param>
        /// <returns><c>true</c> if the token is valid, <c>false</c> otherwise.</returns>
        public static bool IsTokenValid(string token,int hours, string email)
        {
            return IsTokenValid(token, email, DateTime.Now,hours);
        }


        /// <summary>
        /// Core method to validate a token that also offers a timestamp for testing.  In production mode should always be DateTime.Now.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <param name="email">the email address to use in the validation.</param>
        /// <param name="timestamp">The timestamp representing the time in which the validation is performed.</param>
        /// <param name="hours">The hours representing the hours within which the token is valid.</param>
        /// <returns><c>true</c> if the token is valid, <c>false</c> otherwise.</returns>
        public static bool IsTokenValid(string token, string email, DateTime timestamp, int hours)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentException("The token cannot be null");

            try
            {
                string message = Password.DecodeMessageWithPassword(token, SEED);
                string[] messageParts = message.Split('#');

                if (messageParts.Count() != 2)
                {
                    return false;
                    // the token was not generated correctly.
                }
                else
                {
                    string messageEmail = messageParts[0];
                    string messageDate = messageParts[1];

                    // If the emails are the same and the date in which the token was created is no longer than 5 days, then it is valid. Otherwise, it is not. 
                    return (String.Compare(email, messageEmail, true) == 0 && timestamp.Subtract(DateTime.Parse(messageDate)).Days < hours);
                }
            }
            catch (Exception)
            {
                // could not decrypt the message. The token has been tampered with.
                return false;
            }
        }
    }

    public class NotificationsHelper
    {

        #region Constants
        public const string HOST = "smtp.sendgrid.net";
        public const int PORT = 587;
        public const string USERNAME = "azure_a34c075f62bba74426624b9a65795a59@azure.com";
        public const string PASSWORD = "pqx33rsp";
        public const string INVITATION_FROM_EMAIL_ADDRESS = "invitation@gwrikwa.co.za";
        public const string VERIFICATION_FROM_EMAIL_ADDRESS = "registration@gwrikwa.co.za";
        public const string RESET_FROM_EMAIL_ADDRESS = "reset@gwrikwa.co.za";
        public const string SALE_REQUEST_FROM_EMAIL_ADDRESS = "sales@gwrikwa.co.za";
        #endregion

        /// <summary>
        /// Sends an email with a validation token to make sure the email is associated to the account for the user.  Used when registering users.
        /// </summary>
        /// <param name="email">The email associated to the user account</param>
        /// <param name="controllerContext">The context of the controller.  (Required for processing the request)</param>
        public static bool SendEmailWithVerificationToken(string toEmail, string toName, string toUserName, System.Web.Mvc.ControllerContext controllerContext)
        {
            var success = false;

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(HOST, Convert.ToInt32(PORT));

                string token = AccountHelper.GetTokenForValidation(toEmail.Trim().ToLower());
                string authenticationUrl = String.Format("{0}?email={1}&token={2}", GetApplicationUrl(controllerContext) + "/Account/Verify", HttpUtility.UrlEncode(toEmail), HttpUtility.UrlEncode(token));
                string from = VERIFICATION_FROM_EMAIL_ADDRESS;
                string subject = "Grikwa Student Verification";

                string bodyHTML = "<h1><strong>Dear Grikwa User"
                                    + "</strong>, welcome to the Grikwa Notice Board.</h1>"
                                    + "<h4>Your username is <strong>" + toUserName + "</strong>.</h4> Click <a href='"
                                    + authenticationUrl + "'>here</a> to verify your account"
                                    + " <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Grikwa User"
                                    + ", welcome to the Grikwa Notice Board."
                                    + "Your username is " + toUserName + ". Go to the following address: "
                                    + authenticationUrl + " to verify your account. Grikwa Team";

                message.To.Add(toEmail);
                message.From = new MailAddress(from);
                message.Subject = subject;

                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(USERNAME, PASSWORD); // Enter senders User name and password
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);

                success = true;
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                success = false;
                Trace.WriteLine(smtpFailedRecipientException.Message, "Verification Email To: " + toEmail);
            }
            catch (SmtpException smtpException)
            {
                success = false;
                Trace.WriteLine(smtpException.Message, "Verification Email To: " + toEmail);
            }
            catch (Exception e)
            {
                success = false;
                Trace.WriteLine(e.Message, "Verification Email To: " + toEmail);
            }

            return success;

        }

        /// <summary>
        /// Sends an email with a url so users can reset their password.
        /// </summary>
        /// <param name="email">The email associated to the user account</param>
        /// <param name="controllerContext">The context of the controller.  (Required for processing the request)</param>
        public static bool SendPasswordResetEmail(string toEmail,string toName, System.Web.Mvc.ControllerContext controllerContext)
        {
            var success = false;

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(HOST, Convert.ToInt32(PORT));

                string token = AccountHelper.GetTokenForValidation(toEmail.Trim().ToLower());
                string url = String.Format("{0}?email={1}&token={2}", GetApplicationUrl(controllerContext) + "/Account/Reset", HttpUtility.UrlEncode(toEmail), HttpUtility.UrlEncode(token));
                string from = RESET_FROM_EMAIL_ADDRESS;
                string subject = "Grikwa Password Reset";

                string bodyHTML = "<h1><strong>Dear Grikwa User</strong>.</h1>"
                              + "<p>Click <a href='" + url
                              + "'>here</a> to reset your password. "
                              + "This token will exprire after 5 hours. Request new token if this token has expired. <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Grikwa User. "
                              + "Go to this address: " + url
                              + " to reset your password. "
                              + "This token will exprire after 5 hours. Request new token if this token has expired. Grikwa Team";

                message.To.Add(toEmail);
                message.From = new MailAddress(from);
                message.Subject = subject;

                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(USERNAME, PASSWORD); // Enter senders User name and password
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);

                success = true;
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                success = false;
                Trace.WriteLine(smtpFailedRecipientException.Message, "Invitation Email To: " + toEmail);
            }
            catch (SmtpException smtpException)
            {
                success = false;
                Trace.WriteLine(smtpException.Message, "Invitation Email To: " + toEmail);
            }
            catch (Exception e)
            {
                success = false;
                Trace.WriteLine(e.Message, "Invitation Email To: " + toEmail);
            }

            return success;
        }

        public static bool SendRegistrationEmail(string toEmail, string toName, System.Web.Mvc.ControllerContext controllerContext)
        {
            var success = false;

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(HOST, Convert.ToInt32(PORT));

                string token = AccountHelper.GetTokenForValidation(toEmail.Trim().ToLower());
                string url = String.Format("{0}/Account/Verify?token={1}", GetApplicationUrl(controllerContext), controllerContext.HttpContext.Server.UrlEncode(token));
                string from = VERIFICATION_FROM_EMAIL_ADDRESS;

                string BodyText = "";
                string BodyHTML = "";

                message.To.Add(toEmail);
                message.From = new MailAddress(from);
                message.Subject = "Grikwa Invitation";

                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(BodyText, null, MediaTypeNames.Text.Plain));
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(BodyHTML, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(USERNAME, PASSWORD); // Enter senders User name and password
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);

                success = true;
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                success = false;
                Trace.WriteLine(smtpFailedRecipientException.Message, "Invitation Email To: " + toEmail);
            }
            catch (SmtpException smtpException)
            {
                success = false;
                Trace.WriteLine(smtpException.Message, "Invitation Email To: " + toEmail);
            }
            catch (Exception e)
            {
                success = false;
                Trace.WriteLine(e.Message, "Invitation Email To: " + toEmail);
            }

            return success;
        }

        /// <summary>
        /// Generates an email that redirects a new user to register in this location /Referral/Welcome/token
        /// </summary>
        /// <param name="fromEmail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="token"></param>
        /// <param name="controllerContext"></param>
        public static bool SendInvitationEmail(string toEmail, string toName, System.Web.Mvc.ControllerContext controllerContext)
        {

            var success = false;

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(HOST, Convert.ToInt32(PORT));

                string token = AccountHelper.GetTokenForInvitation(toEmail.Trim().ToLower());
                string url = String.Format("{0}/Account/Referrals?token={1}", GetApplicationUrl(controllerContext), controllerContext.HttpContext.Server.UrlEncode(token));
                string from = INVITATION_FROM_EMAIL_ADDRESS;

                string BodyText = "";
                string BodyHTML = "";

                message.To.Add(toEmail);
                message.From = new MailAddress(from);
                message.Subject = "Grikwa Invitation";

                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(BodyText, null, MediaTypeNames.Text.Plain));
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(BodyHTML, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(USERNAME,PASSWORD); // Enter senders User name and password
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);

                success = true;
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                success = false;
                Trace.WriteLine(smtpFailedRecipientException.Message, "Invitation Email To: " + toEmail);
            }
            catch(SmtpException smtpException)
            {
                success = false;
                Trace.WriteLine(smtpException.Message, "Invitation Email To: " + toEmail);
            }
            catch (Exception e)
            {
                success = false;
                Trace.WriteLine(e.Message, "Invitation Email To: " + toEmail);
            }
            
            return success;
        }

        /// <summary>
        /// Retrieves the domain of the web application so the verification urls can be constructed.
        /// </summary>
        /// <param name="controllerContext">ControllerContext is required for the url analysis.</param>
        /// <returns>The base url for the web application.</returns>
        private static string GetApplicationUrl(System.Web.Mvc.ControllerContext controllerContext)
        {
            Uri url = controllerContext.RequestContext.HttpContext.Request.Url;
            return String.Format("{0}://{1}", url.Scheme, url.Authority);
        }


    }
}