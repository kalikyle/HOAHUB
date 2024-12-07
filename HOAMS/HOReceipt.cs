using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HOAMS
{
    public partial class HOReceipt : Form
    {
        SqlConnection connect = new SqlConnection(LOGIN.CS);
        Billing b;
        public HOReceipt(Billing b)
        {
            InitializeComponent();
            this.b = b;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Print(this.panel1);
        }
        public void loadAll()
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void Print(Panel pn1)
        {
            PrinterSettings ps = new PrinterSettings();
            toolTip1.SetToolTip(pictureBox1, "Print");
            panel1 = pn1;
            getprintArea(pn1);
            printPreviewDialog1.Document = printDocument1;
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            printPreviewDialog1.ShowDialog();

        }
        public Bitmap memoring;

        public void getprintArea(Panel pn1) {
            memoring = new Bitmap(pn1.Width, pn1.Height);
            int printHeight = pn1.Height;
            pn1.DrawToBitmap(memoring, new Rectangle(0, 0, pn1.Width, pn1.Height));

        }

        public void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(memoring, (pagearea.Width / 4) - (this.panel1.Width / 4), this.panel1.Location.Y);
        }
    }
}
