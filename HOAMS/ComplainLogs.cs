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
    public partial class ComplainLogs : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public ComplainLogs()
        {
            InitializeComponent();
            LoadComplains();
            
        }
        public string comstats = null;
        public string complainID = null;
        public void LoadComplains()
        {
            string names = null;
            try
            {
                HOcomplains.Rows.Clear();
                connect.Open();


                string all = "SELECT * " +
               "FROM TBL_HORecords AS H " +
               "JOIN TBL_Complaint AS C ON H.ID = C.HoID where H.ID = @HoID";
                SqlCommand cmd = new SqlCommand(all, connect);
                cmd.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    names = dr["Fname"].ToString() + " " + dr["Lname"].ToString();
                    HOcomplains.Rows.Add(dr["ComplainID"].ToString(), names, dr["ComplainSubj"].ToString(), dr["ComplainDate"].ToString(), dr["ComplainStatus"].ToString());
                    

                }
                dr.Close();
                connect.Close();
                HOcomplains.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Complaints cm = new Complaints(this);
            inFopanel.Visible = true;
            cm.Dock = DockStyle.Fill;
            cm.TopLevel = false;
            inFopanel.Controls.Clear();
            inFopanel.Controls.Add(cm);
            cm.BringToFront();
            cm.Show();
        }

        private void HOcomplains_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void HOcomplains_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            adminComplain ad = new adminComplain();
            try
            {

                if (e.RowIndex > -1 && e.ColumnIndex >= 0)
                {
                    ComplainInfo ho = new ComplainInfo(this,ad);

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
                        complainID = dr["ComplainID"].ToString();
                        comstats = dr["ComplainStatus"].ToString();
                    }
                        connect.Close();

                    inFopanel.Visible = true;
                  if (comstats.Equals("SOLVED"))
                    {
                        ho.button2.Visible = false;
                    }
                    else
                    {
                        ho.button2.Visible = true;
                    }
                  

                    ho.TopLevel = false;
                    ho.Dock = DockStyle.Fill;
                    inFopanel.Controls.Clear();
                    inFopanel.Controls.Add(ho);
                    ho.BringToFront();
                    ho.Show();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }
    }
}
