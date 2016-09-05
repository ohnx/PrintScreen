using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS
{
    public partial class Post : Form
    {
        public Post(string link, Image img)
        {
            InitializeComponent();
            textBox1.Text = link;
            pictureBox1.Image = img;
        }
    }
}
