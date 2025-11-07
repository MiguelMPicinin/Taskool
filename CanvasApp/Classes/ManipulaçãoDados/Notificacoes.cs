using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class NotificacoesDB : BaseDB
    {
        public bool InserirNotificacao(Notificacoes notificacao)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO Notificacoes (Texto, Data, CodProjeto, CodUsuario, isFechada) 
                                 VALUES (@Texto, @Data, @CodProjeto, @CodUsuario, @isFechada)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", notificacao.Texto);
                        cmd.Parameters.AddWithValue("@Data", notificacao.Data);
                        cmd.Parameters.AddWithValue("@CodProjeto", notificacao.CodProjeto ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodUsuario", notificacao.CodUsuario);
                        cmd.Parameters.AddWithValue("@isFechada", notificacao.isFechada);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Notificação inserida com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir notificação: " + ex.Message;
                return false;
            }
        }

        public List<Notificacoes> ObterNotificacoesPorUsuario(int usuarioId)
        {
            List<Notificacoes> notificacoes = new List<Notificacoes>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Codigo, Texto, Data, CodProjeto, CodUsuario, isFechada 
                               FROM Notificacoes 
                               WHERE CodUsuario = @codUsuario
                               ORDER BY Data DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codUsuario", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                notificacoes.Add(new Notificacoes
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Texto = reader["Texto"].ToString(),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    CodProjeto = reader["CodProjeto"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    isFechada = Convert.ToBoolean(reader["isFechada"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar notificações: " + ex.Message;
            }
            return notificacoes;
        }

        public List<Notificacoes> ObterNotificacoesNaoLidas(int usuarioId)
        {
            List<Notificacoes> notificacoes = new List<Notificacoes>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Codigo, Texto, Data, CodProjeto, CodUsuario, isFechada 
                               FROM Notificacoes 
                               WHERE CodUsuario = @codUsuario AND isFechada = 0
                               ORDER BY Data DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codUsuario", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                notificacoes.Add(new Notificacoes
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Texto = reader["Texto"].ToString(),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    CodProjeto = reader["CodProjeto"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    isFechada = Convert.ToBoolean(reader["isFechada"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar notificações não lidas: " + ex.Message;
            }
            return notificacoes;
        }

        public bool MarcarNotificacaoComoLida(int codNotificacao)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE Notificacoes SET isFechada = 1 WHERE Codigo = @codNotificacao";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codNotificacao", codNotificacao);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Notificação marcada como lida.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao marcar notificação como lida: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirNotificacao(int codNotificacao)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM Notificacoes WHERE Codigo = @codNotificacao";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codNotificacao", codNotificacao);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Notificação excluída com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir notificação: " + ex.Message;
                return false;
            }
        }

        public int ObterQuantidadeNotificacoesNaoLidas(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT COUNT(*) FROM Notificacoes 
                                 WHERE CodUsuario = @codUsuario AND isFechada = 0";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codUsuario", usuarioId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar notificações não lidas: " + ex.Message;
                return 0;
            }
        }

        public void LimparNotificacoesAntigas(int usuarioId, int dias)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"DELETE FROM Notificacoes 
                                 WHERE CodUsuario = @codUsuario 
                                 AND Data < DATEADD(day, -@dias, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codUsuario", usuarioId);
                        cmd.Parameters.AddWithValue("@dias", dias);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Notificações antigas limpas com sucesso.";
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao limpar notificações antigas: " + ex.Message;
            }
        }
    }
}