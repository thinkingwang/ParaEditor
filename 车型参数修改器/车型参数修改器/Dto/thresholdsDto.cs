using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;

namespace trainTypeEditor.Dto
{
    public class thresholdsDto:Dto
    {
        private readonly thresholds _thresholds;
        static thresholdsDto()
        {
        }
        private thresholdsDto(thresholds th)
        {
            _thresholds = th;
        }

        /// <summary>
        /// 导入外部数据，重写数据库数据
        /// </summary>
        /// <param name="data"></param>
        public static void CreateDataBase(ArrayList data)
        {
            foreach (var threshold in Context.thresholds)
            {
                Context.thresholds.DeleteObject(threshold);
            }
            Context.SaveChanges();
            foreach (var thresholdse in (List<thresholds>)data[0])
            {
                Context.thresholds.AddObject(thresholdse);
            }
            CRH_wheelDto.CreateDataBase((List<CRH_wheel>)data[1]);
            Context.SaveChanges();
        }

        /// <summary>
        /// 获得门限表所有数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<thresholds> GetAll()
        {
            return Context.thresholds.ToList();
        }

        /// <summary>
        /// 获得门限表车型列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetThresholdsTypes()
        {
            //获取所有车型
            var trainTypes =
                (from v in Context.thresholds select v.trainType).Distinct();
            return trainTypes;
        }

        /// <summary>
        /// 获得门限表中指定车型的所有数据
        /// </summary>
        /// <param name="trainType"></param>
        /// <returns></returns>
        public static IEnumerable<thresholdsDto> GetThresholds(string trainType)
        {
            var result = new List<thresholdsDto>();
            var data = from v in Context.thresholds
                       where v.trainType == trainType
                       select v;
            foreach (var thresholdse in data)
            {
                result.Add(new thresholdsDto(thresholdse));
            }
            return result;
        }

