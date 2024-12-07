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
    public partial class AdminAccount : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public string acctId = "";
        public void UpdateLabel(string newText)
        {
            ADD.Text = newText.ToUpper();
        }
        public AdminAccount()
        {
            InitializeComponent();
            ADD.Text = LOGIN.users.ToUpper();
            
            //this will search the data of the user in database
            connect.Open();
            SqlCommand cmd = new SqlCommand("Select * from TBL_User where Username=@user", connect);
            cmd.Parameters.AddWithValue("user", LOGIN.users);
            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())//.Read
            {
                //then put the values in the textboxes
                fname.Text = read["FirstName"].ToString();
                lname.Text = read["LastName"].ToString();
                DateTime? electedDate = read["ElectedDate"] as DateTime?;
                if (electedDate.HasValue)
                {
                    eldate.Text = electedDate.Value.ToShortDateString();
                }
                else
                {
                    eldate.Text = string.Empty;
                }
                Usertxt.Text = read["Username"].ToString();
                passTxt.Text = read["Password"].ToString();
                Postxt.Text = read["Position"].ToString();
                AccTxt.Text = read["Acc_Type"].ToString();
                acctId = read["Acc_ID"].ToString();
                // if the image is null in databse
                if (read.IsDBNull(7))
                {
                    picBox.Image = picBox.Image;// the picture will remain
                }
                else
                {
                    //else will retrive the picture in databases
                    byte[] images = ((byte[])read[7]);
                    MemoryStream msStream = new MemoryStream(images);
                    picBox.Image = Image.FromStream(msStream);
                }

            }

            DoneBTN.Visible = false;
            connect.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            LOGIN.LogActivity("LOG OUT");
            DialogResult diagres = MessageBox.Show("Do You want to Logout?", "Confirm", MessageBoxButtons.YesNo);
            if (diagres == DialogResult.Yes)
            {
                Form mainPage = this.Parent.Parent as Form;
                mainPage.Close();
                LOGIN login = new LOGIN();
                login.Show();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Usertxt.Enabled = true;
        }

        private void Usertxt_TextChanged(object sender, EventArgs e)
        {
            DoneBTN.Visible = true;
        }

        private void DoneBTN_Click(object sender, EventArgs e)
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
                SqlCommand cmd = new SqlCommand("Update TBL_User set Username = @user, FirstName = @fn , LastName = @ln " + (!string.IsNullOrEmpty(imageloc) ? " , Picture = @pic " : "") + " where Username=@usern ", connect);
                cmd.Parameters.AddWithValue("usern", LOGIN.users);
                cmd.Parameters.AddWithValue("user", Usertxt.Text);
                cmd.Parameters.AddWithValue("fn", fname.Text);
                cmd.Parameters.AddWithValue("ln", lname.Text);

                if (!string.IsNullOrEmpty(imageloc))
                {
                    cmd.Parameters.Add(new SqlParameter("@pic", image));
                }


                DialogResult diagres = MessageBox.Show("Do You want to save the Changes?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {
                    cmd.ExecuteNonQuery();//execute the command update
                    MessageBox.Show("Your Account has been Updated!");

                    MainPage form1 = Application.OpenForms["MainPage"] as MainPage;
                    form1.UpdateImage(picBox.Image);
                    form1.UpdateLabel(Usertxt.Text);
                    UpdateLabel(Usertxt.Text);
                    LOGIN.users = Usertxt.Text;


                    Usertxt.Enabled = false;
                    fname.Enabled = false;
                    lname.Enabled = false;
                    DoneBTN.Visible = false;

                   //need fixes in case if the homeowner want to change its photo.
                  /* if (acctId != null && picBox.Image != null)
                    {
                        SqlCommand updateHouserCmd = new SqlCommand("UPDATE TBL_HORecords SET Picture = @pic WHERE ID = @hID", connect);
                        cmd.Parameters.Add(new SqlParameter("@pic", SqlDbType.VarBinary, -1)).Value = (object)image ?? DBNull.Value;
                        updateHouserCmd.Parameters.AddWithValue("@hID", acctId);
                        updateHouserCmd.ExecuteNonQuery();
                        
                    }*/

                    connect.Close();
                }
                else
                {
                    Image defaultImage = Properties.Resources._585e4bf3cb11b227491c339a;
                    SqlCommand cmd1 = new SqlCommand("Select Username, Position, Picture from TBL_User where Username=@user", connect);
                    cmd1.Parameters.AddWithValue("user", LOGIN.users);
                    SqlDataReader read;
                    read = cmd1.ExecuteReader();//just retrive the values again if user dont want changes
                    if (read.Read())
                    {
                        //values remain in the textboxes
                        Usertxt.Text = read["Username"].ToString();
                        Postxt.Text = read["Position"].ToString();

                        byte[] images = null;
                        if (!Convert.IsDBNull(read[2]))
                        {
                            images = (byte[])read[2];
                        }

                        if (images == null)
                        {
                            picBox.Image = defaultImage;
                        }
                        else
                        {
                            MemoryStream msStream = new MemoryStream(images);
                            picBox.Image = Image.FromStream(msStream);

                        }
                       connect.Close();

                    }

                    Usertxt.Enabled = false;
                    Postxt.Enabled = false;
                    DoneBTN.Visible = false;
                    
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            
        }

        private void label9_Click(object sender, EventArgs e)
        {
            

        }
        string imageloc = "";
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files(*.png)|*.png|jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imageloc = dialog.FileName.ToString();
                picBox.ImageLocation = imageloc;

            }
        }

        private void picBox_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DoneBTN.Visible = true;
        }

        private void AdminAccount_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            AddUser add = new AddUser();
            panel6.Hide();
            add.TopLevel = false;
            add.Dock = DockStyle.Fill;
            panel4.Controls.Add(add);
            add.BringToFront();
           
            add.Show();
            
            
      
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ChangePass pass = new ChangePass();
            
            pass.TopLevel = false;
            pass.Dock = DockStyle.Fill;
            panel5.Controls.Add(pass);

            pass.BringToFront();
            pass.Show();
           
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BackUp_RestoreDB add = new BackUp_RestoreDB();
            panel6.Hide();
            add.TopLevel = false;
            add.Dock = DockStyle.Fill;
            panel4.Controls.Add(add);
            add.BringToFront();
            add.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ManageAdmins add = new ManageAdmins();
            panel6.Hide();
            add.TopLevel = false;
            add.Dock = DockStyle.Fill;
            panel4.Controls.Add(add);
            add.BringToFront();
            add.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ActivityLogs add = new ActivityLogs();
            panel6.Hide();
            add.TopLevel = false;
            add.Dock = DockStyle.Fill;
            panel4.Controls.Add(add);
            add.BringToFront();
            add.Show();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            fname.Enabled = true;
        }

        private void label9_Click_1(object sender, EventArgs e)
        {
            lname.Enabled = true;
        }

        private void lname_TextChanged(object sender, EventArgs e)
        {
            DoneBTN.Visible = true;
        }

        private void fname_TextChanged(object sender, EventArgs e)
        {
            DoneBTN.Visible = true;
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            
            panel9.Show();
            panel9.BringToFront();
            pictureBox1.Show();
            label3.Show();
            panel6.Show();
            
        }
    }
}
