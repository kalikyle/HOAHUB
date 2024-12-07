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
    public partial class ManageAdmins : Form
    {
        public static string pin = "";
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public ManageAdmins()
        {
            InitializeComponent();
            sortPos.Text = "ACTIVE";
            LoadAdmins();
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
        public void LoadAdmins()
        {
            try
            {

                adminTBL.Rows.Clear();
                connect.Open();
                string query = "SELECT * FROM TBL_User WHERE Acc_Type = 'ADMINISTRATOR' AND Position <> 'SUPER ADMIN' AND Username <> @User ";
                if (sortPos.Text.Equals("ALL"))
                {
                    query += "";
                }
                else
                {
                    query += "AND (Position = @Position OR Acc_Status = @Position)";

                }
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@Position", sortPos.Text);
                cmd.Parameters.AddWithValue("@User", LOGIN.users);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //string name = dr["FName"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    DateTime date = (DateTime)dr["ElectedDate"];
                    adminTBL.Rows.Add(dr["ID"].ToString(), dr["FirstName"].ToString(), dr["LastName"].ToString(), date.ToShortDateString(), dr["Position"].ToString(), dr["Acc_Status"].ToString());


                }
                dr.Close();
                connect.Close();
                adminTBL.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
            
        }




        private void secuPin_TextChanged(object sender, EventArgs e)
        {
            // if global variable is equals to pin then the panel will be enabled
            if (secuPin.Text == pin)
            {
                panel4.Enabled = true;

            }
            else
            {

                panel4.Enabled = false;

            }
        }
        string id = null;
        private void adminTBL_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            UpBTN.Visible = true;
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {


                    connect.Open();
                     //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                     SqlCommand cm = new SqlCommand("SELECT * FROM TBL_User WHERE ID LIKE '" + adminTBL.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                     SqlDataReader dr = cm.ExecuteReader();
                     dr.Read();
                    if (dr.HasRows)
                    {

                        id = dr["ID"].ToString();
                        Nametxt.Text = dr["FirstName"].ToString();
                        LNametxt.Text = dr["LastName"].ToString();
                        PositionText.Text = dr["Position"].ToString();
                        elDate.Text = dr["ElectedDate"].ToString();
                        UserTextBox.Text = dr["Username"].ToString();
                        PasswordTextBox.Text = dr["Password"].ToString();
                        string stat = dr["Acc_Status"].ToString();

                        if (stat.Equals("ACTIVE"))
                        {
                            radAC.Checked = true;
                        }
                        else
                        {
                            radDE.Checked = true;

                        }





                        /*Nametxt.Text = dr["FirstName"].toString();
                        LNametxt.Text = dr["LastName"].toString();
                        PositionText.Text = dr["Position"].toString();
                        elDate.Value dr["ElectedDate"].toString();
                        UserTextBox.Text = dr["Username"].toString();
                        PasswordTextBox.Text = dr["Password"].toString();*/

                    }

                     dr.Close();
                     connect.Close();

                    //DataGridViewRow selectedRow = adminTBL.Rows[e.RowIndex];

                    // Populate the textboxes with the cell values from the selected row
                    /*Nametxt.Text = selectedRow.Cells["first"].Value.ToString();
                    LNametxt.Text = selectedRow.Cells["MidName"].Value.ToString();
                    PositionText.Text = selectedRow.Cells["LastName"].Value.ToString();
                    
                    elDate.Value = DateTime.Parse(selectedRow.Cells["BD"].Value.ToString());*/
                   


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void UpBTN_Click(object sender, EventArgs e)
        {
            try {

                string stats = null;
                DialogResult diagres = MessageBox.Show("Update the informations?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {
                    connect.Open();

                    string command = "update TBL_User set FirstName = @FirstName, LastName = @LastName, ElectedDate = @Date, Username = @Username, Password = @Password, Position = @Position, Acc_Status = @Status WHERE ID = @id";

                    SqlCommand cmd = new SqlCommand(command, connect);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@FirstName", Nametxt.Text);
                    cmd.Parameters.AddWithValue("@LastName", LNametxt.Text);
                    cmd.Parameters.AddWithValue("@Date", elDate.Value);
                    cmd.Parameters.AddWithValue("@Username", UserTextBox.Text);
                    cmd.Parameters.AddWithValue("@Password", PasswordTextBox.Text);
                    cmd.Parameters.AddWithValue("@Position", PositionText.Text);

                    if (radAC.Checked)
                    {
                        cmd.Parameters.AddWithValue("@Status", "ACTIVE");
                        stats = "ACTIVATIVED";
                    }
                    else if (radDE.Checked)
                    {

                        cmd.Parameters.AddWithValue("@Status", "DEACTIVATED");
                        stats = "DEACTIVATED";
                    }

                    cmd.ExecuteNonQuery();
                    connect.Close();
                }
                MessageBox.Show(string.Format("Officer's Information/s has been Updated!\nUsername: {0}\nPassword: {1}\nStatus: {2}", UserTextBox.Text, PasswordTextBox.Text, stats));
                LoadAdmins();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
            
            
        }

        private void delBTN_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                DialogResult diagres = MessageBox.Show("Do you want to Delete this Homeowner?", "Confirm", MessageBoxButtons.YesNo);
                if (diagres == DialogResult.Yes)
                {

                    connect.Open();
                    

                    SqlCommand cm = new SqlCommand("delete from TBL_User where ID = @id", connect);
                    cm.Parameters.AddWithValue("@id", id);//delete from Users
                    cm.ExecuteNonQuery();


                    connect.Close();

                    

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
            LoadAdmins();*/
        }

        private void sortPos_TextChanged(object sender, EventArgs e)
        {
            LoadAdmins();
        }
    }
}
