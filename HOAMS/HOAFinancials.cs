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
    public partial class HOAFinancials : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
       
        public HOAFinancials()
        {
            InitializeComponent();
            LoadTransacs();
            LoadFinance();
            LoadExpense();
            latestDate();




        }
        public void latestDate()
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand("SELECT MAX(PaymentDate) AS Latest FROM TBL_Payment", connect);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime latest = dr.GetDateTime(0);
                label8.Text = latest.ToString();
                datefee = latest.ToString();

            }
            dr.Close();
            SqlCommand cmd1 = new SqlCommand("SELECT MAX(PaidDate) AS Latest FROM TBL_Expense", connect);
            SqlDataReader dr1 = cmd1.ExecuteReader();
            while (dr1.Read())
            {
                DateTime latest1 = dr1.GetDateTime(0);
                expdate = latest1.ToString();
                label12.Text = latest1.ToString();

            }
            dr1.Close();


            connect.Close();
        }



        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnAddExp_Click(object sender, EventArgs e)
        {
            ExpensesForm ef = new ExpensesForm(this);
            ef.ShowDialog();
        }
        public string type = null;
        public string amount = null;
        public string name = null;
        public static string expdate = null;
        public static string expamount = null;
        public static string datefee = null;
        public void LoadTransacs()
        {

               try
               {
                   HOTransac.Rows.Clear();
                   connect.Open();
                            string all = "SELECT * " +
                                        "FROM TBL_HORecords AS H " +
                                        "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                                        "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID where TransactionID like '%" + txtSearch.Text + "%' OR FName like '%" + txtSearch.Text + "%' OR LName like '%" + txtSearch.Text + "%' OR TransacFee like '%" + txtSearch.Text + "%' OR PaymentDate like '%" + txtSearch.Text + "%' ";

                                        //"JOIN TBL_User AS U ON H.ID = U.Acc_ID " +
                                        //"JOIN TBL_Expense AS E ON U.ID = E.User_ID " +
                                        //"JOIN TBL_Transactions AS EXP ON E.Exp_ID = EXP.Exp_ID";*/
                            /*string all = "SELECT * " +
                         "FROM TBL_HORecords AS H " +
                         "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                         "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                         "JOIN TBL_User AS U ON H.ID = U.Acc_ID ";
                         */

                            /*string all = "SELECT * " +
                           "FROM TBL_Transactions AS T " +
                           "JOIN TBL_Expense AS E ON E.Exp_ID = T.Exp_ID " +
                           "JOIN TBL_User AS U ON U.ID = E.User_ID ";*/
               
             
                            SqlCommand cmd = new SqlCommand(all, connect);
                   SqlDataReader dr = cmd.ExecuteReader();
                   while (dr.Read())
                   {

                    
                             if (dr["TransacType"].ToString().Equals("MONTHLY HOA FEE") || (dr["TransacType"].ToString().Equals("MONTHLY AMORTIZATION AND MONTHLY HOA FEE")))
                                {
                                    name = dr["FName"].ToString() + " " + dr["LName"].ToString();
                                    type = "MONTHLY HOA FEE";
                                    amount = "+ PHP " + dr["TransacFee"].ToString();
                                   
                    }
                             

                       HOTransac.Rows.Add(dr["TransactionID"].ToString(), name, dr["TransacDate"].ToString(), type, amount);


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
        public void LoadExpense()
        {
            string names = null;
            string types = null;
            string amounts = null;
            try
            {
                HOExpenses.Rows.Clear();
                connect.Open();
                

                string all = "SELECT * " +
               "FROM TBL_Transactions AS T " +
               "JOIN TBL_Expense AS E ON E.Exp_ID = T.Exp_ID " +
               "JOIN TBL_User AS U ON U.ID = E.User_ID where TransactionID like '%" + txtSearch.Text + "%' OR FirstName like '%" + txtSearch.Text + "%' OR LastName like '%" + txtSearch.Text + "%' OR TransacDate like '%" + txtSearch.Text + "%' OR Amount like '%" + txtSearch.Text + "%'";


                SqlCommand cmd = new SqlCommand(all, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {


                    
                        names = dr["FirstName"].ToString() + " " + dr["LastName"].ToString();
                        types = dr["TransacType"].ToString();
                        amounts = "- PHP " + dr["Amount"].ToString();
                   



                    HOExpenses.Rows.Add(dr["TransactionID"].ToString(), names, dr["TransacDate"].ToString(), types, amounts);


                }
                dr.Close();
                connect.Close();
                HOExpenses.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }
        public static string oFinance = null;
        public void LoadFinance()
        {

           try
           {

               connect.Open();
               SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Finance", connect);
               SqlDataReader dr = cmd.ExecuteReader();
               while (dr.Read())
               {
                   oFin.Text = "PHP " + dr["HoaFinance"].ToString();
                   oFinance = "PHP " + dr["HoaFinance"].ToString();
                    mfee.Text = "PHP " + dr["HoaPayment"].ToString();
                   oExp.Text = "PHP " + dr["HoaExpenses"].ToString();
                   expamount = "PHP " + dr["HoaExpenses"].ToString();



                }
               dr.Close();
               connect.Close();

           }
           catch (Exception ex)
           {

               MessageBox.Show("Error: " + ex);

           }

}

        private void label3_Click(object sender, EventArgs e)
        {
            HOTransac.Dock = DockStyle.Fill;
            HOExpenses.Dock = DockStyle.None;
            HOTransac.BringToFront();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            HOExpenses.Dock = DockStyle.Fill;
            HOTransac.Dock = DockStyle.None;
            HOExpenses.BringToFront();
        }

        private void HOAFinancials_Click(object sender, EventArgs e)
        {
            
        }

        private void HOAFinancials_Load(object sender, EventArgs e)
        {
            
            cmB.Items.Add("Daily");
            cmB.Items.Add("Monthly");
            cmB.Items.Add("Yearly");
            cmB.Items.Add("All");
            cmB.DropDownStyle = ComboBoxStyle.DropDownList;
            cmB.Text = "All";

            expCmb.Items.Add("Daily");
            expCmb.Items.Add("Monthly");
            expCmb.Items.Add("Yearly");
            expCmb.Items.Add("All");
            expCmb.DropDownStyle = ComboBoxStyle.DropDownList;
            expCmb.Text = "All";

        }

        private void panel12_Click(object sender, EventArgs e)
        {
            HOTransac.Dock = DockStyle.Left;
            HOExpenses.Dock = DockStyle.Fill;
            HOExpenses.BringToFront();
        }

        private void cmB_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string selectedOption = cmB.SelectedItem.ToString();
            if (selectedOption != "All")
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime startDate, endDate;
                string dateColumn;
                decimal totalTransacFee = 0;
                decimal TF = 0;

                using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                {
                    connection.Open();
                    string query = "";

                    // Determine the date range and date column based on the selected sorting option
                    if (selectedOption == "Daily")
                    {
                        startDate = currentDate.Date;
                        endDate = currentDate.Date.AddDays(1);
                        dateColumn = "T.TransacDate";
                    }
                    else if (selectedOption == "Monthly")
                    {
                        startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                        dateColumn = "T.TransacDate";
                    }
                    else if (selectedOption == "Yearly")
                    {
                        startDate = new DateTime(currentDate.Year, 1, 1);
                        endDate = new DateTime(currentDate.Year, 12, 31);
                        dateColumn = "T.TransacDate";
                    }

                    else
                    {
                        // Handle the case when no sorting option is selected or an invalid option is selected
                        return;
                    }

                    // Construct the SQL query using the determined date range and column
                    query = "SELECT H.*, P.*, T.* " +
                            "FROM TBL_HORecords AS H " +
                            "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                            "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                            "WHERE " + dateColumn + " BETWEEN @StartDate AND @EndDate " +
                            "ORDER BY " + dateColumn + " DESC";

                    // Execute the query and process the results
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    SqlDataReader dr = command.ExecuteReader();
                    HOTransac.Rows.Clear();
                    while (dr.Read())
                    {
                        
                        // Process the query results here...
                        if (dr["TransacType"].ToString().Equals("MONTHLY HOA FEE") || (dr["TransacType"].ToString().Equals("MONTHLY AMORTIZATION AND MONTHLY HOA FEE")))
                        {
                            name = dr["FName"].ToString() + " " + dr["LName"].ToString();
                            type = "MONTHLY HOA FEE";
                            amount = "+ PHP " + dr["TransacFee"].ToString();
                            datefee = dr["PaymentDate"].ToString();
                            TF = Convert.ToDecimal(dr["TransacFee"]);
                            totalTransacFee = totalTransacFee + TF;
                            HOTransac.Rows.Add(dr["TransactionID"].ToString(), name, dr["TransacDate"].ToString(), type, amount);
                        }
                       

                    }
                    dr.Close();
                }
                oFin.Text = "PHP " + totalTransacFee.ToString();

            }
            else
            {
                oFin.Text = oFinance;
                LoadTransacs();

            }
           
        }

        private void expCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = expCmb.SelectedItem.ToString();
            if (selectedOption != "All")
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime startDate, endDate;
                string dateColumn;
                decimal totalTransacFee = 0;
                decimal TF = 0;

                using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                {
                    connection.Open();
                    string query = "";

                    // Determine the date range and date column based on the selected sorting option
                    if (selectedOption == "Daily")
                    {
                        startDate = currentDate.Date;
                        endDate = currentDate.Date.AddDays(1).AddSeconds(-1);
                        dateColumn = "T.TransacDate";
                    }
                    else if (selectedOption == "Monthly")
                    {
                        startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                        dateColumn = "T.TransacDate";
                    }
                    else if (selectedOption == "Yearly")
                    {
                        startDate = new DateTime(currentDate.Year, 1, 1);
                        endDate = new DateTime(currentDate.Year, 12, 31);
                        dateColumn = "T.TransacDate";
                    }

                    else
                    {
                        // Handle the case when no sorting option is selected or an invalid option is selected
                        return;
                    }

                    // Construct the SQL query using the determined date range and column
                    query = "SELECT T.*, E.*, U.* " +
                            "FROM TBL_Transactions AS T " +
                             "JOIN TBL_Expense AS E ON E.Exp_ID = T.Exp_ID " +
                             "JOIN TBL_User AS U ON U.ID = E.User_ID " +
                             "WHERE " + dateColumn + " BETWEEN @StartDate AND @EndDate " +
                             "ORDER BY " + dateColumn + " DESC";

                    // Execute the query and process the results
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    SqlDataReader dr = command.ExecuteReader();
                    HOExpenses.Rows.Clear();
                    while (dr.Read())
                    {
                        // Process the query results here...
                        string names = dr["FirstName"].ToString() + " " + dr["LastName"].ToString();
                       string types = dr["TransacType"].ToString();
                        string amounts = "- PHP " + dr["Amount"].ToString();
                        expdate = dr["TransacDate"].ToString();
                        TF = Convert.ToDecimal(dr["Amount"]);
                        totalTransacFee = totalTransacFee + TF;
                        HOExpenses.Rows.Add(dr["TransactionID"].ToString(), names, dr["TransacDate"].ToString(), types, amounts);


                    }
                    dr.Close();
                }
                oExp.Text = "PHP " + totalTransacFee.ToString();

            }
            else
            {
                oExp.Text = expamount;
                LoadExpense();

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadExpense();
            LoadTransacs();
        }
    }
}
