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

        public List<Projeto_Tarefas> ObterTarefasProjetosCompartilhados(int usuarioId)
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
                        WHERE PM.CodMembro = @usuarioId";

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
                Mensagem = "Erro ao buscar tarefas de projetos compartilhados: " + ex.Message;
            }
            return tarefas;
        }

        public List<Projeto_Tarefas> ObterTarefasPorPeriodo(int usuarioId, DateTime inicio, DateTime fim)
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
                        AND A.Data BETWEEN @inicio AND @fim
                        AND PT.isConcluida = 0
                        ORDER BY A.Data ASC, PT.Codigo DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);

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
                Mensagem = "Erro ao buscar tarefas por período: " + ex.Message;
                Console.WriteLine("ERRO em ObterTarefasPorPeriodo: " + ex.Message);
            }

            return tarefas;
        }

        public void DebugAlarmesPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            PT.Codigo as TarefaCodigo,
                            PT.Descricao as TarefaDescricao,
                            A.Data as AlarmeData,
                            A.Hora as AlarmeHora,
                            PT.CodUsuario,
                            PM.CodMembro,
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data BETWEEN @inicio AND @fim
                        ORDER BY A.Data";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine($"=== DEBUG ALARMES PERÍODO: {inicio:dd/MM/yyyy} - {fim:dd/MM/yyyy} ===");
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                Console.WriteLine($"Tarefa {count}: {reader["TarefaCodigo"]} - {reader["TarefaDescricao"]}");
                                Console.WriteLine($"  Alarme: {reader["AlarmeData"]:dd/MM/yyyy} {reader["AlarmeHora"]:HH:mm}");
                                Console.WriteLine($"  Usuário: {reader["CodUsuario"]}, Membro: {reader["CodMembro"]}, Concluída: {reader["isConcluida"]}");
                                Console.WriteLine("---");
                            }

                            if (count == 0)
                            {
                                Console.WriteLine("NENHUMA TAREFA ENCONTRADA com alarme neste período!");
                            }
                            else
                            {
                                Console.WriteLine($"Total encontrado: {count} tarefas");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no debug: " + ex.Message);
            }
        }

        public int ObterQuantidadeTarefasHoje(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime amanha = hoje.AddDays(1);
            return ObterTarefasPorPeriodo(usuarioId, hoje, amanha).Count;
        }

        public int ObterQuantidadeTarefasSemana(int usuarioId)
        {
            DateTime inicioSemana = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime fimSemana = inicioSemana.AddDays(7);
            return ObterTarefasPorPeriodo(usuarioId, inicioSemana, fimSemana).Count;
        }

        public int ObterQuantidadeTarefasMes(int usuarioId)
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            return ObterTarefasPorPeriodo(usuarioId, inicioMes, fimMes).Count;
        }

        public List<Projeto_Tarefas> BuscarTarefasPorTexto(string texto)
        {
            var lista = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Projeto_Tarefas WHERE Descricao LIKE @Texto";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Projeto_Tarefas
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
            catch (Exception)
            {
                Mensagem = "Erro ao buscar tarefas";
            }
            return lista;
        }

        public object ObterInformacoesCompletasTarefa(int codTarefa)
        {
            var tarefa = ObterTarefaPorCodigo(codTarefa);
            var alarme = _alarmeDB.ObterAlarmePorTarefa(codTarefa);
            var subtarefas = _subtarefasDB.ObterSubtarefasPorTarefa(codTarefa);
            var comentarios = _comentariosDB.ObterComentariosPorTarefa(codTarefa);
            var usuario = tarefa != null && !string.IsNullOrEmpty(tarefa.CodUsuario) ?
            _usuarioDB.ObterUsuarioPorCodigo(tarefa.CodUsuario) : null;

            return new
            {
                Tarefa = tarefa,
                Prazo = alarme != null ? alarme.Data : (DateTime?)null,
                DescricaoPrazo = alarme != null ? _alarmeDB.ObterDescricaoPrazo(alarme.Data) : "Sem data definida",
                Lembrete = alarme != null ? alarme.Hora : (DateTime?)null,
                Repeticao = alarme != null ? alarme.Repeticao : RepeticaoAlarme.N,
                Usuario = usuario,
                Subtarefas = subtarefas,
                Comentarios = comentarios
            };
        }

        public bool AtribuirUsuarioTarefa(int codTarefa, string codUsuario)
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
                        Mensagem = "Usuário atribuído com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atribuir usuário: " + ex.Message;
                return false;
            }
        }

        public List<Projeto_Tarefas> FiltrarTarefasUsuario(string codUsuario, string filtro)
        {
            var tarefas = ObterTarefasPorUsuario(codUsuario);
            var agora = DateTime.Now;

            switch (filtro.ToLower())
            {
                case "hoje":
                    return tarefas.FindAll(t =>
                    {
                        var alarme = _alarmeDB.ObterAlarmePorTarefa(t.Codigo);
                        return alarme != null && alarme.Data.Date == agora.Date;
                    });
                case "semana":
                    return tarefas.FindAll(t =>
                    {
                        var alarme = _alarmeDB.ObterAlarmePorTarefa(t.Codigo);
                        return alarme != null && alarme.Data >= agora && alarme.Data <= agora.AddDays(7);
                    });
                case "mês":
                    return tarefas.FindAll(t =>
                    {
                        var alarme = _alarmeDB.ObterAlarmePorTarefa(t.Codigo);
                        return alarme != null && alarme.Data.Month == agora.Month && alarme.Data.Year == agora.Year;
                    });
                default:
                    return tarefas;
            }
        }

        public Projeto_Tarefas ObterTarefaPorCodigo(int codigo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Projeto_Tarefas WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter tarefa: " + ex.Message;
            }
            return null;
        }

        private List<Projeto_Tarefas> ObterTarefasPorUsuario(string codUsuario)
        {
            var tarefas = new List<Projeto_Tarefas>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT PT.* FROM Projeto_Tarefas PT
                                 INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                                 WHERE PM.CodMembro = @CodUsuario OR PT.CodUsuario = @CodUsuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
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
                                    CodUsuario = reader["CodUsuario"]?.ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Mensagem = "Erro ao obter tarefas do usuário";
            }
            return tarefas;
        }

        public bool DefinirPrazoELembreteTarefa(int codTarefa, string codUsuario, DateTime prazo, DateTime horarioLembrete, RepeticaoAlarme repeticao)
        {
            try
            {
                // Converter código do usuário para int
                if (!int.TryParse(codUsuario, out int usuarioId))
                {
                    Mensagem = "Código de usuário inválido";
                    return false;
                }

                // DEBUG
                Console.WriteLine($"Definindo prazo - Tarefa: {codTarefa}, Usuario: {usuarioId}, " +
                                 $"Data: {prazo:dd/MM/yyyy}, Hora: {horarioLembrete:HH:mm}");

                return _alarmeDB.DefinirPrazoELembrete(codTarefa, usuarioId, prazo, horarioLembrete, repeticao);
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao definir prazo e lembrete: " + ex.Message;
                Console.WriteLine("ERRO TarefasDB.DefinirPrazoELembreteTarefa: " + ex.Message);
                return false;
            }
        }

        public bool ResetarConfiguracoesTarefa(int codTarefa)
        {
            return _alarmeDB.ResetarConfiguracoesTarefa(codTarefa);
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

        // =========================================================================
        // MÉTODOS PARA FILTRO POR ALARME - CORRIGIDOS
        // =========================================================================

        public List<Projeto_Tarefas> ObterTarefasComAlarmeHoje(int usuarioId)
        {
            return ObterTarefasComAlarmePorPeriodo(usuarioId, DateTime.Today, DateTime.Today.AddDays(1));
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeSemana(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);
            DateTime fimSemana = inicioSemana.AddDays(7);
            return ObterTarefasComAlarmePorPeriodo(usuarioId, inicioSemana, fimSemana);
        }

        public List<Projeto_Tarefas> ObterTarefasComAlarmeMes(int usuarioId)
        {
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            return ObterTarefasComAlarmePorPeriodo(usuarioId, inicioMes, fimMes);
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
                            A.Data as DataAlarme,
                            A.Hora as HoraAlarme
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
                                var tarefa = new Projeto_Tarefas
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Descricao = reader["Descricao"].ToString(),
                                    isConcluida = Convert.ToBoolean(reader["isConcluida"]),
                                    CodProjeto = Convert.ToInt32(reader["CodProjeto"]),
                                    CodUsuario = reader["CodUsuario"]?.ToString()
                                };

                                // Adicionar informações do alarme para debug
                                Console.WriteLine($"Tarefa {tarefa.Codigo}: {tarefa.Descricao} - " +
                                                $"Alarme: {Convert.ToDateTime(reader["DataAlarme"]):dd/MM/yyyy} " +
                                                $"{Convert.ToDateTime(reader["HoraAlarme"]):HH:mm}");

                                tarefas.Add(tarefa);
                            }
                        }
                    }
                }

                Console.WriteLine($"Encontradas {tarefas.Count} tarefas com alarme no período {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar tarefas com alarme: " + ex.Message;
                Console.WriteLine("ERRO em ObterTarefasComAlarmePorPeriodo: " + ex.Message);
            }

            return tarefas;
        }

        // Métodos para obter quantidades (usados no menu lateral)
        public int ObterQuantidadeTarefasComAlarmeHoje(int usuarioId)
        {
            return ObterTarefasComAlarmeHoje(usuarioId).Count;
        }

        public int ObterQuantidadeTarefasComAlarmeSemana(int usuarioId)
        {
            return ObterTarefasComAlarmeSemana(usuarioId).Count;
        }

        public int ObterQuantidadeTarefasComAlarmeMes(int usuarioId)
        {
            return ObterTarefasComAlarmeMes(usuarioId).Count;
        }

        // Método de debug
        public void DebugAlarmesFiltro(int usuarioId, string filtro)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            PT.Codigo as TarefaCodigo,
                            PT.Descricao as TarefaDescricao,
                            A.Data as AlarmeData,
                            A.Hora as AlarmeHora,
                            PT.isConcluida
                        FROM Projeto_Tarefas PT
                        INNER JOIN Alarme A ON PT.Codigo = A.CodTarefa
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND PT.isConcluida = 0
                        ORDER BY A.Data";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine($"=== DEBUG ALARMES ({filtro}) ===");
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                Console.WriteLine($"Tarefa {count}: {reader["TarefaCodigo"]} - {reader["TarefaDescricao"]}");
                                Console.WriteLine($"  Alarme: {Convert.ToDateTime(reader["AlarmeData"]):dd/MM/yyyy} {Convert.ToDateTime(reader["AlarmeHora"]):HH:mm}");
                                Console.WriteLine($"  Concluída: {reader["isConcluida"]}");
                                Console.WriteLine("---");
                            }

                            if (count == 0)
                            {
                                Console.WriteLine($"NENHUMA TAREFA ENCONTRADA com alarme para {filtro}!");
                            }
                            else
                            {
                                Console.WriteLine($"Total de tarefas com alarme: {count}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no debug de alarmes: " + ex.Message);
            }
        }
    }
}