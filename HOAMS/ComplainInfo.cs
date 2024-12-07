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
    public partial class ComplainInfo : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        adminComplain ad;
        ComplainLogs c;
        
        public ComplainInfo(ComplainLogs c, adminComplain ad)
        {
            InitializeComponent();
            this.c = c;
            this.ad = ad;
            
        }
       
        private void ComplainInfo_Load(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ad.infoPanel.Hide();
            ad.panel4.Visible = false;
            c.inFopanel.Hide();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            try {

                DialogResult diagres = MessageBox.Show("Do you want to Delete this Complain?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {
                    connect.Open();
                    SqlCommand cm5 = new SqlCommand("delete from TBL_Complaint where ComplainID = @id", connect);
                    cm5.Parameters.AddWithValue("@id", c.complainID);//delete from TBL_Relatives
                    cm5.ExecuteNonQuery();
                    connect.Close();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
            c.LoadComplains();
            c.inFopanel.Hide();
            this.Dispose();
        }
        public void loadinfo()
        {
            try
            {
               

                connect.Open();
                string all = "SELECT * " +
                "FROM TBL_HORecords AS H " +
                "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE C.ComplainID = @comID ";
                SqlCommand cm = new SqlCommand(all, connect);
                cm.Parameters.AddWithValue("@comID", adminComplain.complainID);
                SqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    string names = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    honame.Text = names;
                    string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                    addr.Text = add;
                    cont.Text = dr["Contact"].ToString();
                    DateTime dt = (DateTime)dr["ComplainDate"];
                    subd.Text = dt.ToString("MMMM d, yyyy");
                    subj.Text = dr["ComplainSubj"].ToString();
                    msg.Text = dr["ComplainMsg"].ToString();
                    stats.Text = dr["ComplainStatus"].ToString();
                    offname.Text = dr["OfficerName"].ToString();
                    update.Text = dr["UpdateDate"].ToString();
                    sln.Text = dr["solution"].ToString();


                }
                connect.Close();





            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void accbtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                

                connect.Open();
                string command2 = "update TBL_Complaint set ComplainStatus = @ComplainStatus, OfficerName = @OfficerName, UpdateDate = @UpdateDate where ComplainID = @comID";
                SqlCommand usercm1 = new SqlCommand(command2, connect);
                usercm1.Parameters.AddWithValue("@comID", adminComplain.complainID);
                usercm1.Parameters.AddWithValue("@OfficerName", LOGIN.fname + " " + LOGIN.lname);
                usercm1.Parameters.AddWithValue("@ComplainStatus", "ON-GOING");
                usercm1.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                usercm1.ExecuteNonQuery();
                connect.Close();

                MessageBox.Show("The complain are now On-Going");

                loadinfo();
                this.TopLevel = false;
                 this.Dock = DockStyle.Fill;
                 this.panel1.Visible = false;
                 this.button2.Visible = false;
                 this.button1.Visible = false;
                 this.accbtn.Visible = true;
                 this.rejctbtn.Visible = true;
                 ad.infoPanel.Visible = true;
                 ad.infoPanel.Controls.Clear();
                 ad.infoPanel.Controls.Add(this);
                 ad.LoadComplains();
                 this.BringToFront();
                 this.Show();
               
               
                



            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void rejctbtn_Click(object sender, EventArgs e)
        {
            try
            {


                connect.Open();
                string command2 = "update TBL_Complaint set ComplainStatus = @ComplainStatus, OfficerName = @OfficerName, UpdateDate = @UpdateDate where ComplainID = @comID";
                SqlCommand usercm1 = new SqlCommand(command2, connect);
                usercm1.Parameters.AddWithValue("@comID", adminComplain.complainID);
                usercm1.Parameters.AddWithValue("@OfficerName", LOGIN.fname + " " + LOGIN.lname);
                usercm1.Parameters.AddWithValue("@ComplainStatus", "REJECT");
                usercm1.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                usercm1.ExecuteNonQuery();
                connect.Close();

                MessageBox.Show("The complain has been Rejected");

                loadinfo();
                this.TopLevel = false;
                this.Dock = DockStyle.Fill;
                this.panel1.Visible = false;
                this.button2.Visible = false;
                this.button1.Visible = false;
                this.accbtn.Visible = true;
                this.rejctbtn.Visible = true;
                ad.infoPanel.Visible = true;
                ad.infoPanel.Controls.Clear();
                ad.infoPanel.Controls.Add(this);
                ad.LoadComplains();
                this.BringToFront();
                this.Show();






            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void sln_TextChanged(object sender, EventArgs e)
        {
            if (sln.Text.Equals(""))
            {
                finshbtn.Visible = false;
            }
            else {
                finshbtn.Visible = true;
            }
            
        }

        private void finshbtn_Click(object sender, EventArgs e)
        {
            try
            {


                connect.Open();
                string command2 = "update TBL_Complaint set ComplainStatus = @ComplainStatus, OfficerName = @OfficerName, UpdateDate = @UpdateDate, solution = @solution where ComplainID = @comID";
                SqlCommand usercm1 = new SqlCommand(command2, connect);
                usercm1.Parameters.AddWithValue("@comID", adminComplain.complainID);
                usercm1.Parameters.AddWithValue("@OfficerName", LOGIN.fname + " " + LOGIN.lname);
                usercm1.Parameters.AddWithValue("@ComplainStatus", "SOLVED");
                usercm1.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                usercm1.Parameters.AddWithValue("@solution", sln.Text);
                usercm1.ExecuteNonQuery();
                connect.Close();

                MessageBox.Show("The complain has been finished");

                loadinfo();
                this.TopLevel = false;
                this.Dock = DockStyle.Fill;
                this.panel1.Visible = false;
                this.button2.Visible = false;
                this.button1.Visible = false;
                this.accbtn.Visible = false;
                this.rejctbtn.Visible = false;
                this.finshbtn.Visible = false;
                this.sln.Enabled = false;
                ad.infoPanel.Visible = true;
                ad.infoPanel.Controls.Clear();
                ad.infoPanel.Controls.Add(this);
                ad.LoadComplains();
                this.BringToFront();
                this.Show();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }
    }
}
