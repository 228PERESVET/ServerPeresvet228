using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopServer2k18
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            new PeresvetServerNetwork(rtb_console, pb_image);
            new WebServer(80);

        }
    }
}
