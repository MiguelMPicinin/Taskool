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
                                return new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    NomeUsuario = reader["NomeUsuario"].ToString(),
                                    DataNascimento = reader["DataNascimento"].ToString(),
                                    Telefone = reader["Telefone"].ToString(),
                                    Foto = reader["Foto"] as byte[]
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

        public List<Usuario> BuscarUsuariosPorTexto(string texto)
        {
            var lista = new List<Usuario>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = @"SELECT * FROM Usuario WHERE NomeUsuario LIKE @Texto OR Email LIKE @Texto";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    NomeUsuario = reader["NomeUsuario"].ToString(),
                                    DataNascimento = reader["DataNascimento"].ToString(),
                                    Telefone = reader["Telefone"].ToString(),
                                    Foto = reader["Foto"] as byte[]
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
                                // Verificar se a foto do banco é igual à foto fornecida
                                byte[] fotoBanco = reader["Foto"] as byte[];

                                if (fotoBanco != null && foto != null &&
                                    fotoBanco.SequenceEqual(foto))
                                {
                                    return new Usuario
                                    {
                                        Codigo = Convert.ToInt32(reader["Codigo"]),
                                        Nome = reader["Nome"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        NomeUsuario = reader["NomeUsuario"].ToString(),
                                        DataNascimento = reader["DataNascimento"].ToString(),
                                        Telefone = reader["Telefone"].ToString(),
                                        Foto = fotoBanco
                                    };
                                }
                            }
                        }
                    }
                }
                Mensagem = "Imagem ou usuário não reconhecido";

                // Registrar tentativa falha no log
                RegistrarLogAutenticacao(login, false);

                // Tocar bipe
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
                // Remover acentos e converter para minúsculo
                string nomeSemAcentos = RemoverAcentos(nomeCompleto).ToLower();

                // Separar nome e sobrenomes
                string[] partes = nomeSemAcentos.Split(' ');

                if (partes.Length < 2)
                {
                    Mensagem = "Não foi possível gerar aleatório";
                    return null;
                }

                string primeiroNome = partes[0];
                string ultimoSobrenome = partes[partes.Length - 1];
                string anoNascimento = dataNascimento.Split('/')[2].Substring(2); // Pegar últimos 2 dígitos

                // Primeira tentativa: primeiro.nome + ultimo sobrenome + ano
                string usuarioSugerido = $"{primeiroNome}.{ultimoSobrenome}{anoNascimento}";

                // Verificar if já existe
                if (!VerificaUsuarioExistente(usuarioSugerido))
                {
                    return usuarioSugerido;
                }

                // Segunda tentativa: se tiver penúltimo sobrenome
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
                // Não interromper o fluxo principal em caso de erro no log
                Console.WriteLine("Erro ao registrar log: " + ex.Message);
            }
        }

        // =========================================================================
        // MÉTODOS ADICIONAIS (INTEGRADOS DO UsuarioDBTeste)
        // =========================================================================

        public Usuario ObterUsuarioPorCodigo(string codigo)
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
                                return new Usuario
                                {
                                    Codigo = Convert.ToInt32(reader["Codigo"]),
                                    Nome = reader["Nome"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    NomeUsuario = reader["NomeUsuario"].ToString(),
                                    DataNascimento = reader["DataNascimento"].ToString(),
                                    Telefone = reader["Telefone"].ToString(),
                                    Foto = reader["Foto"] as byte[]
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

        public string ObterInicialUsuario(Usuario usuario)
        {
            return !string.IsNullOrEmpty(usuario.Nome) ? usuario.Nome[0].ToString().ToUpper() : "?";
        }

        public bool VerificarPermissaoVisualizacao(string usuarioLogado, string usuarioDono)
        {
            return usuarioLogado == usuarioDono;
        }

        public int ObterQuantidadeTarefasAtrasadas(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Projeto_Tarefas PT
                        INNER JOIN Projeto_Membros PM ON PT.CodProjeto = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId 
                        AND PT.isConcluida = 0 
                        AND PT.DataLimite < GETDATE()";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        int total = Convert.ToInt32(cmd.ExecuteScalar());
                        Console.WriteLine($"📊 Quantidade atrasadas: {total}");
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar tarefas atrasadas: {ex.Message}");
                return 0;
            }
        }

        public int ObterQuantidadeProjetosUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"
                        SELECT COUNT(DISTINCT P.Codigo)
                        FROM Projeto P
                        INNER JOIN Projeto_Membros PM ON P.Codigo = PM.CodProjeto
                        WHERE PM.CodMembro = @usuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        int total = Convert.ToInt32(cmd.ExecuteScalar());
                        Console.WriteLine($"📊 Quantidade de projetos: {total}");
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao contar projetos: {ex.Message}");
                return 0;
            }
        }

        public void DiagnosticoCompletoUsuario(int usuarioId)
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 70));
                Console.WriteLine("🎯 DIAGNÓSTICO COMPLETO DO USUÁRIO");
                Console.WriteLine(new string('=', 70));

                // Para usar os métodos de TarefasDB, precisamos criar uma instância
                var tarefasDB = new TarefasDB();

                DateTime dataServidor = DateTime.Today; // Fallback simples
                Console.WriteLine($"\n📊 DATA DO SISTEMA: {dataServidor:dd/MM/yyyy}");

                // Contagens totais
                int totalTarefas = tarefasDB.ObterQuantidadeTarefasTotaisDoUsuario(usuarioId);
                int totalConcluidas = tarefasDB.ObterQuantidadeTarefasTotaisConcluidasDoUsuario(usuarioId);
                int totalPendentes = tarefasDB.ObterQuantidadeTarefasTotaisPendentesDoUsuario(usuarioId);
                int totalAtrasadas = ObterQuantidadeTarefasAtrasadas(usuarioId);
                int totalProjetos = ObterQuantidadeProjetosUsuario(usuarioId);

                // Contagens de hoje
                int hojeConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeHoje(usuarioId);
                int hojePendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeHoje(usuarioId);

                // Contagens da semana
                int semanaConcluidas = tarefasDB.ObterQuantidadeTarefasConcluidasComAlarmeSemana(usuarioId);
                int semanaPendentes = tarefasDB.ObterQuantidadeTarefasPendentesComAlarmeSemana(usuarioId);

                Console.WriteLine($"\n📈 ESTATÍSTICAS GERAIS:");
                Console.WriteLine($"   • Total de Tarefas: {totalTarefas}");
                Console.WriteLine($"   • Tarefas Concluídas: {totalConcluidas}");
                Console.WriteLine($"   • Tarefas Pendentes: {totalPendentes}");
                Console.WriteLine($"   • Tarefas Atrasadas: {totalAtrasadas}");
                Console.WriteLine($"   • Projetos: {totalProjetos}");

                Console.WriteLine($"\n📅 HOJE ({dataServidor:dd/MM/yyyy}):");
                Console.WriteLine($"   • Concluídas: {hojeConcluidas}");
                Console.WriteLine($"   • Pendentes: {hojePendentes}");

                Console.WriteLine($"\n📅 ESTA SEMANA:");
                Console.WriteLine($"   • Concluídas: {semanaConcluidas}");
                Console.WriteLine($"   • Pendentes: {semanaPendentes}");

                // Cálculo de porcentagem
                double porcentagemConcluidas = totalTarefas > 0 ? Math.Round((totalConcluidas * 100.0) / totalTarefas, 1) : 0;
                Console.WriteLine($"\n🎯 PROGRESSO GERAL: {porcentagemConcluidas}% concluído");

                Console.WriteLine("\n" + new string('=', 70));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no diagnóstico: {ex.Message}");
            }
        }
    }
}