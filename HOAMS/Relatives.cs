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
    public partial class Relatives : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        HOInfo m;
        public Relatives(HOInfo m)
        {
            InitializeComponent();
            this.m = m;
            loadrela();
        }
        public void loadrela()
        {
            try
            {
                relaTBL.Rows.Clear();
                connect.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_Relatives WHERE HoID = @HoID", connect);
                cmd.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                SqlDataReader dr1 = cmd.ExecuteReader();
                while (dr1.Read())
                {
                    DateTime birthdate = (DateTime)dr1["Birthday"];
                    relaTBL.Rows.Add(dr1["FName"].ToString(), dr1["MName"].ToString(), dr1["LName"].ToString(), birthdate.ToShortDateString(), dr1["Age"].ToString(), dr1["Contact"].ToString());


                }
                dr1.Close();
                connect.Close();
                relaTBL.ClearSelection();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }


        }
        public string relID = null;
        private void relaTBL_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button2.Visible = false;
            LogoutBtn.Visible = true;
            UpdateBtn.Visible = true;
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex >= 0)//if row is clicked is greater than -1 and column >= 0
                {


                    connect.Open();
                    //SqlCommand cm = new SqlCommand("select Picture as picture, * from TBL_HORecords where ID like '" + HOData.Rows[e.RowIndex].Cells[0].Value.ToString() + "'", connect);
                    SqlCommand cm = new SqlCommand("SELECT * FROM TBL_Relatives where Contact LIKE '" + relaTBL.Rows[e.RowIndex].Cells[5].Value.ToString() + "'", connect);
                    SqlDataReader dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {


                        relID = dr["FamID"].ToString();


                    }




                    dr.Close();
                    connect.Close();

                    DataGridViewRow selectedRow = relaTBL.Rows[e.RowIndex];

                    // Populate the textboxes with the cell values from the selected row
                    Nametxt.Text = selectedRow.Cells["first"].Value.ToString();
                    MNameTxt.Text = selectedRow.Cells["MidName"].Value.ToString();
                    LNametxt.Text = selectedRow.Cells["LastName"].Value.ToString();
                    Age.Text = selectedRow.Cells["Ages"].Value.ToString();
                    bDate.Value = DateTime.Parse(selectedRow.Cells["BD"].Value.ToString());
                    contact.Text = selectedRow.Cells["cont"].Value.ToString();


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get the values from textboxes
            string value1 = Nametxt.Text;
            string value2 = MNameTxt.Text;
            string value3 = LNametxt.Text;
            string value4 = bDate.Text;
            string value5 = Age.Text;
            string value6 = contact.Text;

            // Add a new row to the DataTable
            relaTBL.Rows.Add(value1, value2, value3, value4, value5, value6);
            // Clear the textboxes
            Nametxt.Text = string.Empty;
            MNameTxt.Text = string.Empty;
            LNametxt.Text = string.Empty;
            bDate.Text = string.Empty;
            Age.Text = string.Empty;
            contact.Text = string.Empty;
        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (relaTBL.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = relaTBL.SelectedRows[0];

                    // Remove the selected row from the DataGridView
                    relaTBL.Rows.Remove(selectedRow);

                    MessageBox.Show("Row deleted successfully.");
                    Nametxt.Text = string.Empty;
                    MNameTxt.Text = string.Empty;
                    LNametxt.Text = string.Empty;
                    Age.Text = string.Empty;
                    bDate.Value = DateTime.Now;
                    contact.Text = string.Empty;
                    button2.Visible = true;
                    LogoutBtn.Visible = false;
                    UpdateBtn.Visible = false;
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (relaTBL.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = relaTBL.SelectedRows[0];

                    // Update the cell values of the selected row with the values from the textboxes
                    selectedRow.Cells["first"].Value = Nametxt.Text;
                    selectedRow.Cells["MidName"].Value = MNameTxt.Text;
                    selectedRow.Cells["LastName"].Value = LNametxt.Text;
                    selectedRow.Cells["Ages"].Value = Age.Text;
                    selectedRow.Cells["BD"].Value = bDate.Value.ToShortDateString();
                    selectedRow.Cells["cont"].Value = contact.Text;

                    MessageBox.Show("Row updated successfully.");
                    Nametxt.Text = string.Empty;
                    MNameTxt.Text = string.Empty;
                    LNametxt.Text = string.Empty;
                    Age.Text = string.Empty;
                    bDate.Value = DateTime.Now;
                    contact.Text = string.Empty;
                    button2.Visible = true;
                    LogoutBtn.Visible = false;
                    UpdateBtn.Visible = false;
                }
                else
                {
                    MessageBox.Show("Please select a row to update.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect.Open();
            SqlCommand cm5 = new SqlCommand("delete from TBL_Relatives where HoID = @id", connect);
            cm5.Parameters.AddWithValue("@id", HomeOwnerRecords.ID);//delete from TBL_Relatives
            cm5.ExecuteNonQuery();

            foreach (DataGridViewRow row in relaTBL.Rows)
            {
                // Skip the last row if it is the new row
                if (!row.IsNewRow)
                {
                    // Retrieve the cell values from the row and create the SQL INSERT statement
                    string first = row.Cells["first"].Value.ToString();
                    string middle = row.Cells["MidName"].Value.ToString();
                    string last = row.Cells["LastName"].Value.ToString();
                    string date = row.Cells["BD"].Value.ToString();
                    string age = row.Cells["Ages"].Value.ToString();
                    string con = row.Cells["cont"].Value.ToString();

                    string query = $"INSERT INTO TBL_Relatives (FName, MName, LName, Age, Birthday, Contact, HoID ) VALUES ('{first}', '{middle}', '{last}', '{age}', '{date}', '{con}', @HoID)";

                    // Insert into the database
                    using (SqlCommand commands = new SqlCommand(query, connect))
                    {
                        commands.Parameters.AddWithValue("@HoID", HomeOwnerRecords.ID);
                        commands.ExecuteNonQuery();
                    }
                }

            }
            MessageBox.Show("Relatives Successfully Updated!");
            connect.Close();
            this.Hide();
            m.loadrela();
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            Nametxt.Text = string.Empty;
            MNameTxt.Text = string.Empty;
            LNametxt.Text = string.Empty;
            Age.Text = string.Empty;
            bDate.Value = DateTime.Now;
            contact.Text = string.Empty;
            button2.Visible = true;
            LogoutBtn.Visible = false;
            UpdateBtn.Visible = false;
        }
    }
}
