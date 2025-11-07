using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class HistoricoDB : BaseDB
    {
        public string Mensagem { get; private set; }

        public bool InserirHistorico(HistoricoModificacoes historico)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO HistoricoModificacoes (CodTarefa, CodUsuario, Data, Texto) 
                                 VALUES (@CodTarefa, @CodUsuario, @Data, @Texto)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", historico.CodTarefa);
                        cmd.Parameters.AddWithValue("@CodUsuario", historico.CodUsuario);
                        cmd.Parameters.AddWithValue("@Data", historico.Data);
                        cmd.Parameters.AddWithValue("@Texto", historico.Texto);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Histórico registrado com sucesso.";
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

        public List<HistoricoModificacoes> ObterHistoricoPorTarefa(int codTarefa)
        {
            List<HistoricoModificacoes> historicos = new List<HistoricoModificacoes>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT H.Codigo, H.CodTarefa, H.CodUsuario, H.Data, H.Texto, U.Nome as NomeUsuario
                                 FROM HistoricoModificacoes H
                                 INNER JOIN Usuario U ON H.CodUsuario = U.Codigo
                                 WHERE H.CodTarefa = @CodTarefa
                                 ORDER BY H.Data DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                historicos.Add(new HistoricoModificacoes
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Texto = reader["Texto"].ToString(),
                                    NomeUsuario = reader["NomeUsuario"].ToString()
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

        public string FormatarTempoRelativo(DateTime data)
        {
            var tempoDecorrido = DateTime.Now - data;

            if (tempoDecorrido.TotalMinutes < 1)
                return "Agora mesmo";
            else if (tempoDecorrido.TotalMinutes < 60)
                return $"{(int)tempoDecorrido.TotalMinutes} min atrás";
            else if (tempoDecorrido.TotalHours < 24)
                return $"{(int)tempoDecorrido.TotalHours} h atrás";
            else if (data.Date == DateTime.Today)
                return $"Hoje às {data:HH:mm}";
            else if (data.Date == DateTime.Today.AddDays(-1))
                return $"Ontem às {data:HH:mm}";
            else
                return data.ToString("dd/MM/yyyy HH:mm");
        }

        public bool RegistrarMovimentacaoTarefa(int codTarefa, int codUsuario, string deStatus, string paraStatus)
        {
            var historico = new HistoricoModificacoes
            {
                CodTarefa = codTarefa,
                CodUsuario = codUsuario,
                Data = DateTime.Now,
                Texto = $"Moveu a tarefa de '{deStatus}' para '{paraStatus}'"
            };

            return InserirHistorico(historico);
        }

        public bool RegistrarCriacaoTarefa(int codTarefa, int codUsuario)
        {
            var historico = new HistoricoModificacoes
            {
                CodTarefa = codTarefa,
                CodUsuario = codUsuario,
                Data = DateTime.Now,
                Texto = "Criou a tarefa"
            };

            return InserirHistorico(historico);
        }

        public bool RegistrarConclusaoTarefa(int codTarefa, int codUsuario, bool concluida)
        {
            var acao = concluida ? "Concluiu a tarefa" : "Reabriu a tarefa";
            var historico = new HistoricoModificacoes
            {
                CodTarefa = codTarefa,
                CodUsuario = codUsuario,
                Data = DateTime.Now,
                Texto = acao
            };

            return InserirHistorico(historico);
        }

        // Método para obter a inicial do usuário
        public static string ObterInicial(string nomeUsuario)
        {
            if (!string.IsNullOrEmpty(nomeUsuario))
                return nomeUsuario[0].ToString().ToUpper();
            return "?";
        }
    }
}