using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace trainTypeEditor.Dto
{
    public class CarListDto:Dto
    {
        private readonly CarList _carList;

        private CarListDto(CarList tt)
        {
            _carList = tt;
        }

        /// <summary>
        /// 车厢列表增加新行
        /// </summary>
        /// <param name="testDateTime"></param>
        public static void Insert(DateTime testDateTime)
        {
            try
            {
                var carNew = new CarList
                {
                    testDateTime = testDateTime,
                    posNo = 0,
                    carNo = "",
                    carNo2 = ""
                };
                var cars = (from v in Context.CarList where v.testDateTime.Equals(testDateTime) orderby v.posNo select v).ToList();
                if (cars.Any())
                {
                    carNew.posNo = Convert.ToByte(cars.ElementAt(cars.Count() - 1).posNo + 1);
                }
                Context.CarList.AddObject(carNew);
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public static void CopyTo(string sourceText, string desText)
        {

            var source = DateTime.Parse(sourceText);
            var des = DateTime.Parse(desText);
            var carSources = from v in Context.CarList where v.testDateTime.Equals(source) select v;
            var cardeses = (from v in Context.CarList where v.testDateTime.Equals(des) select v).ToList();
            foreach (var carList in cardeses)
            {
                Context.CarList.DeleteObject(carList);
            }
            foreach (var carSource in carSources)
            {
                var car = new CarList()
                {
                    testDateTime = des,
                    carNo = carSource.carNo,
                    carNo2 = carSource.carNo2,
                    posNo = carSource.posNo,
                    direction = carSource.direction
                };
                Context.CarList.AddObject(car);
            }
            Context.SaveChanges();
            DetectDto.Repair(sourceText, desText);
        }

        public static IEnumerable<CarListDto> GetCarsByTime(string time)
        {
            var cars = new List<CarListDto>();
            var testDateTime = DateTime.Parse(time);
            var datas = from v in Context.CarList where v.testDateTime.Equals(testDateTime) select v;
            foreach (var carList in datas)
            {
                cars.Add(new CarListDto(carList));
            }
            return cars;
        }

        public static void Delete(string time, int index)
        {
            var testDateTime = DateTime.Parse(time);
            var item = (from v in Context.CarList where v.testDateTime.Equals(testDateTime) select v).ToList().ElementAt(index);
            Context.CarList.DeleteObject(item);
            Context.SaveChanges();
        }

        [DisplayName(@"测试时间"),ReadOnly(true)]
        public DateTime TestDateTime
        {
            get { return _carList.testDateTime; }
            set
            {
                Context.CarList.DeleteObject(_carList);
                Context.SaveChanges();
                _carList.testDateTime = value;
                Context.CarList.AddObject(_carList);
                Context.SaveChanges();
            }
        }

        [DisplayName(@"位置"), ReadOnly(true)]
        public byte posNo
        {
            get { return _carList.posNo; }
            set
            {
                Context.CarList.DeleteObject(_carList);
                Context.SaveChanges();
                _carList.posNo = value;
                Context.CarList.AddObject(_carList);
                Context.SaveChanges();
            }
        }

        [DisplayName(@"车厢号")]
        public string carNo
        {
            get { return _carList.carNo; }
            set
            {
                Context.CarList.DeleteObject(_carList);
                Context.SaveChanges();
                _carList.carNo = value;
                Context.CarList.AddObject(_carList);
                Context.SaveChanges();
            }
        }

        [DisplayName(@"正方向")]
        public bool direction
        {
            get { return _carList.direction; }
            set
            {
                _carList.direction = value;
                Context.SaveChanges();
            }
        }
    }
}