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
namespace HOAMS
{
    public partial class Transactions : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public Transactions()
        {
            InitializeComponent();
            //LoadTransacs();
            // LoadFinance();
            clearPanel();
            HOAFinancials tr = new HOAFinancials();
            tr.Dock = DockStyle.Fill;
            tr.TopLevel = false;
            FormPNL.Controls.Add(tr);
            tr.BringToFront();
            tr.Show();
        }

        private void HOTransac_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void clearPanel()
        {

            FormPNL.Controls.Clear();
        }
        private void btnFinance_Click(object sender, EventArgs e)
        {
            clearPanel();
            HOAFinancials tr = new HOAFinancials();
            tr.Dock = DockStyle.Fill;
            tr.TopLevel = false;
            FormPNL.Controls.Add(tr);
            tr.BringToFront();
            tr.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            clearPanel();
            Amortization tr = new Amortization();
            tr.Dock = DockStyle.Fill;
            tr.TopLevel = false;
            FormPNL.Controls.Add(tr);
            tr.BringToFront();
            tr.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearPanel();
            HOReports tr = new HOReports();
            tr.Dock = DockStyle.Fill;
            tr.TopLevel = false;
            FormPNL.Controls.Add(tr);
            tr.BringToFront();
            tr.Show();
        }
        /*public void LoadTransacs()
{

try
{
HOTransac.Rows.Clear();
connect.Open();
SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Transactions", connect);
SqlDataReader dr = cmd.ExecuteReader();
while (dr.Read())
{
HOTransac.Rows.Add(dr["PaymentID"].ToString(), dr["TransactionID"].ToString(), dr["TransacDate"].ToString(), dr["TransacType"].ToString(), dr["TransacAmount"].ToString());


}
dr.Close();
connect.Close();
HOTransac.ClearSelection();
}
catch (Exception ex)
{

MessageBox.Show("Error: " + ex);

}

}
public void LoadFinance()
{

try
{

connect.Open();
SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Finance", connect);
SqlDataReader dr = cmd.ExecuteReader();
while (dr.Read())
{
hoaf.Text = "PHP " + dr["HoaFinance"].ToString();
hoaA.Text = "PHP " + dr["HoaAmort"].ToString();
fin.Text = "PHP " + dr["HoaPayment"].ToString();
amort.Text = "PHP " + dr["Amortization"].ToString();


}
dr.Close();
connect.Close();

}
catch (Exception ex)
{

MessageBox.Show("Error: " + ex);

}

}*/
    }
}
