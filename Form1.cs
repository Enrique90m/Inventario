using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void facturaBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.facturaBindingSource.EndEdit();
            ////this.tableAdapterManager.UpdateAll(this.baseDeDatosDataSet1);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'baseDeDatosDataSet1.Factura' Puede moverla o quitarla según sea necesario.
            //this.facturaTableAdapter.Fill(this.baseDeDatosDataSet1.Factura);

        }
    }
}
