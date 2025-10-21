using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class FavoritosDB : BaseDB
    {
        public bool AdicionarFavorito(int codUsuario, int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Items_Favoritos (CodUsuario, CodTarefa) VALUES (@CodUsuario, @CodTarefa)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa adicionada aos favoritos.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao adicionar favorito: " + ex.Message;
                return false;
            }
        }

        public bool RemoverFavorito(int codUsuario, int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"DELETE FROM Items_Favoritos WHERE CodUsuario = @CodUsuario AND CodTarefa = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa removida dos favoritos.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao remover favorito: " + ex.Message;
                return false;
            }
        }

        public List<Projeto_Tarefas> ObterTarefasFavoritas(int codUsuario)
        {
            var tarefasFavoritas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"
                        SELECT T.Codigo, T.Descricao, T.isConcluida, T.CodProjeto, T.CodUsuario
                        FROM Projeto_Tarefas T
                        INNER JOIN Items_Favoritos F ON T.Codigo = F.CodTarefa
                        WHERE F.CodUsuario = @CodUsuario
                        ORDER BY T.Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefasFavoritas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefas favoritas: " + ex.Message;
            }
            return tarefasFavoritas;
        }

        public bool EhFavorito(int codUsuario, int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(1) FROM Items_Favoritos WHERE CodUsuario = @CodUsuario AND CodTarefa = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao verificar favorito: " + ex.Message;
                return false;
            }
        }

        public int ObterQuantidadeFavoritos(int codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Items_Favoritos WHERE CodUsuario = @CodUsuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar favoritos: " + ex.Message;
                return 0;
            }
        }
    }
}