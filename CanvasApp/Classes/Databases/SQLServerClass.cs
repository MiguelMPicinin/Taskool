using System;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public abstract class BaseDB : IDisposable
    {
        protected readonly string connectionString = "Data Source=Olimpiada\\SQLEXPRESS;Initial Catalog=dbTarefas2;Integrated Security=True;TrustServerCertificate=True";
        public string Mensagem { get; protected set; }
        public bool Status { get; protected set; }

        public BaseDB()
        {
            Status = true;
            Mensagem = "Pronto para operações de banco de dados.";
        }

        protected SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        public void Dispose()
        {
            // Implementação do dispose, se necessário
        }
    }
}