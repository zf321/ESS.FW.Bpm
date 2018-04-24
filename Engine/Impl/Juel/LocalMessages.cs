using System.Text;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{


    public sealed class LocalMessages
    {
        private const string BUNDLE_NAME = "org.camunda.bpm.engine.impl.juel.misc.LocalStrings";
        //private static readonly ResourceBundle RESOURCE_BUNDLE = ResourceBundle.GetBundle(BUNDLE_NAME);

        public static string Get(string key, params object[] args)
        {
            return string.Format(key, args);
            //string template = null;
            //try
            //{
            //    template = RESOURCE_BUNDLE.GetString(key);
            //}
            //catch (MissingResourceException)
            //{
            //    StringBuilder b = new StringBuilder();
            //    try
            //    {
            //        b.Append(RESOURCE_BUNDLE.GetString("message.unknown"));
            //        b.Append(": ");
            //    }
            //    catch (MissingResourceException)
            //    {
            //    }
            //    b.Append(key);
            //    if (args != null && args.Length > 0)
            //    {
            //        b.Append("(");
            //        b.Append(args[0]);
            //        for (int i = 1; i < args.Length; i++)
            //        {
            //            b.Append(", ");
            //            b.Append(args[i]);
            //        }
            //        b.Append(")");
            //    }
            //    return b.ToString();
            //}
            //return MessageFormat.Format(template, args);
        }
    }

}