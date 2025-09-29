using CanvasApp.Classes.Databases.UsuarioCL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Superpower;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CanvasApp.Classes.Databases
{
    public class PdfService
    {
        public void ExportarTarefasParaPdf(List<Projeto_Tarefas> tarefas, string usuarioNome)
        {
            try
            {
                Document document = new Document(PageSize.A4.Rotate());
                string filePath = Path.Combine(Path.GetTempPath(), $"Tarefas_{usuarioNome}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                Paragraph titulo = new Paragraph($"RELATÓRIO DE TAREFAS - {usuarioNome.ToUpper()}",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY));
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                Paragraph dataEmissao = new Paragraph($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY));
                dataEmissao.Alignment = Element.ALIGN_RIGHT;
                dataEmissao.SpacingAfter = 15;
                document.Add(dataEmissao);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 2f, 2f, 1.5f });

                AdicionarCabecalhoTabela(table);
                AdicionarDadosTarefas(table, tarefas);

                document.Add(table);
                AdicionarEstatisticas(document, tarefas);

                document.Close();
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarCabecalhoTabela(PdfPTable table)
        {
            string[] cabecalhos = { "Código", "Descrição", "Projeto", "Status", "Prazo" };
            BaseColor corCabecalho = new BaseColor(200, 200, 200);

            foreach (string cabecalho in cabecalhos)
            {
                PdfPCell cell = new PdfPCell(new Phrase(cabecalho,
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK)));
                cell.BackgroundColor = corCabecalho;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 8;
                table.AddCell(cell);
            }
        }

        private void AdicionarDadosTarefas(PdfPTable table, List<Projeto_Tarefas> tarefas)
        {
            var projetosDB = new ProjetosDB();
            var alarmeDB = new AlarmeDB();

            foreach (var tarefa in tarefas.OrderBy(t => t.isConcluida).ThenBy(t => t.Codigo))
            {
                table.AddCell(CriarCelula(tarefa.Codigo.ToString(), Element.ALIGN_CENTER));
                table.AddCell(CriarCelula(tarefa.Descricao, Element.ALIGN_LEFT));

                string nomeProjeto = projetosDB.ObterNomeProjeto(tarefa.CodProjeto);
                table.AddCell(CriarCelula(nomeProjeto, Element.ALIGN_LEFT));

                string status = tarefa.isConcluida ? "Concluída" : "Pendente";
                BaseColor corStatus = tarefa.isConcluida ? BaseColor.GREEN : BaseColor.ORANGE;
                table.AddCell(CriarCelulaComCor(status, Element.ALIGN_CENTER, corStatus));

                var alarme = alarmeDB.ObterAlarmePorTarefa(tarefa.Codigo);
                string prazo = alarme != null ? alarme.Data.ToString("dd/MM/yyyy") : "Sem prazo";
                table.AddCell(CriarCelula(prazo, Element.ALIGN_CENTER));
            }
        }

        private PdfPCell CriarCelula(string texto, int alinhamento)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto ?? "",
                FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK)));
            cell.HorizontalAlignment = alinhamento;
            cell.Padding = 5;
            cell.BorderWidth = 0.5f;
            return cell;
        }

        private PdfPCell CriarCelulaComCor(string texto, int alinhamento, BaseColor cor)
        {
            PdfPCell cell = CriarCelula(texto, alinhamento);
            cell.BackgroundColor = cor;
            return cell;
        }

        private void AdicionarEstatisticas(Document document, List<Projeto_Tarefas> tarefas)
        {
            int totalTarefas = tarefas.Count;
            int concluidas = tarefas.Count(t => t.isConcluida);
            int pendentes = totalTarefas - concluidas;
            double percentualConcluidas = totalTarefas > 0 ? (concluidas * 100.0) / totalTarefas : 0;

            Paragraph estatisticas = new Paragraph("\n\nESTATÍSTICAS:",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY));
            document.Add(estatisticas);

            string textoEstatisticas = $@"
Total de Tarefas: {totalTarefas}
Tarefas Concluídas: {concluidas}
Tarefas Pendentes: {pendentes}
Percentual de Conclusão: {percentualConcluidas:F1}%";

            Paragraph dadosEstatisticas = new Paragraph(textoEstatisticas,
                FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK));
            dadosEstatisticas.SpacingBefore = 10;
            document.Add(dadosEstatisticas);
        }

        public void ExportarTarefasUsuarioParaPdf(List<Projeto_Tarefas> tarefas, Usuario usuario, string filtro)
        {
            try
            {
                Document document = new Document(PageSize.A4);
                string filePath = Path.Combine(Path.GetTempPath(),
                    $"Tarefas_{usuario.NomeUsuario}_{filtro}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                AdicionarCabecalhoUsuario(document, usuario, filtro);

                if (tarefas.Any())
                {
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 4f, 2f, 1.5f });

                    string[] cabecalhos = { "#", "Tarefa", "Projeto", "Status" };
                    foreach (string cabecalho in cabecalhos)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(cabecalho,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                        cell.BackgroundColor = new BaseColor(200, 200, 200);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 6;
                        table.AddCell(cell);
                    }

                    for (int i = 0; i < tarefas.Count; i++)
                    {
                        var tarefa = tarefas[i];
                        var projetosDB = new ProjetosDB();

                        table.AddCell(CriarCelula((i + 1).ToString(), Element.ALIGN_CENTER));
                        table.AddCell(CriarCelula(tarefa.Descricao, Element.ALIGN_LEFT));
                        table.AddCell(CriarCelula(projetosDB.ObterNomeProjeto(tarefa.CodProjeto), Element.ALIGN_LEFT));

                        string status = tarefa.isConcluida ? "✓ Concluída" : "⏳ Pendente";
                        table.AddCell(CriarCelula(status, Element.ALIGN_CENTER));
                    }

                    document.Add(table);
                }
                else
                {
                    Paragraph semTarefas = new Paragraph("Nenhuma tarefa encontrada para o filtro selecionado.",
                        FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.GRAY));
                    semTarefas.Alignment = Element.ALIGN_CENTER;
                    semTarefas.SpacingBefore = 50;
                    document.Add(semTarefas);
                }

                document.Close();
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarCabecalhoUsuario(Document document, Usuario usuario, string filtro)
        {
            Paragraph titulo = new Paragraph($"RELATÓRIO DE TAREFAS - {usuario.Nome.ToUpper()}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            document.Add(titulo);

            Paragraph filtroParagraph = new Paragraph($"Período: {filtro}",
                FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY));
            filtroParagraph.Alignment = Element.ALIGN_CENTER;
            filtroParagraph.SpacingAfter = 5;
            document.Add(filtroParagraph);

            Paragraph data = new Paragraph($"Emitido em: {DateTime.Now:dd/MM/yyyy às HH:mm}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.LIGHT_GRAY));
            data.Alignment = Element.ALIGN_CENTER;
            data.SpacingAfter = 20;
            document.Add(data);
        }
    }
}