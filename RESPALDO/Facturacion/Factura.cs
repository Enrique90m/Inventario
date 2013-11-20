using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlServerCe;

namespace Facturacion
{
    public partial class Factura : Form, EnlaceEntreFormularios
    {
        public Factura()
        {
            InitializeComponent();
            Fecha.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
            Facturacion_textbox_Total.Text = "0";
            Facturacion_textbox_subtotal.Text = "0";
            this.conectaBD();//conecta con BD
            this.ponedatos(false);

        }
        //VARIABLES GENERALES - 25/AGO/11
        public DataTable dt;
        public SqlCeDataAdapter da;
        public SqlCeConnection PathBD;
        public DataRow row;
        public int renglon = 0;
        public bool actualiza = false;

        #region Metodos
        public void AgregaACarrito(int clave, string nombre, int precio)//SOLO LOS AGREGA AL DATAGRID , NO HACE NADA A LA BASE DE DATOS
        {
            //Objeto donde ponemos el comando SQL
            //SqlCeCommand ComandoSQL = new SqlCeCommand("UPDATE Productos SET prod_existencia = prod_existencia - 1  WHERE clave = @Param2", PathBD);
            // SqlCeParameter myParam2 = new SqlCeParameter("@Param2", SqlDbType.Int, 4);
            //myParam2.Value = clave;
            // ComandoSQL.Parameters.Add(myParam2);


            //ejecuta el comando
            // ComandoSQL.ExecuteNonQuery();

            //cierra la bd
            // PathBD.Close();
            Facturacion_DatagridArticulos.Rows.Add(clave, nombre, precio);
        }
        public void conectaBD()
        {
            SqlCeConnection PathBD = new SqlCeConnection("Data Source=C:\\Facturacion\\Facturacion\\BaseDeDatos.sdf;Persist Security Info=False;");
            //abre la conexion
            try
            {
                string SeleccionaTodosLosDatos = "SELECT * FROM Factura ORDER BY Num_factura";
                da = new SqlCeDataAdapter(SeleccionaTodosLosDatos, PathBD);
                // Crear los comandos de insertar, actualizar y eliminar
                SqlCeCommandBuilder cb = new SqlCeCommandBuilder(da);
                // Asignar los comandos al DataAdapter
                // (se supone que lo hace automáticamente, pero...)
                da.UpdateCommand = cb.GetUpdateCommand();
                da.InsertCommand = cb.GetInsertCommand();
                da.DeleteCommand = cb.GetDeleteCommand();
                dt = new DataTable();
                // Llenar la tabla con los datos indicados
                da.Fill(dt);

                PathBD.Open();
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
                return;
            }

        }
        public void CambiaDeTabla(String Tabla, string comando)
        {
            try
            {
                da = new SqlCeDataAdapter("SELECT * FROM " + Tabla + " " + comando, PathBD);
                // Crear los comandos de insertar, actualizar y eliminar
                SqlCeCommandBuilder cb = new SqlCeCommandBuilder(da);
                // Asignar los comandos al DataAdapter
                // (se supone que lo hace automáticamente, pero...)
                da.UpdateCommand = cb.GetUpdateCommand();
                da.InsertCommand = cb.GetInsertCommand();
                da.DeleteCommand = cb.GetDeleteCommand();
                dt = new DataTable();
                // Llenar la tabla con lo s datos indicados
                da.Fill(dt);
                PathBD.Open();
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
                return;
            }
        }       
        public void ponedatos(bool activados)
        {
            this.BuscarAlgoenTabla("SELECT * FROM Factura ORDER BY Num_factura");
            if (dt.Rows.Count == 0)//no hay registros
            {
                MessageBox.Show("No hay datos en la tabla factura");
                foreach (Control control in this.groupBox1.Controls)                
                    control.Enabled = false;                
                
                foreach(Control c in this.groupBox2.Controls)
                    if (c is TextBox)
                        c.Text = "";

                foreach (Control c in this.groupBox3.Controls)
                    if (c is TextBox)
                        c.Text = "";

                button_nuevo.Enabled = true;
                return;
            }

            Facturacion_DatagridArticulos.Rows.Clear();
            row = dt.Rows[renglon];
            DatosClienteIdcliente.Text = row["id_Socio"].ToString();
            DatosCliente_Folio.Text = row["Num_Factura"].ToString();
            DatosCliente_cliente.Text = row["cliente"].ToString();
            DatosCliente_DNI.Text = row["DNI"].ToString();
            DatosCliente_direccion.Text = row["Direccion"].ToString();
            DatosCliente_localidad.Text = row["Localidad"].ToString();
            DatosCliente_cp.Text = row["Cp"].ToString();
            DatosCliente_provincia.Text = row["Provincia"].ToString();
            DatosCliente_pais.Text = row["Pais"].ToString();
            Fecha.Text = row["fecha"].ToString();
            Facturacion_textbox_subtotal.Text = row["Subtotal"].ToString();
            Facturacion_textbox_iva.Text = row["Iva"].ToString();
            Facturacion_textbox_Total.Text = row["Total"].ToString();
            Facturacion_DatagridArticulos.Rows.Clear();
           
            for (int i = 0; i < int.Parse(row["TotalDatos"].ToString()); i++)
            {                
                Facturacion_DatagridArticulos.Rows.Add(
                    row["Codigo" + (i + 1)].ToString(),
                    row["Cantidad" + (i + 1)].ToString(),
                    row["Descripcion" + (i + 1)].ToString(),
                    row["PrecioUnitario" + (i + 1)].ToString());
            }

            if (activados == false)
            {
                foreach (Control c in this.groupBox1.Controls)
                    if (c is TextBox)
                        ((TextBox)c).ReadOnly = true;
                    else
                        if (c is Button)
                            if (c.Name.Contains("Guardar"))
                                ((Button)c).Enabled = false;
                            else
                                ((Button)c).Enabled = true;

                foreach (Control c in this.groupBox2.Controls)
                    if (c is TextBox)
                        ((TextBox)c).ReadOnly = true;
                    else
                        if (c is Button)
                            ((Button)c).Enabled = false;

                foreach (Control c in this.groupBox3.Controls)
                    if (c is TextBox)
                        ((TextBox)c).ReadOnly = true;
                    else
                        if (c is Button)
                            ((Button)c).Enabled = false;
               
                Facturacion_DatagridArticulos.ReadOnly = true;
            }

                   
        }
        public void AgregaFactura()
        {
            this.BuscarAlgoenTabla("SELECT * FROM Factura ORDER BY Num_factura");
            bool ContieneArticulosDeInventario = false;
            // SI MODIFICO LA FACTURA, SOLO ACTUALIZA LOS DATOS DEL RENGLON
            if (actualiza == true)
            {
                 int folio = int.Parse(DatosCliente_Folio.Text) - 1;
               // dt.Rows[folio]["Num_Factura"] = dt.Rows.Count + 1;
                dt.Rows[folio]["Cliente"] = DatosCliente_cliente.Text;
                dt.Rows[folio]["Direccion"] = DatosCliente_direccion.Text;
                dt.Rows[folio]["Localidad"] = DatosCliente_localidad.Text;

                if (!string.IsNullOrEmpty(DatosClienteIdcliente.Text) && !string.IsNullOrWhiteSpace(DatosClienteIdcliente.Text))
                    dt.Rows[folio]["id_Socio"] = long.Parse(DatosClienteIdcliente.Text);

                if (!string.IsNullOrEmpty(DatosCliente_DNI.Text) && !string.IsNullOrWhiteSpace(DatosCliente_DNI.Text))
                    dt.Rows[folio]["DNI"] = long.Parse(DatosCliente_DNI.Text);

                dt.Rows[folio]["Direccion"] = !string.IsNullOrEmpty(DatosCliente_direccion.Text) && !string.IsNullOrWhiteSpace(DatosCliente_direccion.Text) ? DatosCliente_direccion.Text : "";
                dt.Rows[folio]["Localidad"] = !string.IsNullOrEmpty(DatosCliente_localidad.Text) && !string.IsNullOrWhiteSpace(DatosCliente_localidad.Text) ? DatosCliente_localidad.Text : "";

                if (!string.IsNullOrEmpty(DatosCliente_cp.Text) && !string.IsNullOrWhiteSpace(DatosCliente_cp.Text))
                    dt.Rows[folio]["Cp"] = long.Parse(DatosCliente_cp.Text);

                dt.Rows[folio]["Provincia"] = !string.IsNullOrEmpty(DatosCliente_provincia.Text) && !string.IsNullOrWhiteSpace(DatosCliente_provincia.Text) ? DatosCliente_provincia.Text : "";
                dt.Rows[folio]["Pais"] = !string.IsNullOrEmpty(DatosCliente_pais.Text) && !string.IsNullOrWhiteSpace(DatosCliente_pais.Text) ? DatosCliente_pais.Text : "";
                dt.Rows[folio]["Fecha"] = Fecha.Text;

                dt.Rows[folio]["Subtotal"] = double.Parse(Facturacion_textbox_subtotal.Text);
                dt.Rows[folio]["Iva"] = double.Parse(Facturacion_textbox_iva.Text);
                dt.Rows[folio]["Total"] = double.Parse(Facturacion_textbox_Total.Text);

                //BUSCA SI EXISTEN ARTICULOS DE INVENTARIO, SI ENCUENTRA UNO MARCA QUE SI HAY Y SE SALE PARA NO SEGUIRLE
                for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                    if (Facturacion_DatagridArticulos.Rows[i].ReadOnly == true)
                    {
                        ContieneArticulosDeInventario = true;
                        i = Facturacion_DatagridArticulos.Rows.Count;
                    }

                for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                {
                    if (Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString() != "")
                        dt.Rows[folio]["Codigo" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString();

                    if (Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString() != "")
                        dt.Rows[folio]["Cantidad" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString();

                    dt.Rows[folio]["Descripcion" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[2].Value.ToString();
                    dt.Rows[folio]["PrecioUnitario" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[3].Value.ToString();
                }
                //TOTAL DE ARTICULOS COMPRADOS
                dt.Rows[folio]["TotalDatos"] = Facturacion_DatagridArticulos.Rows.Count;

                try
                {
                    da.Update(dt);
                    dt.AcceptChanges();
                }
                catch (DBConcurrencyException ex)
                {
                    MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                    return;
                }
                actualiza = false;
                int codigo1;
                //SE AGREGO A LA TABLA FISICA, AHORA BUSCA CADA ARTICULO AGREGADO DE INVENTARIO Y LE RESTA LOS VENDIDOS A LA EXISTENCIA
                for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                    if (Facturacion_DatagridArticulos.Rows[i].ReadOnly == true)
                    {
                        codigo1 = int.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString());
                        this.BuscarAlgoenTabla("SELECT * FROM Productos WHERE clave = " + codigo1.ToString());
                        dt.Rows[0]["prod_existencia"] = long.Parse(dt.Rows[0]["prod_existencia"].ToString()) - long.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString());
                        if (int.Parse(dt.Rows[0]["prod_existencia"].ToString()) == 0)
                            dt.Rows[0]["vendido"] = true;

                    }

                try
                {
                    da.Update(dt);
                    dt.AcceptChanges();
                }
                catch (DBConcurrencyException ex)
                {
                    MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                    return;
                }
                this.BuscarAlgoenTabla("SELECT * FROM Factura ");
                MessageBox.Show("Factura actualizada correctamente!","",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                return;
            }
            

            //ANTES DE ASIGNARLE LOS VALORES VERIFICA QUE LOS HAYA INGRESADO
            row = dt.NewRow();

            row["Num_Factura"] = int.Parse(DatosCliente_Folio.Text);                    
            row["Cliente"] = DatosCliente_cliente.Text;
            row["Direccion"] = DatosCliente_direccion.Text;
            row["Localidad"] = DatosCliente_localidad.Text;

            if (!string.IsNullOrEmpty(DatosClienteIdcliente.Text) && !string.IsNullOrWhiteSpace(DatosClienteIdcliente.Text))
                row["id_Socio"] = long.Parse(DatosClienteIdcliente.Text);

            if (!string.IsNullOrEmpty(DatosCliente_DNI.Text)&& !string.IsNullOrWhiteSpace(DatosCliente_DNI.Text))
                row["DNI"] = long.Parse(DatosCliente_DNI.Text);

            row["Direccion"] = !string.IsNullOrEmpty(DatosCliente_direccion.Text) && !string.IsNullOrWhiteSpace(DatosCliente_direccion.Text) ? DatosCliente_direccion.Text : "";
            row["Localidad"] = !string.IsNullOrEmpty(DatosCliente_localidad.Text) && !string.IsNullOrWhiteSpace(DatosCliente_localidad.Text)  ? DatosCliente_localidad.Text : "";

            if (!string.IsNullOrEmpty(DatosCliente_cp.Text) && !string.IsNullOrWhiteSpace(DatosCliente_cp.Text))
                row["Cp"] = long.Parse(DatosCliente_cp.Text);

            row["Provincia"] = !string.IsNullOrEmpty(DatosCliente_provincia.Text) && !string.IsNullOrWhiteSpace(DatosCliente_provincia.Text) ? DatosCliente_provincia.Text : "";
            row["Pais"] = !string.IsNullOrEmpty(DatosCliente_pais.Text) && !string.IsNullOrWhiteSpace(DatosCliente_pais.Text) ? DatosCliente_pais.Text : "";
            row["Fecha"] = Fecha.Text;

            //if(
            row["Subtotal"] = System.Math.Round(double.Parse(Facturacion_textbox_subtotal.Text), 2);
            row["Iva"] = System.Math.Round(double.Parse(Facturacion_textbox_iva.Text), 2);
            row["Total"] = System.Math.Round(double.Parse(Facturacion_textbox_Total.Text), 2);

            //BUSCA SI EXISTEN ARTICULOS DE INVENTARIO, SI ENCUENTRA UNO MARCA QUE SI HAY Y SE SALE PARA NO SEGUIRLE
            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                if (Facturacion_DatagridArticulos.Rows[i].ReadOnly == true)
                {
                    ContieneArticulosDeInventario = true;
                    i = Facturacion_DatagridArticulos.Rows.Count;
                }
           
            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
            {
                if(Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString()!="")
                row["Codigo" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString();

                if (Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString() != "")
                row["Cantidad" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString();

                row["Descripcion" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[2].Value.ToString();
                row["PrecioUnitario" + (i + 1)] = Facturacion_DatagridArticulos.Rows[i].Cells[3].Value.ToString();
            }            
            //TOTAL DE ARTICULOS COMPRADOS
            row["TotalDatos"] = Facturacion_DatagridArticulos.Rows.Count;

            dt.Rows.Add(row);
            try
            {
                da.Update(dt);
                dt.AcceptChanges();
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                return;
            }

            //NO CONTIENE ARTICULOS DE INVENTARIO, POR LO TANTO NO REALIZA LO DEMAS
            if (ContieneArticulosDeInventario == false)
            {
                MessageBox.Show("Factura agregada correctamente","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
          
            int codigo;
            //SE AGREGO A LA TABLA FISICA, AHORA BUSCA CADA ARTICULO AGREGADO DE INVENTARIO Y LE RESTA LOS VENDIDOS A LA EXISTENCIA
            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                if (Facturacion_DatagridArticulos.Rows[i].ReadOnly == true)
                {   
                    codigo = int.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[0].Value.ToString());
                    this.BuscarAlgoenTabla("SELECT * FROM Productos WHERE clave = "+codigo.ToString());
                    dt.Rows[0]["prod_existencia"] = long.Parse(dt.Rows[0]["prod_existencia"].ToString()) - long.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString());
                    if(int.Parse(dt.Rows[0]["prod_existencia"].ToString())==0)
                       dt.Rows[0]["vendido"] = true;
                    
                }

            try
            {
                da.Update(dt);
                dt.AcceptChanges();
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                return;
            }
            this.BuscarAlgoenTabla("SELECT * FROM Factura ");
            MessageBox.Show("Factura agregada correctamente!","",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        private void BuscarAlgoenTabla(string comando)
        {

            //string path = Application.StartupPath;
            //path = path.Remove(60);
            //path = path + "EM Software - Control_De_Invitados\\BD\\Control_de_invitados_BD.sdf;Persist Security Info=False;";
            SqlCeConnection PathBD = new SqlCeConnection("Data Source=C:\\Facturacion\\Facturacion\\BaseDeDatos.sdf;Persist Security Info=False;");
            //abre la conexion
            try
            {
                da = new SqlCeDataAdapter(comando, PathBD);

                // Crear los comandos de insertar, actualizar y eliminar
                SqlCeCommandBuilder cb = new SqlCeCommandBuilder(da);
                dt = new DataTable();
                // Llenar la tabla con los datos indicados
                da.Fill(dt);

                PathBD.Open();
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
                return;
            }
        }
        private void desabilitatodo()
        {           
            foreach (Control c in this.groupBox1.Controls)
                if (c is TextBox)
                    ((TextBox)c).ReadOnly = true;
                else
                    if (c is Button)
                        ((Button)c).Enabled = false;

            foreach (Control c in this.groupBox2.Controls)
                if (c is TextBox)
                    ((TextBox)c).ReadOnly = true;
                else
                    if (c is Button)
                        ((Button)c).Enabled = false;

            foreach (Control c in this.groupBox3.Controls)
                if (c is TextBox)
                    ((TextBox)c).ReadOnly = true;
                else
                    if (c is Button)
                        ((Button)c).Enabled = false;
        }
        

        #endregion

        //*****************            METODOS        *******************************

        public void impresion(StreamWriter sw)
        {
            sw.WriteLine("          EM Sistems - Contable Software S.A de C.V ");
            sw.WriteLine("                (N.19) Sucursal : San Pedro");
            sw.WriteLine("                   Avenida Gomez Morin #19");
            sw.WriteLine("                    Guadalupe N.L. Mexico");
            sw.WriteLine("\n");
            sw.WriteLine("-------------------------Datos del cliente -----------------------");
            sw.WriteLine("Cliente: " + DatosCliente_cliente.Text);
            sw.WriteLine("Fecha de compra: " + System.DateTime.Now.ToString("dd/MM/yyyy"));
            sw.WriteLine("-------------------------------------------------------------------");
            sw.WriteLine("             Hora:" + System.DateTime.Now.ToString("HH:mm") + "  Dia:" + Fecha.Text);
            sw.WriteLine("***********************************************************************");
            sw.WriteLine(" Descripcion                   Precio ");
            sw.WriteLine("***********************************************************************");

            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
            {
                string cadena = "      " + Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString();//cadena
                int num_caracteres = cadena.Length;
                num_caracteres = 32 - num_caracteres;//obtiene el num total de espacios a agregar
                for (int r = 0; r <= num_caracteres; r++)
                    cadena = cadena + " ";

                cadena = cadena + "$" + Facturacion_DatagridArticulos.Rows[i].Cells[2].Value.ToString();
                sw.WriteLine(cadena);
            }

            sw.WriteLine("***********************************************************************");
            sw.WriteLine("                         Total : $" + Facturacion_textbox_Total.Text);
            // sw.WriteLine("                               Efectivo : $" + efectivo);
            //sw.WriteLine("                              Su Cambio : $" + (efectivo - Ticket.ticketgeneral.total));
            sw.WriteLine("***********************************************************************");
            sw.WriteLine("               Gracias por su compra!");
            sw.WriteLine("                    Vuelva Pronto");

        }


        //********************************************************************************************

        private void Facturacion_boton_imprimir_Click(object sender, EventArgs e)
        {
            /*
            System.Diagnostics.Process oProc = new System.Diagnostics.Process();
            oProc.StartInfo.FileName = "C:\\Program Files (x86)\\Adobe\\Reader 9.0\\Reader\\AcroRd32.exe";
            oProc.StartInfo.Arguments = "/p C:\\Threads.pdf";
            oProc.Start();
            while (!oProc.HasExited)
            { 
            }
             */
            StreamWriter sw = new StreamWriter("C:\\Users\\EnriQuE\\Desktop\\Ticket.txt");
            impresion(sw);
            sw.Close();
            MessageBox.Show("Recoja su ticket en escritorio , Nombre del archivo Ticket");

        }

        private void Facturacion_textbox_cliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }
       
        private void button2_Click(object sender, EventArgs e)
        {            
            Agregar_Producto agregar = new Agregar_Producto(dt, da);
            agregar.Show();
        }
       
        private void button4_Click(object sender, EventArgs e)
        {
            FACTURACION_USUARIOS usuarios = new FACTURACION_USUARIOS();
            usuarios.Show();
        }

        private void button1_Click(object sender, EventArgs e)//nuevoo
        {
            this.BuscarAlgoenTabla("SELECT * FROM Factura ORDER BY Num_factura");
            foreach (Control control in this.groupBox1.Controls)
                control.Enabled = true;

            //HABILITA TODO LO DE LA FACTURA
            foreach (Control control in this.groupBox1.Controls)
                if (control is Button)
                   ((Button)control).Enabled = false;

            foreach (Control control in this.groupBox3.Controls)
                if (control is TextBox)
                    control.Text = "";               

            foreach (Control control in this.groupBox2.Controls)                          
                    if (control is TextBox)
                        if(!control.Name.Contains("Fecha"))
                    {
                        ((TextBox)control).ReadOnly = true;
                        control.Text = "";
                    }
                   

            Button_Guardar.Enabled = true;
            DatosCliente_cancelar.Enabled = true;            
            butto5.Enabled = true;
            DatosCliente_Folio.Enabled = false;
            Facturacion_textbox_subtotal.Text = "0";
            Facturacion_textbox_iva.Text = "0";
            Facturacion_textbox_Total.Text = "0";            
            if (dt.Rows.Count > 0)
                DatosCliente_Folio.Text = (int.Parse(dt.Rows[dt.Rows.Count - 1]["Num_Factura"].ToString()) + 1).ToString();
            else
                DatosCliente_Folio.Text = "1";
           
            Fecha.Text = DateTime.Now.ToShortDateString();
            button1.Enabled = true;

            foreach (Control control in this.groupBox2.Controls)
                if (control is TextBox)                   
                        ((TextBox)control).ReadOnly = false;
            Fecha.ReadOnly = true;

            foreach (Control control in this.groupBox3.Controls)
                if (control is TextBox)
                    if (control.Name.Contains("Codigo") || control.Name.Contains("Cantidad"))
                        ((TextBox)control).ReadOnly = false;
           
            AgregarProducto_agregar.Enabled = true;
            AgregarProducto_verTodos.Enabled = true;

            Facturacion_DatagridArticulos.Rows.Clear();
            Facturacion_DatagridArticulos.ReadOnly = false;

            button_nuevo.Enabled = false;         
        }

        private void button6_Click(object sender, EventArgs e)
        {           
            this.ponedatos(false);
            button_nuevo.Enabled = true;
            actualiza = false;
            this.BuscarAlgoenTabla("SELECT * FROM Factura ORDER BY Num_factura");
        }

        private void Button_Guardar_Click(object sender, EventArgs e)
        {
            //VALIDA QUE HAYA INGRESADO TODO LO QUE DEBE        
            if (string.IsNullOrWhiteSpace(DatosCliente_cliente.Text) || string.IsNullOrWhiteSpace(DatosCliente_direccion.Text) || string.IsNullOrWhiteSpace(DatosCliente_localidad.Text)
                || string.IsNullOrEmpty(DatosCliente_cliente.Text) || string.IsNullOrEmpty(DatosCliente_direccion.Text) || string.IsNullOrEmpty(DatosCliente_localidad.Text))
            {
                MessageBox.Show("Antes de guardar, debe capturar minimo: Cliente, Direccion y Localidad, verifiquelos antes de guardar", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //SI NO TIENE ARTICULOS EN EL DATAGRID, PREGUNTA SI DESEA GUARDAR LA FACTURA SIN NADA
            if (Facturacion_DatagridArticulos.Rows.Count == 0)
            {
                DialogResult respuesta = MessageBox.Show("No tiene ningun articulo agregado, desea guardar la factura sin ningun cargo? ", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta.Equals(DialogResult.Yes))
                {
                    this.AgregaFactura();
                    this.ponedatos(false);
                    return;
                }
                return;
            }

            //CHECA QUE LOS DATOS DEL DATAGRIDVIEW ESTE BIEN INGRESADOS          
            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                if (Facturacion_DatagridArticulos.Rows[i].Cells[2].Value.ToString() == "" || Facturacion_DatagridArticulos.Rows[0].Cells[3].Value.ToString() == "")
                {
                    MessageBox.Show("Falto capturar DESCRIPCION o PRECIO UNITARIO de el articulo numero: " + Facturacion_DatagridArticulos.Rows[i].HeaderCell.Value + ", porfavor verifique", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }           

            //SE VALIDO CORRECTAMENTE TODO AHORA GUARDA LA FACTURA
            this.AgregaFactura();
            this.ponedatos(false);
            
        }

        private void button_imprimir_Click(object sender, EventArgs e)
        {
            printDialog1.ShowDialog();
        }        

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
        }

        private void Facturacion_DatagridArticulos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Valid);
            e.Control.KeyPress += new KeyPressEventHandler(Valid);
        }

        private void Valid(object sender, KeyPressEventArgs e)
        {
            //si la celda donde oprimio el boton es de tipo entero, verifica que no haya ingresado letras o caracteres especiales
            string headerText = Facturacion_DatagridArticulos.Columns[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Name;
            if (headerText.Equals("Cantidad") || headerText.Equals("PrecioUnitario") || headerText.Equals("Codigo"))
            {
                if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/|| e.KeyChar == 8/* codigo ascii del backspace*/)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {//LA TABLA SOLO ACEPTA 5 ARTICULOS
            if (Facturacion_DatagridArticulos.Rows.Count < 5)
            {
                Facturacion_DatagridArticulos.Rows.Add("", "", "", "0");
                Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.Rows.Count - 1].HeaderCell.Value = Facturacion_DatagridArticulos.Rows.Count.ToString();
            }
        }

        private void DatosCliente_DNI_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosCliente_Folio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosCliente_localidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosCliente_cp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosCliente_provincia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosCliente_pais_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar)  && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DatosClienteIdcliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void Facturacion_DatagridArticulos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string headerText = Facturacion_DatagridArticulos.Columns[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Name;
            // SI DEJO EN BLANCO O NULL LA CANTIDAD NO LO DEJA SALIR
            if (headerText.Equals("PrecioUnitario"))
                if (string.IsNullOrEmpty(Facturacion_DatagridArticulos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    if (Facturacion_DatagridArticulos.Rows.Count == 1)//SI ES EL PRIMER RENGLON Y NO PUSO CANTIDAD, NI PARA QUE HACER LO DEMAS, PONE CEROS EN TOTAL,IVAY SUBTOTOTAL
                    {
                        Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.CurrentRow.Index].Cells[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Value = "0";
                        Facturacion_textbox_subtotal.Text = "0";
                        Facturacion_textbox_iva.Text = "0";
                        Facturacion_textbox_Total.Text = "0";
                        return;
                    }

            //SI LA CELDA QUE SE ACABA DE EDITAR ES PRECIOUNITARIO, HACE LA CUENTA DE LOS TOTALES Y LO PONE EN LOS TEXTBOX
            if (Facturacion_DatagridArticulos.Columns[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Name.Equals("PrecioUnitario"))
            {
                double TotalEnGeneral = 0;
                double iva = 0;
                //TOTAL EN GENERAL DE LA FACTURA
                //SI ES EL PRIMER ARTICULO EN SUMAR TOMA EL VALOR DE EL PRIMER ARTICULO
                if (Facturacion_DatagridArticulos.Rows.Count == 1)
                {
                    TotalEnGeneral = double.Parse(Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.CurrentRow.Index].Cells[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Value.ToString());
                    TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);
                    Facturacion_textbox_subtotal.Text = TotalEnGeneral.ToString();
                    iva = (TotalEnGeneral * 0.10);
                    iva = System.Math.Round(iva, 4);
                    Facturacion_textbox_iva.Text = iva.ToString();
                    TotalEnGeneral = TotalEnGeneral + iva;
                    TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);
                    Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();
                }
                else
                {
                    TotalEnGeneral = double.Parse(Facturacion_textbox_Total.Text);
                    //CANTIDAD DE DINERO QUE CUESTA EL ARTICULO
                    double cantidad = double.Parse(Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.CurrentRow.Index].Cells[Facturacion_DatagridArticulos.CurrentCell.ColumnIndex].Value.ToString());
                    //cantidad = System.Math.Round(cantidad, 4);//REDONDEA A CUATRO DECIMALES
                    Facturacion_textbox_subtotal.Text = (cantidad + TotalEnGeneral).ToString();
                    iva = double.Parse(Facturacion_textbox_subtotal.Text) * 0.10;
                    iva = System.Math.Round(iva, 4);// REDONDEA IVA A 4 DECIMALES
                    Facturacion_textbox_iva.Text = iva.ToString();// PONE EL IVA
                    TotalEnGeneral = double.Parse(Facturacion_textbox_subtotal.Text) + iva;
                    TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);// SUMA EL IVA MAS EL SUBTOTAL
                    Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();// PONE EL TOTAL
                }

            }
        }

        private void Facturacion_DatagridArticulos_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //SI ES EL PRIMER ARTICULO
            if (Facturacion_DatagridArticulos.Rows.Count == 0)
            {

                Facturacion_textbox_subtotal.Text = "0";
                Facturacion_textbox_iva.Text = "0";
                Facturacion_textbox_Total.Text = "0";
                return;
            }

            double TotalEnGeneral;
            double iva;
            double subtotal = 0;

            for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                // SUMA TODOS LOS LOS ARTICULOS
                subtotal = subtotal + double.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[3].Value.ToString());

            //PONE EL SUBTOTAL EN TEXTBOX
            Facturacion_textbox_subtotal.Text = subtotal.ToString();
            //SACA EL IVA DE EL SUBTOTAL
            iva = subtotal * 0.10;
            Facturacion_textbox_iva.Text = iva.ToString();
            //SACA EL TOTAL Y LO GUARDA EN EL TEXTBX
            TotalEnGeneral = subtotal + iva;
            Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();
        }

        private void AgregarProducto_CodigoTB_Leave(object sender, EventArgs e)
        {
            //SI LO DEJO EN BLANCO SE SALE
            if (string.IsNullOrEmpty(AgregarProducto_CodigoTB.Text))
                return;

            //VARIABLE DONDE SE GUARDA EL CODIGO
            int Codigo = int.Parse(AgregarProducto_CodigoTB.Text);
            
            //BUSCA EL CODIGO EN LA TABLA
            this.BuscarAlgoenTabla("SELECT * FROM Productos WHERE clave = " + Codigo.ToString());
           
            //NO LO ENCONTRO
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No se encontro el producto, porfavor verifique su codigo", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AgregarProducto_Descripcion.Text = "";
                AgregarProducto_Precio.Text = "";
                AgregarProducto_CodigoTB.Text = "";
                AgregarProducto_Cantidad.Text = "";
                AgregarProducto_Exist.Text = "";
                return;
            }

            //LO ENCONTRO, AHORA CHECA SI HAY EXISTENCIAS           
                if (int.Parse(dt.Rows[0]["prod_existencia"].ToString()) == 0)
                {
                    MessageBox.Show("Ya no hay existencias en almacen de este producto", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    AgregarProducto_Descripcion.Text = "";
                    AgregarProducto_Precio.Text = "";
                    AgregarProducto_CodigoTB.Text = "";
                    AgregarProducto_Cantidad.Text = "";
                    AgregarProducto_Exist.Text = "";
                }
                else// SI HAY EXISTENCIAS, RELLENA LOS TEXTBOX
                {
                    AgregarProducto_Descripcion.Text = dt.Rows[0]["nombre"].ToString();
                    AgregarProducto_Precio.Text = dt.Rows[0]["precio"].ToString();
                    AgregarProducto_Cantidad.Text = "1";//POR DEFAUL AGREGA SOLO UN ARTICULO   
                    AgregarProducto_Exist.Text = dt.Rows[0]["prod_existencia"].ToString();
                    AgregarProd_total.Text = dt.Rows[0]["precio"].ToString();
                }            

        }

        private void AgregarProducto_Cantidad_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AgregarProducto_CodigoTB.Text))
            {               
                AgregarProducto_Cantidad.Text = "";
                return;
            }

            //SI ESTA VACIO PONE UNO PARA QUE NO TRUENE
            if (string.IsNullOrEmpty(AgregarProducto_Cantidad.Text))
            {
                AgregarProducto_Cantidad.Text = "1";
                return;
            }        
                
            //SI CANTIDAD ES MAYOR QUE EXISTENCIA
            int existencia = int.Parse(AgregarProducto_Exist.Text);
            int cantidad = int.Parse(AgregarProducto_Cantidad.Text);

            //LA CANTIDAD PEDIDA ES MAYOR, LE AVISA Y SE SALE
            if (cantidad > existencia)
            {
                MessageBox.Show("La cantidad solicitada de articulos es mayor a las existencias en almacen, porfavor verifique", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AgregarProducto_Cantidad.Text = "1";
                return;
            }

            AgregarProd_total.Text = (double.Parse(AgregarProducto_Precio.Text) * cantidad).ToString();

        }

        private void AgregarProducto_CodigoTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;           
        }

        private void AgregarProducto_Cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            //VERIFICA SI YA ESCOJIO ALGUN ARTICULO  
            if (string.IsNullOrEmpty(AgregarProducto_CodigoTB.Text))
            {
                MessageBox.Show("Seleccione un producto antes de proceder", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AgregarProducto_Cantidad.Text = "";
                return;
            }
        }

        private void AgregarProducto_Descripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void AgregarProducto_agregar_Click(object sender, EventArgs e)
        {            
                    if (string.IsNullOrEmpty(AgregarProducto_CodigoTB.Text) || string.IsNullOrEmpty(AgregarProducto_Descripcion.Text))
                    {
                        MessageBox.Show("Seleccione un producto antes de proceder", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    double TotalEnGeneral = 0;
                    double iva = 0;
                    //SI EL ARTICULO QUE VA A AGREGAR YA ESTA EN EL DATAGRIDVIEW, LE AUMENTA EL NUMERO DE ARTICULOS, TAMBIEN VERIFICA SI NO SE PASO
                    if (Facturacion_DatagridArticulos.Rows.Count > 0)
                    {
                        for (int i = 0; i < Facturacion_DatagridArticulos.Rows.Count; i++)
                            if (Facturacion_DatagridArticulos.Rows[i].Cells[2].Value.ToString().Contains(AgregarProducto_Descripcion.Text))
                                if ((long.Parse(AgregarProducto_Cantidad.Text) + long.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString())) > long.Parse(AgregarProducto_Exist.Text))
                                {
                                    MessageBox.Show("La cantidad solicitada de articulos ha sobrepasado las existencias en almacen.\n \nCodigo: "+ AgregarProducto_CodigoTB.Text+"\n Cantidad solicitada :" + (int.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString()) + 1).ToString() + "\n Cantidad en almacen: "+ AgregarProducto_Exist.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                                else
                                {
                                    Facturacion_DatagridArticulos.Rows[i].Cells[1].Value = (int.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString()) + int.Parse(AgregarProducto_Cantidad.Text)).ToString();
                                    Facturacion_DatagridArticulos.Rows[i].Cells[3].Value = (double.Parse(AgregarProducto_Precio.Text) * double.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[1].Value.ToString())).ToString();
                                                                   
                                    //TOTAL EN GENERAL DE LA FACTURA
                                    //SI ES EL PRIMER ARTICULO EN SUMAR TOMA EL VALOR DE EL PRIMER ARTICULO
                                    if (Facturacion_DatagridArticulos.Rows.Count == 1)
                                    {
                                        TotalEnGeneral = double.Parse(Facturacion_DatagridArticulos.Rows[0].Cells[3].Value.ToString());
                                        TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);
                                        Facturacion_textbox_subtotal.Text = TotalEnGeneral.ToString();
                                        iva = (TotalEnGeneral * 0.10);
                                        iva = System.Math.Round(iva, 4);
                                        Facturacion_textbox_iva.Text = iva.ToString();
                                        TotalEnGeneral = TotalEnGeneral + iva;
                                        Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();
                                        return;
                                    }
                                    else
                                    {
                                        TotalEnGeneral = double.Parse(Facturacion_textbox_Total.Text);
                                        //CANTIDAD DE DINERO QUE CUESTA EL ARTICULO
                                        double cantidad = double.Parse(Facturacion_DatagridArticulos.Rows[i].Cells[3].Value.ToString());
                                        Facturacion_textbox_subtotal.Text = (cantidad + TotalEnGeneral).ToString();
                                        iva = double.Parse(Facturacion_textbox_subtotal.Text) * 0.10;
                                        iva = System.Math.Round(iva, 4);// REDONDEA IVA A 4 DECIMALES
                                        Facturacion_textbox_iva.Text = iva.ToString();// PONE EL IVA
                                        TotalEnGeneral = double.Parse(Facturacion_textbox_subtotal.Text) + iva;
                                        TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);// SUMA EL IVA MAS EL SUBTOTAL
                                        Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();// PONE EL TOTAL
                                        return;
                                    }

                                }

                    }
                                      
