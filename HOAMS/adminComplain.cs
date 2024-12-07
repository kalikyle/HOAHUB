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
    public partial class adminComplain : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public static string complainID = null;
        public adminComplain()
        {
            InitializeComponent();
            LoadComplains();
        }
       

        public void LoadComplains()
        {
            string names = null;
            try
            {
                HOcomplains.Rows.Clear();
                connect.Open();


                string all = "SELECT * " +
               "FROM TBL_HORecords AS H " +
               "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE ComplainStatus = 'PENDING'";
                SqlCommand cmd = new SqlCommand(all, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    names = dr["Fname"].ToString() + " " + dr["Lname"].ToString();
                    HOcomplains.Rows.Add(dr["ComplainID"].ToString(), names, dr["ComplainSubj"].ToString(), dr["ComplainDate"].ToString(), dr["ComplainStatus"].ToString());


                }
                dr.Close();
                connect.Close();
                HOcomplains.ClearSelection();

                // ongoing
                HOgoing.Rows.Clear();
                connect.Open();


                string all1 = "SELECT * " +
               "FROM TBL_HORecords AS H " +
               "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE ComplainStatus = 'ON-GOING'";
                SqlCommand cmd1 = new SqlCommand(all1, connect);
                SqlDataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {

                    names = dr1["Fname"].ToString() + " " + dr1["Lname"].ToString();
                    HOgoing.Rows.Add(dr1["ComplainID"].ToString(), names, dr1["ComplainSubj"].ToString(), dr1["ComplainDate"].ToString(), dr1["ComplainStatus"].ToString());


                }
                dr1.Close();
                connect.Close();
                HOgoing.ClearSelection();

                // finish
                HOfinish.Rows.Clear();
                connect.Open();


                string all2 = "SELECT * " +
               "FROM TBL_HORecords AS H " +
               "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE ComplainStatus = 'SOLVED'";
                SqlCommand cmd2 = new SqlCommand(all2, connect);
                SqlDataReader dr2 = cmd2.ExecuteReader();
                while (dr2.Read())
                {

                    names = dr2["Fname"].ToString() + " " + dr2["Lname"].ToString();
                    HOfinish.Rows.Add(dr2["ComplainID"].ToString(), names, dr2["ComplainSubj"].ToString(), dr2["ComplainDate"].ToString(), dr2["ComplainStatus"].ToString());


                }
                dr2.Close();
                connect.Close();
                HOfinish.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }

        private void HOcomplains_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComplainLogs CL = new ComplainLogs();
            panel4.Visible = true;
           
            try
            {

                if (e.RowIndex > -1 && e.ColumnIndex >= 0)
                {
                    ComplainInfo ho = new ComplainInfo(CL,this);

                    connect.Open();
                    string all = "SELECT * " +
                    "FROM TBL_HORecords AS H " +
                    "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE C.ComplainID LIKE '" + HOcomplains.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";
                    SqlCommand cm = new SqlCommand(all, connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        string names = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        ho.honame.Text = names;
                        string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                        ho.addr.Text = add;
                        ho.cont.Text = dr["Contact"].ToString();
                        DateTime dt = (DateTime)dr["ComplainDate"];
                        ho.subd.Text = dt.ToString("MMMM d, yyyy");
                        ho.subj.Text = dr["ComplainSubj"].ToString();
                        ho.msg.Text = dr["ComplainMsg"].ToString();
                        ho.stats.Text = dr["ComplainStatus"].ToString();
                        ho.offname.Text = dr["OfficerName"].ToString();
                        ho.update.Text = dr["UpdateDate"].ToString();
                        ho.sln.Text = dr["solution"].ToString();
                        complainID = HOcomplains.Rows[e.RowIndex].Cells[0].Value.ToString();

                    }
                    connect.Close();

                    
                    ho.TopLevel = false;
                    ho.Dock = DockStyle.Fill;
                    ho.panel1.Visible = false;
                    ho.button2.Visible = false;
                    ho.button1.Visible = true;
                    ho.accbtn.Visible = true;
                    ho.rejctbtn.Visible = true;
                    infoPanel.Visible = true;  
                    infoPanel.Controls.Clear();
                    infoPanel.Controls.Add(ho);
                    ho.BringToFront();
                    ho.Show();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void HOgoing_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComplainLogs CL = new ComplainLogs();
            panel4.Visible = true;
            try
            {

                if (e.RowIndex > -1 && e.ColumnIndex >= 0)
                {
                    ComplainInfo ho = new ComplainInfo(CL, this);

                    connect.Open();
                    string all = "SELECT * " +
                    "FROM TBL_HORecords AS H " +
                    "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE C.ComplainID LIKE '" + HOgoing.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";
                    SqlCommand cm = new SqlCommand(all, connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        string names = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        ho.honame.Text = names;
                        string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                        ho.addr.Text = add;
                        ho.cont.Text = dr["Contact"].ToString();
                        DateTime dt = (DateTime)dr["ComplainDate"];
                        ho.subd.Text = dt.ToString("MMMM d, yyyy");
                        ho.subj.Text = dr["ComplainSubj"].ToString();
                        ho.msg.Text = dr["ComplainMsg"].ToString();
                        ho.stats.Text = dr["ComplainStatus"].ToString();
                        ho.offname.Text = dr["OfficerName"].ToString();
                        ho.update.Text = dr["UpdateDate"].ToString();
                        ho.sln.Text = dr["solution"].ToString();
                        complainID = HOgoing.Rows[e.RowIndex].Cells[0].Value.ToString();

                    }
                    connect.Close();


                    ho.TopLevel = false;
                    ho.Dock = DockStyle.Fill;
                    ho.panel1.Visible = false;
                    ho.button2.Visible = false;
                    ho.button1.Visible = true;
                    ho.accbtn.Visible = false;
                    ho.rejctbtn.Visible = false;
                    ho.finshbtn.Visible = false;
                    ho.sln.Enabled = true;
                    infoPanel.Visible = true;
                    infoPanel.Controls.Clear();
                    infoPanel.Controls.Add(ho);
                    ho.BringToFront();
                    ho.Show();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void HOfinish_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComplainLogs CL = new ComplainLogs();
            panel4.Visible = true;
            try
            {

                if (e.RowIndex > -1 && e.ColumnIndex >= 0)
                {
                    ComplainInfo ho = new ComplainInfo(CL, this);

                    connect.Open();
                    string all = "SELECT * " +
                    "FROM TBL_HORecords AS H " +
                    "JOIN TBL_Complaint AS C ON H.ID = C.HoID WHERE C.ComplainID LIKE '" + HOfinish.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";
                    SqlCommand cm = new SqlCommand(all, connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        string names = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        ho.honame.Text = names;
                        string add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                        ho.addr.Text = add;
                        ho.cont.Text = dr["Contact"].ToString();
                        DateTime dt = (DateTime)dr["ComplainDate"];
                        ho.subd.Text = dt.ToString("MMMM d, yyyy");
                        ho.subj.Text = dr["ComplainSubj"].ToString();
                        ho.msg.Text = dr["ComplainMsg"].ToString();
                        ho.stats.Text = dr["ComplainStatus"].ToString();
                        ho.offname.Text = dr["OfficerName"].ToString();
                        ho.update.Text = dr["UpdateDate"].ToString();
                        ho.sln.Text = dr["solution"].ToString();
                        complainID = HOfinish.Rows[e.RowIndex].Cells[0].Value.ToString();

                    }
                    connect.Close();


                    ho.TopLevel = false;
                    ho.Dock = DockStyle.Fill;
                    ho.panel1.Visible = false;
                    ho.button2.Visible = false;
                    ho.button1.Visible = true;
                    ho.accbtn.Visible = false;
                    ho.rejctbtn.Visible = false;
                    ho.sln.Enabled = false;
                    infoPanel.Visible = true;
                    infoPanel.Controls.Clear();
                    infoPanel.Controls.Add(ho);
                    ho.BringToFront();
                    ho.Show();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
           
        }

        private void panel5_TabIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
