using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

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
                    conn.Open();
                    string sql = @"INSERT INTO Tarefas_Anexos (CodTarefa, NomeArquivo, Arquivo, DataUpload) 
                         VALUES (@CodTarefa, @NomeArquivo, @Arquivo, @DataUpload)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodTarefa", anexo.CodTarefa);
                        cmd.Parameters.AddWithValue("@NomeArquivo", anexo.NomeArquivo);

                        byte[] arquivoBytes = Convert.FromBase64String(anexo.Arquivo);
                        cmd.Parameters.AddWithValue("@Arquivo", arquivoBytes);

                        cmd.Parameters.AddWithValue("@DataUpload", anexo.DataUpload);

                        int result = cmd.ExecuteNonQuery();
                        Mensagem = "Anexo inserido com sucesso.";
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
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
                    conn.Open();
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
                                string arquivoBase64 = string.Empty;
                                if (!reader.IsDBNull(reader.GetOrdinal("Arquivo")))
                                {
                                    byte[] arquivoData = (byte[])reader["Arquivo"];
                                    arquivoBase64 = Convert.ToBase64String(arquivoData);
                                }

                                anexos.Add(new Tarefas_Anexos
                                {
                                    Codigo = reader.GetInt32(reader.GetOrdinal("Codigo")),
                                    CodTarefa = reader.GetInt32(reader.GetOrdinal("CodTarefa")),
                                    NomeArquivo = reader["NomeArquivo"].ToString(),
                                    Arquivo = arquivoBase64,
                                    DataUpload = reader.GetDateTime(reader.GetOrdinal("DataUpload"))
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
                    conn.Open();
                    string sql = @"DELETE FROM Tarefas_Anexos WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codAnexo);
                        int result = cmd.ExecuteNonQuery();
                        Mensagem = "Anexo excluído com sucesso";
                        return result > 0;
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
                    conn.Open();
                    string sql = @"SELECT Codigo, CodTarefa, NomeArquivo, Arquivo, DataUpload 
                                 FROM Tarefas_Anexos WHERE Codigo = @Codigo";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codAnexo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string arquivoBase64 = string.Empty;
                                if (!reader.IsDBNull(reader.GetOrdinal("Arquivo")))
                                {
                                    byte[] arquivoData = (byte[])reader["Arquivo"];
                                    arquivoBase64 = Convert.ToBase64String(arquivoData);
                                }

                                return new Tarefas_Anexos
                                {
                                    Codigo = reader.GetInt32(reader.GetOrdinal("Codigo")),
                                    CodTarefa = reader.GetInt32(reader.GetOrdinal("CodTarefa")),
                                    NomeArquivo = reader["NomeArquivo"].ToString(),
                                    Arquivo = arquivoBase64,
                                    DataUpload = reader.GetDateTime(reader.GetOrdinal("DataUpload"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter anexo: " + ex.Message;
            }
            return null;
        }

        public bool ValidarTipoArquivo(string nomeArquivo)
        {
            var extensoesPermitidas = new[] { ".txt", ".pdf", ".xlsx", ".xls", ".docx", ".doc", ".html", ".sql" };
            var extensao = Path.GetExtension(nomeArquivo).ToLower();
            return extensoesPermitidas.Contains(extensao);
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
                case ".docx": return "Documento Word";
                case ".doc": return "Documento Word";
                case ".html": return "Página Web";
                case ".sql": return "Script SQL";
                default: return "Arquivo";
            }
        }
    }
}