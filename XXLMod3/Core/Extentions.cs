using GameManagement;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XXLMod3.Core
{
    public static class Extentions
    {
        public static object InvokeMethod(object obj, string methodName, params object[] methodParams)
        {
            var methodParamTypes = methodParams?.Select(p => p.GetType()).ToArray() ?? new Type[] { };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            MethodInfo method = null;
            var type = obj.GetType();
            while (method == null && type != null)
            {
                method = type.GetMethod(methodName, bindingFlags, Type.DefaultBinder, methodParamTypes, null);
                type = type.BaseType;
            }

            return method?.Invoke(obj, methodParams);
        }
    }
}