        /// <summary>
        /// 在门限表中删除指定车型
        /// </summary>
        /// <param name="trainType"></param>
        public static void Delete(string trainType)
        {
            CRH_wheelDto.Delete(trainType);
            var data = from v in Context.thresholds
                       where v.trainType == trainType
                       select v;
            foreach (var thresholdse in data)
            {
                Context.thresholds.DeleteObject(thresholdse);
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// 新建指定车型
        /// </summary>
        /// <param name="trainType"></param>
        /// <param name="name"></param>
        public static void NewThresholds(string trainType,string name)
        {
            var item = DeepCopy(Context.thresholds.FirstOrDefault(m => m.trainType.Equals(trainType)));
            if (item == null)
            {
                return;
            }
            item.name = name;
            Context.thresholds.AddObject(item);
            Context.SaveChanges();
        }

        /// <summary>
        /// 删除指定车型某行数据
        /// </summary>
        /// <param name="trainType"></param>
        /// <param name="index"></param>
        public static void DeleteThresholds(string trainType,int index)
        {
            var data = from v in Context.thresholds
                       where v.trainType == trainType
                       select v;
            Context.thresholds.DeleteObject(data.ToList().ElementAt(index));
            Context.SaveChanges();
        }

        /// <summary>
        /// 复制指定车型数据，并插入到门限表中
        /// </summary>
        /// <param name="trainType"></param>
        /// <param name="name"></param>
        public static void Copy(string trainType,string name)
        {
            CRH_wheelDto.Copy(trainType, name);
            var data = from v in Context.thresholds
                       where v.trainType == trainType
                       select v;
            foreach (var thresholdse in data)
            {
                var holds = DeepCopy(thresholdse);
                holds.trainType = name;
                Context.thresholds.AddObject(holds);
            }
            Context.SaveChanges();
        }
       

        [Category("车型与参数名"), Description("车型与参数名"), ReadOnly(true), DisplayName(@"车型")]
        public string trainType
        {
            get { return _thresholds.trainType; }
            set
            {
                _thresholds.trainType = value;
            }
        }

        [Category("车型与参数名"), Description("车型与参数名"), ReadOnly(true), DisplayName(@"参数描述")]
        public string ParamName
        {
            get
            {
                var showName = ConfigurationManager.AppSettings[_thresholds.name];
                if (string.IsNullOrEmpty(showName))
                {
                    return _thresholds.name;
                }
                return showName;
            }
        }

        [DisplayName(@"标准值")]
        public decimal? standard
        {
            get { return _thresholds.standard; }
            set
            {
                _thresholds.standard = value;
                if (_thresholds.up_level3 < standard)
                {
                    Up3 = "禁用";
                }
                if (_thresholds.up_level2 < standard)
                {
                    Up2 = "禁用";
                }
                if (_thresholds.up_level1 < standard)
                {
                    Up1 = "禁用";
                }
                if (_thresholds.low_level3 > standard)
                {
                    Low3 = "禁用";
                }
                if (_thresholds.low_level2 > standard)
                {
                    Low2 = "禁用";
                }
                if (_thresholds.low_level1 > standard)
                {
                    Low1 = "禁用";
                }
                Context.SaveChanges();
            }
        }

        [DisplayName("上限三级")]
        public string Up3
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.up_level3 - 2000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.up_level3.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.up_level3 = Convert.ToDecimal(2000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter < standard)
                    {
                        throw new Exception("上限值必须大于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.up_level2 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.up_level2) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.up_level1 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.up_level1))
                    {
                        _thresholds.up_level3 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("上限三级值必须小于等于上限二级和上限一级，请重新设定");
                    }
                }
                Context.SaveChanges();
            }
        }

        [DisplayName("上限二级")]
        public string Up2
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.up_level2 - 2000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.up_level2.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.up_level2 = Convert.ToDecimal(2000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter < standard)
                    {
                        throw new Exception("上限值必须大于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.up_level3 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.up_level3) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.up_level1 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.up_level1))
                    {
                        if (Math.Abs(Convert.ToDecimal(_thresholds.up_level2 - 2000)) < Convert.ToDecimal(0.01))
                        {
                            if (desc.Contains("预警"))
                            {
                                desc = desc.Remove(0, desc.IndexOf("预警") + 3);
                            }
                        }
                        else
                        {
                            if (desc.Contains("预警"))
                            {
                                desc = desc.Replace(_thresholds.up_level2.ToString(), value);
                            }
                            else
                            {
                                desc = desc.Insert(0, "大于" + value + "预警,");
                            }
                        }
                        _thresholds.up_level2 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("上限二级值必须大于等于上限三级，小于等于上限一级，请重新设定");
                    }
                }
                SetDesc();
                Context.SaveChanges();
            }
        }

        [DisplayName(@"上限一级")]
        public string Up1
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.up_level1 - 2000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.up_level1.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.up_level1 = Convert.ToDecimal(2000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter < standard)
                    {
                        throw new Exception("上限值必须大于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.up_level2 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.up_level2) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.up_level3 - 2000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.up_level3))
                    {
                        _thresholds.up_level1 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("上限一级值必须大于等于上限三级和上限二级，请重新设定");
                    }
                }
                SetDesc();
                Context.SaveChanges();
            }
        }

