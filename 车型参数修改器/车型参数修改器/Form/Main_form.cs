using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using trainTypeEditor.Dto;

namespace trainTypeEditor.Form
{
    public partial class Main_form : System.Windows.Forms.Form
    {
        public Main_form()
        {
            InitializeComponent();
            thresholds_bn.BindingSource = thresholds_bs;
            thresholds_dgv.DataSource = thresholds_bs;
            crh_wheel_bn.BindingSource = crh_wheel_bs;
            crh_wheel_dgv.DataSource = crh_wheel_bs;
            trainType_dgv.DataSource = trainType_bs;
            trainType_bn.BindingSource = trainType_bs;
            carList_dgv.DataSource = carList_bs;
            carList_bn.BindingSource = carList_bs;
            import_ofd.AddExtension = true;
            import_ofd.DefaultExt = ".json";
            import_ofd.Filter = @"bin files (*.bin)|*.bin|All files (*.*)|*.*";
            export_sfd.DefaultExt = ".json";
            export_sfd.AddExtension = true;
            export_sfd.Filter = @"bin files (*.bin)|*.bin|All files (*.*)|*.*";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshWheelType();
            RefreshTrainType();
            trainType_bs.DataSource = TrainTypeDto.GetAll();
            carListSourceTime_tbx.Enabled = false;
            CarListTime_label.Enabled = false;
            carListHanleSubmit_btn.Enabled = false;
        }

        /// <summary>
        /// 点击combox，切换车型是调用的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void thresholds_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            thresholds_bs.DataSource = thresholdsDto.GetThresholds(thresholds_cbx.Text);
            //索引0,1,2列的单元格的背景色为淡蓝色
            thresholds_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引1列的单元格的背景色为淡蓝色
            thresholds_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
            //索引9列的单元格的背景色为淡蓝色
            thresholds_dgv.Columns[9].DefaultCellStyle.BackColor = Color.Aqua;
            if (crh_wheel_cbx.Items.Contains(thresholds_cbx.Text))
            {
                crh_wheel_cbx.Text = thresholds_cbx.Text;
            }
            else
            {
                crh_wheel_dgv.Rows.Clear();
                crh_wheel_cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// 动车车轮选择改变处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void crh_wheel_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            crh_wheel_bs.DataSource = CRH_wheelDto.GetCrhWheel(crh_wheel_cbx.Text);
            //索引9列的单元格的背景色为淡蓝色
            crh_wheel_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引9列的单元格的背景色为淡蓝色
            crh_wheel_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
            //索引9列的单元格的背景色为淡蓝色
            crh_wheel_dgv.Columns[2].DefaultCellStyle.BackColor = Color.Aqua;

        }

        /// <summary>
        /// “修改”对话框关闭时调用，完成修改值的操作
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            thresholds_dgv.CurrentCell.Value = value;
        }

