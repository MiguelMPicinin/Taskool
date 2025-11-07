using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Classes.ManipulaçãoDados
{
    public class RegraCondicional
    {
        public string Operador { get; set; }
        public double ValorComparacao { get; set; }
        public double DiasAdicionais { get; set; }

        public bool AplicaRegra(double media)
        {
            switch (Operador)
            {
                case "<": return media < ValorComparacao;
                case ">": return media > ValorComparacao;
                case "<=": return media <= ValorComparacao;
                case ">=": return media >= ValorComparacao;
                case "=": return Math.Abs(media - ValorComparacao) < 0.001;
                default: return false;
            }
        }
    }

    public class GerenciadorEsforco
    {
        private List<RegraCondicional> _regras;
        private string _caminhoArquivo = "esforcoDias.txt";

        public GerenciadorEsforco()
        {
            _regras = new List<RegraCondicional>();
            CarregarConfiguracoes();
        }

        public void CarregarConfiguracoes()
        {
            try
            {
                _regras.Clear();

                if (!File.Exists(_caminhoArquivo))
                {
                    CriarArquivoPadrao();
                    return;
                }

                var linhas = File.ReadAllLines(_caminhoArquivo);

                foreach (var linha in linhas)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    var partes = linha.Split(';');
                    if (partes.Length == 3 &&
                        double.TryParse(partes[1], out double valorComparacao) &&
                        double.TryParse(partes[2], out double diasAdicionais))
                    {
                        _regras.Add(new RegraCondicional
                        {
                            Operador = partes[0].Trim(),
                            ValorComparacao = valorComparacao,
                            DiasAdicionais = diasAdicionais
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}");
                CriarArquivoPadrao();
            }
        }

        private void CriarArquivoPadrao()
        {
            try
            {
                var configuracoesPadrao = new List<string>
                {
                    "<;2;5",
                    ">;13;10",
                    ">=;8;7",
                    "<=;3;2"
                };

                File.WriteAllLines(_caminhoArquivo, configuracoesPadrao);
                CarregarConfiguracoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar arquivo padrão: {ex.Message}");
            }
        }

        public double CalcularDiasEntrega(double mediaCartas)
        {
            double diasAdicionais = 0;

            foreach (var regra in _regras)
            {
                if (regra.AplicaRegra(mediaCartas))
                {
                    diasAdicionais += regra.DiasAdicionais;
                }
            }

            // Garantir pelo menos 1 dia
            return diasAdicionais > 0 ? diasAdicionais : 1;
        }

        public List<RegraCondicional> ObterRegras()
        {
            return new List<RegraCondicional>(_regras);
        }

        public void AtualizarRegras(List<RegraCondicional> novasRegras)
        {
            _regras = novasRegras;
            SalvarConfiguracoes();
        }

        private void SalvarConfiguracoes()
        {
            try
            {
                var linhas = _regras.Select(regra =>
                    $"{regra.Operador};{regra.ValorComparacao};{regra.DiasAdicionais}").ToArray();
                File.WriteAllLines(_caminhoArquivo, linhas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações: {ex.Message}");
            }
        }

        public string ObterCaminhoArquivo()
        {
            return Path.GetFullPath(_caminhoArquivo);
        }
    }
}