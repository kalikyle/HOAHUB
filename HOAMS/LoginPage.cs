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

    public partial class LOGIN : Form
    {
        public static string users = "";
        public static string AccType = "";
            public static string uID = null;
        public static string position = null;
        public static string fname = null;
        public static string lname = null;
        public static string status = null;

        //Data Source=KYLES23O\\SQLEXPRESS;Initial Catalog=HOASystem;Integrated Security=True
        //Data Source=Jericho\\SQLEXPRESS01;Initial Catalog=HOASystem;Integrated Security=True
        public static string CS = "Data Source=KYLES23O\\SQLEXPRESS;Initial Catalog=HOASystem;Integrated Security=True";
        SqlConnection connect = new SqlConnection(CS);
        public LOGIN()
        {
            InitializeComponent();
            UpdateIsOverDue();
            UpdateIsOverDueFee();
            GetAccStatus();
            UpdateActivation();
            UpdateAccStatusOnHomeownerType();
            


        }
        public static string GetAccStatus()
        {
            // Retrieve the current Acc_Status from the database
            using (SqlConnection connection = new SqlConnection(CS))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT Acc_Status FROM TBL_User WHERE Acc_Status = 'DEACTIVATED' AND Acc_Type = 'HOMEOWNER'", connection))
                {
                    return (string)command.ExecuteScalar();
                }
            }
        }
        public static void LogActivity(string buttonClicked)
        {
            if (AccType.Equals("ADMINISTRATOR"))
            {
                // Generate a timestamp
                DateTime timestamp = DateTime.Now;

                // Retrieve user ID (assuming it's stored in a variable named 'userId')
                int userId = Convert.ToInt32(uID); // Replace 'userId' with the actual user ID

                // Insert into database
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    try
                    {
                        connection.Open();

                        string query = "INSERT INTO TBL_ActivityLogs (User_ID, DateTime, EventClicked) VALUES (@User_ID, @DateTime, @EventClicked)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@DateTime", timestamp);
                            command.Parameters.AddWithValue("@User_ID", userId);
                            command.Parameters.AddWithValue("@EventClicked", buttonClicked);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

       


        public static void UpdateAccStatusOnHomeownerType()
        {
            
                // Update Acc_Status column
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("UPDATE TBL_User SET Acc_Status = 'DEACTIVATED' WHERE Acc_ID IN (SELECT ID FROM TBL_HORecords WHERE HomeownerType = 'FORMER HOMEOWNER')", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            
        }
        public static void UpdateIsOverDue()
        {
            
                // Execute the SQL update statement
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;


                        command.CommandText = "UPDATE TBL_Payment SET isOverDue = CASE WHEN DATEDIFF(MONTH, NextPaymentDate, @CurrentDateF) >= 5 THEN 1 ELSE 0 END";
                        command.Parameters.AddWithValue("@CurrentDateF", DateTime.Now);
                        command.ExecuteNonQuery();

                    // Update Acc_Status table
                    // Update Acc_Status table
                    command.CommandText = "UPDATE TBL_User SET Acc_Status = 'DEACTIVATED' WHERE Acc_ID IN (SELECT HoID FROM TBL_Payment INNER JOIN TBL_HORecords ON TBL_Payment.HoID = TBL_HORecords.ID WHERE TBL_Payment.isOverDue = 1) AND Acc_Status != 'DEACTIVATED'";
                     command.ExecuteNonQuery();

                    
                }
                }
            
        }
        public static void UpdateIsOverDueFee()
        {
            
                // Execute the SQL update statement
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;


                        command.CommandText = "UPDATE TBL_Payment SET isOverDueFee = CASE WHEN DATEDIFF(MONTH, NextHoaFeePayment, @CurrentDateA) >= 5 THEN 1 ELSE 0 END";
                        command.Parameters.AddWithValue("@CurrentDateA", DateTime.Now);
                        command.ExecuteNonQuery();


                    command.CommandText = "UPDATE TBL_User SET Acc_Status = 'DEACTIVATED' WHERE Acc_ID IN (SELECT HoID FROM TBL_Payment INNER JOIN TBL_HORecords ON TBL_Payment.HoID = TBL_HORecords.ID WHERE TBL_Payment.isOverDue = 1) AND Acc_Status != 'DEACTIVATED'";
                    command.ExecuteNonQuery();
                    

                }
                }
            
        }
        public static void UpdateActivation()
        {
            if (GetAccStatus() != "DEACTIVATED")
            {
                // Execute the SQL update statement
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "UPDATE TBL_User SET Acc_Status = " +
                  "CASE " +
                  "WHEN (TBL_Payment.isOverDue = 1 AND TBL_Payment.isOverDueFee = 1) " +
                  "      OR (TBL_Payment.isOverDue = 1 AND TBL_Payment.isOverDueFee = 0) " +
                  "      OR (TBL_Payment.isOverDue = 0 AND TBL_Payment.isOverDueFee = 1) " +
                  "      THEN 'DEACTIVATED' ELSE 'ACTIVE' " +
                  "END " +
                  "FROM TBL_User " +
                  "INNER JOIN TBL_Payment ON TBL_User.Acc_ID = TBL_Payment.HoID";

                        command.ExecuteNonQuery();


                    }
                }
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
           
            Login();
            GetAccStatus();
        }

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Login();


            }
        }
        public void Login()
        {
            if (UserTextBox.Text == "" || PasswordTextBox.Text == "")
            {
                MessageBox.Show("Username or Password is Empty");

            }
            else
            {

                try
                {
                    connect.Open();
                    // Find the Values in SSMS
                    SqlCommand cmd = new SqlCommand("select * from TBL_User where Username = '" + UserTextBox.Text + "' and Password = '" + PasswordTextBox.Text + "'", connect);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {

                        users = UserTextBox.Text;

                        SqlCommand cmd1 = new SqlCommand("Select * from TBL_User where Username=@user", connect);
                        cmd1.Parameters.AddWithValue("user", LOGIN.users);
                        SqlDataReader read1;
                        read1 = cmd1.ExecuteReader();
                        if (read1.Read())//.Read
                        {
                            fname = read1["FirstName"].ToString();
                            lname = read1["LastName"].ToString();
                            uID = read1["ID"].ToString();
                            //then put the values in the textboxes
                            AccType = read1["Acc_Type"].ToString();
                            HomeOwnerRecords.ID = read1["Acc_ID"].ToString();
                            status = read1["Acc_Status"].ToString();
                            position = read1["Position"].ToString();
                        }


                        if (status.Equals("ACTIVE")) {
                            MainPage dash = new MainPage();
                            if (AccType == "HOMEOWNER")
                            {
                                dash.RecordBtn.Visible = false;
                                dash.TRBtn.Visible = false;


                            }
                            LogActivity("LOG IN");
                            dash.Show();
                            this.Hide();
                        }
                        else{
                            MessageBox.Show("This Account has been Deactivated... \nPlease reach an HOA Officer for further informations");
                        }

                        

                    }
                    else
                    {
                        MessageBox.Show("Incorrect Username or Password!");
                    }
                    connect.Close();


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }


            }
        }


    }
}
