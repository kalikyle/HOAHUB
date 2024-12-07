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
    public partial class AddUser : Form
    {
        //establish connection
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public static string pin = "";
        public static string command = "";
        public AddUser()
        {
            InitializeComponent();
            PositionText.Text = "PRESIDENT";
            connect.Open();
            SqlCommand cmd = new SqlCommand("Select Password from TBL_User where Username=@user", connect);
            cmd.Parameters.AddWithValue("user", LOGIN.users);
            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                //assign security pin value to pin global variable
                pin = read["Password"].ToString();


            }
            else
            {
                MessageBox.Show("No data was Found");

            }
            connect.Close();
        }

        private void DoneBTN_Click(object sender, EventArgs e)
        {

            if (UserTextBox.Text == "" || PasswordTextBox.Text == "")
            {
                MessageBox.Show("Some Fields are Empty");
            }
            else
            {
                try
                {
                    DialogResult diagres = MessageBox.Show("Do you want to Continue? \n The previous "+PositionText.Text+" will be DEACTIVATED", "Confirm", MessageBoxButtons.YesNo);
                    if (diagres == DialogResult.Yes)
                    {
                        //string tables = AccountComb.Text == "ADMINISTRATOR" ? "TBL_User" : "TBL_HOUser";
                        //find the username if already existed
                        SqlCommand cmd1 = new SqlCommand("select * from TBL_User where Username = '" + UserTextBox.Text + "'", connect);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                        DataTable dt = new DataTable();

                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {

                            MessageBox.Show("Username Already Exist! \nTry new One");

                        }
                        else
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
                            string updateQuery = "UPDATE TBL_User SET Acc_Status = 'DEACTIVATED' WHERE Position = '" + PositionText.Text + "'";

                            SqlCommand updateCommand = new SqlCommand(updateQuery, connect);
                            updateCommand.ExecuteNonQuery();

                            string Admin = "ADMINISTRATOR";
                            command = "insert into TBL_User (FirstName, LastName, ElectedDate, Username, Password, Position, Acc_Type, Acc_Status, Picture) values ('" + Nametxt.Text + "','" + LNametxt.Text + "', @ElectedDate, '" + UserTextBox.Text + "','" + PasswordTextBox.Text + "','" + PositionText.Text + "','" + Admin + "', @status, @picture )";

                            SqlCommand cmd = new SqlCommand(command, connect);
                            cmd.Parameters.AddWithValue("@status", "ACTIVE");
                            cmd.Parameters.AddWithValue("@ElectedDate", elDate.Value);
                            if (images != null)//if images is not null 
                            {
                                cmd.Parameters.Add("@picture", SqlDbType.Image).Value = images;
                            }
                            else
                            {//if null
                                cmd.Parameters.Add("@picture", SqlDbType.Image).Value = DBNull.Value;
                            }

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("New User has been Saved!");
                            connect.Close();
                            clears();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
        }
        public void clears()
        {
            Image defaultImage = Properties.Resources._585e4bf3cb11b227491c339a;
            AccPic.Image = defaultImage;
            byte[] images1 = null;

            if (images1 == null)
            {
                AccPic.Image = defaultImage;
                imageloc = null; // Set imageloc to null if the default image is used
            }
            Nametxt.Text = "";
            LNametxt.Text = "";
            elDate.Value = DateTime.Now;
            UserTextBox.Text = "";
            PasswordTextBox.Text = "";
            PositionText.Text = "PRESIDENT";
        }


        private void clear_Click(object sender, EventArgs e)
        {
            clears();
        }
        string imageloc = "";
        private void button1_Click(object sender, EventArgs e)
        {
            //browsing a picture
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files(*.png)|*.png|jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imageloc = dialog.FileName.ToString();
                AccPic.ImageLocation = imageloc;

            }
        }

        private void secuPin_TextChanged(object sender, EventArgs e)
        {


            // if global variable is equals to pin then the panel will be enabled
            if (secuPin.Text == pin)
            {
                panel1.Enabled = true;

            }
            else
            {

                panel1.Enabled = false;

            }
        }

        

        private void AddUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
