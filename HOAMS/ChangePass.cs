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
    public partial class ChangePass : Form
    {
        public static string pass = "";
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public ChangePass()
        {
            InitializeComponent();
            connect.Open();
            //string tables = LOGIN_PAGE.AccType == "ADMINISTRATOR"?"TBL_User":"TBL_HOUser" ; // ternary operator

            SqlCommand cmd = new SqlCommand("Select Password from TBL_User where Username=@user", connect);
            cmd.Parameters.AddWithValue("user", LOGIN.users);

            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())
            {
                //assign security pin value to pin global variable
                pass = read["Password"].ToString();


            }
            else
            {
                MessageBox.Show("No data was Found");

            }
            connect.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (newPassTxt.Text == "") { MessageBox.Show("Field is Empty!"); }
            else
            {
                try
                {
                    connect.Open();
                    //string tables = LOGIN_PAGE.AccType == "ADMINISTRATOR" ? "TBL_User" : "TBL_HOUser";//ternary operators
                    SqlCommand cmd = new SqlCommand("Update TBL_User set Password = @pass where Username = @user", connect);
                    cmd.Parameters.AddWithValue("user", LOGIN.users);
                    cmd.Parameters.AddWithValue("@pass", newPassTxt.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Your Password has been Updated!");
                    connect.Close();

                    DialogResult diagres = MessageBox.Show("Do You want To Re-Login?", "Confirm", MessageBoxButtons.YesNo);
                    if (diagres == DialogResult.Yes)
                    {

                        MainPage mainpage = Application.OpenForms["MainPage"] as MainPage;
                        mainpage.Dispose();
                        this.Close();

                        LOGIN login = new LOGIN();
                        login.Show();
                    }
                    else
                    {

                        this.Close();
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
        }

        private void ConPassText_TextChanged(object sender, EventArgs e)
        {
            // if global variable is equals to pin then the panel will be enabled
            if (ConPassText.Text == pass)
            {
                panel1.Enabled = true;

            }
            else
            {

                panel1.Enabled = false;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            newPassTxt.Text = "";
        }

        private void newPassTxt_TextChanged(object sender, EventArgs e)
        {
            if (newPassTxt.Text.Equals(""))
            {
                button3.Visible = false;
                button2.Visible = false;

            }
            else
            {
                button3.Visible = true;
                button2.Visible = true;
            }
            
        }
    }
}
