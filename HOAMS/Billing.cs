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
    public partial class Billing : Form
    {
        
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public double lastamort = 0; //retrive last amort from tbl_payment
        public double lastfee = 0; // retrive hoapayment
        public string hoa = null; // retrive hoapayment also
        public string amorts = null; // retrive amorts in TBL_finance
        public string hoID = null;
        public string totals1 = null;
        public string totals2 = null;
        public string bals = null;
        public string hoaF = null;
        public string hoaA = null;
        public string paymentID = null;
        public string funds = null;
        public string HOfunds = null;
        public string alreadyPaid = null;
        public string name = null, addr = null;
        public string hn = null;
        public static string datetransact = null;

        public Billing()
        {
            InitializeComponent();
            LoadRecords();
            LoadFinance();
            /*LOGIN.UpdateIsOverDue();
            LOGIN.UpdateIsOverDueFee();
            LOGIN.UpdateActivation();
            LOGIN.UpdateAccStatusOnHomeownerType();*/




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
                hoaA = read["HoaAmort"].ToString();

            }
            connect.Close();

        }
        public void LoadRecords()
        {
            try
            {
                String s1 = "SELECT TBL_HORecords.*, TBL_Payment.*, TBL_User.* " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID";

                //String s = "select * from TBL_HORecords";

                if (all.Checked)
                {
                    s1 += " where TBL_HORecords.ID like '%" + txtSearch.Text + "%' OR Fname like '%" + txtSearch.Text + "%' OR Mname like '%" + txtSearch.Text + "%' OR Lname like '%" + txtSearch.Text + "%' OR Age like '%" + txtSearch.Text + "%' OR Contact like '%" + txtSearch.Text + "%' OR Street like '%" + txtSearch.Text + "%' OR HouseNum like '%" + txtSearch.Text + "%' OR Block like '%" + txtSearch.Text + "%' OR Lot like '%" + txtSearch.Text + "%'  ";
                }
                else if (Idrad.Checked)
                {
                    s1 += " where TBL_HORecords.ID like '%" + txtSearch.Text + "%'";
                }
                else if (fnrad.Checked)
                {
                    s1 += " where Fname like '%" + txtSearch.Text + "%'";

                }
                else if (strad.Checked)
                {

                    s1 += " where Street like '%" + txtSearch.Text + "%'";
                }
                else if (hnrad.Checked)
                {
                    s1 += " where HouseNum like '%" + txtSearch.Text + "%'";
                }
                else if (balrad.Checked)
                {
                    s1 += " where RemainingBalance like '%" + txtSearch.Text + "%'";
                }
                



                HOData.Rows.Clear();
                connect.Open();
                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string stats = dr["Acc_Status"].ToString();
                    string type = dr["HomeownerType"].ToString();

                    if (stats.Equals("ACTIVE") || type != "FORMER HOMEOWNER")
                    {

                        datetransact = dr["PaymentDate"].ToString();
                        HOData.Rows.Add(dr["ID"].ToString(), dr["Fname"].ToString(), dr["Lname"].ToString(), dr["Street"].ToString(), dr["HouseNum"].ToString(), "PHP " + dr["RemainingBalance"].ToString());
                    }

                }
                dr.Close();
                connect.Close();
                HOData.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }
        DateTime nextpay;
        DateTime nextFee;
        public string overdue = null;
        public string overduef = null;
        public string statsu = null;
        private void HOData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pay.Enabled = false;
           
            try
            {

                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {



                    // Get a reference to the existing HOInformations user control on the MainPage form

                    

                    connect.Open();
                    //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    /*SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.Username, TBL_User.Password " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                    "WHERE TBL_HORecords.ID LIKE '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);*/
                    SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.*, TBL_Payment.*, TBL_User.* " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                                    "WHERE TBL_HORecords.ID LIKE '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        overdue = dr["isOverDue"].ToString();
                        overduef = dr["isOverDueFee"].ToString();
                        statsu = dr["Acc_Status"].ToString();
                        hoID = dr["ID"].ToString();
                        name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        name1.Text = name;
                        addr = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                        address.Text = addr;
                        hn = dr["HouseNum"].ToString();
                        contact.Text = dr["Contact"].ToString();
                        type.Text = dr["HomeownerType"].ToString();
                        totals1 = dr["OverallPay"].ToString();
                        tot.Text = "PHP " + dr["OverallPay"].ToString();
                        totfee.Text = "PHP " + dr["OverallHoaFeePay"].ToString();
                        bal.Text = "PHP " + dr["RemainingBalance"].ToString();
                        nextpay = (DateTime)dr["NextPaymentDate"];
                        month.Text = nextpay.ToString("MMMM d, yyyy");
                        nextFee = (DateTime)dr["NextHoaFeePayment"];
                        monthFee.Text = nextFee.ToString("MMMM d, yyyy");
                        amort.Text = "PHP " + dr["LastAmort"].ToString();
                        HoaFee.Text = "PHP " + dr["LastFee"].ToString();
                        lastamort = Convert.ToDouble(dr["LastAmort"].ToString());
                        lastfee = Convert.ToDouble(dr["LastFee"].ToString());
                        bals = dr["RemainingBalance"].ToString();
                        funds = dr["Funds"].ToString();
                        HOfunds = dr["HOFund"].ToString();
                        totals2 = dr["OverallHoaFeePay"].ToString();
                        paymentID = dr["PaymentID"].ToString();
                        alreadyPaid = dr["isPaidOff"].ToString();




                    }

                    dr.Close();
                    connect.Close();


                    amortis.Text = "PHP " + amorts;
                    hfees.Text = "PHP " + hoa;
                   
                    transacID.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
                    date.Text = DateTime.Now.ToShortDateString();
                    LoadTransacs();
                    
                }
                bool AlPaid = Convert.ToBoolean(alreadyPaid);
                if (AlPaid == true)
                {
                    label19.Visible = false;
                    payment.Visible = false;
                    TOTAL.Enabled = false;
                    radMan.Checked = true;
                    radMan.Visible = false;
                    radAuto.Visible = false;
                    fee.Enabled = true;
                    add.Visible = false;
                    label17.Visible = false;
                    label15.Visible = false;
                    label16.Visible = false;
                    label23.Visible = false;
                    amortis.Visible = false;
                    bal.Visible = false;
                    month.Visible = false;
                    amort.Visible = false;
                    congrats.Visible = true;
                }
                else {
                    label19.Visible = true;
                    payment.Visible = true;
                    radMan.Visible = true;
                    radAuto.Visible = true;
                    radMan.Checked = false;
                    radAuto.Checked = true;
                    label17.Visible = true;
                    label15.Visible = true;
                    label16.Visible = true;
                    label23.Visible = true;
                    amortis.Visible = true;
                    bal.Visible = true;
                    month.Visible = true;
                    amort.Visible = true;
                    congrats.Visible = false;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
           
        }


       
        private void pay_Click(object sender, EventArgs e)
        {
            LOGIN.LogActivity("Received the Payment of " + name);
           
            datetransact = DateTime.Now.ToString();
            bool AlPaid = Convert.ToBoolean(alreadyPaid);
            // for amortization payment from TBL_finance
            transacID.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            double amortization = Convert.ToDouble(amorts); // use the amortizartion
            double HOpayment = Convert.ToDouble(payment.Text); // use the payment inputed
            double remainingPay = 0;
            double RemainBal = Convert.ToDouble(bals);
            double fund = 0;
            int remainingMonths = 0;
            DateTime nextpayment = nextpay; 
            double nextAmortizationPayment = 0;
            double t = 0;


            double hoafee = Convert.ToDouble(hoa); // for hoa fee payment from TBL_finance
            double HOAfeepay = Convert.ToDouble(fee.Text);
            double HremainingPay = 0;
            double Hfund = 0;
            int HremainingMonths = 0;
            DateTime Hnextpayment = nextFee;
            double HnextfeePayment = 0;
            double Ht = 0;

            RemainBal = RemainBal - HOpayment;//127650
            double fd = Convert.ToDouble(funds);
            double Hfd = Convert.ToDouble(HOfunds);

           // if lastamort is not equal to 0 // for amortization
            if (lastamort == HOpayment) // default payment
                {
                    remainingPay = amortization;
                    nextpayment = nextpay.AddMonths(1);
                    nextAmortizationPayment = remainingPay;
                    fund = 0;
                }
                else if (lastamort < HOpayment)//advance payment // updated
                {
                    if (HOpayment > amortization)
                    {

                        t = fd + HOpayment; // 28
                        fund = t - ((int)Math.Floor(t / amortization) * amortization);//292 //28
                        remainingPay = amortization - fund; //427 - 28 = 399
                        remainingMonths = (int)(t / amortization);
                        nextpayment = nextpay.AddMonths(remainingMonths);
                        nextAmortizationPayment = remainingPay;

                    }
                    else {
                        fund = HOpayment - ((int)Math.Ceiling(HOpayment / amortization) * lastamort);
                        remainingPay = amortization - fund;
                        remainingMonths = (int)Math.Ceiling(HOpayment / amortization);
                        nextpayment = nextpay.AddMonths(remainingMonths);
                        nextAmortizationPayment = remainingPay;
                    }
                    
                    
                }
                else if (fund == 0 && lastamort > HOpayment) // less payment
                {
                    if (lastamort == amortization)
                    {
                        remainingPay = amortization - HOpayment;
                        nextpayment = nextpay;
                        nextAmortizationPayment = remainingPay;

                    }
                    else
                    {
                        remainingPay = lastamort - HOpayment;
                        nextpayment = nextpay;
                        nextAmortizationPayment = remainingPay;
                    }
                }

            // for Hoa Fee
            if (lastfee == HOAfeepay) // default payment
            {
                HremainingPay = hoafee;
                Hnextpayment = nextFee.AddMonths(1);
                HnextfeePayment = HremainingPay;
                Hfund = 0;
            }
            else if (lastfee < HOAfeepay)//advance payment // updated
            {
                if (HOAfeepay > hoafee)
                {

                    Ht = Hfd + HOAfeepay; // 28
                    Hfund = Ht - ((int)Math.Floor(Ht / hoafee) * hoafee);//292 //28
                    HremainingPay = hoafee - Hfund; //427 - 28 = 399
                    HremainingMonths = (int)(Ht / hoafee);
                    Hnextpayment = nextFee.AddMonths(HremainingMonths);
                    HnextfeePayment = HremainingPay;

                }
                else
                {
                    Hfund = HOAfeepay - ((int)Math.Ceiling(HOAfeepay / hoafee) * lastfee);
                    HremainingPay = hoafee - Hfund;
                    HremainingMonths = (int)Math.Ceiling(HOAfeepay / hoafee);
                    Hnextpayment = nextFee.AddMonths(HremainingMonths);
                    HnextfeePayment = HremainingPay;
                }


            }
            else if (Hfund == 0 && lastfee > HOAfeepay) // less payment
            {
                if (lastfee == hoafee)
                {
                    HremainingPay = hoafee - HOAfeepay;
                    Hnextpayment = nextFee;
                    HnextfeePayment = HremainingPay;

                }
                else
                {
                    HremainingPay = lastfee - HOAfeepay;
                    Hnextpayment = nextFee;
                    HnextfeePayment = HremainingPay;
                }
            }



            
            double overAll = Convert.ToDouble(totals1) + HOpayment;
            double overAfee = Convert.ToDouble(totals2) + HOAfeepay;

            //payment
            connect.Open();
            //add the data to the payment
            SqlCommand cmd = new SqlCommand("update TBL_Payment set OverallPay = @OverallPay, PaymentAmount = @PaymentAmount, PaymentDate = @PaymentDate, RemainingBalance = @RemainingBalance, NextPaymentDate = @NextPaymentDate, LastAmort = @LastAmort, Funds = @Funds, FeePaymentAmount = @FeePaymentAmount, NextHoaFeePayment = @NextHoaFeePayment,  LastFee = @LastFee, HOFund = @HOFund, OverallHoaFeePay = @OverallHoaFeePay, isPaidOff = @isPaidOff, AmortPaidDate = @AmortPaidDate where HoID = @HoID", connect);
            cmd.Parameters.AddWithValue("@HoID", hoID);
            cmd.Parameters.AddWithValue("@OverallPay", overAll);
            cmd.Parameters.AddWithValue("@PaymentAmount", HOpayment);
            cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@RemainingBalance", RemainBal);
            cmd.Parameters.AddWithValue("@NextPaymentDate", nextpayment);
            cmd.Parameters.AddWithValue("@LastAmort", nextAmortizationPayment);
            cmd.Parameters.AddWithValue("@Funds", fund);
            cmd.Parameters.AddWithValue("@FeePaymentAmount", HOAfeepay);
            cmd.Parameters.AddWithValue("@NextHoaFeePayment", Hnextpayment);
            cmd.Parameters.AddWithValue("@LastFee", HnextfeePayment);
            cmd.Parameters.AddWithValue("@HOFund", Hfund);
            cmd.Parameters.AddWithValue("@OverallHoaFeePay", overAfee);
            if (RemainBal <= 0)
            {
                cmd.Parameters.AddWithValue("@isPaidOff", 1);
                cmd.Parameters.AddWithValue("@AmortPaidDate", DateTime.Now);
            }
            else {
                cmd.Parameters.AddWithValue("@isPaidOff", 0);
                cmd.Parameters.AddWithValue("@AmortPaidDate", DateTime.Now);

            }

            cmd.ExecuteNonQuery();
            connect.Close();

            double finance = Convert.ToDouble(hoaF);
            double amort = Convert.ToDouble(hoaA);

            double Afinance = finance + HOAfeepay;
            double Aamort = HOpayment + amort;
            //finance
            connect.Open();
            //add the data to the finance
            SqlCommand cmd1 = new SqlCommand("update TBL_Finance set HoaFinance = @HoaFinance, HoaAmort = @HoaAmort where ID = 1", connect);
            cmd1.Parameters.AddWithValue("@HoaFinance", Afinance);
            cmd1.Parameters.AddWithValue("@HoaAmort", Aamort);
            cmd1.ExecuteNonQuery();
            connect.Close();

            //transactions
            connect.Open();
            //add the data to the finance
            SqlCommand cmd2 = new SqlCommand("insert into TBL_Transactions (TransactionID, TransacDate, TransacType, TransacAmount, TransacAmort, TransacFee, PaymentID) values (@TransactionID, @TransacDate, @TransacType, @TransacAmount, @TransacAmort, @TransacFee, @ID)", connect);
            cmd2.Parameters.AddWithValue("@ID", paymentID);
            cmd2.Parameters.AddWithValue("@TransactionID", transacID.Text);
            cmd2.Parameters.AddWithValue("@TransacDate", DateTime.Now);
            if (HOAfeepay == 0)
            {
                cmd2.Parameters.AddWithValue("@TransacType", "MONTHLY AMORTIZATION");
            }
            else if (HOpayment == 0)
            {
                cmd2.Parameters.AddWithValue("@TransacType", "MONTHLY HOA FEE");
            }
            else {
                cmd2.Parameters.AddWithValue("@TransacType", "MONTHLY AMORTIZATION AND MONTHLY HOA FEE");
            }
           
            cmd2.Parameters.AddWithValue("@TransacAmount", Convert.ToDouble(TOTAL.Text));
            cmd2.Parameters.AddWithValue("@TransacAmort", HOpayment);
            cmd2.Parameters.AddWithValue("@TransacFee", HOAfeepay);
            cmd2.ExecuteNonQuery();
            connect.Close();


            if (HOpayment != 0) {
            //paidamorts
                connect.Open();
                //add the data to the paidamorts
                SqlCommand cmd4 = new SqlCommand("insert into TBL_PaidAmorts (BenName, HouseNum, TransacID, TransacDate, Amort, Balance) values (@BenName, @HouseNum, @TransacID, @TransacDate, @Amort, @Balance)", connect);
                cmd4.Parameters.AddWithValue("@BenName", name);
                cmd4.Parameters.AddWithValue("@HouseNum", hn);
                cmd4.Parameters.AddWithValue("@TransacID", transacID.Text);
                cmd4.Parameters.AddWithValue("@TransacDate", DateTime.Now);
                cmd4.Parameters.AddWithValue("@Amort", HOpayment);
                cmd4.Parameters.AddWithValue("@Balance", RemainBal);
                cmd4.ExecuteNonQuery();
                connect.Close();
            }
            
           



            LoadnewInfo();
            LoadTransacs();
            LoadRecords();
            LoadFinance();
            LOGIN.UpdateIsOverDue();
            LOGIN.UpdateIsOverDueFee();
            LOGIN.UpdateActivation();
            LOGIN.UpdateAccStatusOnHomeownerType();
            LOGIN.GetAccStatus();

            payment.Text = "0";
            fee.Text = "0";
            TOTAL.Text = "0";
            add.Visible = false;
            radAuto.Checked = true;
            DialogResult diagres = MessageBox.Show("Do you want to see the Official Receipt?", "Confirm", MessageBoxButtons.YesNo);
            if (diagres == DialogResult.Yes)
            {
                Pay();
            }
        }
        //to load the transactions
        public void LoadTransacs()
        {

            try
            {
                HOTransac.Rows.Clear();
                connect.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Transactions where PaymentID = @ID", connect);
                cmd.Parameters.AddWithValue("@ID", paymentID);
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
        // to load NewInfo
        public void LoadnewInfo()
        {
            String name = "", adds = "";

            connect.Open();
            //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
            /*SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.Username, TBL_User.Password " +
                            "FROM TBL_HORecords " +
                            "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                            "WHERE TBL_HORecords.ID LIKE '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);*/
            SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.*, TBL_Payment.* " +
                            "FROM TBL_HORecords " +
                            "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                            "WHERE TBL_HORecords.ID LIKE '" + hoID + "'", connect);
            SqlDataReader dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                hoID = dr["ID"].ToString();
                name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                name1.Text = name;
                adds = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                address.Text = adds;
                contact.Text = dr["Contact"].ToString();
                type.Text = dr["HomeownerType"].ToString();
                totals1 = dr["OverallPay"].ToString();
                totals2 = dr["OverallHoaFeePay"].ToString();
                totfee.Text = "PHP " + dr["OverallHoaFeePay"].ToString();
                tot.Text = "PHP " + dr["OverallPay"].ToString();
                bal.Text = "PHP " + dr["RemainingBalance"].ToString();
                nextpay = (DateTime)dr["NextPaymentDate"];
                month.Text = nextpay.ToString("MMMM d, yyyy");
                nextFee = (DateTime)dr["NextHoaFeePayment"];
                monthFee.Text = nextFee.ToString("MMMM d, yyyy");
                amort.Text = "PHP " + dr["LastAmort"].ToString();
                lastamort = Convert.ToDouble(dr["LastAmort"].ToString());
                lastfee = Convert.ToDouble(dr["LastFee"].ToString());
                HoaFee.Text = "PHP " + dr["LastFee"].ToString();
                bals = dr["RemainingBalance"].ToString();
                funds = dr["Funds"].ToString();
                HOfunds = dr["HOFund"].ToString();
                paymentID = dr["PaymentID"].ToString();
                alreadyPaid = dr["isPaidOff"].ToString();
               



            }

            dr.Close();
            connect.Close();


            amortis.Text = "PHP " + amorts;
            hfees.Text = "PHP " + hoa;
            transacID.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            date.Text = DateTime.Now.ToShortDateString();

            bool AlPaid = Convert.ToBoolean(alreadyPaid);
            if (AlPaid == true)
            {
                label19.Visible = false;
                payment.Visible = false;
                TOTAL.Enabled = false;
                radMan.Checked = true;
                radMan.Visible = false;
                radAuto.Visible = false;
                fee.Enabled = true;
                add.Visible = false;
                label17.Visible = false;
                label15.Visible = false;
                label16.Visible = false;
                label23.Visible = false;
                amortis.Visible = false;
                bal.Visible = false;
                month.Visible = false;
                amort.Visible = false;
                congrats.Visible = true;
            }

        }

        private void Billing_Load(object sender, EventArgs e)
        {

        }

        private void HOTransac_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {

                 string all = "SELECT * " +
                             "FROM TBL_HORecords AS H " +
                             "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                             "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                             "WHERE T.TransactionID LIKE '" + HOTransac.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";

                    HOReceipt ho = new HOReceipt(this);
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
                        //DateTime dts = (DateTime)dr["TransacDate"];
                        ho.date.Text = dr["TransacDate"].ToString();
                        ho.paidAmort.Text = "PHP " + dr["TransacAmort"].ToString();
                        ho.paidFee.Text = "PHP " + dr["TransacFee"].ToString();
                        ho.amount.Text = "PHP " + dr["TransacAmount"].ToString();


                    }
                    dr.Close();
                    connect.Close();

                    ho.ShowDialog();

                }



            } catch (Exception ex) { MessageBox.Show("Error: " + ex); }
            

        }
        public void Pay() {
            try
            {
                string all = "SELECT * " +
                             "FROM TBL_HORecords AS H " +
                             "JOIN TBL_Payment AS P ON H.ID = P.HoID " +
                             "JOIN TBL_Transactions AS T ON P.PaymentID = T.PaymentID " +
                             "WHERE T.TransactionID LIKE '" + transacID.Text + "'";

                HOReceipt ho = new HOReceipt(this);
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
                    ho.paidAmort.Text = "PHP " + dr["TransacAmort"].ToString();
                    ho.paidFee.Text = "PHP " + dr["TransacFee"].ToString();
                    ho.amount.Text = "PHP " + dr["TransacAmount"].ToString();


                }
                dr.Close();
                connect.Close();

                ho.ShowDialog();
                




            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex); }

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            HOData.ClearSelection();
            payment.Text = "0";
            fee.Text = "0";
            TOTAL.Text = "0";
            add.Visible = false;
            pay.Enabled = false;
            radAuto.Checked = true;
        }


        private void addbutton_Click(object sender, EventArgs e)
        {
            try {
                double fees = double.Parse(fee.Text);
                if (double.TryParse(payment.Text, out double subtractionResult))
                {
                    if (subtractionResult >= 100 || subtractionResult < 100)
                    {
                        subtractionResult = subtractionResult - 50;
                    }
                    if (double.TryParse(fee.Text, out double currentFee))
                    { 
                       fees = fees + 50;
                        fee.Text = fees.ToString();

                    }

                    
                        
                        payment.Text = subtractionResult.ToString();
                    
                    

                    if (subtractionResult <= 50) {
                        add.Visible = false;
                    }
                }


            } catch { }
        }
       
        private void TOTAL_TextChanged(object sender, EventArgs e)
        {
            pay.Enabled = true;
            TOTAL.SelectionStart = TOTAL.Text.Length;
            double hoafees = Convert.ToDouble(hoa);
            double amortis = Convert.ToDouble(amorts);
            double times2 = hoafees * 2;
            double subtractionResult = 0;
            try
            {
                if (!isUpdatingTotal)
                {
                    double fees = 0;

                    if (double.TryParse(TOTAL.Text, out double total))
                    {
                        if (total >= times2)// >= 100
                        {
                            subtractionResult = total - hoafees;
                            payment.Text = subtractionResult.ToString();

                            fees = Math.Min(Math.Max(subtractionResult, 0), hoafees);
                            fee.Text = fees.ToString();

                            if (subtractionResult >= times2)
                            {
                                add.Visible = true;

                            }
                            else
                            {
                                add.Visible = false;

                            }
                        }

                        else
                        {
                            payment.Text = total.ToString();
                            fee.Text = "0";

                        }

                        if (total <= times2 || fees == 0)
                        {
                            add.Visible = false;
                            
                        }

                        


                    }
                    else
                    {
                        payment.Text = string.Empty;
                        fee.Text = string.Empty;
                    }



                }
            }
            catch { }


            /*  int p = int.Parse(payment.Text);
              int q = int.Parse(TOTAL.Text);

              if ( p >= int.Parse(hoa)) {
                  int s = q - int.Parse(hoa);
                  payment.Text = s.ToString();

              }*/

        }

        private void TOTAL_KeyPress(object sender, KeyPressEventArgs e)
        {
            double balance = Convert.ToDouble(bals) + Convert.ToDouble(hoa);
            try {
                // Check if the pressed key is a digit or a control character (e.g., Backspace)
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    TOTAL.SelectionStart = TOTAL.Text.Length;
                    // Cancel the keypress event
                    e.Handled = true;
                }
                else if (e.KeyChar == '\b') // Backspace key
                {
                    if (TOTAL.Text.Length == 0 || TOTAL.Text.Length == 1)
                    {
                        TOTAL.SelectionStart = TOTAL.Text.Length;
                        // Set the TextBox text to "0"
                        TOTAL.Text = "0";
                        payment.Text = "0";
                        pay.Enabled = false;
                        add.Visible = false;
                        // Cancel the keypress event
                        e.Handled = true;
                    }
                }
                else
                {
                    // If the TextBox text is "0", replace it with the pressed digit
                    if (TOTAL.Text == "0")
                    {
                        TOTAL.Text = e.KeyChar.ToString();
                        TOTAL.SelectionStart = TOTAL.Text.Length;
                        // Cancel the keypress event
                        e.Handled = true;
                    }
                    else
                    {
                        // Get the current payment value
                        double currentPayment = double.Parse(TOTAL.Text);

                        // Get the entered digit
                        double enteredDigit = double.Parse(e.KeyChar.ToString());

                        // Calculate the new payment value if the digit is added
                        double newPayment = currentPayment * 10 + enteredDigit;

                        if (newPayment > balance)
                        {
                            // Show a message box indicating that the limit is reached
                            MessageBox.Show("The input has reached the available amortization balance of PHP " + bals, "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Cancel the keypress event
                            e.Handled = true;
                        }
                    }

                }


            } catch { }
            
        }

        private void radAuto_CheckedChanged(object sender, EventArgs e)
        {
            TOTAL.Enabled = true;
            payment.Enabled = false;
            fee.Enabled = false;

        }

        private void radMan_CheckedChanged(object sender, EventArgs e)
        {
            TOTAL.Enabled = false;
            payment.Enabled = true;
            fee.Enabled = true;
            add.Visible = false;
        }

        private void payment_KeyPress(object sender, KeyPressEventArgs e)
        {
            double balance = Convert.ToDouble(bals);
            try {
                // Check if the pressed key is a digit or a control character (e.g., Backspace)
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    // Cancel the keypress event
                    e.Handled = true;
                }
                else if (e.KeyChar == '\b') // Backspace key
                {
                    if (payment.Text.Length == 0 || payment.Text.Length == 1)
                    {
                        // Set the TextBox text to "0"
                        payment.Text = "0";

                        add.Visible = false;
                        // Cancel the keypress event
                        e.Handled = true;
                    }
                }
                else
                {
                    // If the TextBox text is "0", replace it with the pressed digit
                    if (payment.Text == "0")
                    {
                        payment.Text = e.KeyChar.ToString();
                        payment.SelectionStart = payment.Text.Length;
                        // Cancel the keypress event
                        e.Handled = true;
                    }
                    else
                    {
                        // Get the current payment value
                        double currentPayment = double.Parse(payment.Text);

                        // Get the entered digit
                        double enteredDigit = double.Parse(e.KeyChar.ToString());

                        // Calculate the new payment value if the digit is added
                        double newPayment = currentPayment * 10 + enteredDigit;

                        if (newPayment > balance)
                        {
                            // Show a message box indicating that the limit is reached
                            MessageBox.Show("The input has reached the available amortization balance of PHP " + bals, "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Cancel the keypress event
                            e.Handled = true;
                        }
                    }
                }


            } catch { }
            

        }

        private void fee_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a digit or a control character (e.g., Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Cancel the keypress event
                e.Handled = true;
            }
            else if (e.KeyChar == '\b') // Backspace key
            {
                if (fee.Text.Length == 0 || fee.Text.Length == 1)
                {
                    // Set the TextBox text to "0"
                    fee.Text = "0";
                    
                    add.Visible = false;
                    // Cancel the keypress event
                    e.Handled = true;
                }
            }
            else
            {
                // If the TextBox text is "0", replace it with the pressed digit
                if (fee.Text == "0")
                {
                    fee.Text = e.KeyChar.ToString();
                    fee.SelectionStart = fee.Text.Length;
                    // Cancel the keypress event
                    e.Handled = true;
                }
            }
        }
        private void UpdateTotal()
        {
            isUpdatingTotal = true;
            if (double.TryParse(payment.Text, out double subtractionValue) &&
                double.TryParse(fee.Text, out double feeValue))
            {
                double total = subtractionValue + feeValue;
                TOTAL.Text = total.ToString();

                if (total == 0)
                {
                    pay.Enabled = false;

                }
            }
            else
            {
                TOTAL.Text = string.Empty;
            }
            isUpdatingTotal = false;
        }
        private bool isUpdatingTotal = false;
        private void payment_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void fee_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void month_Click(object sender, EventArgs e)
        {

        }

        private void amort_Click(object sender, EventArgs e)
        {
            payment.Text = lastamort.ToString(); 
        }

        private void HoaFee_Click(object sender, EventArgs e)
        {
            fee.Text = lastfee.ToString();
        }

        private void TOTAL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                // Check if the TextBox is empty or the entire text is selected
                if (TOTAL.Text.Length == 0 || (TOTAL.SelectionLength > 0 && TOTAL.SelectionLength == TOTAL.Text.Length))
                {
                    // Set the TextBox text to "0"
                    TOTAL.Text = "0";
                    // Cancel the key event
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void fee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                // Check if the TextBox is empty or the entire text is selected
                if (fee.Text.Length == 0 || (fee.SelectionLength > 0 && fee.SelectionLength == fee.Text.Length))
                {
                    // Set the TextBox text to "0"
                    fee.Text = "0";
                    // Cancel the key event
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void payment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                // Check if the TextBox is empty or the entire text is selected
                if (payment.Text.Length == 0 || (payment.SelectionLength > 0 && payment.SelectionLength == payment.Text.Length))
                {
                    // Set the TextBox text to "0"
                    payment.Text = "0";
                    // Cancel the key event
                    e.SuppressKeyPress = true;
                }
            }
        }
    }
}
