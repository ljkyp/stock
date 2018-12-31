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
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            ComboBox1.SelectedIndex = 0;

            LoadData();

        }

        //Add 버튼
        private void button2_Click(object sender, EventArgs e)
        {

            if (Validation())
            {
                SqlConnection con = Connection.GetConnection();
                con.Open();

                bool status = false;

                if (ComboBox1.SelectedIndex == 0)
                {
                    status = true;
                }

                String cmdStr = "";

                if (IfProductExists(con, textBox1.Text))
                {
                    cmdStr = " UPDATE [commondb].[dbo].[tProducts]" +
                               "  SET [ProductName] = '" + textBox2.Text + "'" +
                               "     ,[ProductStatus] = '" + status + "'" +
                               " WHERE [ProductCode] = '" + textBox1.Text + "'";
                }
                else
                {
                    cmdStr = "insert into [commondb].[dbo].tProducts(ProductCode, ProductName, ProductStatus)" +
                        " Values('" + textBox1.Text + "', '" + textBox2.Text + "', '" + status + "')";
                }

                SqlCommand cmd = new SqlCommand(cmdStr, con);
                cmd.ExecuteNonQuery();

                con.Close();

                LoadData();

                ResetRecord(); 
            }
        }

        private bool IfProductExists( SqlConnection con, String ProductCode) {

            SqlDataAdapter sda = new SqlDataAdapter("Select * from  [commondb].[dbo].tProducts where ProductCode = " + ProductCode, con);
            DataTable dt = new DataTable();

            sda.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        
        public void LoadData()
        {
            SqlConnection con = Connection.GetConnection();
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter("Select * from [commondb].[dbo].tProducts", con);
            DataTable dt = new DataTable();

            sda.Fill(dt);

            dataGridView1.Rows.Clear();

            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();

                dataGridView1.Rows[n].Cells[0].Value = item["ProductCode"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item["ProductName"].ToString();

                if ((bool)item["ProductStatus"])
                {
                    dataGridView1.Rows[n].Cells[2].Value = "Active";
                }
                else
                {
                    dataGridView1.Rows[n].Cells[2].Value = "deActive";
                }

            }
            con.Close();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

            if (dataGridView1.SelectedRows[0].Cells[2].Value.ToString() == "Active")
            {
                ComboBox1.SelectedIndex = 0;
            }
            else
            {
                ComboBox1.SelectedIndex = 1;
            }
            */

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            int sindex = e.RowIndex;

            if(sindex>= 0)
            {
                textBox1.Text = dataGridView1.Rows[sindex].Cells[0].Value.ToString();
                textBox2.Text = dataGridView1.Rows[sindex].Cells[1].Value.ToString();

                if (dataGridView1.Rows[sindex].Cells[2].Value.ToString() == "Active")
                {
                    ComboBox1.SelectedIndex = 0;
                }
                else
                {
                    ComboBox1.SelectedIndex = 1;
                }

                button2.Text = "Update";
            }

            
        }

        //Delete버튼
        private void button1_Click(object sender, EventArgs e)
        {

            if (Validation())
            {
                DialogResult result = MessageBox.Show("do you want delete?", "message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SqlConnection con = Connection.GetConnection();


                    if (IfProductExists(con, textBox1.Text))
                    {
                        con.Open();
                        String cmdStr = " DELETE [commondb].[dbo].[tProducts]" +
                                   " WHERE [ProductCode] = '" + textBox1.Text + "'";
                        SqlCommand cmd = new SqlCommand(cmdStr, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        MessageBox.Show("not found data", "error message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoadData();
                    ResetRecord();
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ResetRecord();
        }



        private void ResetRecord()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            ComboBox1.SelectedIndex = -1;
            button2.Text = "Add";
            errorProvider1.Clear();
            textBox1.Focus();
        }


        private bool Validation()
        {
            bool Result = false;

            if(string.IsNullOrEmpty(textBox1.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(textBox1, "Product Code Required");
            }
            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(textBox2, "Product Name Required");
            }
            else if (ComboBox1.SelectedIndex == -1)
            {
                errorProvider1.Clear();
                errorProvider1.SetError(ComboBox1, "Select Status");
            }
            else
            {
                Result = true;
            }

            return Result;
        }
    }
}