        /// <summary>
        /// 在修改上下限时弹出修改对话框，“标准值”直接点击单元格修改即可
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void thresholds_dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex <= 2 || e.ColumnIndex >= 9)
            {
                return;
            }
            var levelForm = new EditorLevel(this, thresholds_dgv.CurrentCell.Value.ToString());
            levelForm.ShowDialog();
            e.Cancel = true;
        }

        /// <summary>
        /// 删除车型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteThresholds_button_Click(object sender, EventArgs e)
        {
            var result =  MessageBox.Show(@"确认删除车型" + thresholds_cbx.Text, @"确认对话框",MessageBoxButtons.YesNo );
            if (result == DialogResult.No)
            {
                return;
            }
            if (string.IsNullOrEmpty(thresholds_cbx.Text)||thresholds_cbx.Text.Equals("default"))
            {
                return;
            }
            thresholdsDto.Delete(thresholds_cbx.Text);
            RefreshTrainType();
            RefreshWheelType();
        }

        /// <summary>
        /// 增加车型功能函数，添加成功后显示该车型对应的所有参数列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addThresholds_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(thresholds_tbx.Text))
            {
                return;
            }
            thresholdsDto.Copy(thresholds_cbx.Text, thresholds_tbx.Text);
            RefreshTrainType(thresholds_tbx.Text);
            RefreshWheelType(thresholds_tbx.Text);
        }

        /// <summary>
        /// 刷新车型下拉框
        /// </summary>
        private void RefreshTrainType()
        {
            //获取所有车型
            var trainTypes = thresholdsDto.GetThresholdsTypes();
            thresholds_cbx.Items.Clear();
            foreach (var trainType in trainTypes)
            {
                thresholds_cbx.Items.Add(trainType);
            }
            thresholds_cbx.SelectedIndex = 0;
        }

        /// <summary>
        /// 刷新动车轮对下拉框
        /// </summary>
        private void RefreshWheelType()
        {
            //获取所有车型
            var trainWheels = CRH_wheelDto.GetCrhWheelTypes();
            crh_wheel_cbx.Items.Clear();
            foreach (var trainType in trainWheels)
            {
                crh_wheel_cbx.Items.Add(trainType);
            }
            crh_wheel_cbx.SelectedIndex = 0;
        }

        /// <summary>
        /// 刷新车型下拉框,并设置默认选择项
        /// </summary>
        /// <param name="selectedText">选择项文本</param>
        private void RefreshTrainType(string selectedText)
        {
            //获取所有车型
            var trainTypes = thresholdsDto.GetThresholdsTypes();
            thresholds_cbx.Items.Clear();
            foreach (var trainType in trainTypes)
            {
                thresholds_cbx.Items.Add(trainType);
            }
            thresholds_cbx.SelectedText = selectedText;
        }

        /// <summary>
        /// 刷新动车轮对下拉框,并设置默认选择项
        /// </summary>
        /// <param name="selectedText">选择项文本</param>
        private void RefreshWheelType(string selectedText)
        {
            //获取所有车型
            var trainWheels = CRH_wheelDto.GetCrhWheelTypes();
            crh_wheel_cbx.Items.Clear();
            foreach (var trainType in trainWheels)
            {
                crh_wheel_cbx.Items.Add(trainType);
            }
            crh_wheel_cbx.SelectedText = selectedText;
        }
      

        /// <summary>
        /// 给单元格赋值时，如果产生异常就会进入这个函数，撤销所做的操作，并把异常信息反馈给用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show(e.Exception.Message);
        }


        /// <summary>
        /// 导入操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void import_button_Click(object sender, EventArgs e)
        {
            var reslut = import_ofd.ShowDialog();
            if (reslut == DialogResult.OK)
            {
                var fileName = import_ofd.FileName;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                var data = (ArrayList) formatter.Deserialize(stream);
                thresholdsDto.CreateDataBase(data);
                stream.Close();
                RefreshTrainType();
                RefreshWheelType();
            }
        }

        /// <summary>
        /// 导出操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void export_button_Click(object sender, EventArgs e)
        {
            var result = export_sfd.ShowDialog();
            var saveFile = new ArrayList {thresholdsDto.GetAll(), CRH_wheelDto.GetAll()};
            if (result == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(export_sfd.FileName, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                formatter.Serialize(stream, saveFile);
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
        /// 增加车型轮对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCrh_wheel_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(crh_wheel_tbx.Text))
            {
                return;
            }
            CRH_wheelDto.Copy(crh_wheel_cbx.Text, crh_wheel_tbx.Text);
            RefreshWheelType(crh_wheel_tbx.Text);
        }

        /// <summary>
        /// 删除车型轮对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteCrh_wheel_btn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(@"确认删除车型轮对" + crh_wheel_cbx.Text, @"确认对话框", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            if (string.IsNullOrEmpty(crh_wheel_cbx.Text) || crh_wheel_cbx.Text.Equals("default"))
            {
                return;
            }
            CRH_wheelDto.Delete(thresholds_cbx.Text);
            RefreshWheelType(crh_wheel_tbx.Text);
        }

        /// <summary>
        /// 增加新行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewLine_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(paramName_tbx.Text))
            {
                MessageBox.Show(@"填写参数名");
                return;
            }
            thresholdsDto.NewThresholds(thresholds_cbx.Text,paramName_tbx.Text);
            thresholds_bs.DataSource = thresholdsDto.GetThresholds(thresholds_cbx.Text);

        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delete_btn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in thresholds_dgv.SelectedRows)
            {
                thresholdsDto.DeleteThresholds(thresholds_cbx.Text,row.Index);
            }
            RefreshTrainType();
            RefreshWheelType();
        }

        /// <summary>
        /// 车型表增加一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void add_btn_Click(object sender, EventArgs e)
        {
            TrainTypeDto.NewTrainType();
            trainType_bs.DataSource = TrainTypeDto.GetAll();
        }

        /// <summary>
        /// 车型表删除一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainType_delete_btn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in thresholds_dgv.SelectedRows)
            {
                TrainTypeDto.Delete(row.Index);
            }
            trainType_bs.DataSource = TrainTypeDto.GetAll();
        }

        /// <summary>
        /// 外形补缺
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void profile_submit_Click(object sender, EventArgs e)
        {
            ProfileDetectResultDto.Repair(profileExcept_tbx.Text,profileNormal_tbx.Text);
        }


        private void carListAdd_btn_Click(object sender, EventArgs e)
        {
            if (carList_dgv.ReadOnly)
            {
                MessageBox.Show(@"不允许编辑");
                return;
            }
            DateTime time;
            if (!DateTime.TryParse(carListDesTime_tbx.Text, out time))
            {
                throw new Exception("时间格式不正确");
            }
            CarListDto.Insert(time);
            carList_bs.DataSource = CarListDto.GetCarsByTime(carListDesTime_tbx.Text);
            //索引0,1,2列的单元格的背景色为淡蓝色
            carList_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引1列的单元格的背景色为淡蓝色
            carList_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
        }

        private void carListDelete_btn_Click(object sender, EventArgs e)
        {
            if (carList_dgv.ReadOnly)
            {
                MessageBox.Show(@"不允许编辑");
                return;
            }
            foreach (DataGridViewRow selectedRow in carList_dgv.SelectedRows)
            {
                CarListDto.Delete(selectedRow.Cells[0].Value.ToString(), selectedRow.Index);
            }
            carList_bs.DataSource = CarListDto.GetCarsByTime(carListDesTime_tbx.Text);
            //索引0,1,2列的单元格的背景色为淡蓝色
            carList_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引1列的单元格的背景色为淡蓝色
            carList_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
        }

        /// <summary>
        /// 刷新车厢表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListRefresh_btn_Click(object sender, EventArgs e)
        {
            carList_bs.DataSource = CarListDto.GetCarsByTime(carListDesTime_tbx.Text);
            //索引0,1,2列的单元格的背景色为淡蓝色
            carList_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引1列的单元格的背景色为淡蓝色
            carList_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
            carListNo_tbx.Text = DetectDto.GetEngNum(carListDesTime_tbx.Text);
        }

        /// <summary>
        /// 提交更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListHanleSubmit_btn_Click(object sender, EventArgs e)
        {
            if (!copyFromExistData_cbx.Checked || string.IsNullOrEmpty(carListSourceTime_tbx.Text))
            {
                return;
            }
            CarListDto.CopyTo(carListSourceTime_tbx.Text, carListDesTime_tbx.Text);
            carList_bs.DataSource = CarListDto.GetCarsByTime(carListDesTime_tbx.Text);
            //索引0,1,2列的单元格的背景色为淡蓝色
            carList_dgv.Columns[0].DefaultCellStyle.BackColor = Color.Aqua;
            //索引1列的单元格的背景色为淡蓝色
            carList_dgv.Columns[1].DefaultCellStyle.BackColor = Color.Aqua;
        }

        /// <summary>
        /// 是否从现有数据库中获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyFromExistData_cbx_CheckedChanged(object sender, EventArgs e)
        {
            carListSourceTime_tbx.Enabled = copyFromExistData_cbx.Checked;
            CarListTime_label.Enabled = copyFromExistData_cbx.Checked;
            carListHanleSubmit_btn.Enabled = copyFromExistData_cbx.Checked;
        }

        /// <summary>
        /// 源时刻编辑框失去焦点处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListSourceTime_tbx_Leave(object sender, EventArgs e)
        {
            DateTime time;
            if (DateTime.TryParse(carListSourceTime_tbx.Text, out time))
            {
                carListNo_tbx.Text = DetectDto.GetEngNum(carListSourceTime_tbx.Text);
                DetectDto.Editor(carListDesTime_tbx.Text, carListNo_tbx.Text);
            }
        }
  

        private void carNoModify_btn_Click(object sender, EventArgs e)
        {
            DateTime time;
            if (DateTime.TryParse(carListDesTime_tbx.Text, out time))
            {
                DetectDto.Editor(carListDesTime_tbx.Text, carListNo_tbx.Text);
            }
        }
    }
}
