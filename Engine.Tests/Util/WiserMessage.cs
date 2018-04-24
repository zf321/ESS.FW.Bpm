//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESS.FW.Bpm.Engine.Tests.Util
//{
//    /// <summary>
//	/// This class wraps a received message and provides
//	/// a way to generate a JavaMail MimeMessage from the data.
//	/// 
//	/// @author Jon Stevens
//	/// </summary>
//	public class WiserMessage
//    {
//        internal sbyte[] messageData;
//        internal Wiser wiser;
//        internal string envelopeSender;
//        internal string envelopeReceiver;

//        internal WiserMessage(Wiser wiser, string envelopeSender, string envelopeReceiver, sbyte[] messageData)
//        {
//            this.wiser = wiser;
//            this.envelopeSender = envelopeSender;
//            this.envelopeReceiver = envelopeReceiver;
//            this.messageData = messageData;
//        }

//        /// <summary>
//        /// Generate a JavaMail MimeMessage. </summary>
//        /// <exception cref="MessagingException"> </exception>
//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: public javax.mail.internet.MimeMessage getMimeMessage() throws javax.mail.MessagingException
//        public virtual MimeMessage getMimeMessage()
//        {
//            return new MimeMessage(this.wiser.getSession(), new ByteArrayInputStream(messageData));
//        }

//        /// <summary>
//        /// Get's the raw message DATA.
//        /// </summary>
//        public virtual sbyte[] getData()
//        {
//            return this.messageData;
//        }

//        /// <summary>
//        /// Get's the RCPT TO:
//        /// </summary>
//        public virtual string getEnvelopeReceiver()
//        {
//            return this.envelopeReceiver;
//        }

//        /// <summary>
//        /// Get's the MAIL FROM:
//        /// </summary>
//        public virtual string getEnvelopeSender()
//        {
//            return this.envelopeSender;
//        }
//    }

//}
