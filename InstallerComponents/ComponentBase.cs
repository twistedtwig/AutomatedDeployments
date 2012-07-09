using System;
using System.Collections.Generic;
using System.Reflection;

namespace InstallerComponents
{
    public abstract class ComponentBase
    {
        public string Name { get; protected set; }
        public ComponentType ComponentTypeEnum { get; set; }
        public int Index { get; set; }

        protected ComponentBase(string name, IDictionary<string, string> values, ComponentType componentType)
        {
            Index = -1;

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("no name given");
            }

            Name = name;
            ComponentTypeEnum = componentType;
            AssignValues(values);
        }

        protected void AssignValues(IDictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> valuePair in values)
            {
                AssignPropertyValue(this, valuePair.Key, valuePair.Value);
            }
        }

        /// <summary>
        /// Added the property value to the given object
        /// </summary>
        /// <param name="objectToHavePropertySet"><c>Object</c> The object to have a property set</param>
        /// <param name="propertyName"><c>String</c> The name of the property</param>
        /// <param name="propertyValue"><c>Object</c> The value of the property</param>
        public static void AssignPropertyValue(Object objectToHavePropertySet, String propertyName, Object propertyValue)
        {
            if (objectToHavePropertySet == null) { throw new ArgumentException("objectToHavePropertySet was NULL"); }
            if (propertyName == null) { throw new ArgumentException("propertyName was NULL"); }


            PropertyInfo property = GetProperty(objectToHavePropertySet, propertyName);

            if (property != null)
            {
                property.SetValue(objectToHavePropertySet, propertyValue, null);
            }
        }

        /// <summary>
        /// Returns the <c>PropertyInfo</c> object for the given object and the property name
        /// </summary>
        /// <param name="objectToGetProperty"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(Object objectToGetProperty, string propertyName)
        {
            if (objectToGetProperty == null) { throw new ArgumentException("objectToGetProperty was NULL"); }
            if (propertyName == null) { throw new ArgumentException("propertyName was NULL"); }

            PropertyInfo publicPropertyInfo = objectToGetProperty.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (publicPropertyInfo != null)
            {
                return publicPropertyInfo;
            }

            return objectToGetProperty.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}
