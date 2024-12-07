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
    public partial class ActivityLogs : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public ActivityLogs()
        {
            InitializeComponent();
            LoadRecords();
        }
        public void LoadRecords()
        {
            try
            {

                actLogs.Rows.Clear();
                connect.Open();
                String s1 = "SELECT * " +
                                    "FROM TBL_User " +
                                    "JOIN TBL_ActivityLogs ON TBL_User.ID = TBL_ActivityLogs.User_ID ORDER BY DateTime DESC";


                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string name = dr["FirstName"].ToString() + " " + dr["Lastname"].ToString();
                    actLogs.Rows.Add(dr["ID"].ToString(), name, dr["DateTime"].ToString(), dr["EventClicked"].ToString());


                }
                dr.Close();
                connect.Close();
                actLogs.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            connect.Open();
            SqlCommand cm5 = new SqlCommand("delete from TBL_ActivityLogs ", connect);
            cm5.ExecuteNonQuery();
            connect.Close();
            LoadRecords();
        }
    }
}
