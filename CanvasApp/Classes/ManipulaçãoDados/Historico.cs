using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.ManipulaçãoDados
{
    public class TarefasHistoricoDB : BaseDB
    {
        public bool InserirHistorico(Tarefas_Historico historico)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Tarefas_Historico (CodTarefa, CodUsuario, Acao, DataAcao) VALUES (@CodTarefa, @CodUsuario, @Acao, @DataAcao)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", historico.CodTarefa);
                        cmd.Parameters.AddWithValue("@CodUsuario", historico.CodUsuario);
                        cmd.Parameters.AddWithValue("@Acao", historico.Acao);
                        cmd.Parameters.AddWithValue("@DataAcao", historico.DataAcao);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir histórico: " + ex.Message;
                return false;
            }
        }

        public List<Tarefas_Historico> ObterHistoricoPorTarefa(int codTarefa)
        {
            List<Tarefas_Historico> historicos = new List<Tarefas_Historico>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT H.Codigo, H.CodTarefa, H.CodUsuario, H.Acao, H.DataAcao, U.Nome 
                                    FROM Tarefas_Historico H 
                                    INNER JOIN Usuario U ON H.CodUsuario = U.Codigo 
                                    WHERE H.CodTarefa = @CodTarefa 
                                    ORDER BY H.DataAcao DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                historicos.Add(new Tarefas_Historico
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Acao = reader["Acao"].ToString(),
                                    DataAcao = Convert.ToDateTime(reader["DataAcao"]),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter histórico: " + ex.Message;
            }
            return historicos;
        }

        // MÉTODOS ADICIONADOS PARA RESOLVER OS ERROS CS1061
        public bool RegistrarMovimentacaoTarefa(int codTarefa, int codUsuario, string deStatus, string paraStatus)
        {
            try
            {
                var historico = new Tarefas_Historico
                {
                    CodTarefa = codTarefa,
                    CodUsuario = codUsuario,
                    Acao = $"Moveu a tarefa de '{deStatus}' para '{paraStatus}'",
                    DataAcao = DateTime.Now
                };
                return InserirHistorico(historico);
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao registrar movimentação: " + ex.Message;
                return false;
            }
        }

        public bool RegistrarCriacaoTarefa(int codTarefa, int codUsuario)
        {
            try
            {
                var historico = new Tarefas_Historico
                {
                    CodTarefa = codTarefa,
                    CodUsuario = codUsuario,
                    Acao = "Criou a tarefa",
                    DataAcao = DateTime.Now
                };
                return InserirHistorico(historico);
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao registrar criação: " + ex.Message;
                return false;
            }
        }
    }
}