using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Media;
using System.Windows.Forms;

namespace CanvasApp.Classes.Databases
{
    public class AlarmeDB : BaseDB
    {
        private readonly Timer _timerVerificacao;

        public AlarmeDB()
        {
            _timerVerificacao = new Timer();
            _timerVerificacao.Interval = 60000;
            _timerVerificacao.Tick += VerificarAlarmes;
            _timerVerificacao.Start();
        }

        // =========================================================================
        // MÉTODOS CORRIGIDOS PARA FUSO HORÁRIO BRASIL
        // =========================================================================

        /// <summary>
        /// Obtém data atual do servidor SQL (Brasil UTC-3)
        /// </summary>
        private DateTime ObterDataAtualServidor()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT CAST(GETDATE() as DATE) as DataAtual";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime dataServidor = Convert.ToDateTime(reader["DataAtual"]);
                                Console.WriteLine($"✅ AlarmeDB - Data servidor: {dataServidor:dd/MM/yyyy}");
                                return dataServidor;
                            }
                        }
                    }
                }
                return DateTime.Today;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ AlarmeDB - Erro: {ex.Message}");
                return DateTime.Today;
            }
        }

        /// <summary>
        /// Converte hora do banco para DateTime considerando fuso Brasil
        /// </summary>
        private DateTime ConverterHoraBrasil(object horaValue)
        {
            try
            {
                DateTime dataBase = ObterDataAtualServidor();

                if (horaValue is TimeSpan timeSpan)
                {
                    return dataBase.Add(timeSpan);
                }
                else if (horaValue is DateTime dateTime)
                {
                    return dateTime;
                }
                else if (horaValue != DBNull.Value && horaValue != null)
                {
                    return Convert.ToDateTime(horaValue);
                }
                else
                {
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
        // MÉTODOS PRINCIPAIS CORRIGIDOS
        // =========================================================================

        public bool InserirAlarme(Alarme alarme)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Alarme (CodTarefa, CodUsuario, Data, Hora, Repeticao) 
                                 VALUES (@CodTarefa, @CodUsuario, @Data, @Hora, @Repeticao)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", alarme.CodTarefa);
                        cmd.Parameters.AddWithValue("@CodUsuario", alarme.CodUsuario);
                        cmd.Parameters.AddWithValue("@Data", alarme.Data);
                        cmd.Parameters.AddWithValue("@Hora", alarme.Hora.TimeOfDay);
                        cmd.Parameters.AddWithValue("@Repeticao", alarme.Repeticao.ToString());
                        cmd.ExecuteNonQuery();
                        Mensagem = "Alarme inserido com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir alarme: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarAlarme(Alarme alarme)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Alarme SET Data = @Data, Hora = @Hora, Repeticao = @Repeticao 
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Data", alarme.Data);
                        cmd.Parameters.AddWithValue("@Hora", alarme.Hora.TimeOfDay);
                        cmd.Parameters.AddWithValue("@Repeticao", alarme.Repeticao.ToString());
                        cmd.Parameters.AddWithValue("@Codigo", alarme.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Alarme atualizado com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar alarme: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirAlarme(int codAlarme)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "DELETE FROM Alarme WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codAlarme);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Alarme excluído com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir alarme: " + ex.Message;
                return false;
            }
        }

        public Alarme ObterAlarmePorTarefa(int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Alarme WHERE CodTarefa = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime horaDateTime = ConverterHoraBrasil(reader["Hora"]);

                                var alarme = new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = horaDateTime,
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString())
                                };

                                Console.WriteLine($"✅ Alarme encontrado: Tarefa {codTarefa}");
                                Console.WriteLine($"   Data: {alarme.Data:dd/MM/yyyy}, Hora: {alarme.Hora:HH:mm}");
                                return alarme;
                            }
                            else
                            {
                                Console.WriteLine($"ℹ️  Nenhum alarme para tarefa {codTarefa}");
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter alarme: " + ex.Message;
                Console.WriteLine($"❌ ERRO ObterAlarmePorTarefa: {ex.Message}");
                return null;
            }
        }

        public List<Alarme> ObterAlarmesAtivos()
        {
            List<Alarme> alarmes = new List<Alarme>();
            try
            {
                DateTime dataAtual = ObterDataAtualServidor();

                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Alarme WHERE Data >= @DataAtual";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@DataAtual", dataAtual);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime horaDateTime = ConverterHoraBrasil(reader["Hora"]);

                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = horaDateTime,
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter alarmes ativos: " + ex.Message;
            }
            return alarmes;
        }

        private void VerificarAlarmes(object sender, EventArgs e)
        {
            var agora = DateTime.Now;
            var alarmesAtivos = ObterAlarmesAtivos();

            foreach (var alarme in alarmesAtivos)
            {
                if (DeveDispararAlarme(alarme, agora))
                {
                    DispararNotificacao(alarme);
                    ProcessarProximoAlarme(alarme, agora);
                }
            }
        }

        private bool DeveDispararAlarme(Alarme alarme, DateTime agora)
        {
            // Combina data do alarme com hora do alarme
            DateTime dataHoraAlarme = alarme.Data.Date.Add(alarme.Hora.TimeOfDay);

            return dataHoraAlarme.Date == agora.Date &&
                   dataHoraAlarme.Hour == agora.Hour &&
                   dataHoraAlarme.Minute == agora.Minute;
        }

        private void DispararNotificacao(Alarme alarme)
        {
            MostrarNotificacaoSistema($"Lembrete de Tarefa", $"Hora do alarme para a tarefa #{alarme.CodTarefa}");
            TocarSomAlarme();
        }

        private void ProcessarProximoAlarme(Alarme alarme, DateTime agora)
        {
            switch (alarme.Repeticao)
            {
                case RepeticaoAlarme.D:
                    alarme.Data = alarme.Data.AddDays(1);
                    break;
                case RepeticaoAlarme.S:
                    alarme.Data = alarme.Data.AddDays(7);
                    break;
                case RepeticaoAlarme.M:
                    alarme.Data = ProximoDiaUtil(alarme.Data.AddMonths(1));
                    break;
                case RepeticaoAlarme.N:
                default:
                    ExcluirAlarme(alarme.Codigo);
                    return;
            }

            if (alarme.Data >= agora.Date)
            {
                AtualizarAlarme(alarme);
            }
            else
            {
                ExcluirAlarme(alarme.Codigo);
            }
        }

        private DateTime ProximoDiaUtil(DateTime data)
        {
            while (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
            {
                data = data.AddDays(1);
            }
            return data;
        }

        private void MostrarNotificacaoSistema(string titulo, string mensagem)
        {
            try
            {
                MessageBox.Show(mensagem, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao mostrar notificação: " + ex.Message);
            }
        }

        private void TocarSomAlarme()
        {
            try
            {
                SystemSounds.Exclamation.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao tocar som do alarme: " + ex.Message);
            }
        }

        public string ObterDescricaoPrazo(DateTime prazo)
        {
            DateTime hoje = ObterDataAtualServidor();

            if (prazo.Date == hoje) return "Para hoje";
            if (prazo.Date == hoje.AddDays(1)) return "Para amanhã";

            var diferenca = (prazo.Date - hoje).Days;
            if (diferenca <= 7) return $"Para {prazo.ToString("dddd", new System.Globalization.CultureInfo("pt-BR"))}";

            return $"Para {prazo:dd/MM/yyyy}";
        }

        public bool DefinirPrazoELembrete(int codTarefa, int codUsuario, DateTime prazo, DateTime horarioLembrete, RepeticaoAlarme repeticao)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Verifica se já existe alarme
                    string checkSql = "SELECT Codigo FROM Alarme WHERE CodTarefa = @CodTarefa";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        var result = checkCmd.ExecuteScalar();
                        bool existe = result != null && result != DBNull.Value;

                        if (existe)
                        {
                            int codAlarme = Convert.ToInt32(result);
                            // Atualiza alarme existente
                            string updateSql = @"UPDATE Alarme SET Data = @Data, Hora = @Hora, Repeticao = @Repeticao, CodUsuario = @CodUsuario 
                                               WHERE Codigo = @CodAlarme";
                            using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Data", prazo.Date);
                                updateCmd.Parameters.AddWithValue("@Hora", horarioLembrete.TimeOfDay);
                                updateCmd.Parameters.AddWithValue("@Repeticao", repeticao.ToString());
                                updateCmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                                updateCmd.Parameters.AddWithValue("@CodAlarme", codAlarme);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Insere novo alarme
                            string insertSql = @"INSERT INTO Alarme (CodTarefa, CodUsuario, Data, Hora, Repeticao) 
                                               VALUES (@CodTarefa, @CodUsuario, @Data, @Hora, @Repeticao)";
                            using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                insertCmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                                insertCmd.Parameters.AddWithValue("@Data", prazo.Date);
                                insertCmd.Parameters.AddWithValue("@Hora", horarioLembrete.TimeOfDay);
                                insertCmd.Parameters.AddWithValue("@Repeticao", repeticao.ToString());
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        Mensagem = "Prazo e alarme salvos com sucesso!";
                        Console.WriteLine($"✅ Alarme salvo: Tarefa={codTarefa}, Data={prazo:dd/MM/yyyy}, Hora={horarioLembrete:HH:mm}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao definir prazo e lembrete: " + ex.Message;
                Console.WriteLine("ERRO AlarmeDB.DefinirPrazoELembrete: " + ex.Message);
                return false;
            }
        }

        public List<Alarme> ObterAlarmesPorPeriodo(int usuarioId, DateTime inicio, DateTime fim)
        {
            List<Alarme> alarmes = new List<Alarme>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"
                        SELECT A.* 
                        FROM Alarme A
                        INNER JOIN Projeto_Tarefas PT ON A.CodTarefa = PT.Codigo
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId
                        AND A.Data >= @inicio AND A.Data < @fim
                        AND PT.isConcluida = 0
                        ORDER BY A.Data ASC, A.Hora ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@inicio", inicio);
                        cmd.Parameters.AddWithValue("@fim", fim);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime horaDateTime = ConverterHoraBrasil(reader["Hora"]);

                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = horaDateTime,
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter alarmes por período: " + ex.Message;
            }
            return alarmes;
        }

        public bool ResetarConfiguracoesTarefa(int codTarefa)
        {
            try
            {
                var alarme = ObterAlarmePorTarefa(codTarefa);
                if (alarme != null)
                {
                    return ExcluirAlarme(alarme.Codigo);
                }
                return true;
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao resetar configurações: " + ex.Message;
                return false;
            }
        }
    }
}