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
using System.Diagnostics;

namespace HOAMS
{
    public partial class SubtituteReports : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public SubtituteReports()
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
                SqlCommand cmd = new SqlCommand("select * from TBL_Subtitute", connect);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    HOsub.Rows.Add(dr["SubID"].ToString(), dr["FormerHO"].ToString(), dr["SubHO"].ToString(), dr["SubDate"].ToString(), dr["Street"].ToString(), dr["Block"].ToString(), dr["Lot"].ToString());


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

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                byte[] reportData;
                connect.Open();
                string selectQuery = "SELECT SubReport FROM TBL_Subtitute WHERE SubID = @SubID";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connect);
                selectCommand.Parameters.AddWithValue("@SubID", HOsub.Rows[e.RowIndex].Cells[0].Value);
                reportData = (byte[])selectCommand.ExecuteScalar();
                connect.Close();


                string filePath = "HOAMS.SubtituteReport.pdf";

                // Create a file stream and write the binary data
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(reportData, 0, reportData.Length);
                }

                // Optionally, you can open the file to validate if it can be viewed correctly
                Process.Start(filePath);
            }
        }
    }
}
