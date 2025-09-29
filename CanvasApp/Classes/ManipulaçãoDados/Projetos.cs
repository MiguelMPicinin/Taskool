using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class ProjetosDB : BaseDB
    {
        public string ObterNomeProjeto(int codProjeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT Nome FROM Projeto WHERE Codigo = @codProjeto";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codProjeto", codProjeto);
                        return cmd.ExecuteScalar()?.ToString() ?? "Projeto";
                    }
                }
            }
            catch
            {
                return "Projeto";
            }
        }

        public bool InserirProjeto(Projetos projeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Projeto (Nome, CodUsuario, NaoPertube) VALUES (@Nome, @CodUsuario, @NaoPertube)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", projeto.Nome);
                        cmd.Parameters.AddWithValue("@CodUsuario", projeto.CodUsuario);
                        cmd.Parameters.AddWithValue("@NaoPertube", projeto.NaoPertube);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Projeto inserido com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir Projeto: " + ex.Message;
                return false;
            }
        }

        public bool CriarProjetoCompartilhado(Projetos projeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Inserir projeto
                    string sqlProjeto = @"INSERT INTO Projeto (Nome, CodUsuario, NaoPertube) 
                                          VALUES (@Nome, @CodUsuario, @NaoPertube);
                                          SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sqlProjeto, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", projeto.Nome);
                        cmd.Parameters.AddWithValue("@CodUsuario", projeto.CodUsuario);
                        cmd.Parameters.AddWithValue("@NaoPertube", projeto.NaoPertube);

                        object result = cmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            Mensagem = "Erro ao criar projeto.";
                            return false;
                        }

                        projeto.Codigo = Convert.ToInt32(result);
                    }

                    // Adicionar proprietário como membro do projeto
                    string sqlMembro = @"INSERT INTO Projeto_Membros (CodProjeto, CodMembro) VALUES (@CodProjeto, @CodMembro)";
                    using (SqlCommand cmdMembro = new SqlCommand(sqlMembro, conn))
                    {
                        cmdMembro.Parameters.AddWithValue("@CodProjeto", projeto.Codigo);
                        cmdMembro.Parameters.AddWithValue("@CodMembro", projeto.CodUsuario);
                        cmdMembro.ExecuteNonQuery();
                    }

                    Mensagem = "Projeto criado com sucesso e compartilhado com o proprietário.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao criar projeto compartilhado: " + ex.Message;
                return false;
            }
        }

        public List<Projetos> ObterProjetosPorUsuario(int usuarioId)
        {
            List<Projetos> projetos = new List<Projetos>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "SELECT Codigo, Nome, NaoPertube, CodUsuario FROM Projeto WHERE CodUsuario = @usuarioId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projetos.Add(new Projetos
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    NaoPertube = Convert.ToBoolean(reader["NaoPertube"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar projetos: " + ex.Message;
            }
            return projetos;
        }

        public List<Projetos> ObterProjetosCompartilhados(int usuarioId)
        {
            List<Projetos> projetos = new List<Projetos>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT P.Codigo, P.Nome, P.NaoPertube, P.CodUsuario
                        FROM Projeto P
                        INNER JOIN Projeto_Membros PM ON P.Codigo = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId AND P.CodUsuario != @usuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projetos.Add(new Projetos
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    NaoPertube = Convert.ToBoolean(reader["NaoPertube"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar projetos compartilhados: " + ex.Message;
            }
            return projetos;
        }

        public List<Projetos> ObterTodosProjetosUsuario(int usuarioId)
        {
            var projetos = new List<Projetos>();

            var projetosProprios = ObterProjetosPorUsuario(usuarioId);
            if (projetosProprios != null)
                projetos.AddRange(projetosProprios);

            var projetosCompartilhados = ObterProjetosCompartilhados(usuarioId);
            if (projetosCompartilhados != null)
                projetos.AddRange(projetosCompartilhados);

            return projetos;
        }

        public int ObterProprietarioProjeto(int codProjeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT CodUsuario FROM Projeto WHERE Codigo = @codProjeto";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codProjeto", codProjeto);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}