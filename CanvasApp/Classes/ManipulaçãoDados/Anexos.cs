using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace CanvasApp.Classes.ManipulaçãoDados
{
    public class AnexosDB : BaseDB
    {
        public bool InserirAnexo(Tarefas_Anexos anexo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Tarefas_Anexos (CodTarefa, NomeArquivo, Arquivo, DataUpload) VALUES (@CodTarefa, @NomeArquivo, @Arquivo, @DataUpload)";
                    using (SqlCommand cmd = new SqlCommand(sql,conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", anexo.CodTarefa);
                        cmd.Parameters.AddWithValue("@NomeArquivo", anexo.NomeArquivo);
                        cmd.Parameters.AddWithValue("@Arquivo", anexo.Arquivo);
                        cmd.Parameters.AddWithValue("@DataUpload", anexo.DataUpload);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Anexo inserido com sucesso.";
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Mensagem = "Erro ao inserir anexo: " + ex.Message;
                return false;
            }
        }

        public List<Tarefas_Anexos> ListarAnexosPorTarefa(int codTarefa)
        {
            List<Tarefas_Anexos> anexos = new List<Tarefas_Anexos>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT Codigo, CodTarefa, NomeArquivo, Arquivo, DataUpload
                                 FROM Tarefas_Anexos 
                                 WHERE CodTarefa = @CodTarefa
                                 ORDER BY DataUpload DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", codTarefa);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] arquivoData = null;
                                var arquivoValue = reader["Arquivo"];
                                if (arquivoValue != DBNull.Value)
                                {
                                    arquivoData = (byte[])arquivoValue;
                                }
                                anexos.Add(new Tarefas_Anexos
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    NomeArquivo = reader["NomeArquivo"].ToString(),
                                    Arquivo = arquivoData.ToString(),
                                    DataUpload = Convert.ToDateTime(reader["DataUpload"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao listar anexos: " + ex.Message;
            }
            return anexos;
        }

        public bool ExcluirAnexo(int codAnexo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"DELETE FROM Tarefas_Anexos WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codAnexo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Anexo excluido com sucesso";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir anexo: " + ex.Message;
                return false;
            }
        }

        public Tarefas_Anexos ObterAnexoPorCodigo(int codAnexo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT Codigo, CodTarefa, NomeArquivo, Arquivo, DataUpload FROM Tarefas_Anexos WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand (sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codAnexo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] arquivoData = null;
                                var arquivoValue = reader["Arquivo"];
                                if (arquivoValue != DBNull.Value)
                                {
                                    arquivoData = (byte[])arquivoValue;
                                }

                                return new Tarefas_Anexos
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    CodTarefa = Convert.ToInt32(reader["CodTarefa"]),
                                    NomeArquivo = reader["Nome"].ToString(),
                                    Arquivo = arquivoData.ToString(),
                                    DataUpload = Convert.ToDateTime(reader["DataUpload"])
                                };
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Mensagem = "Erro ao obter anexo: " + ex.Message;
            }
            return null;
        }

        public bool ValidarTipoArquivo(string nomeArquivo)
        {
            var extensoesPermitidas = new[] { ".txt", ".pdf", ".xlsx", ".xls", ".docx", ".doc", ".html", ".sql" };
            var extensao = Path.GetExtension(nomeArquivo).ToLower();
            return Array.Exists(extensoesPermitidas, ext => ext == extensao);
        }

        public string ObterDescricaoTipoArquivo(string nomeArquivo)
        {
            var extensao = Path.GetExtension(nomeArquivo).ToLower();
            switch (extensao)
            {
                case ".txt": return "Arquivo de Texto";
                case ".pdf": return "Documento PDF";
                case ".xlsx": return "Planilha Excel";
                case ".xls": return "Planilha Excel";
                case ".docx": return "Documento World";
                case ".doc": return "Documento World";
                case ".html": return "Página Web";
                case ".sql": return "Script SQL";
                default: return "Arquivo";
            }
        }
    }
}