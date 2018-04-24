using System.IO;
using System.Xml;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///      
    /// </summary>
    public class BeansConfigurationHelper
    {
        public static ProcessEngineConfiguration ParseProcessEngineConfiguration(XmlDocument resource,
            string beanName)
        {
            //DefaultListableBeanFactory beanFactory = new DefaultListableBeanFactory();
            //XmlBeanDefinitionReader xmlBeanDefinitionReader = new XmlBeanDefinitionReader(beanFactory);
            //xmlBeanDefinitionReader.ValidationMode = XmlBeanDefinitionReader.VALIDATION_XSD;
            //xmlBeanDefinitionReader.loadBeanDefinitions(springResource);
            //ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)beanFactory.getBean(beanName);
            //if (processEngineConfiguration.Beans == null)
            //{
            //    processEngineConfiguration.Beans = new SpringBeanFactoryProxyMap(beanFactory);
            //}
            //return processEngineConfiguration;
            return null;
        }

        public static ProcessEngineConfiguration ParseProcessEngineConfigurationFromInputStream(Stream inputStream,
            string beanName)
        {
            //Resource springResource = new InputStreamResource(inputStream);
            //return ParseProcessEngineConfiguration(springResource, beanName);
            return null;
        }
        /// <summary>
        /// 流程引擎加载配置 camunda.cfg.xml 暂时使用StandaloneInMemProcessEngineConfiguration
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="beanName"></param>
        /// <returns></returns>
        public static ProcessEngineConfiguration ParseProcessEngineConfigurationFromResource(string resource,
            string beanName)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(resource);
            //return ParseProcessEngineConfiguration(doc, beanName);
            return new StandaloneInMemProcessEngineConfiguration();
            //return new TxProcessEngineConfiguration();
        }
    }
}