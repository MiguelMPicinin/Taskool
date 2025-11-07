using System;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public abstract class BaseDB : IDisposable
    {
        protected readonly string connectionString = "Data Source=OLIMPIADA\\SQLEXPRESS;Initial Catalog=dbTarefas;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        public string Mensagem { get; protected set; }
        public bool Status { get; protected set; }

        public BaseDB()
        {
            Status = true;
            Mensagem = "Pronto para operações de banco de dados.";
        }

        protected SqlConnection GetConnection()
        {
            // CORREÇÃO: Retorna a conexão SEM abrir
            return new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            // Implementação do dispose, se necessário
        }
    }
}