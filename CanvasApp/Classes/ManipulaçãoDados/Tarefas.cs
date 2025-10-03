using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CanvasApp.Classes.Databases
{
    public class TarefasDB : BaseDB
    {
        private readonly NotificacoesDB _notificacoesDB;
        private readonly ProjetosDB _projetosDB;
        private readonly UsuarioDB _usuarioDB;
        private readonly MembrosDB _membrosDB;
        private readonly AlarmeDB _alarmeDB;
        private readonly SubtarefasDB _subtarefasDB;
        private readonly ComentariosDB _comentariosDB;

        public TarefasDB(NotificacoesDB notificacoesDB, ProjetosDB projetosDB, UsuarioDB usuarioDB,
                        MembrosDB membrosDB, AlarmeDB alarmeDB, SubtarefasDB subtarefasDB, ComentariosDB comentariosDB)
        {
            _notificacoesDB = notificacoesDB;
            _projetosDB = projetosDB;
            _usuarioDB = usuarioDB;
            _membrosDB = membrosDB;
            _alarmeDB = alarmeDB;
            _subtarefasDB = subtarefasDB;
            _comentariosDB = comentariosDB;
        }

        public TarefasDB()
        {
            _notificacoesDB = new NotificacoesDB();
            _usuarioDB = new UsuarioDB();
            _projetosDB = new ProjetosDB();
            _membrosDB = new MembrosDB(_notificacoesDB, _projetosDB, _usuarioDB);
            _alarmeDB = new AlarmeDB();
            _subtarefasDB = new SubtarefasDB();
            _comentariosDB = new ComentariosDB();
        }

        // =========================================================================
        // MÉTODOS CORRIGIDOS PARA FUSO HORÁRIO BRASIL (UTC-3)
        // =========================================================================

        /// <summary>
        /// Obtém a data atual confiável do servidor SQL (evita problemas de fuso horário)
        /// </summary>
        private DateTime ObterDataAtualServidor()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT CAST(GETDATE() as DATE) as DataAtual, GETDATE() as DataHoraCompleta";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime dataServidor = Convert.ToDateTime(reader["DataAtual"]);
                                DateTime dataHoraServidor = Convert.ToDateTime(reader["DataHoraCompleta"]);

                                Console.WriteLine($"✅ Data do servidor SQL: {dataServidor:dd/MM/yyyy}");
                                Console.WriteLine($"✅ Data/hora servidor: {dataHoraServidor:dd/MM/yyyy HH:mm:ss}");
                                Console.WriteLine($"✅ Fuso horário: Brasil (UTC-3)");

                                return dataServidor;
                            }
                        }
                    }
                }

                // Fallback para DateTime.Today com informação do fuso
                var hojeSistema = DateTime.Today;
                var timezone = TimeZoneInfo.Local;
                Console.WriteLine($"⚠️  Fallback sistema: {hojeSistema:dd/MM/yyyy}");
                Console.WriteLine($"⚠️  Fuso sistema: {timezone.DisplayName}");
                return hojeSistema;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter data do servidor: {ex.Message}");
                var hojeSistema = DateTime.Today;
                Console.WriteLine($"⚠️  Usando fallback: {hojeSistema:dd/MM/yyyy}");
                return hojeSistema;
            }
        }

        /// <summary>
        /// Converte hora do banco considerando o fuso horário do Brasil
        /// </summary>
        private DateTime ConverterHoraBrasil(object horaValue)
        {
            try
            {
                DateTime dataBase = ObterDataAtualServidor();

                if (horaValue is TimeSpan timeSpan)
                {
                    DateTime resultado = dataBase.Add(timeSpan);
                    Console.WriteLine($"✅ Hora convertida: {resultado:HH:mm} (base: {dataBase:dd/MM/yyyy})");
                    return resultado;
                }
                else if (horaValue is DateTime dateTime)
                {
                    // Ajusta para o fuso do Brasil se necessário
                    if (dateTime.Kind == DateTimeKind.Utc)
                    {
                        TimeZoneInfo brasilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, brasilTimeZone);
                    }
                    return dateTime;
                }
                else if (horaValue != DBNull.Value && horaValue != null)
                {
                    return Convert.ToDateTime(horaValue);
                }
                else
                {
                    // Hora padrão 9:00 AM
                    return dataBase.AddHours(9);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao converter hora: {ex.Message}");
                return ObterDataAtualServidor().AddHours(9);
            }
        }

        // =========================================================================
        // MÉTODOS PRINCIPAIS CORRIGIDOS - FILTROS POR DATA
        // =========================================================================

        public List<Projeto_Tarefas> ObterTarefasComAlarmeHoje(int usuarioId)
        {
            try
            {
                DateTime dataAtual = ObterDataAtualServidor();
                DateTime hoje = dataAtual;
                DateTime amanha = hoje.AddDays(1);

                Console.WriteLine($"\n=== FILTRO HOJE (BRASIL UTC-3) ===");
                Console.WriteLine($"📍 Usuário: {usuarioId}");
                Console.WriteLine($"📅 Hoje: {hoje:dd/MM/yyyy}");
                Console.WriteLine($"📅 Amanhã: {amanha:dd/MM/yyyy}");

                var resultado = ObterTarefasComAlarmePorPeriodo(usuarioId, hoje, amanha);
                Console.WriteLine($"✅ Tarefas para hoje: {resultado.Count}");

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em ObterTarefasComAlarmeHoje: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeSemana(int usuarioId)
        {
            try
            {
                DateTime dataAtual = ObterDataAtualServidor();
                DateTime hoje = dataAtual;

                // Segunda-feira como início da semana (padrão Brasil)
                DateTime inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek + (int)DayOfWeek.Monday);
                if (hoje.DayOfWeek == DayOfWeek.Sunday)
                    inicioSemana = hoje.AddDays(-6); // Domingo volta para segunda anterior

                DateTime fimSemana = inicioSemana.AddDays(7);

                Console.WriteLine($"\n=== FILTRO SEMANA (BRASIL UTC-3) ===");
                Console.WriteLine($"📍 Usuário: {usuarioId}");
                Console.WriteLine($"📅 Início semana: {inicioSemana:dd/MM/yyyy}");
                Console.WriteLine($"📅 Fim semana: {fimSemana:dd/MM/yyyy}");

                var resultado = ObterTarefasComAlarmePorPeriodo(usuarioId, inicioSemana, fimSemana);
                Console.WriteLine($"✅ Tarefas para semana: {resultado.Count}");

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em ObterTarefasComAlarmeSemana: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeMes(int usuarioId)
        {
            try
            {
                DateTime dataAtual = ObterDataAtualServidor();
                DateTime hoje = dataAtual;
                DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
                DateTime fimMes = inicioMes.AddMonths(1);

                Console.WriteLine($"\n=== FILTRO MÊS (BRASIL UTC-3) ===");
                Console.WriteLine($"📍 Usuário: {usuarioId}");
                Console.WriteLine($"📅 Início mês: {inicioMes:dd/MM/yyyy}");
                Console.WriteLine($"📅 Fim mês: {fimMes:dd/MM/yyyy}");

                var resultado = ObterTarefasComAlarmePorPeriodo(usuarioId, inicioMes, fimMes);
                Console.WriteLine($"✅ Tarefas para mês: {resultado.Count}");

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em ObterTarefasComAlarmeMes: {ex.Message}");
                return new List<Projeto_Tarefas>();
            }
        }

        private List<Projeto_Tarefas> ObterTarefasComAlarmePorPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT DISTINCT 
                            PT.Codigo, 
                            PT.Descricao, 
                            PT.isConcluida, 
                            PT.CodProjeto, 
                            PT.CodUsuario,
                            A.Data as AlarmeData,
                            A.Hora as AlarmeHora,
                            P.Nome as ProjetoNome
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        INNER JOIN Projeto P ON PT.CodProjeto = P.Codigo
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicio AND A.Data < @fim
                        AND PT.isConcluida = 0
                        ORDER BY A.Data ASC, A.Hora ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);

                        Console.WriteLine($"🔍 Executando query: {inicio:dd/MM/yyyy} - {fim:dd/MM/yyyy}");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var tarefa = new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
                                };

                                // Debug detalhado
                                DateTime dataAlarme = Convert.ToDateTime(reader["AlarmeData"]);
                                DateTime horaAlarme = ConverterHoraBrasil(reader["AlarmeHora"]);
                                string projetoNome = reader["ProjetoNome"].ToString();

                                Console.WriteLine($"📌 Tarefa {tarefa.Codigo}: {tarefa.Descricao}");
                                Console.WriteLine($"   📁 Projeto: {projetoNome}");
                                Console.WriteLine($"   ⏰ Alarme: {dataAlarme:dd/MM/yyyy} {horaAlarme:HH:mm}");

                                tarefas.Add(tarefa);
                            }
                        }
                    }
                }

                Console.WriteLine($"✅ Total encontrado: {tarefas.Count} tarefas");
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar tarefas com alarme: " + ex.Message;
                Console.WriteLine($"❌ ERRO em ObterTarefasComAlarmePorPeriodo: {ex.Message}");
            }

            return tarefas;
        }

        // =========================================================================
        // MÉTODOS PARA QUANTIDADE (MENU LATERAL) - CORRIGIDOS
        // =========================================================================

        public int ObterQuantidadeTarefasComAlarmeHoje(int usuarioId)
        {
            try
            {
                int quantidade = ObterTarefasComAlarmeHoje(usuarioId).Count;
                Console.WriteLine($"📊 Quantidade hoje: {quantidade}");
                return quantidade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter quantidade de hoje: {ex.Message}");
                return 0;
            }
        }

        public int ObterQuantidadeTarefasComAlarmeSemana(int usuarioId)
        {
            try
            {
                int quantidade = ObterTarefasComAlarmeSemana(usuarioId).Count;
                Console.WriteLine($"📊 Quantidade semana: {quantidade}");
                return quantidade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter quantidade da semana: {ex.Message}");
                return 0;
            }
        }

        public int ObterQuantidadeTarefasComAlarmeMes(int usuarioId)
        {
            try
            {
                int quantidade = ObterTarefasComAlarmeMes(usuarioId).Count;
                Console.WriteLine($"📊 Quantidade mês: {quantidade}");
                return quantidade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter quantidade do mês: {ex.Message}");
                return 0;
            }
        }

        // =========================================================================
        // MÉTODO DE DIAGNÓSTICO PARA BRASIL (ÚNICO - SEM DUPLICAÇÃO)
        // =========================================================================

        public void DiagnosticoCompletoBrasil(int usuarioId)
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 70));
                Console.WriteLine("🎯 DIAGNÓSTICO COMPLETO - BRASIL (UTC-3)");
                Console.WriteLine(new string('=', 70));

                // 1. Informações de data e fuso
                DateTime systemToday = DateTime.Today;
                DateTime systemNow = DateTime.Now;
                DateTime dataServidor = ObterDataAtualServidor();
                TimeZoneInfo timezone = TimeZoneInfo.Local;

                Console.WriteLine($"\n📊 INFORMAÇÕES DE DATA E FUSO:");
                Console.WriteLine($"   • Sistema (Today): {systemToday:dd/MM/yyyy}");
                Console.WriteLine($"   • Sistema (Now): {systemNow:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"   • Servidor SQL: {dataServidor:dd/MM/yyyy}");
                Console.WriteLine($"   • Fuso horário: {timezone.DisplayName}");
                Console.WriteLine($"   • UTC Offset: {timezone.BaseUtcOffset}");

                // 2. Verificar consistência
                if (systemToday != dataServidor)
                {
                    Console.WriteLine($"\n⚠️  ALERTA: Data do sistema diferente do servidor!");
                    Console.WriteLine($"   Diferença: {(systemToday - dataServidor).Days} dias");
                }
                else
                {
                    Console.WriteLine($"\n✅ Datas consistentes entre sistema e servidor");
                }

                // 3. Verificar tarefas para hoje
                Console.WriteLine($"\n🔍 TAREFAS PARA HOJE ({dataServidor:dd/MM/yyyy}):");

                var tarefasHoje = ObterTarefasComAlarmeHoje(usuarioId);
                if (tarefasHoje.Any())
                {
                    foreach (var tarefa in tarefasHoje)
                    {
                        Console.WriteLine($"   📌 {tarefa.Descricao} (ID: {tarefa.Codigo})");
                    }
                    Console.WriteLine($"\n✅ Total: {tarefasHoje.Count} tarefas para hoje");
                }
                else
                {
                    Console.WriteLine($"   ❌ Nenhuma tarefa com alarme para hoje");
                }

                Console.WriteLine("\n" + new string('=', 70));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no diagnóstico: {ex.Message}");
            }
        }

        // =========================================================================
        // MÉTODOS DE DEBUG PARA ALARMES
        // =========================================================================

        public void DebugAlarmesFiltro(int usuarioId, string contexto)
        {
            try
            {
                Console.WriteLine($"\n=== DEBUG ALARMES - {contexto} ===");

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            A.Codigo as AlarmeCodigo,
                            A.CodTarefa,
                            A.Data as AlarmeData,
                            A.Hora as AlarmeHora,
                            PT.Descricao as TarefaDescricao,
                            P.Nome as ProjetoNome
                        FROM Alarme A
                        INNER JOIN Projeto_Tarefas PT ON A.CodTarefa = PT.Codigo
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        INNER JOIN Projeto P ON PT.CodProjeto = P.Codigo
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.isConcluida = 0
                        ORDER BY A.Data ASC, A.Hora ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read())
                            {
                                DateTime dataAlarme = Convert.ToDateTime(reader["AlarmeData"]);
                                TimeSpan horaAlarme = (TimeSpan)reader["AlarmeHora"];
                                string tarefaDesc = reader["TarefaDescricao"].ToString();
                                string projetoNome = reader["ProjetoNome"].ToString();

                                Console.WriteLine($"   {++count}. {tarefaDesc} | Projeto: {projetoNome}");
                                Console.WriteLine($"      📅 Data: {dataAlarme:dd/MM/yyyy} | ⏰ Hora: {horaAlarme:hh\\:mm}");
                            }

                            if (count == 0)
                            {
                                Console.WriteLine($"   ℹ️  Nenhum alarme encontrado para o usuário {usuarioId}");
                            }
                            else
                            {
                                Console.WriteLine($"   📊 Total de alarmes: {count}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no debug de alarmes: {ex.Message}");
            }
        }

        public void DebugAlarmesPeriodoEspecifico(int usuarioId, DateTime inicio, DateTime fim, string periodo)
        {
            try
            {
                Console.WriteLine($"\n=== DEBUG ALARMES {periodo.ToUpper()} ===");
                Console.WriteLine($"   📅 Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}");

                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            A.Codigo as AlarmeCodigo,
                            A.CodTarefa,
                            A.Data as AlarmeData,
                            A.Hora as AlarmeHora,
                            PT.Descricao as TarefaDescricao,
                            P.Nome as ProjetoNome
                        FROM Alarme A
                        INNER JOIN Projeto_Tarefas PT ON A.CodTarefa = PT.Codigo
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        INNER JOIN Projeto P ON PT.CodProjeto = P.Codigo
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
                            int count = 0;
                            while (reader.Read())
                            {
                                DateTime dataAlarme = Convert.ToDateTime(reader["AlarmeData"]);
                                TimeSpan horaAlarme = (TimeSpan)reader["AlarmeHora"];
                                string tarefaDesc = reader["TarefaDescricao"].ToString();
                                string projetoNome = reader["ProjetoNome"].ToString();

                                Console.WriteLine($"   {++count}. {tarefaDesc}");
                                Console.WriteLine($"      📁 Projeto: {projetoNome}");
                                Console.WriteLine($"      📅 Data: {dataAlarme:dd/MM/yyyy} | ⏰ Hora: {horaAlarme:hh\\:mm}");
                            }

                            if (count == 0)
                            {
                                Console.WriteLine($"   ❌ Nenhum alarme encontrado no período especificado");
                            }
                            else
                            {
                                Console.WriteLine($"   ✅ Total no período: {count} alarmes");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no debug de período: {ex.Message}");
            }
        }

        // =========================================================================
        // MÉTODOS ORIGINAIS (MANTIDOS PARA COMPATIBILIDADE)
        // =========================================================================

        public bool InserirTarefa(Projeto_Tarefas tarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Projeto_Tarefas (Descricao, isConcluida, CodProjeto, CodUsuario) 
                                 VALUES (@Descricao, @isConcluida, @CodProjeto, @CodUsuario);
                                 SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@CodProjeto", tarefa.CodProjeto);
                        cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario ?? (object)DBNull.Value);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            tarefa.Codigo = Convert.ToInt32(result);
                        }

                        Mensagem = "Tarefa inserida com sucesso";

                        int proprietarioId = _projetosDB.ObterProprietarioProjeto(tarefa.CodProjeto);
                        string nomeProjeto = _projetosDB.ObterNomeProjeto(tarefa.CodProjeto);

                        var notificacao = new Notificacoes
                        {
                            Texto = $"A tarefa '{tarefa.Descricao}' foi criada no projeto {nomeProjeto}",
                            Data = DateTime.Now,
                            CodProjeto = tarefa.CodProjeto,
                            CodUsuario = proprietarioId,
                            isFechada = false
                        };

                        _notificacoesDB.InserirNotificacao(notificacao);

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
                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
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

        public bool CriarTarefaCompartilhada(Projeto_Tarefas tarefa, int usuarioCriador)
        {
            try
            {
                if (!_membrosDB.EhMembroDoProjeto(usuarioCriador, tarefa.CodProjeto))
                {
                    Mensagem = "Você não tem permissão para criar tarefas neste projeto.";
                    return false;
                }

                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Projeto_Tarefas (Descricao, isConcluida, CodProjeto, CodUsuario) 
                                 VALUES (@Descricao, @isConcluida, @CodProjeto, @CodUsuario);
                                 SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                        cmd.Parameters.AddWithValue("@isConcluida", tarefa.isConcluida);
                        cmd.Parameters.AddWithValue("@CodProjeto", tarefa.CodProjeto);
                        cmd.Parameters.AddWithValue("@CodUsuario", tarefa.CodUsuario ?? (object)DBNull.Value);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            tarefa.Codigo = Convert.ToInt32(result);
                        }

                        Mensagem = "Tarefa criada com sucesso!";
                        _notificacoesDB.NotificarMembrosNovaTarefa(tarefa, usuarioCriador, _membrosDB, _projetosDB, _usuarioDB);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao criar tarefa: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarStatusTarefa(int tarefaId, bool isConcluida)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto_Tarefas SET isConcluida = @isConcluida WHERE Codigo = @tarefaId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@isConcluida", isConcluida);
                        cmd.Parameters.AddWithValue("@tarefaId", tarefaId);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Status da tarefa atualizado com sucesso.";

                        if (isConcluida)
                        {
                            string sqlInfo = @"SELECT pt.Descricao, pt.CodProjeto, p.CodUsuario 
                                             FROM Projeto_Tarefas pt 
                                             INNER JOIN Projeto p ON pt.CodProjeto = p.Codigo 
                                             WHERE pt.Codigo = @tarefaId";
                            using (SqlCommand cmdInfo = new SqlCommand(sqlInfo, conn))
                            {
                                cmdInfo.Parameters.AddWithValue("@tarefaId", tarefaId);
                                using (SqlDataReader reader = cmdInfo.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string descricaoTarefa = reader["Descricao"].ToString();
                                        int codProjeto = Convert.ToInt32(reader["CodProjeto"]);
                                        int proprietarioId = Convert.ToInt32(reader["CodUsuario"]);
                                        string nomeProjeto = _projetosDB.ObterNomeProjeto(codProjeto);

                                        var notificacao = new Notificacoes
                                        {
                                            Texto = $"{Sessao.UsuarioLogado.Nome} completou a tarefa '{descricaoTarefa}' no projeto {nomeProjeto}",
                                            Data = DateTime.Now,
                                            CodProjeto = codProjeto,
                                            CodUsuario = proprietarioId,
                                            isFechada = false
                                        };

                                        _notificacoesDB.InserirNotificacao(notificacao);
                                    }
                                }
                            }
                        }

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

        public List<Projeto_Tarefas> ObterTarefasPorProjeto(int projetoId)
        {
            List<Projeto_Tarefas> tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario FROM Projeto_Tarefas WHERE CodProjeto = @projetoId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
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
                    string query = "SELECT Codigo, Descricao, isConcluida, CodProjeto, CodUsuario FROM Projeto_Tarefas WHERE CodProjeto = @projetoId AND isConcluida = @isConcluida ORDER BY Codigo DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@projetoId", projetoId);
                        cmd.Parameters.AddWithValue("@isConcluida", isConcluida);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tarefas.Add(new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
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

        public bool AtribuirTarefaUsuario(int codTarefa, string codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto_Tarefas 
                         SET CodUsuario = @CodUsuario 
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



    }
}