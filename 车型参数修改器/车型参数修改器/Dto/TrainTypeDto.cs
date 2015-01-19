using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace trainTypeEditor.Dto
{
    public class TrainTypeDto : Dto
    {
        private readonly TrainType _trainType;
        private TrainTypeDto(TrainType tt)
        {
            _trainType = tt;
        }
        public static IEnumerable<TrainTypeDto> GetAll()
        {
            var result = new List<TrainTypeDto>();
            foreach (var thresholdse in Context.TrainType)
            {
                result.Add(new TrainTypeDto(thresholdse));
            }
            return result;
        }

        public static void Delete(int index)
        {
            var data = Context.TrainType.ToList().ElementAt(index);
            Context.TrainType.DeleteObject(data);
            Context.SaveChanges();
        }
        public static void NewTrainType()
        {
            Context.TrainType.AddObject(new TrainType() {trainType1 = "", format = "", trainNoFrom = 0, trainNoTo = 0});
            Context.SaveChanges();
        }


        [DisplayName(@"车型")]
        public string trainType1
        {
            get { return _trainType.trainType1; }
            set
            {
                _trainType.trainType1 = value;
                Context.SaveChanges();
            }
        }

        [DisplayName(@"车号开始数字")]
        public int trainNoFrom
        {
            get { return _trainType.trainNoFrom; }
            set
            {
                Context.TrainType.DeleteObject(_trainType);
                Context.SaveChanges();
                _trainType.trainNoFrom = value; 
                Context.TrainType.AddObject(_trainType);
                Context.SaveChanges();
            }
        }

        [DisplayName(@"车号结束数字")]
        public int trainNoTo
        {
            get { return _trainType.trainNoTo; }
            set
            {
                Context.TrainType.DeleteObject(_trainType);
                Context.SaveChanges();
                _trainType.trainNoTo = value;
                Context.TrainType.AddObject(_trainType);
                Context.SaveChanges();
            }
        }

        [DisplayName(@"车型显示格式")]
        public string format
        {
            get { return _trainType.format; }
            set
            {
                _trainType.format = value;
                Context.SaveChanges();
            }
        }

    }
}