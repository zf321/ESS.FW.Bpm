using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    /// </summary>
    public class ClassDelegateUtil
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        public static object InstantiateDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations)
        {
            return InstantiateDelegate(clazz.FullName, fieldDeclarations);
        }

        public static object InstantiateDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
        {
            var artifactFactory = Context.ProcessEngineConfiguration.ArtifactFactory;

            try
            {
                var clazz = ReflectUtil.LoadClass(className);

                var @object = artifactFactory.GetArtifact<object>(clazz);

                ApplyFieldDeclaration(fieldDeclarations, @object);
                return @object;
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInstantiatingClass(className, e);
            }
        }

        public static void ApplyFieldDeclaration(IList<FieldDeclaration> fieldDeclarations, object target)
        {
            if (fieldDeclarations != null)
                foreach (var declaration in fieldDeclarations)
                    ApplyFieldDeclaration(declaration, target);
        }

        public static void ApplyFieldDeclaration(FieldDeclaration declaration, object target)
        {
            var setterMethod = ReflectUtil.GetSetter(declaration.Name, target.GetType(), declaration.Value.GetType());

            if (setterMethod != null)
            {
                try
                {
                    setterMethod.Invoke(target, new[] {declaration.Value});
                }
                catch (System.Exception e)
                {
                    throw Log.ExceptionWhileApplyingFieldDeclatation(declaration.Name, target.GetType().FullName, e);
                }
            }
            else
            {
                var field = ReflectUtil.GetField(declaration.Name, target);
                EnsureUtil.EnsureNotNull("Field definition uses unexisting field '" + declaration.Name + "' on class " + target.GetType().FullName, "field", field);
                // Check if the delegate field's type is correct
                if (!FieldTypeCompatible(declaration, field))
                {
                    //throw Log.IncompatibleTypeForFieldDeclaration(declaration, target, field);
                }
                ReflectUtil.SetField(field, target, declaration.Value);
            }
        }

        public static bool FieldTypeCompatible(FieldDeclaration declaration, FieldInfo field)
        {
            if (declaration.Value != null)
                return declaration.Value.GetType().IsSubclassOf(field.FieldType);
            // Null can be set any field type
            return true;
        }
    }
}