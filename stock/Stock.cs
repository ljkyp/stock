using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace stock
{
    public partial class Stock : Form
    {
        public object ComboBox1 { get; private set; }

        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            this.ActiveControl = dateTimePicker1;
            comboBox1.SelectedIndex = 0;
            LoadData();
            Search();
        }

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Focus();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Text = dgview.SelectedRows[0].Cells[0].Value.ToString();
                    textBox2.Text = dgview.SelectedRows[0].Cells[1].Value.ToString();
                    this.dgview.Visible = false;
                    textBox3.Focus();
                }
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(textBox2.Text))
                {
                    textBox3.Focus();
                }
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(textBox3.Text))
                {
                    comboBox1.Focus();
                }
            }
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (comboBox1.SelectedIndex > -1)
                {
                    button1.Focus();
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*
            if(Char.IsDigit(e.KeyChar) & e.KeyChar != Convert.ToInt32(Keys.Back))
            {
                e.Handled = true;
            }
            */
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }


        private void ResetRecord()
        {
            dateTimePicker1.Value = DateTime.Now;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            button1.Text = "Add";
            dateTimePicker1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ResetRecord();
        }

        private bool Validation()
        {
            bool Result = false;

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(textBox1, "Product Code Required");
            }
            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(textBox2, "Product Name Required");
            }
            else if (string.IsNullOrEmpty(textBox3.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(textBox3, "Qualtity Required");
            }
            else if (comboBox1.SelectedIndex == -1)
            {
                errorProvider1.Clear();
                errorProvider1.SetError(comboBox1, "Select Status");
            }
            else
            {
                Result = true;
            }

            return Result;
        }

        private bool IfProductExists(SqlConnection con, String ProductCode)
        {

            SqlDataAdapter sda = new SqlDataAdapter("Select * from  [commondb].[dbo].tStock where ProductCode = " + ProductCode, con);
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

        private void button1_Click(object sender, EventArgs e)
        {

            if (Validation())
            {
                SqlConnection con = Connection.GetConnection();
                con.Open();

                bool status = false;

                if (comboBox1.SelectedIndex == 0)
                {
                    status = true;
                }

                String cmdStr = "";

                if (IfProductExists(con, textBox1.Text))
                {
                    cmdStr = " UPDATE [commondb].[dbo].[tStock]" +
                               "  SET [ProductName] = '" + textBox2.Text + "'" +
                               "     ,[Quantity] = " + textBox3.Text +
                               "     ,[TransDate] = '" + dateTimePicker1.Value.ToString("yyyy/MM/dd") + "'" +
                               "     ,[ProductStatus] = " + status +
                               " WHERE [ProductCode] = " + textBox1.Text;
                }
                else
                {
                    cmdStr = "insert into [commondb].[dbo].tStock(ProductCode, ProductName, Quantity, TransDate, ProductStatus)" +
                        " Values(" + textBox1.Text + ", '" + textBox2.Text + "', " + textBox3.Text + ",'" + dateTimePicker1.Value.ToString("yyyy/MM/dd") + "', '" + status + "')";
                }

                SqlCommand cmd = new SqlCommand(cmdStr, con);
                cmd.ExecuteNonQuery();

                con.Close();

                LoadData();

                ResetRecord();
            }
        }

        public void LoadData()
        {
            SqlConnection con = Connection.GetConnection();
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter("Select * from [commondb].[dbo].tStock", con);
            DataTable dt = new DataTable();

            sda.Fill(dt);

            dataGridView1.Rows.Clear();

            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();

                dataGridView1.Rows[n].Cells[0].Value = n + 1;
                dataGridView1.Rows[n].Cells[1].Value = item["ProductCode"].ToString();
                dataGridView1.Rows[n].Cells[2].Value = item["ProductName"].ToString();
                dataGridView1.Rows[n].Cells[3].Value = float.Parse(item["Quantity"].ToString());
                dataGridView1.Rows[n].Cells[4].Value = Convert.ToDateTime(item["TransDate"].ToString()).ToString("yyyy/MM/dd");
                dataGridView1.Rows[n].Cells[4].Value = DateTime.Parse(item["TransDate"].ToString()).ToString("yyyy/MM/dd"); //위와 동일

                if ((bool)item["ProductStatus"])
                {
                    dataGridView1.Rows[n].Cells[5].Value = "Active";
                }
                else
                {
                    dataGridView1.Rows[n].Cells[5].Value = "deActive";
                }

            }

            if (dataGridView1.Rows.Count > 0)
            {
                label7.Text = dataGridView1.Rows.Count.ToString();

                float dtc = 0;

                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    dtc += float.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());
                    label9.Text = dtc.ToString();
                }
            }
            else
            {
                label7.Text = "0";
                label9.Text = "0";
            }
            con.Close();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        private void button2_Click(object sender, EventArgs e)
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
                        String cmdStr = " DELETE [commondb].[dbo].[tStock]" +
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

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            textBox1.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            //dateTimePicker1.Value = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells[4].Value.ToString());

            if (dataGridView1.SelectedRows[0].Cells[5].Value.ToString() == "Active")
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex = 1;
            }

            button1.Text = "Update";

        }

        bool change = true; 
        private void proCode_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
            if (change) 
            { 
                change = false; 
                textBox1.Text = dgview.SelectedRows[0].Cells[0].Value.ToString(); 
                textBox2.Text = dgview.SelectedRows[0].Cells[1].Value.ToString(); 
                this.dgview.Visible = false; 
                textBox3.Focus(); 
                change = true; 
            } 
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (textBox1.Text.Length > 0 & change)
            {

                this.dgview.Visible = true;
                dgview.BringToFront();
                Search(150, 105, 430, 200, "Pro Code,Pro Name", "100,0");
                this.dgview.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.proCode_MouseDoubleClick);

                SqlConnection con = Connection.GetConnection();
                SqlDataAdapter sda = new SqlDataAdapter("Select * from [commondb].[dbo].tProducts Where productCode like '%" + textBox1.Text + "%'", con);
                DataTable dt = new DataTable();

                sda.Fill(dt);

                //dgview.DataSource = dt; //가능한데 이미 컬럼이 만들어지면 같은 컬럼이 또 생기기 때문에 필드별로 아래처럼 값을 넣어준다. 

                dgview.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int n = dgview.Rows.Add();
                    dgview.Rows[n].Cells[0].Value = row["ProductCode"].ToString();
                    dgview.Rows[n].Cells[1].Value = row["ProductName"].ToString();
                }
            }
            else
            {
                dgview.Visible = false;
            }
        }

        private DataGridView dgview;
        private DataGridViewTextBoxColumn dgviewcol1;
        private DataGridViewTextBoxColumn dgviewcol2;


        void Search()
        {
            dgview = new DataGridView();
            dgviewcol1 = new DataGridViewTextBoxColumn();
            dgviewcol2 = new DataGridViewTextBoxColumn();
            this.dgview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.dgviewcol1, this.dgviewcol2 });
            this.dgview.Name = "dgview";
            dgview.Visible = false;
            this.dgviewcol1.Visible = false;
            this.dgviewcol2.Visible = false;
            this.dgview.AllowUserToAddRows = false;
            this.dgview.RowHeadersVisible = false;
            this.dgview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //this.dgview.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgview_KeyDown); 


            this.Controls.Add(dgview);
            this.dgview.ReadOnly = true;
            dgview.BringToFront();
        }
            //Two Column 
         void Search(int LX, int LY, int DW, int DH, string ColName, String ColSize)
         { 
             this.dgview.Location = new System.Drawing.Point(LX, LY); 
             this.dgview.Size = new System.Drawing.Size(DW, DH); 
 
 
             string[] ClSize = ColSize.Split(','); 
             //Size 
             for (int i = 0; i<ClSize.Length; i++) 
             { 
                 if (int.Parse(ClSize[i]) != 0) 
                 { 
                     dgview.Columns[i].Width = int.Parse(ClSize[i]); 
                 } 
                 else 
                 { 
                     dgview.Columns[i].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill; 
                 }
            } 
             //Name  
             string[] ClName = ColName.Split(',');


            for (int i = 0; i < ClName.Length; i++)
            {
                this.dgview.Columns[i].HeaderText = ClName[i];
                this.dgview.Columns[i].Visible = true;
            }
        }

    }
}
