using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HOAMS
{

    public partial class HOBilling : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        
        public string hoa = null;
        public string amorts = null;
        public string hoaF = null;
        public string hoaA1 = null;
        public HOBilling()
        {
            InitializeComponent();
            LoadFinance();
            LoadRecs();
            LoadTransacs();
            
        }
        public void LoadFinance()
        {
            connect.Open();

            SqlCommand cmd = new SqlCommand("Select HoaPayment, Amortization, HoaFinance, HoaAmort from TBL_Finance where ID = 1", connect);
            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())//.Read
            {
                hoa = read["HoaPayment"].ToString();
                amorts = read["Amortization"].ToString();
                hoaF = read["HoaFinance"].ToString();
                hoaA1 = read["HoaAmort"].ToString();

            }
            connect.Close();

        }
        public void LoadTransacs()
        {

            try
            {
                HOTransac.Rows.Clear();
                connect.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Transactions where PaymentID = @ID", connect);
                cmd.Parameters.AddWithValue("@ID", payID);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    HOTransac.Rows.Add(dr["TransactionID"].ToString(), dr["TransacDate"].ToString(), dr["TransacType"].ToString(), dr["TransacAmount"].ToString());


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
        public string payID = null;
        public string hoaA = null;
        public void LoadRecs() {
            try
            {
                string all = "SELECT * " +
                             "FROM TBL_HORecords AS H " +
                             "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                             "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                             "WHERE H.ID LIKE '" + HomeOwnerRecords.ID + "'";

                
                connect.Open();
                SqlCommand cmd = new SqlCommand(all, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    payID = dr["PaymentID"].ToString();
                    name1.Text = dr["Fname"].ToString() + " " + dr["Lname"].ToString();
                    contact.Text = dr["Contact"].ToString();
                    string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                    address.Text = add;
                    type.Text = dr["HomeownerType"].ToString();
                    tot.Text = "PHP " + dr["OverallPay"].ToString();
                    bal.Text = "PHP " + dr["RemainingBalance"].ToString();
                    DateTime nextpay = (DateTime)dr["NextPaymentDate"];
                    month.Text = nextpay.ToString("MMMM d, yyyy");
                    amort.Text = "PHP " + dr["LastAmort"].ToString();
                    hoaA = dr["LastAmort"].ToString();
                    amortis.Text = "PHP " + amorts;
                    hfees.Text = "PHP " + hoa;
                    DateTime nextpay1 = (DateTime)dr["NextHoaFeePayment"];
                    monthFee.Text = nextpay1.ToString("MMMM d, yyyy");
                    HoaFee.Text = "PHP " + dr["LastFee"].ToString();
                    ofee.Text = "PHP " + dr["OverallHoaFeePay"].ToString();





                }
                dr.Close();
                connect.Close();
               
               

            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex); }


        }

        private void HOTransac_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Billing b = new Billing();
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {

                    string all = "SELECT * " +
                             "FROM TBL_HORecords AS H " +
                             "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                             "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                             "WHERE T.TransactionID LIKE '" + HOTransac.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";

                    HOReceipt ho = new HOReceipt(b);
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(all, connect);
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();

                    if (dr.HasRows)
                    {
                        ho.transac.Text = dr["TransactionID"].ToString();
                        string nm = dr["Fname"].ToString() + " " + dr["Lname"].ToString();
                        ho.name.Text = nm;
                        string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                        ho.add.Text = add;
                        ho.type.Text = dr["TransacType"].ToString();
                        // DateTime dts = (DateTime)dr["TransacDate"];
                        ho.date.Text = dr["TransacDate"].ToString();
                        ho.amount.Text = "PHP " + dr["TransacAmount"].ToString();


                    }
                    dr.Close();
                    connect.Close();

                    ho.ShowDialog();


                }


            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex); }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
