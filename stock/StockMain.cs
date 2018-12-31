using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stock
{
    public partial class StockMain : Form
    {

        public StockMain()
        {
            InitializeComponent();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products pro = new Products();
            pro.MdiParent = this;
            pro.StartPosition = FormStartPosition.CenterScreen;
            pro.Show();

        }

        //열려있는 모든 폼에서 한번씩 물어보기 때문에 한번만 물어 볼수 있게 close 변수를 사용 
        bool close = true;

        private void StockMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (close)
            {
                DialogResult result = MessageBox.Show("do you want exit?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    close = false;
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
     
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock stk = new Stock();
            stk.MdiParent = this;
            stk.StartPosition = FormStartPosition.CenterScreen;
            stk.Show();
        }
    }
}
