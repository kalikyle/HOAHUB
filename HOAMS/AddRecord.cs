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
using Microsoft.Reporting.WinForms;
using System.Reflection;

namespace HOAMS
{
    public partial class AddRecord : Form
    {
        HomeOwnerRecords f;

        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public AddRecord(HomeOwnerRecords f)
        {
            InitializeComponent();
            
            this.f = f;
            panel3.Enabled = false;
            label29.Visible = false;
            textBox2.Visible = false;
            label34.Visible = true;
            label35.Visible = true;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            label22.Visible = false;
            nextpayment.Visible = false;
            LoadRecords();
            Loads();
        }
        string imageloc = "";
        public void LoadRecords()
        {
            try
            {

                HOData.Rows.Clear();
                connect.Open();
                String s1 = "SELECT TBL_HORecords.*, TBL_Payment.*, TBL_User.* " +
                                   "FROM TBL_HORecords " +
                                   "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                   "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID";
                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string stats = dr["Acc_Status"].ToString();
                    string type = dr["HomeownerType"].ToString();

                    if (stats.Equals("ACTIVE") || type != "FORMER HOMEOWNER")
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        HOData.Rows.Add(dr["ID"].ToString(), name, dr["HouseNum"].ToString(), "PHP " + dr["RemainingBalance"].ToString());
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
        
        
        public void UpdateHomeowner()
        {
            try {
               

                // Insert data into tbl_User table
                string command1 = "update TBL_User set Acc_Status = @Acc_Status where Acc_ID = @AccID";
                SqlCommand usercmd = new SqlCommand(command1, connect);
                usercmd.Parameters.AddWithValue("@AccID", HoId);
                usercmd.Parameters.AddWithValue("@Acc_Status", "DEACTIVATED");
                usercmd.ExecuteNonQuery();


                string command2 = "update TBL_HORecords set HomeownerType = @HomeownerType where ID = @AccID";
                SqlCommand usercmd1 = new SqlCommand(command2, connect);
                usercmd1.Parameters.AddWithValue("@AccID", HoId);
                usercmd1.Parameters.AddWithValue("@HomeownerType", "FORMER HOMEOWNER");
                usercmd1.ExecuteNonQuery();

                SqlCommand cm5 = new SqlCommand("delete from TBL_Relatives where HoID = @id", connect);
                cm5.Parameters.AddWithValue("@id", HoId);//delete from TBL_Relatives
                cm5.ExecuteNonQuery();


            } catch(Exception ex ) { MessageBox.Show("Error: " + ex); }
        
        
        
        }
        public string subname = null;
        public string st = null;
        public string blk = null;
        public string lt = null;
        private void button4_Click(object sender, EventArgs e)
        {
            LOGIN l = new LOGIN();
            Report r = new Report();
            

            try
            {
                DialogResult diagres = MessageBox.Show("Do You want to Add this Homeowner?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {

                    byte[] images = null;
                    if (!string.IsNullOrEmpty(imageloc))// imagelocation path is not empty 
                    {
                        //comvert image to binary
                        FileStream Streams = new FileStream(imageloc, FileMode.Open, FileAccess.Read);
                        BinaryReader bin = new BinaryReader(Streams);
                        images = bin.ReadBytes((int)Streams.Length);
                    }

                    // Add an Values into the table in SSMS
                    connect.Open();

                    string command = "insert into TBL_HORecords (Fname, Mname, Lname, birthdate, birthplace, Age, CivilStatus, Gender, Religion, Occupation, Contact, Email, Street, HouseNum, Block, Lot, Picture, HomeownerType) values (@Fname, @Mname, @Lname, @birthday, @birthplace, @Age, @CivilStatus, @Gender, @Religion, @Occupation, @Contact, @Email, @Street, @HouseNum, @Block, @Lot, @picture,@HomeownerType)";
                    SqlCommand cmd = new SqlCommand(command, connect);


                    cmd.Parameters.AddWithValue("@Fname", Fname.Text);
                    cmd.Parameters.AddWithValue("@Mname", Mname.Text);
                    cmd.Parameters.AddWithValue("@Lname", Lname.Text);
                    cmd.Parameters.AddWithValue("@birthday", BirthD.Value);
                    cmd.Parameters.AddWithValue("@birthplace", BirthP.Text);
                    cmd.Parameters.AddWithValue("@Age", Age.Text);
                    cmd.Parameters.AddWithValue("@CivilStatus", Civil.Text);
                    cmd.Parameters.AddWithValue("@Gender", Sex.Text);
                    cmd.Parameters.AddWithValue("@Religion", Religion.Text);
                    cmd.Parameters.AddWithValue("@Occupation", Occ.Text);
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text);
                    cmd.Parameters.AddWithValue("@Email", Email.Text);
                    cmd.Parameters.AddWithValue("@Street", Street.Text);
                    cmd.Parameters.AddWithValue("@HouseNum", HouseNum.Text);
                    cmd.Parameters.AddWithValue("@Block", Block.Text);
                    cmd.Parameters.AddWithValue("@Lot", Lot.Text);

                    if (radNew.Checked)
                    {
                        cmd.Parameters.AddWithValue("@HomeownerType", "NEW HOMEOWNER");

                    }
                    else if (radSub.Checked)
                    {
                        
                        cmd.Parameters.AddWithValue("@HomeownerType", "SUBITTUTE HOMEOWNER");
                        subname = Fname.Text + " " + Mname.Text + " " + Lname.Text;
                        st = Street.Text;
                        blk = Block.Text;
                        lt = Lot.Text;
                       
                        UpdateHomeowner();
                    }
                    else if (radEx.Checked)
                    {
                        cmd.Parameters.AddWithValue("@HomeownerType", "EXISTING HOMEOWNER");
                    }


                    if (images != null)//if images is not null 
                    {
                        cmd.Parameters.Add("@picture", SqlDbType.Image).Value = images;
                    }
                    else
                    {//if null
                        cmd.Parameters.Add("@picture", SqlDbType.Image).Value = DBNull.Value;
                    }
                    cmd.ExecuteNonQuery();

                    string username = Lname.Text + HouseNum.Text;
                    string password = "QWERTY1234";

                    // Insert data into tbl_User table
                    string command1 = "insert into tbl_User (FirstName, LastName, Username, Password, Picture, Acc_Type, Acc_Status, Acc_ID) values (@FirstName, @LastName, @username, @password, @picture, @type, @Acc_Status, (select max(ID) from TBL_HORecords))";
                    SqlCommand usercmd = new SqlCommand(command1, connect);
                    usercmd.Parameters.AddWithValue("@FirstName", Fname.Text);
                    usercmd.Parameters.AddWithValue("@LastName", Lname.Text);
                    usercmd.Parameters.AddWithValue("@username", username);
                    usercmd.Parameters.AddWithValue("@password", password);
                    usercmd.Parameters.AddWithValue("@type", "HOMEOWNER");
                    usercmd.Parameters.AddWithValue("@Acc_Status", "ACTIVE");

                    if (images != null)
                    {
                        usercmd.Parameters.Add("@picture", SqlDbType.Image).Value = images;
                    }
                    else
                    {
                        usercmd.Parameters.Add("@picture", SqlDbType.Image).Value = DBNull.Value;
                    }

                    usercmd.ExecuteNonQuery();

                    DateTime nextPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 7);

                    //insert data in payment
                    string command2 = "insert into TBL_Payment (OverallPay, PaymentAmount, PaymentDate, RemainingBalance, NextPaymentDate, isPaidOff, LastAmort, Funds, FeePaymentAmount, NextHoaFeePayment, LastFee, HOFund, OverallHoaFeePay, isOverDue, isOverDueFee, HoID) values (@OverallPay, @PaymentAmount, @PaymentDate, @RemainingBalance, @NextPaymentDate, @isPaidOff, @LastAmort, @Funds, @FeePaymentAmount, @NextHoaFeePayment, @LastFee, @HOFund, @OverallHoaFeePay, @isOverDue, @isOverDueFee, (select max(ID) from TBL_HORecords))";
                    SqlCommand usercm1 = new SqlCommand(command2, connect);
                    usercm1.Parameters.AddWithValue("@OverallPay", 0);
                    usercm1.Parameters.AddWithValue("@PaymentAmount", 0);
                    if (radNew.Checked)
                    {
                        usercm1.Parameters.AddWithValue("@RemainingBalance", total);
                        usercm1.Parameters.AddWithValue("@NextPaymentDate", nextPaymentDate.AddMonths(1));
                    }
                    else if (radSub.Checked)
                    {
                        usercm1.Parameters.AddWithValue("@RemainingBalance", Convert.ToDouble(textBox1.Text));
                        usercm1.Parameters.AddWithValue("@NextPaymentDate", nextpaymentd.ToString());

                        


                        //Configure the report viewer

                        string reportPath = "HOAMS.SubtituteHomeowner.rdlc";
                        using (Stream reportStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath))
                        {
                            r.reportViewer1.LocalReport.LoadReportDefinition(reportStream);
                        }


                        // r.reportViewer1.LocalReport.ReportPath = "C:\\Users\\Garage88fort\\source\\repos\\HOAMS\\HOAMS\\AmortizationReport.rdlc";
                        r.reportViewer1.LocalReport.DataSources.Clear();

                        ReportParameter Date = new ReportParameter("Date", DateTime.Now.ToShortDateString());
                        r.reportViewer1.LocalReport.SetParameters(Date);

                        ReportParameter User = new ReportParameter("User", LOGIN.fname + " " + LOGIN.lname);
                        r.reportViewer1.LocalReport.SetParameters(User);

                        ReportParameter Former = new ReportParameter("Former", forname);
                        r.reportViewer1.LocalReport.SetParameters(Former);

                        ReportParameter Subtitute = new ReportParameter("Subtitute", subname);
                        r.reportViewer1.LocalReport.SetParameters(Subtitute);

                        ReportParameter Street = new ReportParameter("Street", st);
                        r.reportViewer1.LocalReport.SetParameters(Street);

                        ReportParameter blks = new ReportParameter("blk", blk);
                        r.reportViewer1.LocalReport.SetParameters(blks);

                        ReportParameter lot = new ReportParameter("lot", lt);
                        r.reportViewer1.LocalReport.SetParameters(lot);

                        ReportParameter President = new ReportParameter("President", " "); //needed
                        r.reportViewer1.LocalReport.SetParameters(President);

                        ReportParameter Vice = new ReportParameter("Vice", " "); //needed
                        r.reportViewer1.LocalReport.SetParameters(Vice);

                        ReportParameter Secretary = new ReportParameter("Secretary", " "); //needed
                        r.reportViewer1.LocalReport.SetParameters(Secretary);

                        ReportParameter Auditor = new ReportParameter("Auditor", " "); //needed
                        r.reportViewer1.LocalReport.SetParameters(Auditor);

                        ReportParameter Treasurer = new ReportParameter("Treasurer", " "); //needed
                        r.reportViewer1.LocalReport.SetParameters(Treasurer);

                        // Serialize the report data as a binary object
                        byte[] reportData;
                        string mimeType, encoding, extension;
                        string[] streamIds;
                        Warning[] warnings;

                        reportData = r.reportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                        {
                            connection.Open();
                            string insertQuery = "INSERT INTO TBL_Subtitute (FormerHO, SubHO, Street, Block, Lot, SubDate, SubReport) VALUES (@FormerHO, @SubHO, @Street, @Block, @Lot, @SubDate, @SubReport)";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@SubReport", reportData);
                            insertCommand.Parameters.AddWithValue("@FormerHO", forname);
                            insertCommand.Parameters.AddWithValue("@SubHO", subname);
                            insertCommand.Parameters.AddWithValue("@Street", st);
                            insertCommand.Parameters.AddWithValue("@Block", blk);
                            insertCommand.Parameters.AddWithValue("@Lot", lt);
                            insertCommand.Parameters.AddWithValue("@SubDate", DateTime.Now);
                            insertCommand.ExecuteNonQuery();
                        }

                    }
                    else if (radEx.Checked)
                    {
                        usercm1.Parameters.AddWithValue("@RemainingBalance", Convert.ToDouble(textBox1.Text));
                        usercm1.Parameters.AddWithValue("@NextPaymentDate", nextpayment.Value.ToString());
                    }
                   
                    usercm1.Parameters.AddWithValue("@isPaidOff", 0);
                    usercm1.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                    usercm1.Parameters.AddWithValue("@LastAmort", Convert.ToDouble(amort));
                    usercm1.Parameters.AddWithValue("@Funds", 0);
                    usercm1.Parameters.AddWithValue("@FeePaymentAmount", 0);
                    usercm1.Parameters.AddWithValue("@NextHoaFeePayment", nextPaymentDate.AddMonths(1));
                    usercm1.Parameters.AddWithValue("@LastFee", Convert.ToDouble(fee));
                    usercm1.Parameters.AddWithValue("@HOFund", 0);
                    usercm1.Parameters.AddWithValue("@OverallHoaFeePay", 0);
                    usercm1.Parameters.AddWithValue("@isOverDue", Convert.ToInt32(isoverdue));
                    usercm1.Parameters.AddWithValue("@isOverDueFee", 0);
                    usercm1.ExecuteNonQuery();


                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Skip the last row if it is the new row
                        if (!row.IsNewRow)
                        {
                            // Retrieve the cell values from the row and create the SQL INSERT statement
                            string first = row.Cells["first"].Value.ToString();
                            string middle = row.Cells["MidName"].Value.ToString();
                            string last = row.Cells["LastName"].Value.ToString();
                            string date = row.Cells["BD"].Value.ToString();
                            string age = row.Cells["Ages"].Value.ToString();
                            string con = row.Cells["cont"].Value.ToString();

                            string query = $"INSERT INTO TBL_Relatives (FName, MName, LName, Age, Birthday, Contact, HoID ) VALUES ('{first}', '{middle}', '{last}', '{age}', '{date}', '{con}', (select max(ID) from TBL_HORecords))";

                            // Insert into the database
                            using (SqlCommand commands = new SqlCommand(query, connect))
                            {
                                commands.ExecuteNonQuery();
                            }
                        }

                    }
                    LOGIN.UpdateIsOverDue();
                    LOGIN.UpdateIsOverDueFee();
                    LOGIN.GetAccStatus();
                    LOGIN.UpdateActivation();
                    LOGIN.UpdateAccStatusOnHomeownerType();

                    LOGIN.LogActivity("Added a New Homeowner Named: " + Fname.Text + " " + Lname.Text);
                    MessageBox.Show(string.Format("New Homeowner has been saved!\nUsername: {0}\nPassword: {1}", username, password));
                    connect.Close();

                    if (radSub.Checked)
                    {
                        DialogResult diagres1 = MessageBox.Show("Do You want to see the created Homeowner Report?", "Confirm", MessageBoxButtons.YesNo);
                        if (diagres1 == DialogResult.Yes)
                        {
                            r.ShowDialog();
                        }
                    }


                    HOInfo m = new HOInfo(f);
                    m.acc();
                    m.TopLevel = false;
                    m.Dock = DockStyle.Fill;
                    f.inFopanel.Controls.Clear();
                    f.inFopanel.Controls.Add(m);
                    f.LoadRecords();
                    this.Hide();
                }
                else
                {
                    clearForm();

                }
            }


            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //browsing a picture
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files(*.png)|*.png|jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imageloc = dialog.FileName.ToString();
                HOPic.ImageLocation = imageloc;
            }
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void AddRecord_Load(object sender, EventArgs e)
        {
            this.ActiveControl = panel2;//stop focusing in textboxes
        }

        private void UpBTN_Click(object sender, EventArgs e)
        {
            HOInfo m = new HOInfo(f);
            try
            {
                DialogResult diagres = MessageBox.Show("Update the informations?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {
                    byte[] images = null;
                    bool isImageUpdated = false;

                    // Check if a new image has been selected by the user
                    if (!string.IsNullOrEmpty(imageloc))
                    {
                        // Read the new image from disk
                        using (var stream = new FileStream(imageloc, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                images = reader.ReadBytes((int)stream.Length);
                                isImageUpdated = true;
                            }
                        }
                    }
                    /* if (!string.IsNullOrEmpty(imageloc))// imagelocation path is not empty 
                      {
                          //comvert image to binary
                          FileStream Streams = new FileStream(imageloc, FileMode.Open, FileAccess.Read);
                          BinaryReader bin = new BinaryReader(Streams);
                          images = bin.ReadBytes((int)Streams.Length);
                      }*/

                    // Add an Values into the table in SSMS
                    connect.Open();

                    string command = "update TBL_HORecords set Fname=@Fname, Mname=@Mname, Lname=@Lname, birthdate=@birthday, birthplace=@birthplace, Age=@Age, CivilStatus=@CivilStatus, Gender=@Gender, Religion=@Religion, Occupation=@Occupation, Contact=@Contact, Email=@Email, Street=@Street, HouseNum=@HouseNum, Block=@Block, Lot=@Lot";
                    if (isImageUpdated)
                    {
                        command += ", Picture=@picture";
                    }
                    command += " where ID = @id";
                    SqlCommand cmd = new SqlCommand(command, connect);
                    cmd.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);
                    cmd.Parameters.AddWithValue("@Fname", Fname.Text);
                    cmd.Parameters.AddWithValue("@Mname", Mname.Text);
                    cmd.Parameters.AddWithValue("@Lname", Lname.Text);
                    cmd.Parameters.AddWithValue("@birthday", BirthD.Value);
                    cmd.Parameters.AddWithValue("@birthplace", BirthP.Text);
                    cmd.Parameters.AddWithValue("@Age", Age.Text);
                    cmd.Parameters.AddWithValue("@CivilStatus", Civil.Text);
                    cmd.Parameters.AddWithValue("@Gender", Sex.Text);
                    cmd.Parameters.AddWithValue("@Religion", Religion.Text);
                    cmd.Parameters.AddWithValue("@Occupation", Occ.Text);
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text);
                    cmd.Parameters.AddWithValue("@Email", Email.Text);
                    cmd.Parameters.AddWithValue("@Street", Street.Text);
                    cmd.Parameters.AddWithValue("@HouseNum", HouseNum.Text);
                    cmd.Parameters.AddWithValue("@Block", Block.Text);
                    cmd.Parameters.AddWithValue("@Lot", Lot.Text);

                    if (isImageUpdated)
                    {
                        cmd.Parameters.Add("@picture", SqlDbType.Image).Value = images;
                    }
                    cmd.ExecuteNonQuery();



                    // Insert data into tbl_User table
                    string command1 = "update tbl_User set FirstName = @FirstName, LastName = @LastName, Username = @username, Password = @password";
                    



                    if (isImageUpdated)
                    {
                        command1 += ", Picture = @picture";
                    }
                    if (CHAC.Checked)
                    {
                        command1 += ", Acc_Status = @Acc_Status";
                    }
                    else if (CHDEAC.Checked)
                    {
                        command1 += ", Acc_Status = @Acc_Status";
                    }

                    command1 += " where Acc_ID = @id";

                    SqlCommand usercmd = new SqlCommand(command1, connect);
                    usercmd.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);
                    if (CHAC.Checked)
                    {
                        usercmd.Parameters.AddWithValue("@Acc_Status", "ACTIVE");
                    }
                    else if (CHDEAC.Checked)
                    {
                        usercmd.Parameters.AddWithValue("@Acc_Status", "DEACTIVATED");
                    }
                    usercmd.Parameters.AddWithValue("@FirstName", Fname.Text);
                    usercmd.Parameters.AddWithValue("@LastName", Lname.Text);
                    usercmd.Parameters.AddWithValue("@username", txtUser.Text);
                    usercmd.Parameters.AddWithValue("@password", txtPass.Text);


                    if (isImageUpdated)
                    {
                        usercmd.Parameters.Add("@picture", SqlDbType.Image).Value = images;
                    }

                    usercmd.ExecuteNonQuery();

                   
                    SqlCommand cm5 = new SqlCommand("delete from TBL_Relatives where HoID = @id", connect);
                    cm5.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_Relatives
                    cm5.ExecuteNonQuery();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Skip the last row if it is the new row
                        if (!row.IsNewRow)
                        {
                            // Retrieve the cell values from the row and create the SQL INSERT statement
                            string first = row.Cells["first"].Value.ToString();
                            string middle = row.Cells["MidName"].Value.ToString();
                            string last = row.Cells["LastName"].Value.ToString();
                            string date = row.Cells["BD"].Value.ToString();
                            string age = row.Cells["Ages"].Value.ToString();
                            string con = row.Cells["cont"].Value.ToString();

                            string query = $"INSERT INTO TBL_Relatives (FName, MName, LName, Age, Birthday, Contact, HoID ) VALUES ('{first}', '{middle}', '{last}', '{age}', '{date}', '{con}', @HoID)";

                            // Insert into the database
                            using (SqlCommand commands = new SqlCommand(query, connect))
                            {
                                commands.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                                commands.ExecuteNonQuery();
                            }
                        }

                    }
                    LOGIN.LogActivity("Updated " + Fname.Text + " " + Lname.Text+"'s Informations");
                    MessageBox.Show(string.Format("Homeowner's Information/s has been Updated!\nUsername: {0}\nPassword: {1}", txtUser.Text, txtPass.Text));
                    connect.Close();


                    m.acc();
                    m.TopLevel = false;
                    m.Dock = DockStyle.Fill;
                    m.cpasslbl.Visible = false;
                    m.tabs.TabPages.Remove(m.tabPage2);
                    m.groupBox21.Visible = false;
                    f.inFopanel.Controls.Clear();
                    f.inFopanel.Controls.Add(m);
                    f.LoadRecords();
                    m.BringToFront();
                    m.Show();
                    this.Dispose();
                }
                else
                {
                    this.Dispose();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }
        public void clearForm()
        {

            Image defaultImage = Properties.Resources._585e4bf3cb11b227491c339a;
            HOPic.Image = defaultImage;
            byte[] images = null;

            if (images == null)
            {
                HOPic.Image = defaultImage;
                imageloc = null; // Set imageloc to null if the default image is used
            }
            Fname.Text = "";
            Mname.Text = "";
            Lname.Text = "";
            Contact.Text = "";
            Email.Text = "";
            BirthD.Value = DateTime.Now;
            Age.Text = "";
            BirthP.Text = "";
            Civil.Text = "";
            Religion.Text = "";
            HouseNum.Text = "";
            Block.Text = "";
            Lot.Text = "";
            Sex.Text = "";
            Street.Text = "";
            Occ.Text = "";
            txtUser.Text = "";
            txtPass.Text = "";
        }

        private void ClearBTN_Click(object sender, EventArgs e)
        {
            clearForm();

        }

        private void BirthD_ValueChanged(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - BirthD.Value.Year;
            Age.Text = age.ToString();

        }

        private void delBTN_Click(object sender, EventArgs e)
        {

            try
            {
                DialogResult diagres = MessageBox.Show("Do you want to Delete this Homeowner?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {

                    connect.Open();
                    SqlCommand cm5 = new SqlCommand("delete from TBL_Relatives where HoID = @id", connect);
                    cm5.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_Relatives
                    cm5.ExecuteNonQuery();


                    SqlCommand cm4 = new SqlCommand("delete from TBL_Transactions where PaymentID = @id", connect);
                    cm4.Parameters.AddWithValue("@id", HomeOwnerRecords.payID);//delete from TBL_Transactions
                    cm4.ExecuteNonQuery();

                    SqlCommand cm3 = new SqlCommand("DELETE FROM TBL_Payment WHERE HoID = @id", connect);
                    cm3.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_Payment
                    cm3.ExecuteNonQuery();

                    SqlCommand cm2 = new SqlCommand("DELETE FROM TBL_User WHERE Acc_ID = @id", connect);
                    cm2.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_Users
                    cm2.ExecuteNonQuery();

                    SqlCommand cm = new SqlCommand("delete from TBL_HORecords where ID = @id", connect);
                    cm.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_HORecords
                    cm.ExecuteNonQuery();


                    connect.Close();

                    f.LoadRecords();
                    this.Dispose();
                    f.inFopanel.Controls.Clear();

                }
                else
                {
                    this.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)

        {
            label29.Visible = false;
            textBox2.Visible = false;
            panel3.Enabled = false;
            label34.Visible = true;
            label35.Visible = true;
            groupBox3.Enabled = false;
            groupBox1.Enabled = true;
            label22.Visible = false;
            nextpayment.Visible = false;
            Loads();

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            LoadRecords();
            panel3.Enabled = true;
            label29.Visible = true;
            textBox2.Visible = true;
            label34.Visible = false;
            label35.Visible = false;
            groupBox3.Enabled = false;
            groupBox1.Enabled = false;
            textBox1.Text = string.Empty;
            label22.Visible = false;
            nextpayment.Visible = false;



        }

        private void Fname_TextChanged(object sender, EventArgs e)
        {


        }

        private void Lname_TextChanged(object sender, EventArgs e)
        {

        }
        public String amort = null;
        public String fee = null;
        public double total = 0;
        public void Loads()
        {
           
            connect.Open();

            SqlCommand cmd = new SqlCommand("Select HoaPayment, Amortization from TBL_Finance where ID = 1", connect);
            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())//.Read
            {
                amort = read["Amortization"].ToString();
                fee = read["HoaPayment"].ToString();


            }

            total = Convert.ToDouble(amort) * 12 * 25;
            textBox1.Text = "PHP "+total.ToString() + ".00";
            connect.Close();

        }

        private void rstBTN_Click(object sender, EventArgs e)
        {
             txtPass.Text = "QWERTY1234";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get the values from textboxes
            string value1 = rFname.Text;
            string value2 = rMname.Text;
            string value3 = rLname.Text;
            string value4 = rBD.Text;
            string value5 = rAge.Text;
            string value6 = rCon.Text;

            // Add a new row to the DataTable
            dataGridView1.Rows.Add(value1, value2, value3, value4, value5, value6);
            // Clear the textboxes
            rFname.Text = string.Empty;
            rMname.Text = string.Empty;
            rLname.Text = string.Empty;
            rBD.Text = string.Empty;
            rAge.Text = string.Empty;
            rCon.Text = string.Empty;

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        public string relID = null;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button2.Visible = false;
            button3.Visible = true;
            button4.Visible = true;
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {


                    connect.Open();
                    //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlCommand cm = new SqlCommand("SELECT * FROM TBL_Relatives where Contact LIKE '" + dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() + "'", connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {


                        relID = dr["FamID"].ToString();


                    }




                    dr.Close();
                    connect.Close();

                    DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                    // Populate the textboxes with the cell values from the selected row
                    rFname.Text = selectedRow.Cells["first"].Value.ToString();
                    rMname.Text = selectedRow.Cells["MidName"].Value.ToString();
                    rLname.Text = selectedRow.Cells["LastName"].Value.ToString();
                    rAge.Text = selectedRow.Cells["Ages"].Value.ToString();
                    rBD.Value = DateTime.Parse(selectedRow.Cells["BD"].Value.ToString());
                    rCon.Text = selectedRow.Cells["cont"].Value.ToString();


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            try
            {
                // Check if a row is selected
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Update the cell values of the selected row with the values from the textboxes
                    selectedRow.Cells["first"].Value = rFname.Text;
                    selectedRow.Cells["MidName"].Value = rMname.Text;
                    selectedRow.Cells["LastName"].Value = rLname.Text;
                    selectedRow.Cells["Ages"].Value = rAge.Text;
                    selectedRow.Cells["BD"].Value = rBD.Value.ToShortDateString();
                    selectedRow.Cells["cont"].Value = rCon.Text;

                    MessageBox.Show("Row updated successfully.");
                    rFname.Text = string.Empty;
                    rMname.Text = string.Empty;
                    rLname.Text = string.Empty;
                    rAge.Text = string.Empty;
                    rBD.Value = DateTime.Now;
                    rCon.Text = string.Empty;
                    button2.Visible = true;
                    button3.Visible = false;
                    button4.Visible = false;
                }
                else
                {
                    MessageBox.Show("Please select a row to update.");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Remove the selected row from the DataGridView
                    dataGridView1.Rows.Remove(selectedRow);

                    MessageBox.Show("Row deleted successfully.");
                    rFname.Text = string.Empty;
                    rMname.Text = string.Empty;
                    rLname.Text = string.Empty;
                    rAge.Text = string.Empty;
                    rBD.Value = DateTime.Now;
                    rCon.Text = string.Empty;
                    button2.Visible = true;
                    button3.Visible = false;
                    button4.Visible = false;
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            rFname.Text = string.Empty;
            rMname.Text = string.Empty;
            rLname.Text = string.Empty;
            rAge.Text = string.Empty;
            rBD.Value = DateTime.Now;
            rCon.Text = string.Empty;
            button2.Visible = true;
            button3.Visible = false;
            button4.Visible = false;
        }

        private void radEx_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            groupBox3.Enabled = true;
            textBox1.Text = string.Empty;
            label22.Visible = true;
            nextpayment.Visible = true;
            label29.Visible = false;
            textBox2.Visible = false;
            panel3.Enabled = false;
        }
        public string HoId = null;
        public string nextpaymentd = null;
        public string forname = null;
        public string isoverdue = null;
        private void HOData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {

                    connect.Open();
                    String s1 = "SELECT TBL_HORecords.*, TBL_Payment.*, TBL_User.* " +
                                  "FROM TBL_HORecords " +
                                  "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                  "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID where TBL_HORecords.ID LIKE '";
                    
                    SqlCommand cm = new SqlCommand(s1 + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        
                        nextpaymentd = dr["NextPaymentDate"].ToString();
                        isoverdue = dr["isOverDue"].ToString();
                        HoId = dr["ID"].ToString();
                        textBox1.Text = dr["RemainingBalance"].ToString();
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        forname = name;
                        textBox2.Text = name;
                        Street.Text = dr["Street"].ToString();
                        HouseNum.Text = dr["HouseNum"].ToString();
                        Block.Text = dr["Block"].ToString();
                        Lot.Text = dr["Lot"].ToString();

                    }
                    dr.Close();
                    connect.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }
        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
