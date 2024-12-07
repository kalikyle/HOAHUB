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
using System.Diagnostics;

namespace HOAMS
{
    public partial class Amortization : Form
    {
        
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public Amortization()
        {
            InitializeComponent();
            
            LoadFinance();
            LoadTransacs();
            LoadAmort();
            latestDate();
            

        }
        public string type = null;
        public string amount = null;
        public string name = null;
        public static string overallamort = null;
        public static string Dates = null;
        public static string d1 = null;
        public void latestDate()
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand("SELECT MAX(TransacDate) AS Latest FROM TBL_PaidAmorts", connect);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime latest = dr.GetDateTime(0);
                label6.Text = latest.ToString();
                d1 = latest.ToString();

            }
            dr.Close();

            SqlCommand cmd1 = new SqlCommand("SELECT MAX(DateRemitt) AS Latest FROM TBL_AmortReport", connect);
            SqlDataReader dr1 = cmd1.ExecuteReader();
            while (dr1.Read())
            {
                DateTime latest1 = dr1.GetDateTime(0);
                Dates = latest1.ToShortDateString();

            }
            dr.Close();

            connect.Close();
        }
            public void LoadTransacs()
        {

            try
            {
                HOTransac.Rows.Clear();
                connect.Open();
                
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_PaidAmorts", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    
                   
                    HOTransac.Rows.Add(dr["TransacID"].ToString(), dr["BenName"].ToString(), dr["TransacDate"].ToString(), "+ PHP " +dr["Amort"].ToString());


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
        public void LoadAmort()
        {

            try
            {
                HOAmort.Rows.Clear();
                connect.Open();
               

                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_AmortReport", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {


                    
                    HOAmort.Rows.Add(dr["ReportID"].ToString(), dr["OfficerName"].ToString(), dr["DateRemitt"].ToString(), "- PHP " + dr["AmountRemitt"].ToString());
                    

                }
                dr.Close();
                connect.Close();
                HOAmort.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }
        public void LoadFinance()
        {

            try
            {

                connect.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Finance", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oFin.Text = "PHP " + dr["HoaAmort"].ToString();
                    mfee.Text = "PHP " + dr["Amortization"].ToString();
                    overallamort = dr["HoaAmort"].ToString();
                    DateTime dates = (DateTime)dr["LastAmortDate"];
                    date.Text = dates.ToShortDateString();
                    Dates = dates.ToShortDateString();


                }
                dr.Close();
                connect.Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }

        }
        private void btnAddExp_Click(object sender, EventArgs e)
        {

        }

        private void Remit_Click(object sender, EventArgs e)
        {
            label6.Text = Dates;
            Report r = new Report();
            connect.Open();
           
            SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_PaidAmorts", connect);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Configure the report viewer
            ReportDataSource rds = new ReportDataSource("DataSet2", dt);
            string reportPath = "HOAMS.AmortizationReport.rdlc";
            using (Stream reportStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath))
            {
                r.reportViewer1.LocalReport.LoadReportDefinition(reportStream);
            }


            // r.reportViewer1.LocalReport.ReportPath = "C:\\Users\\Garage88fort\\source\\repos\\HOAMS\\HOAMS\\AmortizationReport.rdlc";
            r.reportViewer1.LocalReport.DataSources.Clear();
            r.reportViewer1.LocalReport.DataSources.Add(rds);
            r.reportViewer1.RefreshReport();

            ReportParameter date = new ReportParameter("date",DateTime.Now.ToShortDateString());
            r.reportViewer1.LocalReport.SetParameters(date);

            ReportParameter lastDate = new ReportParameter("lastDate", Dates);
            r.reportViewer1.LocalReport.SetParameters(lastDate);

            ReportParameter Total = new ReportParameter("Total", "PHP " + overallamort);
            r.reportViewer1.LocalReport.SetParameters(Total);

            ReportParameter Officer = new ReportParameter("Officer", LOGIN.fname + " " + LOGIN.lname);
            r.reportViewer1.LocalReport.SetParameters(Officer);

            // Serialize the report data as a binary object
            byte[] reportData;
            string mimeType, encoding, extension;
            string[] streamIds;
            Warning[] warnings;

            reportData = r.reportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            // Save the report data to the database
            using (SqlConnection connection = new SqlConnection(LOGIN.CS))
            {
                connection.Open();
                string insertQuery = "INSERT INTO TBL_AmortReport (reportData, OfficerName, DateRemitt, AmountRemitt) VALUES (@reportData, @OfficerName, @DateRemitt, @AmountRemitt)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@reportData", reportData);
                insertCommand.Parameters.AddWithValue("@OfficerName", LOGIN.fname + " " + LOGIN.lname);
                insertCommand.Parameters.AddWithValue("@DateRemitt", DateTime.Now);
                insertCommand.Parameters.AddWithValue("@AmountRemitt", overallamort);
                insertCommand.ExecuteNonQuery();
            }

            connect.Close();
            //update tbl Finance
            connect.Open();
            SqlCommand cmd1 = new SqlCommand("update TBL_Finance set HoaAmort = @HoaAmort, LastAmortDate = @LastAmortDate where ID = 1", connect);
            cmd1.Parameters.AddWithValue("@HoaAmort", 0);
            cmd1.Parameters.AddWithValue("@LastAmortDate", DateTime.Now);
            cmd1.ExecuteNonQuery();
            connect.Close();

            //delete all form tbl_paidamort
            connect.Open();
            SqlCommand cmd2 = new SqlCommand("delete from TBL_PaidAmorts", connect);
            cmd2.ExecuteNonQuery();
            connect.Close();
            LoadFinance();
            LoadTransacs();
            LoadAmort();
            LOGIN.LogActivity("Remitted Amortization for SHFC");
            r.ShowDialog();
        }

        private void HOAmort_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                byte[] reportData;
                connect.Open();
                string selectQuery = "SELECT reportData FROM TBL_AmortReport WHERE ReportID = @ReportID";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connect);
                selectCommand.Parameters.AddWithValue("@ReportID", HOAmort.Rows[e.RowIndex].Cells[0].Value);
                reportData = (byte[])selectCommand.ExecuteScalar();
                connect.Close();



                string filePath = "HOAMS.Amortization.pdf";

                // Create a file stream and write the binary data
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(reportData, 0, reportData.Length);
                }

                // Optionally, you can open the file to validate if it can be viewed correctly
                Process.Start(filePath);


                // Load the report data into the Report form
                /* Report reportForm = new Report();
                 try
                 {
                     reportForm.reportViewer1.Clear();
                     using (MemoryStream reportStream = new MemoryStream(reportData))
                     {
                         reportForm.reportViewer1.LocalReport.LoadReportDefinition(reportStream);
                     }

                     reportForm.reportViewer1.RefreshReport();
                     reportForm.ShowDialog();
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("An error occurred while loading the report: " + ex.Message);
                 }*/
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            HOTransac.Dock = DockStyle.Fill;
            HOAmort.Dock = DockStyle.None;
            HOTransac.BringToFront();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            HOAmort.Dock = DockStyle.Fill;
            HOTransac.Dock = DockStyle.None;
            HOAmort.BringToFront();
        }

        private void Amortization_Click(object sender, EventArgs e)
        {
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel12_Click(object sender, EventArgs e)
        {
            HOTransac.Dock = DockStyle.Left;
            HOAmort.Dock = DockStyle.Fill;
            HOAmort.BringToFront();
        }

        private void Amortization_Load(object sender, EventArgs e)
        {
            cmB.Items.Add("Daily");
            cmB.Items.Add("Monthly");
            cmB.Items.Add("Yearly");
            cmB.Items.Add("All");
            cmB.DropDownStyle = ComboBoxStyle.DropDownList;
            cmB.Text = "All";
        }

        private void cmB_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = cmB.SelectedItem.ToString();
            if (selectedOption != "All")
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime startDate, endDate;
                string dateColumn;
                decimal totalTransacFee = 0;
                decimal TF = 0;

                using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                {
                    connection.Open();
                    string query = "";

                    // Determine the date range and date column based on the selected sorting option
                    if (selectedOption == "Daily")
                    {
                        startDate = currentDate.Date;
                        endDate = currentDate.Date.AddDays(1).AddSeconds(-1);
                        dateColumn = "TransacDate";
                    }
                    else if (selectedOption == "Monthly")
                    {
                        startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                        dateColumn = "TransacDate";
                    }
                    else if (selectedOption == "Yearly")
                    {
                        startDate = new DateTime(currentDate.Year, 1, 1);
                        endDate = new DateTime(currentDate.Year, 12, 31);
                        dateColumn = "TransacDate";
                    }

                    else
                    {
                        // Handle the case when no sorting option is selected or an invalid option is selected
                        return;
                    }

                    // Construct the SQL query using the determined date range and column
                    query = "SELECT * FROM TBL_PaidAmorts " +
                             "WHERE " + dateColumn + " BETWEEN @StartDate AND @EndDate " +
                             "ORDER BY " + dateColumn + " DESC";

                    // Execute the query and process the results
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    SqlDataReader dr = command.ExecuteReader();
                    HOTransac.Rows.Clear();
                    while (dr.Read())
                    {
                        
                       
                        TF = Convert.ToDecimal(dr["Amort"]);
                        totalTransacFee = totalTransacFee + TF;
                        HOTransac.Rows.Add(dr["TransacID"].ToString(), dr["BenName"].ToString(), dr["TransacDate"].ToString(), "+ PHP " + dr["Amort"].ToString());

                    }
                  
                     
                    dr.Close();
                }
                oFin.Text = "PHP " + totalTransacFee.ToString();

            }
            else
            {
                oFin.Text = "PHP " + overallamort;
                LoadTransacs();

            }
        }
    }
}
