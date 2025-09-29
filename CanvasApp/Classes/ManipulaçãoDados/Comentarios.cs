using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class ComentariosDB : BaseDB
    {
        public bool InserirComentario(Tarefas_Comentarios comentario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Tarefas_Comentarios (CodUsuario, CodTarefa, Comentario, Data) 
                                 VALUES (@CodUsuario, @CodTarefa, @Comentario, @Data)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", comentario.CodUsuario);
                        cmd.Parameters.AddWithValue("@CodTarefa", comentario.CodTarefa);
                        cmd.Parameters.AddWithValue("@Comentario", comentario.Comentario);
                        cmd.Parameters.AddWithValue("@Data", comentario.Data);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Comentário inserido com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir comentário: " + ex.Message;
                return false;
            }
        }

        public List<Tarefas_Comentarios> ObterComentariosPorTarefa(int codTarefa)
        {
            List<Tarefas_Comentarios> comentarios = new List<Tarefas_Comentarios>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Tarefas_Comentarios 
                                 WHERE CodTarefa = @CodTarefa 
                                 ORDER BY Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comentarios.Add(new Tarefas_Comentarios
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodUsuario = reader["CodUsuario"].ToString(),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    Comentario = reader["Comentario"].ToString(),
                                    Data = Convert.ToDateTime(reader["Data"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter comentários: " + ex.Message;
            }
            return comentarios;
        }

        public string FormatarDataComentario(DateTime data)
        {
            return data.ToString("dd/MM/yyyy HH:mm");
        }
    }
}