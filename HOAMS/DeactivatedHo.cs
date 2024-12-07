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
using Microsoft.Reporting.WinForms;
using System.Reflection;
using System.IO;

namespace HOAMS
{
    public partial class DeactivatedHo : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public DeactivatedHo()
        {
            InitializeComponent();
            LoadRecords();
           

        }
        public void LoadRecords()
        {
            try
            {

                HOsub.Rows.Clear();
                connect.Open();
                String s1 = "SELECT * " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID WHERE Acc_Status = 'DEACTIVATED' AND HomeownerType <> 'FORMER HOMEOWNER'";


                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string name = dr["FName"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    HOsub.Rows.Add(dr["ID"].ToString(), name, dr["HouseNum"].ToString(), dr["Street"].ToString(), dr["Block"].ToString(), dr["Lot"].ToString());


                }
                dr.Close();
                connect.Close();
                HOsub.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }

        private void HOsub_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {
                    DialogResult diagres = MessageBox.Show("Do You want to Activate this Homeowner?", "Confirm", MessageBoxButtons.YesNo);
                    if (diagres == DialogResult.Yes)
                    {

                        connect.Open();
                        // Insert data into tbl_User table
                        string command1 = "UPDATE TBL_User SET Acc_Status = @Acc_Status where Acc_ID = '" + HOsub.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";
                        SqlCommand usercmd = new SqlCommand(command1, connect);
                        usercmd.Parameters.AddWithValue("@Acc_Status", "ACTIVE");
                        usercmd.ExecuteNonQuery();
                        connect.Close();
                        LoadRecords();
                        
                    }
                }
                
            }
            catch (Exception ex) {
                MessageBox.Show("Error: " + ex);
            }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Report r = new Report();
            connect.Open();
            String s1 = "SELECT * " +
                                     "FROM TBL_HORecords " +
                                     "JOIN TBL_User ON TBL_HORecords.ID = TBL_User.Acc_ID WHERE Acc_Status = 'DEACTIVATED' AND HomeownerType <> 'FORMER HOMEOWNER'";
            SqlCommand cmd = new SqlCommand(s1, connect);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Configure the report viewer
            ReportDataSource rds = new ReportDataSource("DeactAcc", dt);
            string reportPath = "HOAMS.DeactivatedList.rdlc";
            using (Stream reportStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath))
            {
                r.reportViewer1.LocalReport.LoadReportDefinition(reportStream);
            }

            r.reportViewer1.LocalReport.DataSources.Clear();
            r.reportViewer1.LocalReport.DataSources.Add(rds);
            r.reportViewer1.RefreshReport();


            ReportParameter Officer = new ReportParameter("user", LOGIN.fname + " " + LOGIN.lname);
            r.reportViewer1.LocalReport.SetParameters(Officer);

            ReportParameter pr = new ReportParameter("president", LOGIN.fname + " " + LOGIN.lname);
            r.reportViewer1.LocalReport.SetParameters(pr);

            connect.Close();
            r.ShowDialog();
        }
    }
}
