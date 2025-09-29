using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class SubtarefasDB : BaseDB
    {
        public bool InserirSubtarefa(Tarefas_SubTarefas subtarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Tarefas_SubTarefas (Texto, CodTarefa, isConcluida) 
                                 VALUES (@Texto, @CodTarefa, @isConcluida)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", subtarefa.Texto);
                        cmd.Parameters.AddWithValue("@CodTarefa", subtarefa.CodTarefa);
                        cmd.Parameters.AddWithValue("@isConcluida", subtarefa.isConcluida);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Subtarefa inserida com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir subtarefa: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarSubtarefa(Tarefas_SubTarefas subtarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Tarefas_SubTarefas SET Texto = @Texto, isConcluida = @isConcluida 
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", subtarefa.Texto);
                        cmd.Parameters.AddWithValue("@isConcluida", subtarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@Codigo", subtarefa.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Subtarefa atualizada com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar subtarefa: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirSubtarefa(int codSubtarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "DELETE FROM Tarefas_SubTarefas WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codSubtarefa);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Subtarefa excluída com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir subtarefa: " + ex.Message;
                return false;
            }
        }

        public List<Tarefas_SubTarefas> ObterSubtarefasPorTarefa(int codTarefa)
        {
            List<Tarefas_SubTarefas> subtarefas = new List<Tarefas_SubTarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Tarefas_SubTarefas WHERE CodTarefa = @CodTarefa ORDER BY Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subtarefas.Add(new Tarefas_SubTarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Texto = reader["Texto"].ToString(),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter subtarefas: " + ex.Message;
            }
            return subtarefas;
        }
    }
}