using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stock
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Clear();
            textBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //To-do check login and password
            string strConn = "Data Source=124.35.1.165,1516;User ID=svc1adm; Password=#ahnDBadm@2013;";
            String strSql = "Select* from[commondb].[dbo].StaffCode_ct where Stf_Code = '" + textBox1.Text + "' and password = '" + textBox2.Text + "'";

            SqlConnection con = new SqlConnection(strConn);

            SqlDataAdapter sda = new SqlDataAdapter("Select * from [commondb].[dbo].StaffCode_ct as aa  where aa.[STF_CODE] = '" + textBox1.Text + "' ", con);
            DataSet dt = new DataSet();

            sda.Fill(dt, "StaffCode");

          
            if (dt.Tables[0].Rows.Count == 1)
            {
                String dpwd = dt.Tables[0].Rows[0]["password"].ToString();
                String dpwd2 = dt.Tables["StaffCode"].Rows[0]["password"].ToString();
                String tpwd = textBox2.Text;

                if (dpwd == tpwd)
                {
                    this.Hide();

                    StockMain main = new StockMain();
                    main.Show();
                }
                else{
                    MessageBox.Show("로그인 아이디 또는 패스워드가 틀립니다. ", "에러 메세지 박스", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("로그인 아이디 또는 패스워드가 틀립니다. ", "에러 메세지 박스2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button1_Click(sender, e);
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(sender, e);
            }
            else
            {
                return;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.Focus();
            }
            else
            {
                return;
            }
        }
    }
}
