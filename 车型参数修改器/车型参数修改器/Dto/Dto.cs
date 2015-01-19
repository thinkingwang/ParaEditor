using System;
using System.Configuration;
using System.Reflection;

namespace trainTypeEditor.Dto
{
    public class Dto
    {
        protected static readonly thresholdsContext Context; 
        static Dto()
        {
            //Context = new thresholdsContext(ip);
            Context = new thresholdsContext(string.Format(ConfigurationManager.ConnectionStrings["thresholdsContext"].ConnectionString,"Password=sa123"));
            
        }
        /// <summary>
        /// 深拷贝功能函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected static T DeepCopy<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}