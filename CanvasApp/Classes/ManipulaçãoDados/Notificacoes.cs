using CanvasApp.Classes.Databases.UsuarioCL;
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
                    string sql = @"INSERT INTO Notificacoes (Texto, data, CodProjeto, CodUsuario, isFechada) 
                             VALUES (@Texto, @data, @CodProjeto, @CodUsuario, @isFechada)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", notificacao.Texto);
                        cmd.Parameters.AddWithValue("@data", notificacao.Data);
                        cmd.Parameters.AddWithValue("@CodProjeto", notificacao.CodProjeto);
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
                    string query = @"SELECT Codigo, Texto, data, CodProjeto, CodUsuario, isFechada 
                               FROM Notificacoes 
                               WHERE CodUsuario = @codUsuario
                               ORDER BY data DESC";

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
                                    Data = Convert.ToDateTime(reader["data"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
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
                    string query = @"SELECT Codigo, Texto, data, CodProjeto, CodUsuario, isFechada 
                               FROM Notificacoes 
                               WHERE CodUsuario = @codUsuario AND isFechada = 0
                               ORDER BY data DESC";

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
                                    Data = Convert.ToDateTime(reader["data"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
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

        public bool MarcarNotificacaoComoLida(int codNotificacao)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
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

        public void NotificarMembrosNovaTarefa(Projeto_Tarefas tarefa, int usuarioCriador, MembrosDB membrosDB, ProjetosDB projetosDB, UsuarioDB usuarioDB)
        {
            try
            {
                var membros = membrosDB.ObterMembrosProjeto(tarefa.CodProjeto);
                string nomeProjeto = projetosDB.ObterNomeProjeto(tarefa.CodProjeto);
                string nomeCriador = usuarioDB.ObterNomeUsuario(usuarioCriador);

                foreach (var membro in membros)
                {
                    // Não notificar o criador da tarefa
                    if (int.Parse(membro.Codigo) == usuarioCriador)
                        continue;

                    var notificacao = new Notificacoes
                    {
                        Texto = $"{nomeCriador} criou a tarefa '{tarefa.Descricao}' no projeto {nomeProjeto}",
                        Data = DateTime.Now,
                        CodProjeto = tarefa.CodProjeto,
                        CodUsuario = int.Parse(membro.Codigo),
                        isFechada = false
                    };

                    InserirNotificacao(notificacao);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao notificar membros: " + ex.Message);
            }
        }
    }
}