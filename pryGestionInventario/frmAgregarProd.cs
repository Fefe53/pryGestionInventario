using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace pryGestionInventario
{
    public partial class frmAgregarProd : Form
    {
        public frmAgregarProd()
        {
            InitializeComponent();
        }
        OleDbConnection conexion;
        clsClase clsclase = new clsClase();
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Precio inválido.");
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("Stock inválido.");
                return;
            }

            string conexionString = @"Provider=Microsoft.JET.OLEDB.4.0;Data Source=Productos.mdb;";
            using (OleDbConnection conexion = new OleDbConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string consulta = "INSERT INTO Productos (Codigo, Nombre, Descripcion, Precio, Stock) " +
                                      "VALUES (?, ?, ?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("?", txtCodigo.Text);
                        cmd.Parameters.AddWithValue("?", txtNombre.Text);
                        cmd.Parameters.AddWithValue("?", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("?", precio);
                        cmd.Parameters.AddWithValue("?", stock);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Producto agregado correctamente.");
                    CargarProductos(); // Refresca el DataGridView
                    LimpiarCampos();   // Limpia las cajas de texto
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void frmAgregarProd_Load(object sender, EventArgs e)
        {// Establece la ruta de la base de datos
            string rutaBD = Path.Combine(Application.StartupPath, "Productos.mdb");

            // Verifica si el archivo de base de datos existe
            if (!File.Exists(rutaBD))
            {
                MessageBox.Show("Base de datos no encontrada en: " + rutaBD);
                return;
            }

            // Crear la cadena de conexión
            string cadenaConexion = $"Provider=Microsoft.JET.OLEDB.4.0;Data Source=Productos.mdb;";

            // Inicializar la conexión
            conexion = new OleDbConnection(cadenaConexion);

            // Abrir la conexión a la base de datos
            try
            {
                conexion.Open(); // ABRIR LA CONEXIÓN AQUÍ
                CargarProductos();   // Cargar los datos en el DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
            dgvProductos.ColumnCount = 5;
            dgvProductos.Columns[0].Name = "Código";
            dgvProductos.Columns[1].Name = "Nombre";
            dgvProductos.Columns[2].Name = "Descripción";
            dgvProductos.Columns[3].Name = "Precio";
            dgvProductos.Columns[4].Name = "Stock";

            CargarProductos();
        }
        private void CargarProductos()
        {
            dgvProductos.Rows.Clear();
            string conexionString = @"Provider=Microsoft.JET.OLEDB.4.0;Data Source=Productos.mdb;";
            using (OleDbConnection conexion = new OleDbConnection(conexionString))
            {
                string consulta = "SELECT * FROM Productos";
                OleDbCommand cmd = new OleDbCommand(consulta, conexion);
                conexion.Open();

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dgvProductos.Rows.Add(
                        reader["Codigo"].ToString(),
                        reader["Nombre"].ToString(),
                        reader["Descripcion"].ToString(),
                        reader["Precio"].ToString(),
                        reader["Stock"].ToString()
                    );
                }
                reader.Close();
            }
        }
        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtCodigo.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Selecciona un producto para modificar.");
                return;
            }

            // Validar precio
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Replace(".", ","), out precio))
            {
                MessageBox.Show("Ingrese un precio válido.");
                return;
            }

            // Validar stock
            int stock;
            if (!int.TryParse(txtStock.Text, out stock))
            {
                MessageBox.Show("Ingrese un stock válido.");
                return;
            }

            string sql = "UPDATE Productos SET NOMBRE = ?, DESCRIPCION = ?, PRECIO = ?, STOCK = ? WHERE CODIGO = ?";

            try
            {
                // Asegúrate de que la conexión esté abierta antes de ejecutar el comando
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                using (OleDbCommand cmd = new OleDbCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("?", txtNombre.Text);
                    cmd.Parameters.AddWithValue("?", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("?", precio);
                    cmd.Parameters.AddWithValue("?", stock);
                    cmd.Parameters.AddWithValue("?", txtCodigo.Text);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto modificado correctamente.");
                        CargarProductos();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message);
            }
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
                txtCodigo.Text = fila.Cells["CODIGO"].Value.ToString();
                txtNombre.Text = fila.Cells["NOMBRE"].Value.ToString();
                txtDescripcion.Text = fila.Cells["DESCRIPCION"].Value.ToString();
                txtPrecio.Text = fila.Cells["PRECIO"].Value.ToString();
                txtStock.Text = fila.Cells["STOCK"].Value.ToString();
            }
        }
    }
}

