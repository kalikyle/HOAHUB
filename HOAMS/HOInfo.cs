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
    public partial class HOInfo : Form
    {
        HomeOwnerRecords f;
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public HOInfo(HomeOwnerRecords f)
        {
            InitializeComponent();
            this.f = f;
            loadrela();
            



        }
        public string payID = null;
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

        private void button2_Click(object sender, EventArgs e)
        {
            f.inFopanel.Dock = DockStyle.Bottom;
            this.Dispose();
        }
        public void loadrela()
        {
            try
            {
                relaTBL.Rows.Clear();
                connect.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Relatives WHERE HoID = @HoID", connect);
                cmd.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                SqlDataReader dr1 = cmd.ExecuteReader();
                while (dr1.Read())
                {
                    DateTime birthdate = (DateTime)dr1["Birthday"];

                    relaTBL.Rows.Add(dr1["FName"].ToString(), dr1["MName"].ToString(), dr1["LName"].ToString(), birthdate.ToShortDateString(), dr1["Age"].ToString(), dr1["Contact"].ToString());


                }
                dr1.Close();
                connect.Close();
                relaTBL.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }


        public string status = null;
            private void UpdateBtn_Click(object sender, EventArgs e)
        {

            try
            {

                AddRecord add = new AddRecord(f);
                connect.Open();
                //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.* " +
                                "FROM TBL_HORecords " +
                                "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                                "WHERE TBL_HORecords.ID LIKE '" + HomeOwnerRecords.ID + "'", connect);
                SqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {

                    if (dr.IsDBNull(0))
                    {
                        // Use a default image if the picture data is null
                        add.HOPic.Image = add.HOPic.Image;

                    }
                    else
                    {
                        long len = dr.GetBytes(0, 0, null, 0, 0);
                        byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                        dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                        MemoryStream ms = new MemoryStream(array);
                        Bitmap bit = new Bitmap(ms);
                        add.HOPic.Image = bit;
                    }




                    add.Fname.Text = dr["Fname"].ToString();
                    add.Mname.Text = dr["Mname"].ToString();
                    add.Lname.Text = dr["Lname"].ToString();
                    add.Contact.Text = dr["Contact"].ToString();
                    add.Email.Text = dr["Email"].ToString();
                    add.BirthD.Value = DateTime.Parse(dr["birthdate"].ToString());
                    add.Age.Text = dr["Age"].ToString();
                    add.BirthP.Text = dr["birthplace"].ToString();
                    add.Civil.Text = dr["CivilStatus"].ToString();
                    add.Religion.Text = dr["Religion"].ToString();
                    add.HouseNum.Text = dr["HouseNum"].ToString();
                    add.Block.Text = dr["Block"].ToString();
                    add.Lot.Text = dr["lot"].ToString();
                    add.Sex.Text = dr["Gender"].ToString();
                    add.Street.Text = dr["Street"].ToString();
                    add.Occ.Text = dr["Occupation"].ToString();
                    add.txtUser.Text = dr["Username"].ToString();
                    add.txtPass.Text = dr["Password"].ToString();
                    status = dr["Acc_Status"].ToString();




                }




                dr.Close();
                connect.Close();

                try
                {
                    add.dataGridView1.Rows.Clear();
                    connect.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Relatives WHERE HoID = @HoID", connect);
                    cmd.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                    SqlDataReader dr1 = cmd.ExecuteReader();
                    while (dr1.Read())
                    {
                        DateTime birthdate = (DateTime)dr1["Birthday"];
                        
                        add.dataGridView1.Rows.Add(dr1["FName"].ToString(), dr1["MName"].ToString(), dr1["LName"].ToString(), birthdate.ToShortDateString() , dr1["Age"].ToString(), dr1["Contact"].ToString());


                    }
                    dr1.Close();
                    connect.Close();
                    add.dataGridView1.ClearSelection();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error: " + ex);

                }


                add.label25.Visible = false;
                //add.panel4.Enabled = true;
                
                add.label1.Visible = false;
                add.label17.Visible = false;
                add.doneBTN.Visible = false;
                add.ClearBTN.Visible = false;
                add.radNew.Visible = false;
                add.radSub.Visible = false;
                add.radEx.Visible = false;
                add.rstBTN.Visible = true;
                add.groupBox4.Enabled = true;
                if (status.Equals("ACTIVE"))
                {
                    add.CHDEAC.Visible = true;

                }
                else {
                    add.CHAC.Visible = true;

                }

                //act like message box

                add.ShowDialog();
               


            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }
        public void acc() {
            try
            {




                String name = "", add = "";


                connect.Open();
                SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.*, TBL_Payment.* " +
                               "FROM TBL_HORecords " +
                               "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                               "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                               "WHERE TBL_HORecords.ID LIKE '" + HomeOwnerRecords.ID + "'", connect);
                //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HomeOwnerRecords.ID + "'", connect);
                SqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {

                    if (dr.IsDBNull(0))
                    {
                        // Use a default image if the picture data is null
                        HOPic.Image = HOPic.Image;

                    }
                    else
                    {
                        long len = dr.GetBytes(0, 0, null, 0, 0);
                        byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                        dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                        MemoryStream ms = new MemoryStream(array);
                        Bitmap bit = new Bitmap(ms);
                        HOPic.Image = bit;
                    }




                    fname.Text = dr["Fname"].ToString();
                    mname.Text = dr["Mname"].ToString();
                    lname.Text = dr["Lname"].ToString();
                    name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    BigName.Text = name;
                    BigCon.Text = dr["Contact"].ToString();
                    add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                    Address.Text = add;
                    con.Text = dr["Contact"].ToString();
                    email.Text = dr["Email"].ToString();
                    DateTime birthdate = (DateTime)dr["birthdate"];
                    birthd.Text = birthdate.ToShortDateString();
                    age.Text = dr["Age"].ToString();
                    birthp.Text = dr["birthplace"].ToString();
                    cilstat.Text = dr["CivilStatus"].ToString();
                    rel.Text = dr["Religion"].ToString();
                    street.Text = dr["Street"].ToString();
                    HN.Text = dr["HouseNum"].ToString();
                    blk.Text = dr["Block"].ToString();
                    lot.Text = dr["lot"].ToString();
                    label4.Text = dr["HomeownerType"].ToString();
                    Usertxt.Text = dr["Username"].ToString();
                    passTxt.Text = dr["Password"].ToString();
                    label13.Text = dr["Acc_Status"].ToString() + " ACCOUNT";

                    DateTime FP = (DateTime)dr["NextHoaFeePayment"];
                    label33.Text = FP.ToString("MMMM d, yyyy");
                    label31.Text = "PHP " + dr["OverallHoaFeePay"].ToString();
                    DateTime PD = (DateTime)dr["NextPaymentDate"];
                    label32.Text = PD.ToString("MMMM d, yyyy");
                    label30.Text = "PHP " + dr["RemainingBalance"].ToString();
                    label29.Text = "PHP " + dr["OverallPay"].ToString();

                    payID = dr["PaymentID"].ToString();



                }




                dr.Close();
                connect.Close();



            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
            LogoutBtn.Visible = false;
            label3.Visible = false;
            editLbl.Visible = false;
        }
        public void HOAccount()
        {
            try
            {




                String name = "", add = "";

     
                connect.Open();
                SqlCommand cm = new SqlCommand("SELECT TBL_HORecords.Picture AS picture, TBL_HORecords.*, TBL_User.*, TBL_Payment.* " +
                               "FROM TBL_HORecords " +
                               "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID " +
                               "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID " +
                               "WHERE TBL_HORecords.ID LIKE '" + HomeOwnerRecords.ID + "'", connect);
                //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HomeOwnerRecords.ID + "'", connect);
                SqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {

                    if (dr.IsDBNull(0))
                    {
                        // Use a default image if the picture data is null
                        HOPic.Image = HOPic.Image;

                    }
                    else
                    {
                        long len = dr.GetBytes(0, 0, null, 0, 0);
                        byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                        dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                        MemoryStream ms = new MemoryStream(array);
                        Bitmap bit = new Bitmap(ms);
                        HOPic.Image = bit;
                    }




                    fname.Text = dr["Fname"].ToString();
                    mname.Text = dr["Mname"].ToString();
                    lname.Text = dr["Lname"].ToString();
                    name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    BigName.Text = name;
                    BigCon.Text = dr["Contact"].ToString();
                    add = dr["HouseNum"].ToString() + ", Block " + dr["Block"].ToString() + ", Lot " + dr["Lot"].ToString() + ", " + dr["Street"].ToString() + " St., \nVilla Balderama, Nagpayong II, \nPinagbuhatan, Pasig City";
                    Address.Text = add;
                    con.Text = dr["Contact"].ToString();
                    email.Text = dr["Email"].ToString();
                    DateTime birthdate = (DateTime)dr["birthdate"];
                    birthd.Text = birthdate.ToShortDateString();
                    age.Text = dr["Age"].ToString();
                    birthp.Text = dr["birthplace"].ToString();
                    cilstat.Text = dr["CivilStatus"].ToString();
                    rel.Text = dr["Religion"].ToString();
                    street.Text = dr["Street"].ToString();
                    HN.Text = dr["HouseNum"].ToString();
                    blk.Text = dr["Block"].ToString();
                    lot.Text = dr["lot"].ToString();
                    label4.Text = dr["HomeownerType"].ToString();
                    Usertxt.Text = dr["Username"].ToString();
                    passTxt.Text = dr["Password"].ToString();
                    label13.Text = dr["Acc_Status"].ToString() + " ACCOUNT";

                    DateTime FP = (DateTime)dr["NextHoaFeePayment"];
                    label33.Text = FP.ToString("MMMM d, yyyy");
                    label31.Text = "PHP " + dr["OverallHoaFeePay"].ToString();
                    DateTime PD = (DateTime)dr["NextPaymentDate"];
                    label32.Text = PD.ToString("MMMM d, yyyy");
                    label30.Text = "PHP " + dr["RemainingBalance"].ToString();
                    label29.Text = "PHP " + dr["OverallPay"].ToString();


                    payID = dr["PaymentID"].ToString();

                }




                dr.Close();
                connect.Close();



            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
            button2.Visible = false;
            UpdateBtn.Visible = false;



        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            DialogResult diagres = MessageBox.Show("Do You want to Logout?", "Confirm", MessageBoxButtons.YesNo);
            if (diagres == DialogResult.Yes)
            {
                Form mainPage = this.Parent.Parent as Form;
                mainPage.Close();
                LOGIN login = new LOGIN();
                login.Show();
            }
        }

        private void editLbl_Click(object sender, EventArgs e)
        {
            Usertxt.Enabled = true;
        }
        string imageloc = "";
        private void Usertxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) { //enter in keyboard
                try
                {

                    

                    connect.Open();
                    SqlCommand cmd = new SqlCommand("Update TBL_User set Username = @user  where Username=@usern ", connect);
                    cmd.Parameters.AddWithValue("usern", LOGIN.users);
                    cmd.Parameters.AddWithValue("user", Usertxt.Text);

                   


                    DialogResult diagres = MessageBox.Show("Do You want to save the Changes?", "Confirm", MessageBoxButtons.YesNo);
                    if (diagres == DialogResult.Yes)
                    {
                        cmd.ExecuteNonQuery();//execute the command update
                        MessageBox.Show("Your Account has been Updated!");

                        MainPage form1 = Application.OpenForms["MainPage"] as MainPage;
                       
                        form1.UpdateLabel(Usertxt.Text);
                        LOGIN.users = Usertxt.Text;


                        Usertxt.Enabled = false;
                        connect.Close();
                    }
                    else
                    {
                        SqlCommand cmd1 = new SqlCommand("Select Username, Picture from TBL_User where Username=@user", connect);
                        cmd1.Parameters.AddWithValue("user", LOGIN.users);
                        SqlDataReader read;
                        read = cmd1.ExecuteReader();//just retrive the values again if user dont want changes
                        if (read.Read())
                        {
                            //values remain in the textboxes
                            Usertxt.Text = read["Username"].ToString();

                        }

                        Usertxt.Enabled = false;

                       
                        connect.Close();
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files(*.png)|*.png|jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imageloc = dialog.FileName.ToString();
                HOPic.ImageLocation = imageloc;

            }
        }

        private void HOPic_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                byte[] image = null;
                if (!string.IsNullOrEmpty(imageloc)) // check if a new image has been selected
                {
                    FileStream Streams = new FileStream(imageloc, FileMode.Open, FileAccess.Read);
                    BinaryReader bin = new BinaryReader(Streams);
                    image = bin.ReadBytes((int)Streams.Length);
                }

                connect.Open();
                SqlTransaction transaction = connect.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("Update TBL_HORecords set  " + (!string.IsNullOrEmpty(imageloc) ? " Picture = @pic " : "") + " where ID=@usern ", connect, transaction);
                    cmd.Parameters.AddWithValue("usern", HomeOwnerRecords.ID);

                    if (!string.IsNullOrEmpty(imageloc))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pic", image));
                    }

                    SqlCommand cmd2 = new SqlCommand("UPDATE TBL_User SET " + (!string.IsNullOrEmpty(imageloc) ? " Picture = @pic " : "") + " WHERE Acc_ID = @usern", connect, transaction);
                    cmd2.Parameters.AddWithValue("usern", HomeOwnerRecords.ID);

                    if (!string.IsNullOrEmpty(imageloc))
                    {
                        cmd2.Parameters.Add(new SqlParameter("@pic", image));
                    }

                    DialogResult diagres = MessageBox.Show("Do You want to save the Changes?", "Confirm", MessageBoxButtons.YesNo);
                    if (diagres == DialogResult.Yes)
                    {
                        try
                        {
                            cmd.ExecuteNonQuery(); // execute the command to update TBL_HORecords
                            cmd2.ExecuteNonQuery(); // execute the command to update TBL_User
                            transaction.Commit(); // commit the transaction
                            MessageBox.Show("Your Account has been Updated!");

                            MainPage form1 = Application.OpenForms["MainPage"] as MainPage;
                            form1.UpdateImage(HOPic.Image);
                            form1.UpdateLabel(Usertxt.Text);
                            LOGIN.users = Usertxt.Text;

                            Usertxt.Enabled = false;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // rollback the transaction in case of an error
                            MessageBox.Show("Error: " + ex);
                        }
                    }
                    else
                    {
                        SqlCommand cmd1 = new SqlCommand("Select Picture from TBL_HORecords where ID=@user", connect);
                        cmd1.Parameters.AddWithValue("user", HomeOwnerRecords.ID);
                        SqlDataReader read;
                        read = cmd1.ExecuteReader();//just retrive the values again if user dont want changes
                        if (read.Read())
                        {
                            //values remain in the textboxes
                            byte[] images = ((byte[])read[1]);

                            if (images == null)
                            {
                                HOPic.Image = null;
                            }
                            else
                            {
                                MemoryStream msStream = new MemoryStream(images);
                                HOPic.Image = Image.FromStream(msStream);

                            }

                        }

                        Usertxt.Enabled = false;
                    }
                }
                finally
                {
                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void cpasslbl_Click(object sender, EventArgs e)
        {
            ChangePass pass = new ChangePass();
            pass.FormBorderStyle = FormBorderStyle.FixedSingle;

            pass.Show();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            groupBox21.Visible = true;
            f.inFopanel.Dock = DockStyle.Fill;
            //f.inFopanel.Height += 200;
            f.inFopanel.BringToFront();
            button1.Visible = false;
            button3.Visible = true;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox21.Visible = false;
            f.inFopanel.Dock = DockStyle.Bottom;
            f.inFopanel.BringToFront();
            button1.Visible = true;
            button3.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
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
                    f.inFopanel.Controls.Clear();

                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void HOInfo_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Relatives ho = new Relatives(this);
            ho.ShowDialog();
        }
    }
  }

