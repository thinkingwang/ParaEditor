using System;
using System.Configuration;
using System.Data.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace trainTypeEditor.Dto
{
    public class ProfileDetectResultDto : Dto
    {
        private static readonly string _picturePath = ConfigurationManager.AppSettings["ProfilePath"];
        private readonly ProfileDetectResult result;

        private ProfileDetectResultDto(ProfileDetectResult tt)
        {
            result = tt;
        }

        /// <summary>
        /// 外形补缺
        /// </summary>
        /// <param name="thisTimeText"></param>
        /// <param name="lastTimeText"></param>
        public static void Repair(string thisTimeText, string lastTimeText)
        {
            var thisTime = DateTime.Parse(thisTimeText);
            var lastTime = DateTime.Parse(lastTimeText);
            if (thisTime.ToString(CultureInfo.InvariantCulture).Equals(lastTime.ToString(CultureInfo.InvariantCulture)))
            {
                return;
            }
            var sourPara = new ObjectParameter("in_ThisTestDateTime", typeof(DateTime)) { Value = thisTime };
            var desPara = new ObjectParameter("in_LastTestDateTime", typeof(DateTime)) { Value = lastTime };
            var result = new ObjectParameter("out_Result", typeof(int)) { Value = -1 };
            Context.ExecuteFunction("proc_BatchDatafillByLastTime", desPara, sourPara,result);
            switch (Convert.ToInt32(result.Value))
            {
                case 0:
                    MessageBox.Show(@"出错");
                    return;
                case 2:
                    MessageBox.Show(@"ProfileDetectResult表无上次日期数据");
                    return;
                case 3:
                    MessageBox.Show(@"ProfileDetectResult_real表无上次日期数据");
                    return;
            }
            if (Context.ProfileDetectResult.FirstOrDefault(m => m.testDateTime.Equals(lastTime)) != null)
            {
                var oldValue = lastTime.ToString("yyyyMMdd_HHmmss");
                var newValue = thisTime.ToString("yyyyMMdd_HHmmss");
                var sourcePath = _picturePath + @"\" + oldValue;
                if (!Directory.Exists(sourcePath))
                {
                    MessageBox.Show(lastTime + @"外形数据不存在");
                    return;
                }
                var desPath = _picturePath + @"\" + newValue;
                var files = Directory.GetFiles(sourcePath);
                Directory.CreateDirectory(desPath);
                foreach (var file in files)
                {
                    File.Copy(file, file.Replace(oldValue, newValue));
                }
            }
            else
            {
                MessageBox.Show(@"数据库不存在该项：" + lastTime);
            }
        }
    }
}