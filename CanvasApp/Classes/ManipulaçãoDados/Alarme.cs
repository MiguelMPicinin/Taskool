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

        // CORREÇÃO: Método para converter objeto para TimeSpan de forma segura
        private TimeSpan ConverterParaTimeSpan(object valor)
        {
            try
            {
                if (valor == null || valor == DBNull.Value)
                    return TimeSpan.Zero;

                if (valor is DateTime)
                    return ((DateTime)valor).TimeOfDay;

                if (valor is TimeSpan)
                    return (TimeSpan)valor;

                if (TimeSpan.TryParse(valor.ToString(), out TimeSpan resultado))
                    return resultado;

                return TimeSpan.Zero;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public string ObterDescricaoPrazo(DateTime data)
        {
            try
            {
                TimeSpan diferenca = data.Date - DateTime.Today;
                if (diferenca.Days == 0)
                    return "Hoje";
                else if (diferenca.Days == 1)
                    return "Amanhã";
                else if (diferenca.Days > 1 && diferenca.Days <= 7)
                    return $"Em {diferenca.Days} dias";
                else
                    return data.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter descrição do prazo: " + ex.Message;
                return string.Empty;
            }
        }

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
                        cmd.Parameters.AddWithValue("@Hora", alarme.Hora);
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
                        cmd.Parameters.AddWithValue("@Hora", alarme.Hora);
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
                                // CORREÇÃO: Usar método de conversão seguro
                                TimeSpan hora = ConverterParaTimeSpan(reader["Hora"]);

                                return new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = hora,
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString())
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter alarme: " + ex.Message;
                return null;
            }
        }

        public List<Alarme> ObterAlarmesAtivos()
        {
            List<Alarme> alarmes = new List<Alarme>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Alarme WHERE Data >= @DataAtual";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@DataAtual", DateTime.Today);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // CORREÇÃO: Usar método de conversão seguro
                                TimeSpan hora = ConverterParaTimeSpan(reader["Hora"]);

                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = hora,
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
            DateTime dataHoraAlarme = alarme.Data.Date.Add(alarme.Hora);
            return dataHoraAlarme <= agora && dataHoraAlarme.AddMinutes(1) >= agora;
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

        // CORREÇÃO: Método original com int como CodUsuario
        public bool DefinirPrazoELembrete(int codTarefa, int codUsuario, DateTime data, TimeSpan hora, RepeticaoAlarme repeticao)
        {
            try
            {
                // CORREÇÃO: Validar se a data está dentro dos limites do SQL Server
                if (data < new DateTime(1753, 1, 1))
                {
                    Mensagem = "Data inválida para o SQL Server. Use uma data posterior a 01/01/1753.";
                    return false;
                }

                using (SqlConnection conn = GetConnection())
                {
                    string checkSql = "SELECT Codigo FROM Alarme WHERE CodTarefa = @CodTarefa";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        var result = checkCmd.ExecuteScalar();
                        bool existe = result != null && result != DBNull.Value;

                        if (existe)
                        {
                            int codAlarme = Convert.ToInt32(result);
                            string updateSql = @"UPDATE Alarme SET Data = @Data, Hora = @Hora, Repeticao = @Repeticao, CodUsuario = @CodUsuario 
                                       WHERE Codigo = @CodAlarme";
                            using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Data", data);
                                updateCmd.Parameters.AddWithValue("@Hora", hora);
                                updateCmd.Parameters.AddWithValue("@Repeticao", repeticao.ToString());
                                updateCmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                                updateCmd.Parameters.AddWithValue("@CodAlarme", codAlarme);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string insertSql = @"INSERT INTO Alarme (CodTarefa, CodUsuario, Data, Hora, Repeticao) 
                                       VALUES (@CodTarefa, @CodUsuario, @Data, @Hora, @Repeticao)";
                            using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                                insertCmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                                insertCmd.Parameters.AddWithValue("@Data", data);
                                insertCmd.Parameters.AddWithValue("@Hora", hora);
                                insertCmd.Parameters.AddWithValue("@Repeticao", repeticao.ToString());
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        Mensagem = "Prazo e alarme salvos com sucesso!";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao definir prazo e lembrete: " + ex.Message;
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
                                // CORREÇÃO: Usar método de conversão seguro
                                TimeSpan hora = ConverterParaTimeSpan(reader["Hora"]);

                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = hora,
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

        public List<Alarme> ObterAlarmesProximos(int usuarioId, int dias = 7)
        {
            List<Alarme> alarmes = new List<Alarme>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"
                                    SELECT A.*, PT.Descricao as DescricaoTarefa 
                                    FROM Alarme A 
                                    INNER JOIN Projeto_Tarefas PT ON A.CodTarefa = PT.Codigo 
                                    INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto 
                                    WHERE PM.CodMembro = @usuarioId 
                                    AND A.Data BETWEEN @hoje AND @dataFutura 
                                    AND PT.isConcluida = 0 
                                    ORDER BY A.Data ASC, A.Hora ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@hoje", DateTime.Today);
                        cmd.Parameters.AddWithValue("@dataFutura", DateTime.Today.AddDays(dias));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TimeSpan hora = ConverterParaTimeSpan(reader["Hora"]);

                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = hora,
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString()),
                                });
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Mensagem = "Erro ao obter alarmes próximos: " + ex.Message;
            }
            return alarmes;
        }

        public bool VerificarAlarmeExistente(int codTarefa)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Alarme WHERE CodTarefa = @CodTarefa";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Mensagem = "Erro ao verificar alarme: " + ex.Message;
                return false;
            }
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