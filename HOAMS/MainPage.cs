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

namespace HOAMS
{
    public partial class MainPage : Form
    {
        HomeOwnerRecords s = new HomeOwnerRecords();
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        
        public MainPage()
        {
            InitializeComponent();
            ADMIN.Text = LOGIN.users.ToUpper();
            SetCircularPictureBox(MiniPic);
            //retrive the picture from db
            connect.Open();
            //string tables = LOGIN_PAGE.AccType == "ADMINISTRATOR" ? "TBL_User" : "TBL_HOUser";
            SqlCommand cmd = new SqlCommand("Select Picture from TBL_User where Username=@user", connect);
            cmd.Parameters.AddWithValue("user", LOGIN.users);
            SqlDataReader read;
            read = cmd.ExecuteReader();
            if (read.Read())//.Read
            {


                // if the image is null in databse
                if (read.IsDBNull(0))
                {
                    MiniPic.Image = MiniPic.Image;// the picture will remain
                }
                else
                {
                    //else will retrive the picture in databases
                    byte[] images = ((byte[])read[0]);
                    MemoryStream msStream = new MemoryStream(images);
                    MiniPic.Image = Image.FromStream(msStream);
                }

            }

            connect.Close();
        }
        private void SetCircularPictureBox(PictureBox picBox)
        {
            // Set the picture box's Region property to a circle
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, picBox.Width, picBox.Height);
            picBox.Region = new System.Drawing.Region(path);

        }
        public void UpdateImage(Image image)
        {
            MiniPic.Image = image;
        }
        public void UpdateLabel(string newText)
        {
            ADMIN.Text = newText.ToUpper();
        }

        private void ADMIN_Click(object sender, EventArgs e)
        {
            if (LOGIN.AccType == "ADMINISTRATOR")
            {
                clearPanel();
                AdminAccount ad = new AdminAccount();
                ad.TopLevel = false;
                ad.Dock = DockStyle.Fill;
                FormPanel.Controls.Add(ad);
                ad.BringToFront();
                ad.Show();


            }
            else if (LOGIN.AccType == "HOMEOWNER")
            {
                clearPanel();
                HOInfo ho = new HOInfo(s);
                ho.TopLevel = false;
                ho.FormBorderStyle = FormBorderStyle.None;
                ho.Dock = DockStyle.Fill;
                FormPanel.Controls.Add(ho);
                ho.UpdateBtn.Visible = false;
                ho.button2.Visible = false;
                ho.button1.Visible = false;
                ho.button3.Visible = false;
                ho.groupBox21.Visible = true;
                ho.BringToFront();
                ho.HOAccount();
                ho.LoadTransacs();
                ho.Show();
               

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            LOGIN.LogActivity("LOG OUT");
            DialogResult diagres = MessageBox.Show("Do You want to Logout?", "Confirm", MessageBoxButtons.YesNo);
            if (diagres == DialogResult.Yes)
            {

                this.Dispose();
                LOGIN login = new LOGIN();
                login.Show();
            }
        }

        private void RecordBtn_Click(object sender, EventArgs e)
        {
            clearPanel();
            HomeOwnerRecords ho = new HomeOwnerRecords();
            ho.TopLevel = false;
            ho.Dock = DockStyle.Fill;
            FormPanel.Controls.Add(ho);
            ho.BringToFront();
            ho.LoadRecords();
            ho.Show();

        }

        private void Dashboard_Btn_Click(object sender, EventArgs e)
        {
            clearPanel();
            DashBoard ds = new DashBoard();
            ds.Dock = DockStyle.Fill;
            ds.TopLevel = false;
            FormPanel.Controls.Add(ds);
            ds.BringToFront();
            ds.Show();
        }

        private void bilingBTN_Click(object sender, EventArgs e)
        {
            if (LOGIN.AccType == "ADMINISTRATOR")
            {
                clearPanel();
                Billing ds = new Billing();
                ds.Dock = DockStyle.Fill;
                ds.TopLevel = false;
                FormPanel.Controls.Add(ds);
                ds.BringToFront();
                ds.Show();
            }
            else if (LOGIN.AccType == "HOMEOWNER")
            {
                clearPanel();
                HOBilling ds = new HOBilling();
                ds.Dock = DockStyle.Fill;
                ds.TopLevel = false;
                FormPanel.Controls.Add(ds);
                ds.BringToFront();
                ds.Show();
            }
        }
        private void TRBtn_Click(object sender, EventArgs e)
        {
           
        }

        private void TRBtn_Click_1(object sender, EventArgs e)
        {
            clearPanel();
            Transactions tr = new Transactions();
            tr.Dock = DockStyle.Fill;
            tr.TopLevel = false;
            FormPanel.Controls.Add(tr);
            tr.BringToFront();
            tr.Show();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            clearPanel();
            DashBoard ds = new DashBoard();
            ds.Dock = DockStyle.Fill;
            ds.TopLevel = false;
            FormPanel.Controls.Add(ds);
            ds.BringToFront();
            ds.Show();
        }

        private void ComBTN_Click(object sender, EventArgs e)
        {
            if (LOGIN.AccType == "ADMINISTRATOR")
            {
                clearPanel();
                adminComplain ds = new adminComplain();
                ds.Dock = DockStyle.Fill;
                ds.TopLevel = false;
                FormPanel.Controls.Add(ds);
                ds.BringToFront();
                ds.Show();
            }
            else if (LOGIN.AccType == "HOMEOWNER")
            {
                clearPanel();
                ComplainLogs cm = new ComplainLogs();
                cm.Dock = DockStyle.Fill;
                cm.TopLevel = false;
                FormPanel.Controls.Add(cm);
                cm.BringToFront();
                cm.Show();
            }
           
        }

       
        public void clearPanel()
        {

            FormPanel.Controls.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void OffBtn_Click(object sender, EventArgs e)
        {
            clearPanel();
            Officers cm = new Officers();
            cm.Dock = DockStyle.Fill;
            cm.TopLevel = false;
            FormPanel.Controls.Add(cm);
            cm.BringToFront();
            cm.Show();
        }
    }
}
