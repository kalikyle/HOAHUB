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
    public partial class Overdue : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public Overdue()
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
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID WHERE (isOverDue = 1 OR isOverDueFee = 1) AND HomeownerType <> 'FORMER HOMEOWNER'";


                SqlCommand cmd = new SqlCommand(s1, connect);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (Convert.ToInt32(dr["isOverDue"]) == 1 || Convert.ToInt32(dr["isOverDueFee"]) == 1)
                    {
                        string fors = "";
                        DateTime dateA = DateTime.Now;
                        DateTime dateB = DateTime.Now;
                        string date1 = null;

                        if (Convert.ToInt32(dr["isOverDue"]) == 1 && Convert.ToInt32(dr["isOverDueFee"]) == 1)
                        {
                            fors = "AMORTIZATION & HOA FEE";
                            dateA = (DateTime)dr["NextPaymentDate"];
                            dateB = (DateTime)dr["NextHoaFeePayment"];
                            date1 = dateA.ToShortDateString() + " | " + dateB.ToShortDateString();

                        }
                        else if (Convert.ToInt32(dr["isOverDueFee"]) == 1 && Convert.ToInt32(dr["isOverDue"]) == 0)
                        {
                            fors = "HOA FEE";
                            dateA = (DateTime)dr["NextHoaFeePayment"];
                            date1 = dateA.ToShortDateString();
                        }
                        else if (Convert.ToInt32(dr["isOverDueFee"]) == 0 && Convert.ToInt32(dr["isOverDue"]) == 1)
                        {
                            fors = "AMORTIZATION";
                            dateA = (DateTime)dr["NextPaymentDate"];
                            date1 = dateA.ToShortDateString();
                        }

                        string name = dr["FName"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        HOsub.Rows.Add(dr["ID"].ToString(), name, date1, dr["HouseNum"].ToString(), dr["Street"].ToString(), dr["Block"].ToString(), dr["Lot"].ToString(), fors);
                    }

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
        
        private void Overdue_Load(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Report r = new Report();
            connect.Open();
            String s1 = "SELECT * " +
                                    "FROM TBL_HORecords " +
                                    "JOIN TBL_Payment ON TBL_HORecords.ID = TBL_Payment.HoID WHERE (isOverDue = 1 OR isOverDueFee = 1) AND HomeownerType <> 'FORMER HOMEOWNER'";
            SqlCommand cmd = new SqlCommand(s1, connect);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Configure the report viewer
            ReportDataSource rds = new ReportDataSource("overdue", dt);
            string reportPath = "HOAMS.Overdue.rdlc";
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
