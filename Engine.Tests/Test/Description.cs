using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Engine.Tests.Test
{


    /// <summary>
    /// <para>A <code>Description</code> describes a test which is to be run or has been run. <code>Descriptions</code>
    /// can be atomic (a single test) or compound (containing children tests). <code>Descriptions</code> are used
    /// to provide feedback about the tests that are about to run (for example, the tree view
    /// visible in many IDEs) or tests that have been run (for example, the failures view).</para>
    /// 
    /// <para><code>Descriptions</code> are implemented as a single class rather than a Composite because
    /// they are entirely informational. They contain no logic aside from counting their tests.</para>
    /// 
    /// <para>In the past, we used the raw <seealso cref="junit.framework.TestCase"/>s and <seealso cref="junit.framework.TestSuite"/>s
    /// to display the tree of tests. This was no longer viable in JUnit 4 because atomic tests no longer have
    /// a superclass below <seealso cref="Object"/>. We needed a way to pass a class and name together. Description
    /// emerged from this.</para>
    /// </summary>
    [Serializable]
    public class Description
    {
        private static readonly Regex METHOD_AND_CLASS_NAME_PATTERN = new Regex("(.*)\\((.*)\\)");

        /// <summary>
        /// Create a <code>Description</code> named <code>name</code>.
        /// Generally, you will add children to this <code>Description</code>.
        /// </summary>
        /// <param name="name"> the name of the <code>Description</code> </param>
        /// <param name="annotations"> meta-data about the test, for downstream interpreters </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateSuiteDescription(string name, params Attribute[] annotations)
        {
            return new Description(null, name, annotations);
        }

        /// <summary>
        /// Create a <code>Description</code> named <code>name</code>.
        /// Generally, you will add children to this <code>Description</code>.
        /// </summary>
        /// <param name="name"> the name of the <code>Description</code> </param>
        /// <param name="uniqueId"> an arbitrary object used to define uniqueness (in <seealso cref="#equals(Object)"/> </param>
        /// <param name="annotations"> meta-data about the test, for downstream interpreters </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateSuiteDescription(string name, string uniqueId, params Attribute[] annotations)
        {
            return new Description(null, name, uniqueId, annotations);
        }

        /// <summary>
        /// Create a <code>Description</code> of a single test named <code>name</code> in the 'class' named
        /// <code>className</code>. Generally, this will be a leaf <code>Description</code>. This method is a better choice
        /// than <seealso cref="#createTestDescription(Class, String, Attribute...)"/> for test runners whose test cases are not
        /// defined in an actual Java <code>Class</code>.
        /// </summary>
        /// <param name="className"> the class name of the test </param>
        /// <param name="name"> the name of the test (a method name for test annotated with <seealso cref="org.junit.Test"/>) </param>
        /// <param name="annotations"> meta-data about the test, for downstream interpreters </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateTestDescription(string className, string name, params Attribute[] annotations)
        {
            return new Description(null, FormatDisplayName(name, className), annotations);
        }

        /// <summary>
        /// Create a <code>Description</code> of a single test named <code>name</code> in the class <code>clazz</code>.
        /// Generally, this will be a leaf <code>Description</code>.
        /// </summary>
        /// <param name="clazz"> the class of the test </param>
        /// <param name="name"> the name of the test (a method name for test annotated with <seealso cref="org.junit.Test"/>) </param>
        /// <param name="annotations"> meta-data about the test, for downstream interpreters </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateTestDescription(Type clazz, string name, params Attribute[] annotations)
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            return new Description(clazz, FormatDisplayName(name, clazz.FullName), annotations);
        }

        /// <summary>
        /// Create a <code>Description</code> of a single test named <code>name</code> in the class <code>clazz</code>.
        /// Generally, this will be a leaf <code>Description</code>.
        /// (This remains for binary compatibility with clients of JUnit 4.3)
        /// </summary>
        /// <param name="clazz"> the class of the test </param>
        /// <param name="name"> the name of the test (a method name for test annotated with <seealso cref="org.junit.Test"/>) </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateTestDescription(Type clazz, string name)
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            return new Description(clazz, FormatDisplayName(name, clazz.FullName));
        }

        /// <summary>
        /// Create a <code>Description</code> of a single test named <code>name</code> in the class <code>clazz</code>.
        /// Generally, this will be a leaf <code>Description</code>.
        /// </summary>
        /// <param name="name"> the name of the test (a method name for test annotated with <seealso cref="org.junit.Test"/>) </param>
        /// <returns> a <code>Description</code> named <code>name</code> </returns>
        public static Description CreateTestDescription(string className, string name, string uniqueId)
        {
            return new Description(null, FormatDisplayName(name, className), uniqueId);
        }

        private static string FormatDisplayName(string name, string className)
        {
            return string.Format("{0}({1})", name, className);
        }

        /// <summary>
        /// Create a <code>Description</code> named after <code>testClass</code>
        /// </summary>
        /// <param name="testClass"> A <seealso cref="Class"/> containing tests </param>
        /// <returns> a <code>Description</code> of <code>testClass</code> </returns>
        public static Description CreateSuiteDescription(Type testClass)
        {
            return new Description(testClass, testClass.FullName, testClass.GetCustomAttributes(true) as Attribute[]);
        }

        /// <summary>
        /// Describes a Runner which runs no tests
        /// </summary>
        public static readonly Description EMPTY = new Description(null, "No Tests");

        /// <summary>
        /// Describes a step in the test-running mechanism that goes so wrong no
        /// other description can be used (for example, an exception thrown from a Runner's
        /// constructor
        /// </summary>
        public static readonly Description TEST_MECHANISM = new Description(null, "Test mechanism");

        private readonly List<Description> fChildren = new List<Description>();
        private readonly string fDisplayName;
        private readonly string fUniqueId;
        private readonly Attribute[] fAnnotations;
        private Type fTestClass;

        private Description(Type clazz, string displayName, params Attribute[] annotations) : this(clazz, displayName, displayName, annotations)
        {
        }

        private Description(Type clazz, string displayName, string uniqueId, params Attribute[] annotations)
        {
            if ((string.ReferenceEquals(displayName, null)) || (displayName.Length == 0))
            {
                throw new System.ArgumentException("The display name must not be empty.");
            }
            if ((uniqueId == null))
            {
                throw new System.ArgumentException("The unique id must not be null.");
            }
            fTestClass = clazz;
            fDisplayName = displayName;
            fUniqueId = uniqueId;
            fAnnotations = annotations;
        }

        /// <returns> a user-understandable label </returns>
        public virtual string DisplayName
        {
            get
            {
                return fDisplayName;
            }
        }

        /// <summary>
        /// Add <code>Description</code> as a child of the receiver.
        /// </summary>
        /// <param name="description"> the soon-to-be child. </param>
        public virtual void AddChild(Description description)
        {
            Children.Add(description);
        }

        /// <returns> the receiver's children, if any </returns>
        public virtual List<Description> Children
        {
            get
            {
                return fChildren;
            }
        }

        /// <returns> <code>true</code> if the receiver is a suite </returns>
        public virtual bool Suite
        {
            get
            {
                return !Test;
            }
        }

        /// <returns> <code>true</code> if the receiver is an atomic test </returns>
        public virtual bool Test
        {
            get
            {
                return Children.Count == 0;
            }
        }

        /// <returns> the total number of atomic tests in the receiver </returns>
        public virtual int TestCount()
        {
            if (Test)
            {
                return 1;
            }
            int result = 0;
            foreach (Description child in Children)
            {
                result += child.TestCount();
            }
            return result;
        }

        public override int GetHashCode()
        {
            return fUniqueId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Description))
            {
                return false;
            }
            Description d = (Description)obj;
            return fUniqueId.Equals(d.fUniqueId);
        }

        public override string ToString()
        {
            return DisplayName;
        }

        /// <returns> true if this is a description of a Runner that runs no tests </returns>
        public virtual bool Empty
        {
            get
            {
                return Equals(EMPTY);
            }
        }

        /// <returns> a copy of this description, with no children (on the assumption that some of the
        ///         children will be added back) </returns>
        public virtual Description ChildlessCopy()
        {
            return new Description(fTestClass, fDisplayName, fAnnotations);
        }

        /// <returns> the annotation of type annotationType that is attached to this description node,
        ///         or null if none exists </returns>
        public virtual T GetAnnotation<T>(Type annotationType) where T : Attribute
        {
            foreach (Attribute each in fAnnotations)
            {
                if (each.GetType().Equals(annotationType))
                {
                    return (T)Activator.CreateInstance(annotationType);
                }
            }
            return default(T);
        }

        /// <returns> all of the annotations attached to this description node </returns>
        public virtual ICollection<Attribute> Annotations
        {
            get
            {
                return (fAnnotations);
            }
        }

        /// <returns> If this describes a method invocation,
        ///         the class of the test instance. </returns>
        public virtual Type TestClass
        {
            get
            {
                if (fTestClass != null)
                {
                    return fTestClass;
                }
                string name = ClassName;
                if (string.ReferenceEquals(name, null))
                {
                    return null;
                }
                try
                {
                    fTestClass = Type.GetType(name);
                    return fTestClass;
                }
                catch (System.Exception ex)
                {
                    return null;
                }
            }
        }

        /// <returns> If this describes a method invocation,
        ///         the name of the class of the test instance </returns>
        public virtual string ClassName
        {
            get
            {
                return fTestClass != null ? fTestClass.FullName : MethodAndClassNamePatternGroupOrDefault(2, ToString());
            }
        }

        /// <returns> If this describes a method invocation,
        ///         the name of the method (or null if not) </returns>
        public virtual string MethodName
        {
            get
            {
                return MethodAndClassNamePatternGroupOrDefault(1, null);
            }
        }

        private string MethodAndClassNamePatternGroupOrDefault(int group, string defaultString)
        {
            Match matcher = METHOD_AND_CLASS_NAME_PATTERN.Match(ToString());
            return matcher.Success ? matcher.Groups[group].Value : defaultString;
        }
    }
}
