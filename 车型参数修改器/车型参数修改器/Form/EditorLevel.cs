using System;

namespace trainTypeEditor.Form
{
    public partial class EditorLevel : System.Windows.Forms.Form
    {
        private readonly Main_form _frm;
        public EditorLevel(Main_form frm,string value)
        {
            InitializeComponent();
            _frm = frm;
            if (value.Equals("禁用"))
            {
                checkBox1.Checked = true;
            }
            else
            {
                textBox1.Text = value;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !checkBox1.Checked;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _frm.SetValue(checkBox1.Checked ? "禁用" : textBox1.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }



    }
}
