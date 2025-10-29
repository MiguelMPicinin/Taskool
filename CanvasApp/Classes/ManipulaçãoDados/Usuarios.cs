using CanvasApp.Classes.Databases.UsuarioCL;
using CanvasApp.Classes.Errors;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CanvasApp.Classes.Databases
{
    public class UsuarioDB : BaseDB
    {
        public string ObterNomeUsuario(int codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT Nome FROM Usuario WHERE Codigo = @codUsuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@codUsuario", codUsuario);
                        return cmd.ExecuteScalar()?.ToString() ?? "Usuário";
                    }
                }
            }
            catch
            {
                return "Usuário";
            }
        }

        public bool InserirUsuario(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"INSERT INTO Usuario (Nome, Email, NomeUsuario, DataNascimento, Telefone, Foto)
                                 VALUES (@Nome, @Email, @NomeUsuario, @DataNascimento, @Telefone, @Foto)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email);
                        cmd.Parameters.AddWithValue("@NomeUsuario", usuario.NomeUsuario);
                        cmd.Parameters.AddWithValue("@DataNascimento", usuario.DataNascimento);
                        cmd.Parameters.AddWithValue("@Telefone", usuario.Telefone);
                        cmd.Parameters.AddWithValue("@Foto", usuario.Foto ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Usuário inserido com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao inserir Usuário: " + ex.Message;
                return false;
            }
        }

        // MÉTODO CORRIGIDO: BuscarPorLogin
        public Usuario BuscarPorLogin(string login)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Usuario WHERE NomeUsuario = @Login OR Email = @Login";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Login", login);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // CORREÇÃO: Converter dados de forma segura
                                byte[] foto = null;
                                var fotoData = reader["Foto"];
                                if (fotoData != DBNull.Value)
                                {
                                    foto = (byte[])fotoData;
                                }

                                return new Usuario
                                {
                                    // CORREÇÃO: Usar Convert.ToInt32 para garantir que é int
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    NomeUsuario = reader["NomeUsuario"]?.ToString() ?? "",
                                    DataNascimento = reader["DataNascimento"]?.ToString() ?? "",
                                    Telefone = reader["Telefone"]?.ToString() ?? "",
                                    Foto = foto
                                };
                            }
                            else
                            {
                                Mensagem = "Usuário não encontrado.";
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar usuário: " + ex.Message;
                return null;
            }
        }

        public bool AtualizarUsuarioCompleto(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Usuario SET Nome = @Nome, Email = @Email, NomeUsuario = @NomeUsuario,
                                 DataNascimento = @DataNascimento, Telefone = @Telefone, Foto = @Foto
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email);
                        cmd.Parameters.AddWithValue("@NomeUsuario", usuario.NomeUsuario);
                        cmd.Parameters.AddWithValue("@DataNascimento", usuario.DataNascimento);
                        cmd.Parameters.AddWithValue("@Telefone", usuario.Telefone);
                        cmd.Parameters.AddWithValue("@Foto", usuario.Foto ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Codigo", usuario.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Usuário atualizado com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar usuário: " + ex.Message;
                return false;
            }
        }

        public bool AtualizarPerfilUsuario(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"UPDATE Usuario SET Nome = @Nome, Email = @Email, Foto = @Foto
                                 WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email);
                        cmd.Parameters.AddWithValue("@Foto", usuario.Foto ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Codigo", usuario.Codigo);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Perfil atualizado com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao atualizar perfil: " + ex.Message;
                return false;
            }
        }

        public bool ExcluirUsuario(int codUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "DELETE FROM Usuario WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codUsuario);
                        cmd.ExecuteNonQuery();
                        Mensagem = "Usuário excluído com sucesso.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao excluir usuário: " + ex.Message;
                return false;
            }
        }

        public Usuario ObterUsuarioPorCodigo(int codigo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Usuario WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] foto = null;
                                var fotoData = reader["Foto"];
                                if (fotoData != DBNull.Value)
                                {
                                    foto = (byte[])fotoData;
                                }

                                return new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    NomeUsuario = reader["NomeUsuario"]?.ToString() ?? "",
                                    DataNascimento = reader["DataNascimento"]?.ToString() ?? "",
                                    Telefone = reader["Telefone"]?.ToString() ?? "",
                                    Foto = foto
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter usuário: " + ex.Message;
            }
            return null;
        }

        public List<Usuario> BuscarUsuariosPorTexto(string texto)
        {
            var lista = new List<Usuario>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Usuario 
                                 WHERE NomeUsuario LIKE @Texto OR Email LIKE @Texto OR Nome LIKE @Texto
                                 ORDER BY Nome";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] foto = null;
                                var fotoData = reader["Foto"];
                                if (fotoData != DBNull.Value)
                                {
                                    foto = (byte[])fotoData;
                                }

                                lista.Add(new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    NomeUsuario = reader["NomeUsuario"]?.ToString() ?? "",
                                    DataNascimento = reader["DataNascimento"]?.ToString() ?? "",
                                    Telefone = reader["Telefone"]?.ToString() ?? "",
                                    Foto = foto
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar usuários: " + ex.Message;
            }
            return lista;
        }

        public bool VerificaUsuarioExistente(string nomeUsuario)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Usuario WHERE NomeUsuario = @NomeUsuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NomeUsuario", nomeUsuario);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao verificar usuário existente: " + ex.Message;
                return false;
            }
        }

        public bool VerificaEmailExistente(string email)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT COUNT(*) FROM Usuario WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao verificar email existente: " + ex.Message;
                return false;
            }
        }

        public Usuario AutenticarPorFoto(string login, byte[] foto)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Usuario 
                               WHERE (NomeUsuario = @Login OR Email = @Login)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Login", login);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] fotoBanco = null;
                                var fotoData = reader["Foto"];
                                if (fotoData != DBNull.Value)
                                {
                                    fotoBanco = (byte[])fotoData;
                                }

                                if (fotoBanco != null && foto != null &&
                                    fotoBanco.SequenceEqual(foto))
                                {
                                    return new Usuario
                                    {
                                        Codigo = Convert.ToInt32(reader["Codigo"]),
                                        Nome = reader["Nome"]?.ToString() ?? "",
                                        Email = reader["Email"]?.ToString() ?? "",
                                        NomeUsuario = reader["NomeUsuario"]?.ToString() ?? "",
                                        DataNascimento = reader["DataNascimento"]?.ToString() ?? "",
                                        Telefone = reader["Telefone"]?.ToString() ?? "",
                                        Foto = fotoBanco
                                    };
                                }
                            }
                        }
                    }
                }
                Mensagem = "Imagem ou usuário não reconhecido";
                RegistrarLogAutenticacao(login, false);
                System.Media.SystemSounds.Beep.Play();
                return null;
            }
            catch (Exception ex)
            {
                Mensagem = "Erro na autenticação: " + ex.Message;
                RegistrarLogAutenticacao(login, false);
                return null;
            }
        }

        public string GerarNomeUsuarioAleatorio(string nomeCompleto, string dataNascimento)
        {
            try
            {
                string nomeSemAcentos = RemoverAcentos(nomeCompleto).ToLower();
                string[] partes = nomeSemAcentos.Split(' ');

                if (partes.Length < 2)
                {
                    Mensagem = "Não foi possível gerar aleatório";
                    return null;
                }

                string primeiroNome = partes[0];
                string ultimoSobrenome = partes[partes.Length - 1];
                string anoNascimento = dataNascimento.Split('/')[2].Substring(2);

                string usuarioSugerido = $"{primeiroNome}.{ultimoSobrenome}{anoNascimento}";

                if (!VerificaUsuarioExistente(usuarioSugerido))
                {
                    return usuarioSugerido;
                }

                if (partes.Length >= 3)
                {
                    string penultimoSobrenome = partes[partes.Length - 2];
                    usuarioSugerido = $"{primeiroNome}.{penultimoSobrenome}{anoNascimento}";

                    if (!VerificaUsuarioExistente(usuarioSugerido))
                    {
                        return usuarioSugerido;
                    }
                }

                Mensagem = "Não foi possível gerar aleatório";
                return null;
            }
            catch
            {
                Mensagem = "Não foi possível gerar aleatório";
                return null;
            }
        }

        private string RemoverAcentos(string texto)
        {
            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void RegistrarLogAutenticacao(string usuario, bool sucesso)
        {
            try
            {
                UsersErrors logger = new UsersErrors(usuario);
                logger.Incluir(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao registrar log: " + ex.Message);
            }
        }

        public string ObterInicialUsuario(Usuario usuario)
        {
            return !string.IsNullOrEmpty(usuario.Nome) ? usuario.Nome[0].ToString().ToUpper() : "?";
        }

        public bool VerificarPermissaoVisualizacao(int usuarioLogado, int usuarioDono)
        {
            return usuarioLogado == usuarioDono;
        }

        public int ObterQuantidadeUsuarios()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT COUNT(*) FROM Usuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao contar usuários: " + ex.Message;
                return 0;
            }
        }

        public void DiagnosticoCompletoUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT COUNT(*) FROM Usuario WHERE Codigo = @usuarioId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        int count = (int)cmd.ExecuteScalar();
                        Console.WriteLine($"✅ Diagnóstico: Usuário {usuarioId} encontrado - {count} registro(s)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no diagnóstico: {ex.Message}");
            }
        }

        public List<Usuario> ObterTodosUsuarios()
        {
            var lista = new List<Usuario>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT * FROM Usuario ORDER BY Nome";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] foto = null;
                                var fotoData = reader["Foto"];
                                if (fotoData != DBNull.Value)
                                {
                                    foto = (byte[])fotoData;
                                }

                                lista.Add(new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    NomeUsuario = reader["NomeUsuario"]?.ToString() ?? "",
                                    DataNascimento = reader["DataNascimento"]?.ToString() ?? "",
                                    Telefone = reader["Telefone"]?.ToString() ?? "",
                                    Foto = foto
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao buscar todos os usuários: " + ex.Message;
            }
            return lista;
        }

        public Usuario ObterUsuarioBasicoPorCodigo(int codigo)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = "SELECT Codigo, Nome, Email, NomeUsuario FROM Usuario WHERE Codigo = @Codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    NomeUsuario = reader["NomeUsuario"]?.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao obter usuário: " + ex.Message;
            }
            return null;
        }
    }
}