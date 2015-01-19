using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace trainTypeEditor.Dto
{
    public class DetectDto : Dto
    {
        private static readonly string _picturePath = ConfigurationManager.AppSettings["PicturePath"];
        private readonly Detect _detect;

        private DetectDto(Detect tt)
        {
            _detect = tt;
        }

        /// <summary>
        ///     车号补缺
        /// </summary>
        public static void Repair(string sourceText, string desText)
        {
            try
            {
                DateTime source = DateTime.Parse(sourceText);
                DateTime des = DateTime.Parse(desText);
                if (source.Equals(des))
                {
                    MessageBox.Show(@"起止时间相等");
                    return;
                }
                //var sourPara = new ObjectParameter("sourceTestTime", typeof(DateTime)) { Value = source };
                //var desPara = new ObjectParameter("destTestTime", typeof(DateTime)) { Value = des };
                //Context.ExecuteFunction("CopyCarNoDestSource", desPara, sourPara);
                if (Context.Detect.FirstOrDefault(m => m.testDateTime.Equals(source)) != null)
                {
                    string sourcePath = _picturePath + @"\" + source.ToString(@"yyyy\\MM");
                    if (!Directory.Exists(sourcePath))
                    {
                        MessageBox.Show(@"文件路径" + sourcePath + @"不存在");
                        return;
                    }
                    string desPath = _picturePath + @"\" + des.ToString(@"yyyy\\MM");
                    string oldValue = source.ToString("yyyyMMdd_HHmmss");
                    string newValue = des.ToString("yyyyMMdd_HHmmss");
                    string[] files = Directory.GetFiles(sourcePath);
                    Directory.CreateDirectory(desPath);
                    foreach (string file in files)
                    {
                        if (file.Contains(oldValue) && file.EndsWith(".jpg", true, CultureInfo.CurrentCulture))
                        {
                            File.Copy(file, file.Replace(oldValue, newValue));
                        }
                    }
                }
                else
                {
                    MessageBox.Show(@"数据库不存在该项：" + source);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     车组号编辑
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="no"></param>
        public static void Editor(string sourceText, string no)
        {
            DateTime source = DateTime.Parse(sourceText);
            Detect detect = Context.Detect.FirstOrDefault(m => m.testDateTime.Equals(source));
            if (detect != null)
            {
                detect.engNum = no;
            }
            Context.SaveChanges();
        }

        public static string GetEngNum(string time)
        {
            DateTime source = DateTime.Parse(time);
            Detect detect = Context.Detect.FirstOrDefault(m => m.testDateTime.Equals(source));
            return detect == null ? "" : detect.engNum;
        }
    }
}