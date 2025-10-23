using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases
{
    public enum RepeticaoAlarme
    {
        N = 0,  // Nunca repetir
        D = 1,  // Repetir todos os dias
        S = 2,  // Repetir toda semana (segunda-feira)
        M = 3   // Repetir todo mês (primeiro dia útil)
    }

    public class Alarme
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public int CodUsuario { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public RepeticaoAlarme Repeticao { get; set; }

        // Construtor padrão
        public Alarme()
        {
            Data = DateTime.Today;
            Hora = new TimeSpan(9, 0, 0);
            Repeticao = RepeticaoAlarme.N;
        }

        // Método para obter descrição da repetição - CORRIGIDO para C# 7.3
        public string ObterDescricaoRepeticao()
        {
            switch (Repeticao)
            {
                case RepeticaoAlarme.N:
                    return "Nunca repetir";
                case RepeticaoAlarme.D:
                    return "Repetir todos os dias";
                case RepeticaoAlarme.S:
                    return "Repetir toda semana";
                case RepeticaoAlarme.M:
                    return "Repetir todo mês";
                default:
                    return "Desconhecido";
            }
        }

        // Método para calcular próxima ocorrência baseada na repetição - CORRIGIDO para C# 7.3
        public DateTime CalcularProximaOcorrencia(DateTime dataBase)
        {
            switch (Repeticao)
            {
                case RepeticaoAlarme.D:
                    return CalcularProximoDiaUtil(dataBase.AddDays(1));
                case RepeticaoAlarme.S:
                    return CalcularProximaSegundaFeira(dataBase);
                case RepeticaoAlarme.M:
                    return CalcularPrimeiroDiaUtilMes(dataBase.AddMonths(1));
                case RepeticaoAlarme.N:
                default:
                    return dataBase; // Nunca repetir - retorna a mesma data
            }
        }

        // Calcular próximo dia útil (segunda a sexta)
        private DateTime CalcularProximoDiaUtil(DateTime data)
        {
            DateTime resultado = data;
            while (resultado.DayOfWeek == DayOfWeek.Saturday || resultado.DayOfWeek == DayOfWeek.Sunday)
            {
                resultado = resultado.AddDays(1);
            }
            return resultado;
        }

        // Calcular próxima segunda-feira
        private DateTime CalcularProximaSegundaFeira(DateTime data)
        {
            DateTime resultado = data.AddDays(1);
            while (resultado.DayOfWeek != DayOfWeek.Monday)
            {
                resultado = resultado.AddDays(1);
            }
            return resultado;
        }

        // Calcular primeiro dia útil do mês
        private DateTime CalcularPrimeiroDiaUtilMes(DateTime data)
        {
            DateTime primeiroDiaMes = new DateTime(data.Year, data.Month, 1);
            return CalcularProximoDiaUtil(primeiroDiaMes);
        }

        // Verificar se é dia útil (segunda a sexta)
        public static bool EhDiaUtil(DateTime data)
        {
            return data.DayOfWeek >= DayOfWeek.Monday && data.DayOfWeek <= DayOfWeek.Friday;
        }

        // Override do ToString para exibição amigável - CORRIGIDO para C# 7.3
        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy} às {1:hh\\:mm} - {2}", Data, Hora, ObterDescricaoRepeticao());
        }
    }
}