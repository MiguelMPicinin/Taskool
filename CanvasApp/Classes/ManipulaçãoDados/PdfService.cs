using CanvasApp.Classes.Databases.UsuarioCL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Classes.Databases
{
    public class PdfService
    {
        // MÉTODO PARA EXPORTAR MEMBROS
        public void ExportarMembrosParaPdf(List<Usuario> membros, string nomeProjeto, int codProjeto)
        {
            try
            {
                Document document = new Document(PageSize.A4);
                string filePath = Path.Combine(Path.GetTempPath(),
                    $"Membros_Projeto_{nomeProjeto}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Cabeçalho
                AdicionarCabecalhoMembros(document, nomeProjeto, codProjeto);

                if (membros.Any())
                {
                    // Tabela de membros
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 3f, 3f, 2f });

                    AdicionarCabecalhoTabelaMembros(table);
                    AdicionarDadosMembros(table, membros);

                    document.Add(table);

                    // Estatísticas
                    AdicionarEstatisticasMembros(document, membros);
                }
                else
                {
                    Paragraph semMembros = new Paragraph("Nenhum membro encontrado neste projeto.",
                        FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.GRAY));
                    semMembros.Alignment = Element.ALIGN_CENTER;
                    semMembros.SpacingBefore = 50;
                    document.Add(semMembros);
                }

                document.Close();

                // Abrir o PDF
                System.Diagnostics.Process.Start(filePath);

                MessageBox.Show($"PDF exportado com sucesso!\n\nArquivo: {filePath}",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarCabecalhoMembros(Document document, string nomeProjeto, int codProjeto)
        {
            // Título principal
            Paragraph titulo = new Paragraph($"RELATÓRIO DE MEMBROS DO PROJETO",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            document.Add(titulo);

            // Nome do projeto
            Paragraph projeto = new Paragraph($"{nomeProjeto}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLUE));
            projeto.Alignment = Element.ALIGN_CENTER;
            projeto.SpacingAfter = 5;
            document.Add(projeto);

            // Código do projeto
            Paragraph codigo = new Paragraph($"Código do Projeto: {codProjeto}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY));
            codigo.Alignment = Element.ALIGN_CENTER;
            codigo.SpacingAfter = 10;
            document.Add(codigo);

            // Data de emissão
            Paragraph dataEmissao = new Paragraph($"Emitido em: {DateTime.Now:dd/MM/yyyy às HH:mm}",
                FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.LIGHT_GRAY));
            dataEmissao.Alignment = Element.ALIGN_RIGHT;
            dataEmissao.SpacingAfter = 20;
            document.Add(dataEmissao);
        }

        private void AdicionarCabecalhoTabelaMembros(PdfPTable table)
        {
            string[] cabecalhos = { "#", "Nome", "Usuário", "Status" };
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

        private void AdicionarDadosMembros(PdfPTable table, List<Usuario> membros)
        {
            for (int i = 0; i < membros.Count; i++)
            {
                var membro = membros[i];

                // Número
                table.AddCell(CriarCelula((i + 1).ToString(), Element.ALIGN_CENTER));

                // Nome completo
                table.AddCell(CriarCelula(membro.Nome ?? "N/A", Element.ALIGN_LEFT));

                // Nome de usuário
                table.AddCell(CriarCelula(membro.NomeUsuario ?? "N/A", Element.ALIGN_LEFT));

                // Status (sempre ativo para membros do projeto)
                table.AddCell(CriarCelulaComCor("Ativo", Element.ALIGN_CENTER, BaseColor.GREEN));
            }
        }

        private void AdicionarEstatisticasMembros(Document document, List<Usuario> membros)
        {
            int totalMembros = membros.Count;

            Paragraph estatisticas = new Paragraph("\n\nRESUMO:",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY));
            document.Add(estatisticas);

            string textoEstatisticas = $@"Total de Membros: {totalMembros}
Data da Exportação: {DateTime.Now:dd/MM/yyyy HH:mm}
Relatório gerado automaticamente pelo sistema Taskool";

            Paragraph dadosEstatisticas = new Paragraph(textoEstatisticas,
                FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK));
            dadosEstatisticas.SpacingBefore = 10;
            document.Add(dadosEstatisticas);
        }

        // Métodos auxiliares para criar células
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

        // MÉTODOS EXISTENTES PARA TAREFAS
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

                AdicionarCabecalhoTabelaTarefas(table);
                AdicionarDadosTarefas(table, tarefas);

                document.Add(table);
                AdicionarEstatisticasTarefas(document, tarefas);

                document.Close();
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarCabecalhoTabelaTarefas(PdfPTable table)
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

        private void AdicionarEstatisticasTarefas(Document document, List<Projeto_Tarefas> tarefas)
        {
            int totalTarefas = tarefas.Count;
            int concluidas = tarefas.Count(t => t.isConcluida);
            int pendentes = totalTarefas - concluidas;
            double percentualConcluidas = totalTarefas > 0 ? (concluidas * 100.0) / totalTarefas : 0;

            Paragraph estatisticas = new Paragraph("\n\nESTATÍSTICAS:",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY));
            document.Add(estatisticas);

            string textoEstatisticas = $@"Total de Tarefas: {totalTarefas}
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