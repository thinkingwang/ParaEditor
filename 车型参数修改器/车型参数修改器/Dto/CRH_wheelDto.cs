using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using trainTypeEditor.Common;

namespace trainTypeEditor.Dto
{
    public class CRH_wheelDto : Dto
    {
        private readonly CRH_wheel _crhWheel;
        private CRH_wheelDto(CRH_wheel crh)
        {
            _crhWheel = crh;
        }
        public static IEnumerable<CRH_wheel> GetAll()
        {
            return Context.CRH_wheel.ToList();
        }
        public static IEnumerable<string> GetCrhWheelTypes()
        {
            //获取所有车型
            var trainTypes =
                (from v in Context.CRH_wheel select v.trainType).Distinct();
            return trainTypes;
        }

        public static IEnumerable<CRH_wheelDto> GetCrhWheel(string trainType)
        {
            var result = new List<CRH_wheelDto>();
            var data = from v in Context.CRH_wheel
                where v.trainType == trainType
                select v;
            foreach (var crh in data)
            {
                result.Add(new CRH_wheelDto(crh));
            }
            return result;
        }

        public static void Delete(string trainType)
        {
            var data = from v in Context.CRH_wheel
                where v.trainType == trainType
                select v;
            foreach (var crh in data)
            {
                Context.CRH_wheel.DeleteObject(crh);
            }
            Context.SaveChanges();
        }

        public static void Copy(string trainType, string name)
        {
            var data = from v in Context.CRH_wheel
                where v.trainType == trainType
                select v;
            foreach (var crh in data)
            {
                var holds = DeepCopy(crh);
                holds.trainType = name;
                Context.CRH_wheel.AddObject(holds);
            }
            Context.SaveChanges();
        }
         public static void CreateDataBase(IEnumerable<CRH_wheel> data)
        {
            foreach (var threshold in Context.CRH_wheel)
            {
                Context.CRH_wheel.DeleteObject(threshold);
            }
            Context.SaveChanges();
            foreach (var thresholdse in data)
            {
                Context.CRH_wheel.AddObject(thresholdse);
            }
            Context.SaveChanges();
        }
        /// <summary>
        /// 深拷贝功能函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T DeepCopy<T>(T obj)
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

        [ ReadOnly(true), DisplayName(@"车型")]
        public string trainType
        {
            get { return _crhWheel.trainType; }
        }

        [ ReadOnly(true), DisplayName(@"轴序号")]
        public byte axleNo
        {
            get { return _crhWheel.axleNo; }
        }

        [ReadOnly(true), DisplayName(@"轮位置")]
        public wheel wheelNo
        {
            get {return (wheel) _crhWheel.wheelNo; }
        }

        [ DisplayName(@"轴位")]
        public byte axlePos
        {
            get { return _crhWheel.axlePos; }
            set
            {
                _crhWheel.axlePos = value;
                Context.SaveChanges();
            }
        }
        
        [ DisplayName(@"轮位")]
        public byte wheelPos
        {
            get { return _crhWheel.wheelPos; }
            set
            {
                _crhWheel.wheelPos = value;
                Context.SaveChanges();
            }
        }
    }
}