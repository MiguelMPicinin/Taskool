using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CanvasApp.Classes.Databases
{
    public class TarefasDB : BaseDB
    {
        // CONSTRUTORES
        public TarefasDB() { }

        public TarefasDB(NotificacoesDB notificacoesDB, ProjetosDB projetosDB, UsuarioDB usuarioDB, MembrosDB membrosDB, AlarmeDB alarmeDB, SubtarefasDB subtarefasDB, ComentariosDB comentariosDB)
        {
            // Inicialização das dependências se necessário
        }

        // MÉTODOS DE ALARME CORRIGIDOS
        public List<Projeto_Tarefas> ObterTarefasComAlarmeHoje(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime amanha = hoje.AddDays(1);
            return ObterTarefasComAlarmeNoPeriodo(usuarioId, hoje, amanha);
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeSemana(int usuarioId)
        {
            DateTime inicioSemana = GetInicioSemana(DateTime.Today);
            DateTime fimSemana = inicioSemana.AddDays(7);
            return ObterTarefasComAlarmeNoPeriodo(usuarioId, inicioSemana, fimSemana);
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeMes(int usuarioId)
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            return ObterTarefasComAlarmeNoPeriodo(usuarioId, inicioMes, fimMes);
        }

        private List<Projeto_Tarefas> ObterTarefasComAlarmeNoPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT DISTINCT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicio AND A.Data < @fim
                        AND PT.isConcluida = 0
                        ORDER BY A.Data ASC, A.Hora ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao obter tarefas com alarme no período: " + ex.Message;
            }
            return tarefas;
        }

        // MÉTODOS DE QUANTIDADE PARA DASHBOARD
        public int ObterQuantidadeTarefasComAlarmeHoje(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime amanha = hoje.AddDays(1);
            return ObterQuantidadeTarefasComAlarmeNoPeriodo(usuarioId, hoje, amanha);
        }

        public int ObterQuantidadeTarefasComAlarmeSemana(int usuarioId)
        {
            DateTime inicioSemana = GetInicioSemana(DateTime.Today);
            DateTime fimSemana = inicioSemana.AddDays(7);
            return ObterQuantidadeTarefasComAlarmeNoPeriodo(usuarioId, inicioSemana, fimSemana);
        }

        public int ObterQuantidadeTarefasComAlarmeMes(int usuarioId)
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            return ObterQuantidadeTarefasComAlarmeNoPeriodo(usuarioId, inicioMes, fimMes);
        }

        private int ObterQuantidadeTarefasComAlarmeNoPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicio AND A.Data < @fim
                        AND PT.isConcluida = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas com alarme no período: " + ex.Message;
                return 0;
            }
        }

        // MÉTODOS CORRIGIDOS PARA DASHBOARD - COM ALARME
        public int ObterQuantidadeTarefasConcluidasComAlarmeHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @hoje AND A.Data < @amanha
                        AND PT.isConcluida = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", hoje);
                        cmd.Parameters.AddWithValue("@amanha", amanha);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas de hoje: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasPendentesComAlarmeHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @hoje AND A.Data < @amanha
                        AND PT.isConcluida = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", hoje);
                        cmd.Parameters.AddWithValue("@amanha", amanha);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas pendentes de hoje: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasConcluidasComAlarmeSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicioSemana AND A.Data < @fimSemana
                        AND PT.isConcluida = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicioSemana", inicioSemana);
                        cmd.Parameters.AddWithValue("@fimSemana", fimSemana);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas da semana: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasPendentesComAlarmeSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicioSemana AND A.Data < @fimSemana
                        AND PT.isConcluida = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicioSemana", inicioSemana);
                        cmd.Parameters.AddWithValue("@fimSemana", fimSemana);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas pendentes da semana: " + ex.Message;
                return 0;
            }
        }

        // MÉTODO DE DIAGNÓSTICO
        public void DiagnosticoCompletoBrasil(int usuarioId)
        {
            try
            {
                Console.WriteLine($"=== DIAGNÓSTICO COMPLETO - Usuário: {usuarioId} ===");
                Console.WriteLine($"Tarefas com alarme hoje: {ObterQuantidadeTarefasComAlarmeHoje(usuarioId)}");
                Console.WriteLine($"Tarefas com alarme semana: {ObterQuantidadeTarefasComAlarmeSemana(usuarioId)}");
                Console.WriteLine($"Tarefas com alarme mês: {ObterQuantidadeTarefasComAlarmeMes(usuarioId)}");

                var tarefasTotais = ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                var tarefasConcluidas = ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                var tarefasPendentes = ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);

                Console.WriteLine($"Tarefas totais: {tarefasTotais}");
                Console.WriteLine($"Tarefas concluídas: {tarefasConcluidas}");
                Console.WriteLine($"Tarefas pendentes: {tarefasPendentes}");
                Console.WriteLine("=== FIM DO DIAGNÓSTICO ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no diagnóstico: {ex.Message}");
            }
        }

        // MÉTODO PARA PROJETOS COMPARTILHADOS
        public bool CriarTarefaCompartilhada(Projeto_Tarefas tarefa)
        {
            return InserirTarefa(tarefa);
        }

        // MÉTODO AUXILIAR PARA INÍCIO DA SEMANA
        private DateTime GetInicioSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        // MÉTODOS CRUD BÁSICOS
        public bool InserirTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Projeto_Tarefas (Descricao, isConcluida, CodProjeto, CodUsuario) 
                                 VALUES (@Descricao, @isConcluida, @CodProjeto, @CodUsuario)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@CodProjeto", tarefa.CodProjeto);
                        cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa inserida com sucesso";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir Tarefa: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto_Tarefas SET Descricao = @Descricao, CodUsuario = @CodUsuario
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                        cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Codigo", tarefa.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa atualizada com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar tarefa: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirTarefa(int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sqlSubtarefas = "DELETE FROM Tarefas_SubTarefas WHERE CodTarefa = @CodTarefa";
                    string sqlComentarios = "DELETE FROM Tarefas_Comentarios WHERE CodTarefa = @CodTarefa";
                    string sqlFavoritos = "DELETE FROM Items_Favoritos WHERE CodTarefa = @CodTarefa";
                    string sqlAlarmes = "DELETE FROM Alarme WHERE CodTarefa = @CodTarefa";
                    string sqlTarefa = "DELETE FROM Projeto_Tarefas WHERE Codigo = @CodTarefa";

                    using (SqlCommand cmd = new SqlCommand(sqlSubtarefas, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlComentarios, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlFavoritos, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlAlarmes, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlTarefa, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                    }

                    Mensagem = "Tarefa excluída com sucesso.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir tarefa: " + ex.Message;
                return false;
            }
        }

        public Projeto_Tarefas ObterTarefaPorCodigo(int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario
                                 FROM Projeto_Tarefas WHERE Codigo = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                return new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue)
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefa: " + ex.Message;
                return null;
            }
        }

        public List<Projeto_Tarefas> ObterTarefasPorUsuario(int usuarioId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        ORDER BY PT.Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao buscar tarefas do usuário: " + ex.Message;
            }
            return tarefas;
        }

        public List<Projeto_Tarefas> ObterTarefasPorProjeto(int projetoId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario 
                                  FROM Projeto_Tarefas 
                                  WHERE CodProjeto = @projetoId
                                  ORDER BY Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao buscar as Tarefas do Projeto: " + ex.Message;
            }
            return tarefas;
        }

        public bool AtualizarStatusTarefa(int tarefaId, bool isConcluida)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto_Tarefas SET isConcluida = @isConcluida 
                                 WHERE Codigo = @tarefaId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@isConcluida", isConcluida);
                        cmd.Parameters.AddWithValue("@tarefaId", tarefaId);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Status da tarefa atualizado com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar status da tarefa: " + ex.Message;
                return false;
            }
        }

        public bool AtribuirTarefaUsuario(int codTarefa, int codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto_Tarefas SET CodUsuario = @CodUsuario 
                                 WHERE Codigo = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa atribuída com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atribuir tarefa: " + ex.Message;
                return false;
            }
        }

        public List<Projeto_Tarefas> ObterTarefasPendentesPorProjeto(int projetoId)
        {
            return ObterTarefasPorStatus(projetoId, false);
        }

        public List<Projeto_Tarefas> ObterTarefasConcluidasPorProjeto(int codigoProjeto)
        {
            return ObterTarefasPorStatus(codigoProjeto, true);
        }

        private List<Projeto_Tarefas> ObterTarefasPorStatus(int projetoId, bool isConcluida)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario 
                                  FROM Projeto_Tarefas 
                                  WHERE CodProjeto = @projetoId AND isConcluida = @isConcluida 
                                  ORDER BY Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        cmd.Parameters.AddWithValue("@isConcluida", isConcluida);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao buscar tarefas por status: " + ex.Message;
            }
            return tarefas;
        }

        public int ObterQuantidadeTarefasPorProjeto(int projetoId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT COUNT(*) FROM Projeto_Tarefas WHERE CodProjeto = @projetoId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasConcluidasPorProjeto(int projetoId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Projeto_Tarefas 
                                 WHERE CodProjeto = @projetoId AND isConcluida = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasTotaisDoUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas totais: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasTotaisConcluidasDoUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId AND PT.isConcluida = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas totais: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasTotaisPendentesDoUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId AND PT.isConcluida = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas pendentes totais: " + ex.Message;
                return 0;
            }
        }

        public List<Projeto_Tarefas> ObterTarefasTotaisConcluidasDoUsuario(int usuarioId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId AND PT.isConcluida = 1
                        ORDER BY PT.Codigo DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao buscar tarefas concluídas: " + ex.Message;
            }
            return tarefas;
        }

        public List<Projeto_Tarefas> ObterTarefasTotaisPendentesDoUsuario(int usuarioId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId AND PT.isConcluida = 0
                        ORDER BY PT.Codigo DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                tarefas.Add(new Projeto_Tarefas
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
                Mensagem = "Erro ao buscar tarefas pendentes: " + ex.Message;
            }
            return tarefas;
        }

        public int ObterQuantidadeTarefasConcluidasPorData(int usuarioId, DateTime data)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId 
                        AND PT.isConcluida = 1
                        AND EXISTS (
                            SELECT 1 FROM Tarefas_Comentarios TC 
                            WHERE TC.CodTarefa = PT.Codigo 
                            AND CAST(TC.Data AS DATE) = @Data
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@Data", data.Date);
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas por data: " + ex.Message;
                return 0;
            }
        }

        public int ObterQuantidadeTarefasConcluidasPorDataAlternativo(int usuarioId, DateTime data)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        LEFT JOIN Tarefas_Historico TH ON PT.Codigo = TH.CodTarefa
                        WHERE PM.CodMembro = @usuarioId 
                        AND PT.isConcluida = 1
                        AND (
                            (TH.Acao = 'Concluir' AND CAST(TH.DataAcao AS DATE) = @Data)
                            OR 
                            (PT.isConcluida = 1 AND NOT EXISTS (
                                SELECT 1 FROM Tarefas_Historico TH2 
                                WHERE TH2.CodTarefa = PT.Codigo 
                                AND TH2.Acao = 'Reabrir'
                                AND TH2.DataAcao > TH.DataAcao
                            ))
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@Data", data.Date);
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar tarefas concluídas (alternativo): " + ex.Message;
                return 0;
            }
        }

        public List<ProjetosTarefasDatas> ObterDadosGraficoTarefasPorProjeto(int usuarioId)
        {
            var dados = new List<ProjetosTarefasDatas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT
                            P.Nome as NomeProjeto,
                            COUNT(PT.Codigo) as QuantidadeTarefas
                        FROM Projeto P
                        LEFT JOIN Projeto_Tarefas PT ON P.Codigo = PT.CodProjeto
                        INNER JOIN Projeto_Membros PM ON P.Codigo = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        GROUP BY P.Codigo, P.Nome
                        ORDER BY COUNT(PT.Codigo) DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dados.Add(new ProjetosTarefasDatas
                                {
                                    NomeProjeto = reader["NomeProjeto"].ToString(),
                                    QuantidadeTarefas = Convert.ToInt32(reader["QuantidadeTarefas"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar dados do gráfico: " + ex.Message;
            }
            return dados;
        }
    }
}