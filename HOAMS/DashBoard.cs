using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOAMS
{
    public partial class DashBoard : Form
    {
        private Timer timer;
        private List<Image> images;
        private int currentImageIndex;
        public DashBoard()
        {
            InitializeComponent();
           
            // Load the images from resources
            images = new List<Image>
            {
                Properties.Resources._350359402_781531943597490_4244555300642015700_n__1_,
                Properties.Resources._66687219_2361462737264900_8946747010229207040_n,
                Properties.Resources._66623979_2361461930598314_8936856705568866304_n,
                Properties.Resources.Screenshot_2023_06_01_224446
                // Add more images as needed
            };

            // Set the initial image
            currentImageIndex = 0;
            pictureBox1.Image = images[currentImageIndex];

            // Set up the timer
            timer = new Timer();
            timer.Interval = 5000; // 5 seconds
            timer.Tick += timer1_Tick;
            timer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Increment the image index
            currentImageIndex++;
            if (currentImageIndex >= images.Count)
                currentImageIndex = 0;

            // Change the image in the PictureBox
            pictureBox1.Image = images[currentImageIndex];
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DashBoard_Load(object sender, EventArgs e)
        {
            /*label22.Parent = pictureBox1;
            label22.BackColor = Color.Transparent;
            label23.Parent = pictureBox1;
            label23.BackColor = Color.Transparent;*/
            pictureBox3.Parent = pictureBox1;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox2.Parent = pictureBox1;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox4.Parent = pictureBox1;
            pictureBox4.BackColor = Color.Transparent;
            label21.Text = LOGIN.position;
            HOReports ne = new HOReports();
            Amortization m = new Amortization();
            HOAFinancials h = new HOAFinancials();
            ne.CountHomeowners();
            popTxt.Text = HOReports.pop.ToString();
            hOPOP.Text = HOReports.ho1.ToString();
            m.LoadFinance();
            label7.Text = "PHP " + Amortization.overallamort.ToString();
            date.Text = Amortization.Dates.ToString();
            label6.Text = Amortization.d1.ToString();
            label12.Text = HOAFinancials.expdate.ToString();
            oExp.Text = HOAFinancials.expamount.ToString();
            oFin.Text = HOAFinancials.oFinance.ToString();
            label8.Text = HOAFinancials.datefee.ToString();

            UpdateDateTimeLabel();

            // Start a timer to update the label text every second
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += timer2_Tick;
            timer.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            UpdateDateTimeLabel();
        }
        private void UpdateDateTimeLabel()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            label20.Text = now.ToString("dddd, MMMM d, yyyy");

            // Update the time label
            label19.Text = now.ToString("hh:mm:ss tt");

            // Update the greeting label
            if (now.Hour < 12)
            {
                label17.Text = "GOOD MORNING, " + LOGIN.fname.ToUpper();
            }
            else if (now.Hour < 18)
            {
                label17.Text = "GOOD AFTERNOON, " + LOGIN.fname.ToUpper();
            }
            else
            {
                label17.Text = "GOOD EVENING, " + LOGIN.fname.ToUpper();
            }
        }
    }
}
