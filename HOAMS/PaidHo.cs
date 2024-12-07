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
using System.IO;
using System.Reflection;

namespace HOAMS
{
    public partial class PaidHo : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public PaidHo()
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
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID WHERE isPaidOff = 1";
                                    

                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string name = dr["FName"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    DateTime date = (DateTime)dr["AmortPaidDate"];
                    HOsub.Rows.Add(dr["ID"].ToString(),name,date.ToShortDateString(), dr["HouseNum"].ToString(), dr["Street"].ToString(), dr["Block"].ToString(), dr["Lot"].ToString());


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

        private void button1_Click(object sender, EventArgs e)
        {
            Report r = new Report();
            connect.Open();
            String s1 = "SELECT * " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID WHERE isPaidOff = 1";
            SqlCommand cmd = new SqlCommand(s1, connect);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Configure the report viewer
            ReportDataSource rds = new ReportDataSource("paidHO", dt);
            string reportPath = "HOAMS.PaidAmortHO.rdlc";
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
