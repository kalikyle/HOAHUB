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
using System.IO;

namespace HOAMS
{
    public partial class HomeOwnerRecords : Form
    {
        public static string ID = "";
        public static string payID = "";

        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public HomeOwnerRecords()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            AddRecord rec = new AddRecord(this);
            rec.label26.Visible = false;
            rec.label27.Visible = false;
            rec.UpBTN.Visible = false;
            rec.delBTN.Visible = false;
           
            rec.dataGridView1.ReadOnly = true;
           
            rec.ShowDialog();
        }
        public void LoadRecords()
        {
            try
            {

                HOData.Rows.Clear();
                connect.Open();
                SqlCommand cmd = new SqlCommand("select * from TBL_HORecords where ID like '%" + txtSearch.Text + "%' OR Fname like '%" + txtSearch.Text + "%' OR Mname like '%" + txtSearch.Text + "%' OR Lname like '%" + txtSearch.Text + "%' OR Age like '%" + txtSearch.Text + "%' OR Contact like '%" + txtSearch.Text + "%' OR Street like '%" + txtSearch.Text + "%' OR HouseNum like '%" + txtSearch.Text + "%' OR Block like '%" + txtSearch.Text + "%' OR Lot like '%" + txtSearch.Text + "%'  ", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string name = dr["Fname"].ToString()+ " " +dr["Mname"].ToString() +" "+ dr["Lname"].ToString();
                    HOData.Rows.Add(dr["ID"].ToString(), name, dr["Age"].ToString(), dr["Contact"].ToString(), dr["Street"].ToString(), dr["HouseNum"].ToString(), dr["Block"].ToString(), dr["Lot"].ToString(), dr["HomeownerType"].ToString());


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
        public static Point hoPos;
        private void HOData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void HOData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }

        private void HOData_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {

                    HOInfo ho = new HOInfo(this);
                    ho.groupBox21.Visible = false;

                    /* if (ho == null || ho.IsDisposed)
                     {
                         ho = new HOInformation(); // create new instance of user control
                         ho.Name = "hoInformation1"; // set name of user control
                         ho.Dock = DockStyle.Fill;
                         ho.Location = hoPos;
                         this.ParentForm.Controls.Add(ho); // add user control to parent form's controls

                     }*/

                    // Get a reference to the existing HOInformations user control on the MainPage form

                    String name = "", add = "", acc = "";

                    connect.Open();
                    //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.*, TBL_Payment.* " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                                    "WHERE TBL_HORecords.ID LIKE '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {

                        if (dr.IsDBNull(0))
                        {
                            // Use a default image if the picture data is null
                            ho.HOPic.Image = ho.HOPic.Image;

                        }
                        else
                        {
                            long len = dr.GetBytes(0, 0, null, 0, 0);
                            byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                            dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                            MemoryStream ms = new MemoryStream(array);
                            Bitmap bit = new Bitmap(ms);
                            ho.HOPic.Image = bit;
                        }



                        ID = dr["ID"].ToString();
                        payID = dr["PaymentID"].ToString();
                        ho.fname.Text = dr["Fname"].ToString();
                        ho.mname.Text = dr["Mname"].ToString();
                        ho.lname.Text = dr["Lname"].ToString();
                        name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        ho.BigName.Text = name;
                        ho.BigCon.Text = dr["Contact"].ToString();
                        add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", "+ dr["Street"].ToString() + " St., Villa Balderama, \nNagpayong II, Pinagbuhatan, Pasig City";
                        ho.Address.Text = add;
                        ho.con.Text = dr["Contact"].ToString();
                        ho.email.Text = dr["Email"].ToString();
                        DateTime birthdate = (DateTime)dr["birthdate"];
                        ho.birthd.Text = birthdate.ToShortDateString();
                        ho.age.Text = dr["Age"].ToString();
                        ho.birthp.Text = dr["birthplace"].ToString();
                        ho.cilstat.Text = dr["CivilStatus"].ToString();
                        ho.rel.Text = dr["Religion"].ToString();
                        ho.street.Text = dr["Street"].ToString();
                        ho.HN.Text = dr["HouseNum"].ToString();
                        ho.blk.Text = dr["Block"].ToString();
                        ho.lot.Text = dr["lot"].ToString();
                        ho.label4.Text = dr["HomeownerType"].ToString();
                        ho.Usertxt.Text = dr["Username"].ToString();
                        ho.passTxt.Text = dr["Password"].ToString();
                        ho.label13.Text = dr["Acc_Status"].ToString() + " ACCOUNT";

                        DateTime FP = (DateTime)dr["NextHoaFeePayment"];
                        ho.label33.Text = FP.ToString("MMMM d, yyyy");
                        ho.label31.Text = "PHP " + dr["OverallHoaFeePay"].ToString();
                        DateTime PD = (DateTime)dr["NextPaymentDate"];
                        ho.label32.Text = PD.ToString("MMMM d, yyyy");
                        ho.label30.Text = "PHP " + dr["RemainingBalance"].ToString();
                        ho.label29.Text = "PHP " + dr["OverallPay"].ToString();

                        acc = dr["HomeownerType"].ToString();



                    }




                    dr.Close();
                    connect.Close();
                    
                    inFopanel.Visible = true;
                    if (acc.Equals("FORMER HOMEOWNER"))
                    {
                        ho.UpdateBtn.Visible = false;
                        ho.button4.Visible = true;
                    }


                    ho.LogoutBtn.Visible = false;
                    ho.editLbl.Visible = false;
                    ho.cpasslbl.Visible = false;
                    ho.label3.Visible = false;
                    ho.tabs.TabPages.Remove(ho.tabPage2);
                    ho.TopLevel = false;
                    ho.Dock = DockStyle.Fill;
                    inFopanel.Controls.Clear();
                    inFopanel.Controls.Add(ho);
                    ho.payID = payID;
                    ho.LoadTransacs();
                    ho.BringToFront();
                    ho.Show();
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void HOData_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
