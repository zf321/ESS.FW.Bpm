

//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
//using ESS.FW.Bpm.Engine.Impl.Util;

//namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
//{

//    /// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class MailActivityBehavior : AbstractBpmnActivityBehavior
//    {

//        protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BpmnBehaviorLogger;

//        protected internal IExpression to;
//        protected internal IExpression from;
//        protected internal IExpression cc;
//        protected internal IExpression bcc;
//        protected internal IExpression subject;
//        protected internal IExpression text;
//        protected internal IExpression html;
//        protected internal IExpression charset;

//        public override void Execute(IActivityExecution execution)
//        {
//            string toStr = getStringFromField(to, execution);
//            string fromStr = getStringFromField(from, execution);
//            string ccStr = getStringFromField(cc, execution);
//            string bccStr = getStringFromField(bcc, execution);
//            string subjectStr = getStringFromField(subject, execution);
//            string textStr = getStringFromField(text, execution);
//            string htmlStr = getStringFromField(html, execution);
//            string charSetStr = getStringFromField(charset, execution);

//            Email email = createEmail(textStr, htmlStr);

//            addTo(email, toStr);
//            setFrom(email, fromStr);
//            addCc(email, ccStr);
//            addBcc(email, bccStr);
//            setSubject(email, subjectStr);
//            MailServerProperties = email;
//            setCharset(email, charSetStr);

//            try
//            {
//                email.send();
//            }
//            catch (EmailException e)
//            {
//                throw LOG.sendingEmailException(toStr, e);
//            }
//            leave(execution);
//        }

//        protected internal virtual Email createEmail(string text, string html)
//        {
//            if (!string.ReferenceEquals(html, null))
//            {
//                return createHtmlEmail(text, html);
//            }
//            else if (!string.ReferenceEquals(text, null))
//            {
//                return createTextOnlyEmail(text);
//            }
//            else
//            {
//                throw LOG.emailFormatException();
//            }
//        }

//        protected internal virtual HtmlEmail createHtmlEmail(string text, string html)
//        {
//            HtmlEmail email = new HtmlEmail();
//            try
//            {
//                email.HtmlMsg = html;
//                if (!string.ReferenceEquals(text, null))
//                { // for email clients that don't support html
//                    email.TextMsg = text;
//                }
//                return email;
//            }
//            catch (EmailException e)
//            {
//                throw LOG.emailCreationException("HTML", e);
//            }
//        }

//        protected internal virtual SimpleEmail createTextOnlyEmail(string text)
//        {
//            SimpleEmail email = new SimpleEmail();
//            try
//            {
//                email.Msg = text;
//                return email;
//            }
//            catch (EmailException e)
//            {
//                throw LOG.emailCreationException("text-only", e);
//            }
//        }

//        protected internal virtual void addTo(Email email, string to)
//        {
//            string[] tos = splitAndTrim(to);
//            if (tos != null)
//            {
//                foreach (string t in tos)
//                {
//                    try
//                    {
//                        email.addTo(t);
//                    }
//                    catch (EmailException e)
//                    {
//                        throw LOG.addRecipientException(t, e);
//                    }
//                }
//            }
//            else
//            {
//                throw LOG.missingRecipientsException();
//            }
//        }

//        protected internal virtual void setFrom(Email email, string from)
//        {
//            string fromAddress = null;

//            if (!string.ReferenceEquals(from, null))
//            {
//                fromAddress = from;
//            }
//            else
//            { // use default configured from address in process engine config
//                fromAddress = Context.ProcessEngineConfiguration.MailServerDefaultFrom;
//            }

//            try
//            {
//                email.From = fromAddress;
//            }
//            catch (EmailException e)
//            {
//                throw LOG.addSenderException(from, e);
//            }
//        }

//        protected internal virtual void addCc(Email email, string cc)
//        {
//            string[] ccs = splitAndTrim(cc);
//            if (ccs != null)
//            {
//                foreach (string c in ccs)
//                {
//                    try
//                    {
//                        email.addCc(c);
//                    }
//                    catch (EmailException e)
//                    {
//                        throw LOG.addCcException(c, e);
//                    }
//                }
//            }
//        }

//        protected internal virtual void addBcc(Email email, string bcc)
//        {
//            string[] bccs = splitAndTrim(bcc);
//            if (bccs != null)
//            {
//                foreach (string b in bccs)
//                {
//                    try
//                    {
//                        email.addBcc(b);
//                    }
//                    catch (EmailException e)
//                    {
//                        throw LOG.addBccException(b, e);
//                    }
//                }
//            }
//        }

//        protected internal virtual void setSubject(Email email, string subject)
//        {
//            email.Subject = !string.ReferenceEquals(subject, null) ? subject : "";
//        }

//        protected internal virtual Email MailServerProperties
//        {
//            set
//            {
//                ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

//                string host = processEngineConfiguration.MailServerHost;
//                EnsureUtil.EnsureNotNull("Could not send email: no SMTP host is configured", "host", host);
//                value.HostName = host;

//                int port = processEngineConfiguration.MailServerPort;
//                value.SmtpPort = port;

//                value.TLS = processEngineConfiguration.MailServerUseTLS;

//                string user = processEngineConfiguration.MailServerUsername;
//                string password = processEngineConfiguration.MailServerPassword;
//                if (!string.ReferenceEquals(user, null) && !string.ReferenceEquals(password, null))
//                {
//                    value.setAuthentication(user, password);
//                }
//            }
//        }

//        protected internal virtual void setCharset(Email email, string charSetStr)
//        {
//            if (charset != null)
//            {
//                email.Encoding = charSetStr;
//            }
//        }

//        protected internal virtual string[] splitAndTrim(string str)
//        {
//            if (!string.ReferenceEquals(str, null))
//            {
//                string[] splittedStrings = str.Split(",", true);
//                for (int i = 0; i < splittedStrings.Length; i++)
//                {
//                    splittedStrings[i] = splittedStrings[i].Trim();
//                }
//                return splittedStrings;
//            }
//            return null;
//        }

//        protected internal virtual string getStringFromField(IExpression expression, IDelegateExecution execution)
//        {
//            if (expression != null)
//            {
//                object value = expression.GetValue(execution);
//                if (value != null)
//                {
//                    return value.ToString();
//                }
//            }
//            return null;
//        }

//    }

//}

