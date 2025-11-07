using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CanvasApp.Classes.Databases
{
    public class TarefasDB : BaseDB
    {
        public TarefasDB() { }

        public TarefasDB(NotificacoesDB notificacoesDB, ProjetosDB projetosDB, UsuarioDB usuarioDB, MembrosDB membrosDB, AlarmeDB alarmeDB, SubtarefasDB subtarefasDB, ComentariosDB comentariosDB)
        {
        }

        private object ValidarDataParaSQL(DateTime data)
        {
            if (data == DateTime.MinValue || data < new DateTime(1753, 1, 1))
                return DBNull.Value;
            return data;
        }

        // MÉTODOS DO KANBAN - CORRIGIDOS
        public bool InserirTarefaKanban(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO Projeto_Tarefas 
                             (CodProjeto, CodUsuario, CodResponsavel, Descricao, isConcluida, isFazendo, Cor, dataConclusao, dataLimite)
                             VALUES 
                             (@CodProjeto, @CodUsuario, @CodResponsavel, @Descricao, @isConcluida, @isFazendo, @Cor, @dataConclusao, @dataLimite)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", tarefa.CodProjeto);

                        if (tarefa.CodUsuario.HasValue)
                            cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario.Value);
                        else
                            cmd.Parameters.AddWithValue("@CodUsuario", DBNull.Value);

                        if (tarefa.CodResponsavel.HasValue)
                            cmd.Parameters.AddWithValue("@CodResponsavel", tarefa.CodResponsavel.Value);
                        else
                            cmd.Parameters.AddWithValue("@CodResponsavel", DBNull.Value);

                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao ?? "");
                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@isFazendo", tarefa.isFazendo);
                        cmd.Parameters.AddWithValue("@Cor", tarefa.Cor ?? "#ffe079");
                        cmd.Parameters.AddWithValue("@dataConclusao", ValidarDataParaSQL(tarefa.dataConclusao));
                        cmd.Parameters.AddWithValue("@dataLimite", ValidarDataParaSQL(tarefa.dataLimite));

                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa criada com sucesso no Kanban!";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao criar tarefa no Kanban: " + ex.Message;
                return false;
            }
        }

        public List<Projeto_Tarefas> ObterTarefasKanbanPorProjeto(int codProjeto)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Codigo, CodProjeto, CodUsuario, CodResponsavel, Descricao, 
                                           isConcluida, isFazendo, Cor, dataConclusao, dataLimite
                                    FROM Projeto_Tarefas 
                                    WHERE CodProjeto = @CodProjeto
                                    ORDER BY 
                                        CASE 
                                            WHEN isConcluida = 1 THEN 3
                                            WHEN isFazendo = 1 THEN 2
                                            ELSE 1
                                        END,
                                        Codigo ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var codResponsavelValue = reader["CodResponsavel"];
                                var dataConclusaoValue = reader["dataConclusao"];
                                var dataLimiteValue = reader["dataLimite"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    CodResponsavel = codResponsavelValue == DBNull.Value ? null : (int?)Convert.ToInt32(codResponsavelValue),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    isFazendo = Convert.ToBoolean(reader["isFazendo"]),
                                    Cor = reader["Cor"]?.ToString() ?? "#ffe079",
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar tarefas do Kanban: " + ex.Message;
            }
            return tarefas;
        }

        public bool AtualizarStatusKanban(int codTarefa, bool isConcluida, bool isFazendo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE Projeto_Tarefas 
                             SET isConcluida = @isConcluida, 
                                 isFazendo = @isFazendo,
                                 dataConclusao = @dataConclusao
                             WHERE Codigo = @Codigo";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@isConcluida", isConcluida);
                        cmd.Parameters.AddWithValue("@isFazendo", isFazendo);

                        if (isConcluida)
                            cmd.Parameters.AddWithValue("@dataConclusao", DateTime.Now);
                        else
                            cmd.Parameters.AddWithValue("@dataConclusao", DBNull.Value);

                        cmd.Parameters.AddWithValue("@Codigo", codTarefa);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Mensagem = rowsAffected > 0 ? "Status atualizado com sucesso!" : "Tarefa não encontrada.";
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar status: " + ex.Message;
                return false;
            }
        }

        public List<string> ObterCoresPostIt()
        {
            return new List<string>
            {
                "#ffe079", // Amarelo
                "#f097ca", // Rosa
                "#98d366", // Verde
                "#82d3e5"  // Azul
            };
        }

        public List<Projeto_Tarefas> ObterTarefasPorStatusKanban(int codProjeto, string status)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Codigo, CodProjeto, CodUsuario, CodResponsavel, Descricao, 
                                           isConcluida, isFazendo, Cor, dataConclusao, dataLimite
                                    FROM Projeto_Tarefas 
                                    WHERE CodProjeto = @CodProjeto";

                    if (!string.IsNullOrEmpty(status))
                    {
                        switch (status.ToLower())
                        {
                            case "afazer":
                                query += " AND isConcluida = 0 AND isFazendo = 0";
                                break;
                            case "fazendo":
                                query += " AND isConcluida = 0 AND isFazendo = 1";
                                break;
                            case "feito":
                                query += " AND isConcluida = 1";
                                break;
                        }
                    }

                    query += " ORDER BY Codigo ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var codResponsavelValue = reader["CodResponsavel"];
                                var dataConclusaoValue = reader["dataConclusao"];
                                var dataLimiteValue = reader["dataLimite"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    CodResponsavel = codResponsavelValue == DBNull.Value ? null : (int?)Convert.ToInt32(codResponsavelValue),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    isFazendo = Convert.ToBoolean(reader["isFazendo"]),
                                    Cor = reader["Cor"]?.ToString() ?? "#ffe079",
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue)
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

        // MÉTODOS ORIGINAIS MANTIDOS E CORRIGIDOS
        public bool CriarTarefaCompartilhada(Projeto_Tarefas tarefa)
        {
            return InserirTarefaKanban(tarefa);
        }

        public (int concluidas, int pendentes) ContarTarefasComAlarmeHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            PT.Codigo,
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @hoje AND A.Data < @amanha
                        AND PT.isConcluida IN (0, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", hoje);
                        cmd.Parameters.AddWithValue("@amanha", amanha);

                        List<bool> statusTarefas = new List<bool>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isConcluida = Convert.ToBoolean(reader["isConcluida"]);
                                statusTarefas.Add(isConcluida);
                            }
                        }

                        int concluidas = statusTarefas.Count(status => status == true);
                        int pendentes = statusTarefas.Count(status => status == false);

                        Console.WriteLine($"🔔 Tarefas com alarme HOJE: {concluidas} concluídas, {pendentes} pendentes (Total: {statusTarefas.Count})");
                        return (concluidas, pendentes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar tarefas com alarme hoje: {ex.Message}");
                return (0, 0);
            }
        }

        public (int concluidas, int pendentes) ContarTarefasComAlarmeSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            PT.Codigo,
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicioSemana AND A.Data < @fimSemana
                        AND PT.isConcluida IN (0, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicioSemana", inicioSemana);
                        cmd.Parameters.AddWithValue("@fimSemana", fimSemana);

                        List<bool> statusTarefas = new List<bool>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isConcluida = Convert.ToBoolean(reader["isConcluida"]);
                                statusTarefas.Add(isConcluida);
                            }
                        }

                        int concluidas = statusTarefas.Count(status => status == true);
                        int pendentes = statusTarefas.Count(status => status == false);

                        Console.WriteLine($"🔔 Tarefas com alarme SEMANA: {concluidas} concluídas, {pendentes} pendentes (Total: {statusTarefas.Count})");
                        return (concluidas, pendentes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar tarefas com alarme semana: {ex.Message}");
                return (0, 0);
            }
        }

        public (int concluidas, int pendentes) ContarTarefasComDataLimiteHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @hoje AND PT.dataLimite < @amanha
                        AND PT.isConcluida IN (0, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", hoje);
                        cmd.Parameters.AddWithValue("@amanha", amanha);

                        List<bool> statusTarefas = new List<bool>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isConcluida = Convert.ToBoolean(reader["isConcluida"]);
                                statusTarefas.Add(isConcluida);
                            }
                        }

                        int concluidas = statusTarefas.Count(status => status == true);
                        int pendentes = statusTarefas.Count(status => status == false);

                        Console.WriteLine($"📅 Tarefas com data limite HOJE: {concluidas} concluídas, {pendentes} pendentes (Total: {statusTarefas.Count})");
                        return (concluidas, pendentes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar tarefas com data limite hoje: {ex.Message}");
                return (0, 0);
            }
        }

        public (int concluidas, int pendentes) ContarTarefasComDataLimiteSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @inicioSemana AND PT.dataLimite < @fimSemana
                        AND PT.isConcluida IN (0, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicioSemana", inicioSemana);
                        cmd.Parameters.AddWithValue("@fimSemana", fimSemana);

                        List<bool> statusTarefas = new List<bool>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isConcluida = Convert.ToBoolean(reader["isConcluida"]);
                                statusTarefas.Add(isConcluida);
                            }
                        }

                        int concluidas = statusTarefas.Count(status => status == true);
                        int pendentes = statusTarefas.Count(status => status == false);

                        Console.WriteLine($"📅 Tarefas com data limite SEMANA: {concluidas} concluídas, {pendentes} pendentes (Total: {statusTarefas.Count})");
                        return (concluidas, pendentes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar tarefas com data limite semana: {ex.Message}");
                return (0, 0);
            }
        }

        public DateTime ObterDataAlarmeTarefa(int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Data FROM Alarme WHERE CodTarefa = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDateTime(result) : DateTime.MinValue;
                    }
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

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
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario,
                               PT.dataLimite, PT.dataConclusao
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
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

        public int ObterQuantidadeTarefasConcluidasComAlarmeHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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

        public List<Projeto_Tarefas> ObterTarefasComDataLimiteHoje(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime amanha = hoje.AddDays(1);
            return ObterTarefasComDataLimiteNoPeriodo(usuarioId, hoje, amanha);
        }

        public List<Projeto_Tarefas> ObterTarefasComDataLimiteSemana(int usuarioId)
        {
            DateTime inicioSemana = GetInicioSemana(DateTime.Today);
            DateTime fimSemana = inicioSemana.AddDays(7);
            return ObterTarefasComDataLimiteNoPeriodo(usuarioId, inicioSemana, fimSemana);
        }

        public List<Projeto_Tarefas> ObterTarefasComDataLimiteNoPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT 
                            PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario,
                            PT.dataLimite, PT.dataConclusao
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @inicio AND PT.dataLimite < @fim
                        ORDER BY PT.dataLimite ASC";

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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefas com data limite no período: " + ex.Message;
            }
            return tarefas;
        }

        public List<Projeto_Tarefas> ObterTarefasComDataLimiteHojeAlternativo(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario,
                       PT.dataLimite, PT.dataConclusao
                FROM Projeto_Tarefas PT
                INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                WHERE PM.CodMembro = @usuarioId
                AND PT.dataLimite = @hoje
                ORDER BY PT.dataLimite ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", hoje);

                        List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
                                });
                            }
                        }
                        return tarefas;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefas de hoje (alternativo): " + ex.Message;
                return new List<Projeto_Tarefas>();
            }
        }

        public List<Projeto_Tarefas> ObterTarefasComDataLimiteSemanaAlternativo(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario,
                       PT.dataLimite, PT.dataConclusao
                FROM Projeto_Tarefas PT
                INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                WHERE PM.CodMembro = @usuarioId
                AND PT.dataLimite >= @inicioSemana AND PT.dataLimite < @fimSemana
                ORDER BY PT.dataLimite ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicioSemana", inicioSemana);
                        cmd.Parameters.AddWithValue("@fimSemana", fimSemana);

                        List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
                                });
                            }
                        }
                        return tarefas;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefas da semana (alternativo): " + ex.Message;
                return new List<Projeto_Tarefas>();
            }
        }

        public int ObterQuantidadeTarefasConcluidasComDataLimiteHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @hoje AND PT.dataLimite < @amanha
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

        public int ObterQuantidadeTarefasPendentesComDataLimiteHoje(int usuarioId)
        {
            try
            {
                DateTime hoje = DateTime.Today;
                DateTime amanha = hoje.AddDays(1);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @hoje AND PT.dataLimite < @amanha
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

        public int ObterQuantidadeTarefasConcluidasComDataLimiteSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @inicioSemana AND PT.dataLimite < @fimSemana
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

        public int ObterQuantidadeTarefasPendentesComDataLimiteSemana(int usuarioId)
        {
            try
            {
                DateTime inicioSemana = GetInicioSemana(DateTime.Today);
                DateTime fimSemana = inicioSemana.AddDays(7);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(DISTINCT PT.Codigo)
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.dataLimite >= @inicioSemana AND PT.dataLimite < @fimSemana
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

        private DateTime GetInicioSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        public bool InserirTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO Projeto_Tarefas (Descricao, isConcluida, CodProjeto, CodUsuario, dataLimite, dataConclusao) 
                                 VALUES (@Descricao, @isConcluida, @CodProjeto, @CodUsuario, @dataLimite, @dataConclusao)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@CodProjeto", tarefa.CodProjeto);

                        if (tarefa.CodUsuario.HasValue)
                            cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario.Value);
                        else
                            cmd.Parameters.AddWithValue("@CodUsuario", DBNull.Value);

                        cmd.Parameters.AddWithValue("@dataLimite", ValidarDataParaSQL(tarefa.dataLimite));
                        cmd.Parameters.AddWithValue("@dataConclusao", ValidarDataParaSQL(tarefa.dataConclusao));

                        cmd.ExecuteNonQuery();
                        Mensagem = "Tarefa inserida com sucesso";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao Inserir Tarefa: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarDataLimiteTarefa(int codTarefa, DateTime novaDataLimite)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE Projeto_Tarefas SET dataLimite = @dataLimite
                         WHERE Codigo = @Codigo";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@dataLimite", ValidarDataParaSQL(novaDataLimite));
                        cmd.Parameters.AddWithValue("@Codigo", codTarefa);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Mensagem = rowsAffected > 0 ? "Data limite atualizada com sucesso!" : "Tarefa não encontrada.";
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar data limite: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE Projeto_Tarefas 
                         SET Descricao = @Descricao, 
                             CodUsuario = @CodUsuario,
                             CodResponsavel = @CodResponsavel,
                             isConcluida = @isConcluida,
                             isFazendo = @isFazendo,
                             Cor = @Cor,
                             dataConclusao = @dataConclusao,
                             dataLimite = @dataLimite
                         WHERE Codigo = @Codigo";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);

                        if (tarefa.CodUsuario.HasValue)
                            cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario.Value);
                        else
                            cmd.Parameters.AddWithValue("@CodUsuario", DBNull.Value);

                        if (tarefa.CodResponsavel.HasValue)
                            cmd.Parameters.AddWithValue("@CodResponsavel", tarefa.CodResponsavel.Value);
                        else
                            cmd.Parameters.AddWithValue("@CodResponsavel", DBNull.Value);

                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@isFazendo", tarefa.isFazendo);
                        cmd.Parameters.AddWithValue("@Cor", tarefa.Cor ?? "#ffe079");
                        cmd.Parameters.AddWithValue("@dataConclusao", ValidarDataParaSQL(tarefa.dataConclusao));
                        cmd.Parameters.AddWithValue("@dataLimite", ValidarDataParaSQL(tarefa.dataLimite));
                        cmd.Parameters.AddWithValue("@Codigo", tarefa.Codigo);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Mensagem = rowsAffected > 0 ? "Tarefa atualizada com sucesso!" : "Tarefa não encontrada.";
                        return rowsAffected > 0;
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
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Primeiro excluir o histórico da tarefa
                            string sqlHistorico = "DELETE FROM Tarefas_Historico WHERE CodTarefa = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlHistorico, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Excluir subtarefas
                            string sqlSubtarefas = "DELETE FROM Tarefas_SubTarefas WHERE CodTarefa = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlSubtarefas, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                cmd.ExecuteNonQuery();
                            }

                            // 3. Excluir comentários
                            string sqlComentarios = "DELETE FROM Tarefas_Comentarios WHERE CodTarefa = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlComentarios, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                cmd.ExecuteNonQuery();
                            }

                            // 4. Excluir favoritos
                            string sqlFavoritos = "DELETE FROM Items_Favoritos WHERE CodTarefa = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlFavoritos, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                cmd.ExecuteNonQuery();
                            }

                            // 5. Excluir alarmes
                            string sqlAlarmes = "DELETE FROM Alarme WHERE CodTarefa = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlAlarmes, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                cmd.ExecuteNonQuery();
                            }

                            // 6. Finalmente excluir a tarefa
                            string sqlTarefa = "DELETE FROM Projeto_Tarefas WHERE Codigo = @CodTarefa";
                            using (SqlCommand cmd = new SqlCommand(sqlTarefa, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    Mensagem = "Tarefa excluída com sucesso.";
                                    return true;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    Mensagem = "Tarefa não encontrada.";
                                    return false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Mensagem = "Erro ao excluir tarefa: " + ex.Message;
                            return false;
                        }
                    }
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
                    conn.Open();
                    string sql = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario, dataLimite, dataConclusao
                                 FROM Projeto_Tarefas WHERE Codigo = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                return new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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

        // MÉTODOS ADICIONADOS PARA PLANNING POKER
        public List<Projeto_Tarefas> ObterTarefasPorUsuario(int usuarioId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario, PT.dataLimite, PT.dataConclusao
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
                    string query = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario, dataLimite, dataConclusao 
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
                    string query = @"SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario, dataLimite, dataConclusao 
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario, PT.dataLimite, PT.dataConclusao
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
                    string query = @"
                        SELECT PT.Codigo, PT.Descricao, PT.isConcluida, PT.CodProjeto, PT.CodUsuario, PT.dataLimite, PT.dataConclusao
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
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
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
                    conn.Open();
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
                    conn.Open();
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
                    conn.Open();
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

        public List<Projeto_Tarefas> ObterTarefasParaCaminhoCritico(int usuarioId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT 
                            PT.Codigo, 
                            PT.Descricao, 
                            PT.isConcluida, 
                            PT.CodProjeto, 
                            PT.CodUsuario,
                            PT.dataLimite, 
                            PT.dataConclusao
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.isConcluida = 0
                        AND PT.dataLimite >= @dataAtual
                        AND PT.dataLimite IS NOT NULL
                        ORDER BY PT.dataLimite ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@dataAtual", DateTime.Today);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var dataLimiteValue = reader["dataLimite"];
                                var dataConclusaoValue = reader["dataConclusao"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue),
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue)
                                });
                            }
                        }
                    }
                }

                Console.WriteLine($"✅ Encontradas {tarefas.Count} tarefas para caminho crítico");
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefas para caminho crítico: " + ex.Message;
                Console.WriteLine($"❌ Erro em ObterTarefasParaCaminhoCritico: {ex.Message}");
            }
            return tarefas;
        }

        public List<Projeto_Tarefas> ListarTarefasPorProjeto(int codProjeto)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Codigo, CodProjeto, CodUsuario, CodResponsavel, Descricao, 
                                       isConcluida, isFazendo, Cor, dataConclusao, dataLimite
                                FROM Projeto_Tarefas 
                                WHERE CodProjeto = @CodProjeto
                                ORDER BY Codigo DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var codUsuarioValue = reader["CodUsuario"];
                                var codResponsavelValue = reader["CodResponsavel"];
                                var dataConclusaoValue = reader["dataConclusao"];
                                var dataLimiteValue = reader["dataLimite"];

                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = codUsuarioValue == DBNull.Value ? null : (int?)Convert.ToInt32(codUsuarioValue),
                                    CodResponsavel = codResponsavelValue == DBNull.Value ? null : (int?)Convert.ToInt32(codResponsavelValue),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    isFazendo = Convert.ToBoolean(reader["isFazendo"]),
                                    Cor = reader["Cor"]?.ToString() ?? "#ffe079",
                                    dataConclusao = dataConclusaoValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataConclusaoValue),
                                    dataLimite = dataLimiteValue == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataLimiteValue)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao listar tarefas do projeto: " + ex.Message;
            }
            return tarefas;
        }

        public bool InserirTarefaComCor(Projeto_Tarefas tarefa)
        {
            return InserirTarefaKanban(tarefa);
        }

        public List<Projeto_Tarefas> ObterTodasTarefasDoUsuario(int usuarioId)
        {
            return ObterTarefasPorUsuario(usuarioId);
        }
    }
}