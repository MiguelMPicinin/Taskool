using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CanvasApp.Classes.Databases
{
    public class ProjetosDB : BaseDB
    {
        // MÉTODO ADICIONADO PARA CORRIGIR O ERRO
        public bool CriarProjetoCompartilhado(Projetos projeto)
        {
            return InserirProjeto(projeto);
        }

        public List<Projetos> ObterProjetosPorUsuario(int usuarioId)
        {
            List<Projetos> projetos = new List<Projetos>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "SELECT Codigo, Nome, CodUsuario, NaoPertube FROM Projeto WHERE CodUsuario = @usuarioId";
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

        public Projetos ObterProjetoPorCodigo(int codigo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT Codigo, Nome, CodUsuario, NaoPertube FROM Projeto WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Projetos
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    CodUsuario = Convert.ToInt32(reader["CodUsuario"]),
                                    NaoPertube = Convert.ToBoolean(reader["NaoPertube"])
                                };
                            }
                            else
                            {
                                Mensagem = "Projeto não encontrado.";
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter projeto: " + ex.Message;
                return null;
            }
        }

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
                    string sql = @"INSERT INTO Projeto (Nome, CodUsuario, NaoPertube) 
                                 VALUES (@Nome, @CodUsuario, @NaoPertube)";
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

        public bool AtualizarProjeto(Projetos projeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Projeto SET Nome = @Nome, NaoPertube = @NaoPertube 
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", projeto.Nome);
                        cmd.Parameters.AddWithValue("@NaoPertube", projeto.NaoPertube);
                        cmd.Parameters.AddWithValue("@Codigo", projeto.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Projeto atualizado com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar projeto: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirProjeto(int codProjeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Primeiro excluir tarefas e membros associados
                    string sqlTarefas = "DELETE FROM Projeto_Tarefas WHERE CodProjeto = @CodProjeto";
                    string sqlMembros = "DELETE FROM Projeto_Membros WHERE CodProjeto = @CodProjeto";
                    string sqlProjeto = "DELETE FROM Projeto WHERE Codigo = @CodProjeto";

                    using (SqlCommand cmd = new SqlCommand(sqlTarefas, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlMembros, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlProjeto, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmd.ExecuteNonQuery();
                    }

                    Mensagem = "Projeto excluído com sucesso.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir projeto: " + ex.Message;
                return false;
            }
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

            return projetos.OrderBy(p => p.Nome).ToList();
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
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public int ObterQuantidadeProjetosUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"
                        SELECT COUNT(DISTINCT P.Codigo)
                        FROM Projeto P
                        INNER JOIN Projeto_Membros PM ON P.Codigo = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar projetos: " + ex.Message;
                return 0;
            }
        }
    }
}