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
    public partial class Complaints : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        ComplainLogs c;
        public Complaints(ComplainLogs c)
        {
            InitializeComponent();
            this.c = c;
            HOAccount();
        }
        public void HOAccount()
        {
            try
            {




                String name = "", add = "";


                connect.Open();
                SqlCommand cm = new SqlCommand("SELECT * " +
                               "FROM TBL_HORecords " +
                               "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                               "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                               "WHERE TBL_HORecords.ID LIKE '" + HomeOwnerRecords.ID + "'", connect);
                
                SqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {

                   
                    name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    nametxt.Text = name;
                    con.Text = dr["Contact"].ToString();
                    add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                    addT.Text = add;
                    
                }



                DateTime dt = DateTime.Now;
                date.Text = dt.ToString("MMMM d, yyyy");
                dr.Close();
                connect.Close();



            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
          



        }

        private void button2_Click(object sender, EventArgs e)
        {
            try {


                connect.Open();
                string command2 = "insert into TBL_Complaint (ComplainSubj, ComplainMsg, ComplainDate, ComplainStatus, HoID) values (@ComplainSubj, @ComplainMsg, @ComplainDate, @ComplainStatus,@HoID)";
                SqlCommand usercm1 = new SqlCommand(command2, connect);
                usercm1.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                usercm1.Parameters.AddWithValue("@ComplainSubj", subject.Text);
                usercm1.Parameters.AddWithValue("@ComplainMsg", msg.Text);
                usercm1.Parameters.AddWithValue("@ComplainDate", DateTime.Now);
                usercm1.Parameters.AddWithValue("@ComplainStatus", "PENDING");
                usercm1.ExecuteNonQuery();
                connect.Close();

                MessageBox.Show("Your Complain has been submitted to the Officers.");
                subject.Text = "";
                msg.Text = "";

                c.LoadComplains();
                c.inFopanel.Hide();
                this.Dispose();


            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            subject.Text = "";
            msg.Text = "";
            c.inFopanel.Hide();
           // c.inFopanel.Dock = DockStyle.Bottom;
            this.Dispose();
        }
    }
}
