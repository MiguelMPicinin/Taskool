using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CanvasApp.Classes.Databases
{
    public class MembrosDB : BaseDB
    {
        // CONSTRUTORES CORRIGIDOS
        public MembrosDB() { }

        public MembrosDB(NotificacoesDB notificacoesDB, ProjetosDB projetosDB, UsuarioDB usuarioDB)
        {
            // Inicialização das dependências se necessário
        }

        public bool EhMembroDoProjeto(int usuarioId, int projetoId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Projeto_Membros 
                                WHERE CodProjeto = @CodProjeto AND CodMembro = @CodMembro";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", projetoId);
                        cmd.Parameters.AddWithValue("@CodMembro", usuarioId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao verificar membro do projeto: " + ex.Message;
                return false;
            }
        }

        public bool RemoverMembroProjeto(int codProjeto, string codUsuario)
        {
            try
            {
                // CORREÇÃO: Converter string para int
                if (!int.TryParse(codUsuario, out int usuarioId))
                {
                    Mensagem = "Código de usuário inválido.";
                    return false;
                }

                using (SqlConnection conn = GetConnection())
                {
                    string sql = "DELETE FROM Projeto_Membros WHERE CodProjeto = @CodProjeto AND CodMembro = @CodMembro";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmd.Parameters.AddWithValue("@CodMembro", usuarioId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Mensagem = "Membro removido com sucesso!";
                            return true;
                        }
                        else
                        {
                            Mensagem = "Membro não encontrado no projeto.";
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao remover membro: " + ex.Message;
                return false;
            }
        }

        public List<Usuario> ObterMembrosProjeto(int codProjeto)
        {
            List<Usuario> membros = new List<Usuario>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"
                        SELECT U.Codigo, U.Nome, U.Email, U.NomeUsuario, U.DataNascimento, U.Telefone
                        FROM Usuario U
                        INNER JOIN Projeto_Membros PM ON U.Codigo = PM.CodMembro
                        WHERE PM.CodProjeto = @CodProjeto
                        ORDER BY U.Nome";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                membros.Add(new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    NomeUsuario = reader["NomeUsuario"].ToString(),
                                    DataNascimento = reader["DataNascimento"].ToString(),
                                    Telefone = reader["Telefone"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter membros do projeto: " + ex.Message;
            }
            return membros;
        }

        public bool AdicionarMembroAoProjeto(int codProjeto, int codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Verificar se o membro já existe
                    string sqlVerificar = @"SELECT COUNT(*) FROM Projeto_Membros 
                                          WHERE CodProjeto = @CodProjeto AND CodMembro = @CodMembro";
                    using (SqlCommand cmdVerificar = new SqlCommand(sqlVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmdVerificar.Parameters.AddWithValue("@CodMembro", codUsuario);
                        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (existe > 0)
                        {
                            Mensagem = "Usuário já é membro deste projeto.";
                            return false;
                        }
                    }

                    // Adicionar o membro
                    string sqlInserir = @"INSERT INTO Projeto_Membros (CodProjeto, CodMembro) 
                                        VALUES (@CodProjeto, @CodMembro)";
                    using (SqlCommand cmdInserir = new SqlCommand(sqlInserir, conn))
                    {
                        cmdInserir.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmdInserir.Parameters.AddWithValue("@CodMembro", codUsuario);
                        cmdInserir.ExecuteNonQuery();
                    }

                    Mensagem = "Membro adicionado com sucesso!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao adicionar membro: " + ex.Message;
                return false;
            }
        }

        public bool CompartilharProjeto(int codProjeto, int codUsuarioProprietario, string emailUsuarioCompartilhar)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Verificar se o usuário existe
                    string sqlVerificarUsuario = "SELECT Codigo FROM Usuario WHERE Email = @Email";
                    int codUsuarioCompartilhar;

                    using (SqlCommand cmdVerificar = new SqlCommand(sqlVerificarUsuario, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Email", emailUsuarioCompartilhar);
                        var result = cmdVerificar.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            Mensagem = "Usuário não encontrado.";
                            return false;
                        }

                        codUsuarioCompartilhar = Convert.ToInt32(result);
                    }

                    // Verificar se já é membro
                    string sqlVerificarMembro = @"SELECT COUNT(*) FROM Projeto_Membros 
                                                WHERE CodProjeto = @CodProjeto AND CodMembro = @CodMembro";
                    using (SqlCommand cmdVerificar = new SqlCommand(sqlVerificarMembro, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmdVerificar.Parameters.AddWithValue("@CodMembro", codUsuarioCompartilhar);
                        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (existe > 0)
                        {
                            Mensagem = "Usuário já é membro deste projeto.";
                            return false;
                        }
                    }

                    // Adicionar como membro
                    string sqlInserir = @"INSERT INTO Projeto_Membros (CodProjeto, CodMembro) 
                                        VALUES (@CodProjeto, @CodMembro)";
                    using (SqlCommand cmdInserir = new SqlCommand(sqlInserir, conn))
                    {
                        cmdInserir.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        cmdInserir.Parameters.AddWithValue("@CodMembro", codUsuarioCompartilhar);
                        cmdInserir.ExecuteNonQuery();
                    }

                    Mensagem = "Projeto compartilhado com sucesso!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao compartilhar projeto: " + ex.Message;
                return false;
            }
        }

        public int ObterQuantidadeMembrosProjeto(int codProjeto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Projeto_Membros WHERE CodProjeto = @CodProjeto";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodProjeto", codProjeto);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar membros: " + ex.Message;
                return 0;
            }
        }

        public List<int> ObterProjetosDoUsuario(int codUsuario)
        {
            List<int> projetos = new List<int>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT CodProjeto FROM Projeto_Membros WHERE CodMembro = @CodMembro";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CodMembro", codUsuario);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projetos.Add(Convert.ToInt32(reader["CodProjeto"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter projetos do usuário: " + ex.Message;
            }
            return projetos;
        }
    }
}