            //EL ARTICULO NO ESTA EN EL DATAGRIDVIEW NI NADA
            if (Facturacion_DatagridArticulos.Rows.Count > 5)
            {
                MessageBox.Show("Maximo 5 articulo por factura", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Facturacion_DatagridArticulos.Rows.Add(
                AgregarProducto_CodigoTB.Text,
                AgregarProducto_Cantidad.Text,
                AgregarProducto_Descripcion.Text,
                AgregarProd_total.Text);
            Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.Rows.Count-1].ReadOnly = true;
            Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.Rows.Count - 1].HeaderCell.Value = Facturacion_DatagridArticulos.Rows.Count.ToString();
           
            //TOTAL EN GENERAL DE LA FACTURA
            //SI ES EL PRIMER ARTICULO EN SUMAR TOMA EL VALOR DE EL PRIMER ARTICULO
            if (Facturacion_DatagridArticulos.Rows.Count == 1)
            {
                TotalEnGeneral = double.Parse(AgregarProd_total.Text);
                TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);
                Facturacion_textbox_subtotal.Text = TotalEnGeneral.ToString();
                iva = (TotalEnGeneral * 0.10);
                iva = System.Math.Round(iva, 4);
                Facturacion_textbox_iva.Text = iva.ToString();
                TotalEnGeneral = TotalEnGeneral + iva;
                Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();
                return;
            }
            else
            {
                TotalEnGeneral = double.Parse(Facturacion_textbox_Total.Text);
                //CANTIDAD DE DINERO QUE CUESTA EL ARTICULO
                double cantidad = double.Parse(Facturacion_DatagridArticulos.Rows[Facturacion_DatagridArticulos.Rows.Count-1].Cells[3].Value.ToString());
                Facturacion_textbox_subtotal.Text = (cantidad + TotalEnGeneral).ToString();
                iva = double.Parse(Facturacion_textbox_subtotal.Text) * 0.10;
                iva = System.Math.Round(iva, 4);// REDONDEA IVA A 4 DECIMALES
                Facturacion_textbox_iva.Text = iva.ToString();// PONE EL IVA
                TotalEnGeneral = double.Parse(Facturacion_textbox_subtotal.Text) + iva;
                TotalEnGeneral = System.Math.Round(TotalEnGeneral, 4);// SUMA EL IVA MAS EL SUBTOTAL
                Facturacion_textbox_Total.Text = TotalEnGeneral.ToString();// PONE EL TOTAL
                return;
            }

        }

        private void button_buscar_Click(object sender, EventArgs e)
        {
            this.desabilitatodo();            
           
            Facturacion_DatagridArticulos.Rows.Clear();
            DatosCliente_cancelar.Enabled = true;
            DatosCliente_Folio.ReadOnly = false;
            DatosCliente_Folio.Enabled = true;
           

        }

        private void button_siguiente_Click(object sender, EventArgs e)
        {
            if (renglon < dt.Rows.Count-1)
            {
                renglon++;
                this.ponedatos(true);
            }
        }

        private void button_anterior_Click(object sender, EventArgs e)
        {
            if (renglon == 0)
                return;

            if (renglon > 0)
            {
                renglon--;
                this.ponedatos(true);
            }            
        }

        private void DatosClienteIdcliente_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DatosClienteIdcliente.Text) || string.IsNullOrWhiteSpace(DatosClienteIdcliente.Text))
                return;

            int Num_cliente = int.Parse(DatosClienteIdcliente.Text);          
            //BUSCA EL CODIGO EN LA TABLA
               this.BuscarAlgoenTabla("SELECT * FROM Clientes WHERE NUM_CLIENTE = " + Num_cliente.ToString());
             //this.BuscarAlgoenTabla("SELECT * FROM Productos ");
            
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Cliente no encontrado, porfavor verifique sus datos","",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                DatosClienteIdcliente.Text = "";
                return;
            }

            DataRow roow = dt.Rows[0];
            DatosCliente_cliente.Text = roow["NOMBRE"].ToString();
            DatosCliente_cp.Text = roow["CP"].ToString();
            DatosCliente_direccion.Text = roow["DIRECCION"].ToString();
            DatosCliente_DNI.Text = roow["DNI"].ToString();
            DatosCliente_localidad.Text = roow["LOCALIDAD"].ToString();
            DatosCliente_pais.Text = roow["PAIS"].ToString();
            DatosCliente_provincia.Text = roow["PROVINCIA"].ToString();           
        }

        private void AgregarProducto_verTodos_Click(object sender, EventArgs e)
        {
            VerTodosLosProductos ver = new VerTodosLosProductos(dt,da);
            ver.Show();
        }

        private void DatosCliente_Folio_Leave(object sender, EventArgs e)
        {
            if (DatosCliente_Folio.ReadOnly == true || DatosCliente_Folio.Text=="" || string.IsNullOrWhiteSpace(DatosCliente_Folio.Text))
                return;
            this.BuscarAlgoenTabla("SELECT * FROM Factura WHERE Num_Factura = "+DatosCliente_Folio.Text);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No se encontro la factura con el folio ingresado, porfavor verifique!","",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }

            this.BuscarAlgoenTabla("SELECT * FROM Factura");
            renglon = int.Parse(DatosCliente_Folio.Text)-1;
            this.ponedatos(false);
        }

        private void button_eliminar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show("Esta seguro de que desea eliminar la factura: " + DatosCliente_Folio.Text + "?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (respuesta.Equals(DialogResult.No))
                return;

            int numero = int.Parse(DatosCliente_Folio.Text);
            this.BuscarAlgoenTabla("DELETE FROM Factura WHERE Num_Factura = "+numero.ToString());
            //if (dt.Rows.Count >= 0)
              //  dt.Rows[0].Delete();
         
            try
            {
                da.Update(dt);
                dt.AcceptChanges();
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show("Error de concurrencia:\n" + ex.Message);
                return;
            }

            MessageBox.Show("Factura eliminada correctamente!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            this.BuscarAlgoenTabla("SELECT * FROM Factura");
            if(dt.Rows.Count>0)
            renglon = dt.Rows.Count-1;
            
            this.ponedatos(false);
        }

        private void button_editar_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.groupBox1.Controls)
                control.Enabled = true;

            //HABILITA TODO LO DE LA FACTURA
            foreach (Control control in this.groupBox1.Controls)
                if (control is Button)
                    ((Button)control).Enabled = false;


            Button_Guardar.Enabled = true;
            DatosCliente_cancelar.Enabled = true;
            butto5.Enabled = true;
            DatosCliente_Folio.Enabled = false;           

            foreach (Control control in this.groupBox2.Controls)
                if (control is TextBox)
                    ((TextBox)control).ReadOnly = false;
            Fecha.ReadOnly = true;

            foreach (Control control in this.groupBox3.Controls)
                if (control is TextBox)
                    if (control.Name.Contains("Codigo") || control.Name.Contains("Cantidad"))
                        ((TextBox)control).ReadOnly = false;

            AgregarProducto_agregar.Enabled = true;
            AgregarProducto_verTodos.Enabled = true;
            actualiza = true;

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            foreach (Control c in this.groupBox2.Controls)
                if (c is TextBox && c.Name!="Fecha")
                    c.Text = "";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ListaFactura list = new ListaFactura(dt,da);
            list.Show();
        }



    }
    interface EnlaceEntreFormularios
    {
        void AgregaACarrito(int clave, string nombre, int precio);
    }
}