        [DisplayName(@"下限三级")]
        public string Low3
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.low_level3 + 1000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.low_level3.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.low_level3 = Convert.ToDecimal(-1000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter > standard)
                    {
                        throw new Exception("下限值必须小于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.low_level2 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.low_level2) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.low_level1 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.low_level1))
                    {
                        _thresholds.low_level3 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("下限三级值必须大于等于下限二级和下限一级，请重新设定");
                    }
                }
                Context.SaveChanges();
            }
        }

        [DisplayName(@"下限二级")]
        public string Low2
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.low_level2 + 1000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.low_level2.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.low_level2 = Convert.ToDecimal(-1000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter > standard)
                    {
                        throw new Exception("下限值必须小于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.low_level3 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.low_level3) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.low_level1 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter > _thresholds.low_level1))
                    {
                        _thresholds.low_level2 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("下限二级值必须大于等于下限一级小于等于下限三级，请重新设定");
                    }
                }
                SetDesc();
                Context.SaveChanges();
            }
        }

        [DisplayName(@"下限一级")]
        public string Low1
        {
            get
            {
                if (Math.Abs(Convert.ToDecimal(_thresholds.low_level1 + 1000)) < Convert.ToDecimal(0.01))
                {
                    return "禁用";
                }
                return _thresholds.low_level1.ToString();
            }
            set
            {
                if (value.Equals("禁用"))
                {
                    _thresholds.low_level1 = Convert.ToDecimal(-1000);
                }
                else
                {
                    decimal valueInter = Convert.ToDecimal(value);
                    if (valueInter > standard)
                    {
                        throw new Exception("下限值必须小于标准值，请重新设定");
                    }
                    if ((Math.Abs(Convert.ToDecimal(_thresholds.low_level2 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.low_level2) &&
                        (Math.Abs(Convert.ToDecimal(_thresholds.low_level3 + 1000)) < Convert.ToDecimal(0.01) ||
                         valueInter < _thresholds.low_level3))
                    {
                        _thresholds.low_level1 = Convert.ToDecimal(value);
                    }
                    else
                    {
                        throw new Exception("下限一级值必须小于等于下限二级和下限三级，请重新设定");
                    }
                }
                SetDesc();
                Context.SaveChanges();
            }
        }

        [DisplayName(@"描述"), ReadOnly(true)]
        public string desc
        {
            get { return _thresholds.desc; }
            set
            {
                _thresholds.desc = value;
                Context.SaveChanges();
            }
        }

        private void SetDesc()
        {
            if (!Up1.Equals("禁用") && !Up2.Equals("禁用") && !Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm或小于{1}mm预警，大于{2}mm或小于{3}mm报警",
                    Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (!Up1.Equals("禁用") && Up2.Equals("禁用") && !Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("小于{0}mm预警，大于{1}mm或小于{2}mm报警",
                    Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && !Up2.Equals("禁用") && !Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm或小于{1}mm预警，小于{2}mm报警", Convert.ToString(_thresholds.up_level2),
                    Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && Up2.Equals("禁用") && !Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("小于{0}mm预警，小于{1}mm报警", Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }

            if (!Up1.Equals("禁用") && !Up2.Equals("禁用") && Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm预警，大于{1}mm或小于{2}mm报警",
                    Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (!Up1.Equals("禁用") && !Up2.Equals("禁用") && !Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm或小于{1}mm预警，大于{2}mm报警",
                    Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""));
            }
            if (!Up1.Equals("禁用") && !Up2.Equals("禁用") && Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm预警，大于{1}mm报警", Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""));
            }


            if (!Up1.Equals("禁用") && Up2.Equals("禁用") && Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm或小于{1}mm报警", Convert.ToString(_thresholds.up_level1).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (!Up1.Equals("禁用") && Up2.Equals("禁用") && !Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("小于{0}mm预警，大于{1}mm报警", Convert.ToString(_thresholds.low_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.up_level1).Replace(".00", ""));
            }
            if (!Up1.Equals("禁用") && Up2.Equals("禁用") && Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm报警", Convert.ToString(_thresholds.up_level1).Replace(".00", ""));
            }


            if (Up1.Equals("禁用") && !Up2.Equals("禁用") && Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm预警，小于{1}mm报警", Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && !Up2.Equals("禁用") && !Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm或小于{1}mm预警", Convert.ToString(_thresholds.up_level2).Replace(".00", ""),
                    Convert.ToString(_thresholds.low_level2).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && !Up2.Equals("禁用") && Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("大于{0}mm预警", Convert.ToString(_thresholds.up_level2).Replace(".00", ""));
            }


            if (Up1.Equals("禁用") && Up2.Equals("禁用") && Low2.Equals("禁用") && !Low1.Equals("禁用"))
            {
                desc = string.Format("小于{0}mm报警", Convert.ToString(_thresholds.low_level1).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && Up2.Equals("禁用") && !Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = string.Format("小于{0}mm预警", Convert.ToString(_thresholds.low_level2).Replace(".00", ""));
            }
            if (Up1.Equals("禁用") && Up2.Equals("禁用") && Low2.Equals("禁用") && Low1.Equals("禁用"))
            {
                desc = "";
            }
        }
    }
}