namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class ClassLoaderUtil
    {
        //public static ClassLoader ContextClassloader
        //{
        // get
        // {
        //if (System.SecurityManager != null)
        //{
        //  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass());
        //}
        //else
        //{
        //  return Thread.CurrentThread.ContextClassLoader;
        //}
        // }
        // set
        // {
        //if (System.SecurityManager != null)
        //{
        //  AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass3(value));
        //}
        //else
        //{
        //  Thread.CurrentThread.ContextClassLoader = value;
        //}
        // }
        //}

//	  private class PrivilegedActionAnonymousInnerClass : PrivilegedAction<ClassLoader>
//	  {
//		  public PrivilegedActionAnonymousInnerClass()
//		  {
//		  }

//		  public virtual ClassLoader run()
//		  {
//			return Thread.CurrentThread.ContextClassLoader;
//		  }
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: public static ClassLoader getClassloader(final Class clazz)
//	  public static ClassLoader getClassloader(Type clazz)
//	  {
//		if (System.SecurityManager != null)
//		{
//		  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass2(clazz));
//		}
//		else
//		{
//		  return clazz.ClassLoader;
//		}
//	  }

//	  private class PrivilegedActionAnonymousInnerClass2 : PrivilegedAction<ClassLoader>
//	  {
//		  private Type clazz;

//		  public PrivilegedActionAnonymousInnerClass2(Type clazz)
//		  {
//			  this.clazz = clazz;
//		  }

//		  public virtual ClassLoader run()
//		  {
//			return clazz.ClassLoader;
//		  }
//	  }


        //private class PrivilegedActionAnonymousInnerClass3 : PrivilegedAction<void>
        //{
        // private ClassLoader classLoader;

        // public PrivilegedActionAnonymousInnerClass3(ClassLoader classLoader)
        // {
        //  this.classLoader = classLoader;
        // }

        // public virtual void run()
        // {
        //Thread.CurrentThread.ContextClassLoader = classLoader;
        //return null;
        // }
        //}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static ClassLoader getServletContextClassloader(final javax.servlet.ServletContextEvent sce)
        // public static ClassLoader getServletContextClassloader(ServletContextEvent sce)
        // {
        //if (System.SecurityManager != null)
        //{
        //  return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass4(sce));
        //}
        //else
        //{
        //  return sce.ServletContext.ClassLoader;
        //}
        // }

        //private class PrivilegedActionAnonymousInnerClass4 : PrivilegedAction<ClassLoader>
        //{
        // private ServletContextEvent sce;

        // public PrivilegedActionAnonymousInnerClass4(ServletContextEvent sce)
        // {
        //  this.sce = sce;
        // }

        // public virtual ClassLoader run()
        // {
        //return sce.ServletContext.ClassLoader;
        // }
        //}
    }
}