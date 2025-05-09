using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Net;

namespace pryGestionInventario
{
    internal class clsClase
    {
        private OleDbConnection conexion = new OleDbConnection();
        private OleDbCommand comando = new OleDbCommand();
        private OleDbDataAdapter adaptador = new OleDbDataAdapter();

        private string CadenaConexion = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Productos.mdb";
        private string Tabla = "Productos";


    }
}
