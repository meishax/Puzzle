using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myPuzzle
{
    public partial class Form_Original : Form
    {
        public string picpath;
        public Form_Original()
        {
            InitializeComponent();
        }

        private void pb_Original_MouseDown(object sender, MouseEventArgs e)
        {

        }
        private void Form_Original_Load(object sender, EventArgs e)
        {
            pb_Original.Image = CutPicture.Resize(picpath, 600, 600);
        }

        private void Form_Original_Load_1(object sender, EventArgs e)
        {
            pb_Original.Image = CutPicture.Resize(picpath, 600, 600);
        }

        private void pb_Original_Click(object sender, EventArgs e)
        {

        }
    }
}
