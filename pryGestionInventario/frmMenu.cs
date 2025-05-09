using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryGestionInventario
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void agregarProdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAgregarProd v = new frmAgregarProd();
            v.ShowDialog();
            this.Hide();
        }

        private void buscarProductoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEliminarProd v = new frmEliminarProd();
            v.ShowDialog();
            this.Hide();

        }
    }
}
