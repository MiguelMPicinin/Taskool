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
                                return new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = Convert.ToDateTime(reader["Hora"]),
                                    Repeticao = (RepeticaoAlarme)Enum.Parse(typeof(RepeticaoAlarme), reader["Repeticao"].ToString())
                                };
                            }
                            else
                            {
                                return null;
                            }
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
                                alarmes.Add(new Alarme
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    Data = Convert.ToDateTime(reader["Data"]),
                                    Hora = Convert.ToDateTime(reader["Hora"]),
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
            return alarme.Data.Date == agora.Date &&
                   alarme.Hora.Hour == agora.Hour &&
                   alarme.Hora.Minute == agora.Minute;
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
            if (prazo.Date == DateTime.Today) return "Para hoje";
            if (prazo.Date == DateTime.Today.AddDays(1)) return "Para amanhã";

            var diferenca = (prazo.Date - DateTime.Today).Days;
            if (diferenca <= 7) return $"Para {prazo.ToString("dddd", new System.Globalization.CultureInfo("pt-BR"))}";

            return $"Para {prazo:dd/MM/yyyy}";
        }

        public bool DefinirPrazoELembrete(int codTarefa, int codUsuario, DateTime prazo, DateTime horarioLembrete, RepeticaoAlarme repeticao)
        {
            try
            {
                var alarmeExistente = ObterAlarmePorTarefa(codTarefa);

                if (alarmeExistente != null)
                {
                    alarmeExistente.Data = prazo;
                    alarmeExistente.Hora = horarioLembrete;
                    alarmeExistente.Repeticao = repeticao;
                    return AtualizarAlarme(alarmeExistente);
                }
                else
                {
                    var novoAlarme = new Alarme
                    {
                        CodTarefa = codTarefa,
                        CodUsuario = codUsuario,
                        Data = prazo,
                        Hora = horarioLembrete,
                        Repeticao = repeticao
                    };
                    return InserirAlarme(novoAlarme);
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao definir prazo e lembrete: " + ex.Message;
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
                    alarme.Repeticao = RepeticaoAlarme.N;
                    alarme.Hora = new DateTime(alarme.Data.Year, alarme.Data.Month, alarme.Data.Day, 9, 0, 0);
                    return AtualizarAlarme(alarme);
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