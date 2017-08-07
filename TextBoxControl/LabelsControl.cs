using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextBoxControl
{
    public partial class LabelsControl : UserControl
    {
        public LabelsControl()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
        }
        
        public String Sunrise
        {
            get
            {
                return this.label1.Text;
            }
            set
            {
                this.label1.Text = value;
            }
        }
        public String Sunset
        {
            get
            {
                return this.label2.Text;
            }
            set
            {
                this.label2.Text = value;
            }
        }

        private void LabelsControl_Load(object sender, EventArgs e)
        {

        }
    }
}
