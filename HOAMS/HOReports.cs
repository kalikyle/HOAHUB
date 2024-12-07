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
    public partial class HOReports : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public HOReports()
        {
            InitializeComponent();
            SubtituteReports ho = new SubtituteReports();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FPANEL.Controls.Add(ho);
            ho.BringToFront();
            ho.Show();
            CountHomeowners();
            CountPopulation(ho1);

        }
        public static int ho1 = 0;
        public static int pop = 0;

        public void CountHomeowners()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM TBL_HORecords WHERE HomeownerType <> 'FORMER HOMEOWNER'";
                   

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        

                        int rowCount = (int)command.ExecuteScalar();
                        hOPOP.Text = rowCount.ToString();
                        ho1 = rowCount;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }
        public void CountPopulation(int homeowners)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(LOGIN.CS))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM TBL_Relatives";


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        int rowCount = (int)command.ExecuteScalar();
                        int population = rowCount + homeowners;
                        popTxt.Text = population.ToString();
                        pop = population;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            SubtituteReports ho = new SubtituteReports();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FPANEL.Controls.Add(ho);
            ho.BringToFront();
            ho.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PaidHo ho = new PaidHo();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FPANEL.Controls.Add(ho);
            ho.BringToFront();
            ho.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeactivatedHo ho = new DeactivatedHo();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FPANEL.Controls.Add(ho);
            ho.BringToFront();
            ho.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Overdue ho = new Overdue();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FPANEL.Controls.Add(ho);
            ho.BringToFront();
            ho.Show();
        }
    }
}
