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
    public partial class BackUp_RestoreDB : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        public BackUp_RestoreDB()
        {
            InitializeComponent();
            panel2.Visible = false;
            button1.Enabled = false;
            panel1.Visible = true;
            button2.Enabled = true;
            button4.Visible = false;
            button5.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            button1.Enabled = false;
            panel1.Visible = true;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            button2.Enabled = false;
            panel2.Visible = true;
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fol = new FolderBrowserDialog();
            if (fol.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fol.SelectedPath;
                button4.Visible = true;
            }

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            label6.Visible = true;
            progressBar1.Visible = true;
            // Display loading progress
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            string data = connect.Database.ToString();
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please Enter the file location ");
                button4.Visible = false;

            }
            else
            {
                await Task.Run(() =>
                {
                    UpdateProgressBar(50);
                    string command = "BACKUP DATABASE [" + data + "] TO DISK = '" + textBox1.Text + "\\" + "database" + "." + DateTime.Now.ToString("yyyyy-MM-dd--HH-mm-ss") + ".bak'";
                connect.Open();
                SqlCommand cmd = new SqlCommand(command,connect);
                cmd.ExecuteNonQuery();
                    UpdateProgressBar(100);
                });

                MessageBox.Show("Database: "+data+" has been successfully Backup");
                connect.Close();



                button4.Visible = false;
                progressBar1.Value = 0;
                label6.Visible = false;
                progressBar1.Visible = false;

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog fol = new OpenFileDialog();
            fol.Filter = "SQL SERVER database backup files|*.bak";
            fol.Title = "Database Restore";
            if (fol.ShowDialog()==DialogResult.OK)
            {
                textBox2.Text = fol.FileName;
                button5.Visible = true;
            }

        }
        // indicate that this button is asynchornous
        private async void button5_Click(object sender, EventArgs e)
        {
            label6.Visible = true;
            progressBar1.Visible = true;
            // Display loading progress
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            string data = connect.Database.ToString();
            connect.Open();
            try
            {
                //this is used to run the database restore operation 
                await Task.Run(() =>
                {

                    string str1 = string.Format("ALTER DATABASE [" + data + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                SqlCommand cmd1 = new SqlCommand(str1,connect);
                cmd1.ExecuteNonQuery();

                 UpdateProgressBar(33);

                    string str2 = "USE MASTER RESTORE DATABASE [" + data + "] FROM DISK = '" + textBox2.Text + "' WITH REPLACE;";
                SqlCommand cmd2 = new SqlCommand(str2, connect);
                cmd2.ExecuteNonQuery();

                    UpdateProgressBar(66);

                    string str3 = string.Format("ALTER DATABASE [" + data + "] SET MULTI_USER");
                SqlCommand cmd3 = new SqlCommand(str3, connect);
                cmd3.ExecuteNonQuery();

                    UpdateProgressBar(100);

                });

                MessageBox.Show("Database successfully restored");
                connect.Close();
                

                progressBar1.Value = 0;
                label6.Visible = false;
                progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex);
                label6.Visible = false;
                progressBar1.Visible = false;
            }
            
          
        }
        private void UpdateProgressBar(int value)
        {
            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = value));
        }

    }
}
