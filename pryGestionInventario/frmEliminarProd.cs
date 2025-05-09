using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryGestionInventario
{
    public partial class frmEliminarProd : Form
    {
        public frmEliminarProd()
        {
            InitializeComponent();
        }
        OleDbConnection conexion;
        clsClase clsClase = new clsClase();
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cmbCodigo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un código de producto para eliminar.");
                return;
            }

            string codigo = cmbCodigo.SelectedItem.ToString();

            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro que desea eliminar el producto con código " + codigo + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion == DialogResult.Yes)
            {
                string sql = "DELETE FROM Productos WHERE CODIGO = ?";

                try
                {
                    if (conexion.State != ConnectionState.Open)
                        conexion.Open();

                    using (OleDbCommand cmd = new OleDbCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("?", codigo);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Producto eliminado correctamente.");
                            CargarProductos();              // Recargar el DataGridView
                            CargarCodigosEnComboBox();  // Recargar el ComboBox
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el producto.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            } }
            private void CargarProductos()
        {
            string conexionString = @"Provider=Microsoft.JET.OLEDB.4.0;Data Source=Productos.mdb;";
            using (OleDbConnection conexion = new OleDbConnection(conexionString))

                try
            {
                

                string sql = "SELECT * FROM Productos";
                OleDbDataAdapter adaptador = new OleDbDataAdapter(sql, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla); 
                dgvProductos.DataSource = tabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }
        private void CargarCodigosEnComboBox()
        {
            string consulta = "SELECT CODIGO FROM Productos";
            OleDbCommand cmd = new OleDbCommand(consulta, conexion);
            OleDbDataReader lector = cmd.ExecuteReader();

            cmbCodigo.Items.Clear(); // Limpia los ítems antes de cargar

            while (lector.Read())
            {
                cmbCodigo.Items.Add(lector["CODIGO"].ToString());
            }

            lector.Close();
        }

        private void frmEliminarProd_Load(object sender, EventArgs e)
        {
            // Crear la cadena de conexión
            string cadenaConexion = $"Provider=Microsoft.JET.OLEDB.4.0;Data Source=Productos.mdb;";

            // Inicializar la conexión
            conexion = new OleDbConnection(cadenaConexion);
            conexion.Open();
            CargarProductos();
            CargarCodigosEnComboBox();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cmbCodigo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un código de producto.");
                return;
            }

            string codigoSeleccionado = cmbCodigo.SelectedItem.ToString();

            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                string sql = "SELECT * FROM Productos WHERE Codigo = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conexion);
                cmd.Parameters.AddWithValue("?", codigoSeleccionado);

                OleDbDataAdapter adaptador = new OleDbDataAdapter(cmd);
                DataTable resultado = new DataTable();
                adaptador.Fill(resultado);

                dgvProductos.DataSource = resultado;

                if (resultado.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró ningún producto con ese código.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivo CSV (*.csv)|*.csv";
            guardar.FileName = "ReporteInventario.csv";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(guardar.FileName, false, Encoding.UTF8))
                    {
                        // Escribir encabezados
                        for (int i = 0; i < dgvProductos.Columns.Count; i++)
                        {
                            sw.Write(dgvProductos.Columns[i].HeaderText);
                            if (i < dgvProductos.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // Escribir filas
                        foreach (DataGridViewRow fila in dgvProductos.Rows)
                        {
                            if (!fila.IsNewRow)
                            {
                                for (int i = 0; i < dgvProductos.Columns.Count; i++)
                                {
                                    sw.Write(fila.Cells[i].Value?.ToString());
                                    if (i < dgvProductos.Columns.Count - 1)
                                        sw.Write(",");
                                }
                                sw.WriteLine();
                            }
                        }
                    }

                    MessageBox.Show("Reporte generado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el reporte: " + ex.Message);
                }
            }
        }
    }
}
