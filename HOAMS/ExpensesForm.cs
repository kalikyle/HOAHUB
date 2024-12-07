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
    public partial class ExpensesForm : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        HOAFinancials hf;
        public ExpensesForm(HOAFinancials hf)
        {
            InitializeComponent();
            LoadFinance();
            this.hf = hf;
            transacID.Text = DateTime.Now.ToString("yyyyMMddHHmmss");

        }

        private void btnAddExp_Click(object sender, EventArgs e)
        {
            
            connect.Open();
            string command2 = "insert into TBL_Expense (ExpensesName, Amount, PaidDate, Exp_Desc, User_ID) values (@ExpensesName, @Amount, @PaidDate, @Exp_Desc, @User_ID)";
            SqlCommand usercm1 = new SqlCommand(command2, connect);
            usercm1.Parameters.AddWithValue("@ExpensesName", txtSub.Text);
            usercm1.Parameters.AddWithValue("@Amount", Convert.ToDouble(amount.Text));
            usercm1.Parameters.AddWithValue("@PaidDate", date.Value);
            usercm1.Parameters.AddWithValue("@Exp_Desc", desc.Text);
            usercm1.Parameters.AddWithValue("@User_ID", LOGIN.uID);
            usercm1.ExecuteNonQuery();

            double finances = Convert.ToDouble(finance);
            double expenses = Convert.ToDouble(expense);

            double minus = finances - Convert.ToDouble(amount.Text);
            double plus = expenses + Convert.ToDouble(amount.Text);
            connect.Close();

            connect.Open();
            //add the data to the finance
            SqlCommand cmd1 = new SqlCommand("update TBL_Finance set HoaFinance = @HoaFinance, HoaExpenses = @HoaExpenses where ID = 1", connect);
            cmd1.Parameters.AddWithValue("@HoaFinance", minus);
            cmd1.Parameters.AddWithValue("@HoaExpenses", plus);
            cmd1.ExecuteNonQuery();
            connect.Close();

            //transactions
            connect.Open();
            //add the data to the finance
            SqlCommand cmd2 = new SqlCommand("insert into TBL_Transactions (TransactionID, TransacDate, TransacType, TransacAmount, Exp_ID) values (@TransactionID, @TransacDate, @TransacType, @TransacAmount, (select max(Exp_ID) from TBL_Expense))", connect);
            cmd2.Parameters.AddWithValue("@TransactionID", transacID.Text);
            cmd2.Parameters.AddWithValue("@TransacDate", DateTime.Now);
            cmd2.Parameters.AddWithValue("@TransacType", "HOA EXPENSES");
            cmd2.Parameters.AddWithValue("@TransacAmount", amount.Text);
            cmd2.ExecuteNonQuery();
            connect.Close();


            hf.LoadTransacs();
            hf.LoadFinance();
            hf.LoadExpense();
            hf.latestDate();
            LOGIN.LogActivity("Created a Report for Expenses");
            MessageBox.Show("Expenses has been saved!");
            this.Hide();
        }
        public string finance = null;
        public string expense = null;
        
        
        public void LoadFinance()
        {

            try
            {

                connect.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Finance", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    finance = dr["HoaFinance"].ToString();
                    expense= dr["HoaExpenses"].ToString();


                }
                dr.Close();
                connect.Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }
    }
}
