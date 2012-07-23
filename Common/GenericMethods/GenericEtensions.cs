using System;
using System.Linq;

namespace GenericMethods
{
    public static class GenericEtensions
    {
        /// <summary>
        /// Will check if the object given inherits from a given generic Interface.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool DoesObjectInheritFromGenericInterface(this object item, Type t)
        {
            if (item == null || t == null) return false;
            if (!t.IsGenericType) return false;
            return item.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == t);
        }

        public static bool DoesObjectInheritFromGenericBaseClass(this object item, Type t)
        {
            return DoesObjectInheritFromGenericBase(item.GetType(), t);
        }

        private static bool DoesObjectInheritFromGenericBase(Type objType, Type baseType)
        {
            if(objType == null || baseType == null) return false;
            if (objType == baseType) return true;

            Type newObjType = objType.BaseType;
            return DoesObjectInheritFromGenericBase(newObjType, baseType);
        }
    }
}
