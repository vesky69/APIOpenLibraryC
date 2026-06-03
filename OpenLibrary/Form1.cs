using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; //ADONet
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace OpenLibrary
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = @"Data Source=LAB532; Initial Catalog=library; User ID=breno; Password=LltEr032007.; TrustServerCertificate=True;";

        private List<Livro> livros = new List<Livro>();
        private readonly HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            btnSearch.Click += async (s, e) => await BuscarLivroAsync();
            this.Load += Form1_Load;
            dgvLivros.CellClick += DgvLivros_CellClick;
            dgvLivros.SelectionChanged += DgvLivros_SelectionChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ComboBox com categorias padrão
            var categoriasPadrao = new[] { "Ficção", "Não-Ficção", "Fantasia", "Ciência", "Romance", "Biografia" };
            cboCategoria.Items.AddRange(categoriasPadrao);
            cboCategoria.Text = "Ficção";

            GarantirCategoriasNoBanco(categoriasPadrao);

            //Carrega os livros do bd via ADONet
            CarregarLivrosDoBanco();
            RenderizarTabela();
        }

        private async Task BuscarLivroAsync()
        {
            var query = txtSearch.Text?.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Por favor, digite o título de um livro!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblStatus.Text = "Buscando na API...";
            lblStatus.ForeColor = Color.Black;

            try
            {
                var url = $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(query)}";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var root = JObject.Parse(json);
                var docs = root["docs"] as JArray;
                if (docs != null && docs.Count > 0)
                {
                    var livroJson = docs[0];
                    var coverKey = livroJson.Value<string>("cover_edition_key");
                    var coverUrl = !string.IsNullOrEmpty(coverKey)
                        ? $"https://covers.openlibrary.org/b/olid/{coverKey}-M.jpg"
                        : null;

                    string nomeCategoria = string.IsNullOrWhiteSpace(cboCategoria.Text) ? "Sem Categoria" : cboCategoria.Text.Trim();

                    //ID da categoria no banco de dados
                    int idCategoria = ObterOuCriarIdCategoria(nomeCategoria);

                    var novo = new Livro
                    {
                        Titulo = livroJson.Value<string>("title") ?? "Desconhecido",
                        Autor = (livroJson["author_name"] != null && livroJson["author_name"].HasValues) ? livroJson["author_name"].First.ToString() : "Desconhecido",
                        AnoPublicacao = livroJson.Value<int?>("first_publish_year"),
                        CapaUrl = coverUrl,
                        Descricao = string.Empty,
                        IdCategoria = idCategoria,
                        NomeCategoria = nomeCategoria
                    };

                    //Salva o livro no bd e pega o ID
                    novo.IdLivro = SalvarLivroNoBanco(novo);

                    livros.Add(novo);
                    RenderizarTabela();

                    txtSearch.Clear();
                    lblStatus.Text = "✓ Livro adicionado ao Banco de Dados!";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = Color.FromArgb(39, 174, 96);
                    await Task.Delay(1500);
                    lblStatus.Text = string.Empty;
                    lblStatus.BackColor = SystemColors.Control;
                }
                else
                {
                    lblStatus.Text = "✗ Livro não encontrado.";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = Color.FromArgb(231, 76, 60);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Erro: " + ex);
                lblStatus.Text = "✗ Erro ao buscar/salvar o livro.";
                lblStatus.ForeColor = Color.White;
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
            }
        }

        #region Métodos ADO.NET

        private void CarregarLivrosDoBanco()
        {
            livros.Clear();
            string query = @"
                SELECT L.IdLivro, L.Titulo, L.Autor, L.AnoPublicacao, L.CapaUrl, L.Descricao, L.IdCategoria, C.Nome AS NomeCategoria
                FROM Livros L
                INNER JOIN Categorias C ON L.IdCategoria = C.IdCategoria";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                livros.Add(new Livro
                                {
                                    IdLivro = Convert.ToInt32(reader["IdLivro"]),
                                    Titulo = reader["Titulo"].ToString(),
                                    Autor = reader["Autor"].ToString(),
                                    AnoPublicacao = reader["AnoPublicacao"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AnoPublicacao"]),
                                    CapaUrl = reader["CapaUrl"] == DBNull.Value ? null : reader["CapaUrl"].ToString(),
                                    Descricao = reader["Descricao"].ToString(),
                                    IdCategoria = Convert.ToInt32(reader["IdCategoria"]),
                                    NomeCategoria = reader["NomeCategoria"].ToString()
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao conectar ao banco de dados: " + ex.Message, "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private int SalvarLivroNoBanco(Livro livro)
        {
            string query = @"
                INSERT INTO Livros (Titulo, Autor, AnoPublicacao, CapaUrl, Descricao, IdCategoria)
                VALUES (@Titulo, @Autor, @AnoPublicacao, @CapaUrl, @Descricao, @IdCategoria);
                SELECT SCOPE_IDENTITY();"; //Retorna o ID gerado

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Titulo", livro.Titulo);
                    cmd.Parameters.AddWithValue("@Autor", livro.Autor);
                    cmd.Parameters.AddWithValue("@AnoPublicacao", (object)livro.AnoPublicacao ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CapaUrl", (object)livro.CapaUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descricao", livro.Descricao ?? string.Empty);
                    cmd.Parameters.AddWithValue("@IdCategoria", livro.IdCategoria);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private void ExcluirLivroDoBanco(int idLivro)
        {
            string query = "DELETE FROM Livros WHERE IdLivro = @IdLivro";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdLivro", idLivro);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int ObterOuCriarIdCategoria(string nomeCategoria)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Já existente
                string selectQuery = "SELECT IdCategoria FROM Categorias WHERE Nome = @Nome";
                using (SqlCommand cmdSelect = new SqlCommand(selectQuery, conn))
                {
                    cmdSelect.Parameters.AddWithValue("@Nome", nomeCategoria);
                    object result = cmdSelect.ExecuteScalar();
                    if (result != null) return Convert.ToInt32(result);
                }

                //Cria IdCategoria novo
                string insertQuery = "INSERT INTO Categorias (Nome, Slug) VALUES (@Nome, @Slug); SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@Nome", nomeCategoria);
                    cmdInsert.Parameters.AddWithValue("@Slug", nomeCategoria.ToLower().Replace(" ", "-"));
                    return Convert.ToInt32(cmdInsert.ExecuteScalar());
                }
            }
        }

        private void GarantirCategoriasNoBanco(string[] categorias)
        {
            foreach (var cat in categorias)
            {
                ObterOuCriarIdCategoria(cat);
            }
        }

        #endregion

        private void RenderizarTabela()
        {
            dgvLivros.Rows.Clear();
            foreach (var livro in livros)
            {
                Image img = null;
                if (!string.IsNullOrEmpty(livro.CapaUrl))
                {
                    try
                    {
                        using (var stream = httpClient.GetStreamAsync(livro.CapaUrl).GetAwaiter().GetResult())
                        {
                            img = Image.FromStream(stream);
                        }
                    }
                    catch
                    {
                        img = null;
                    }
                }

                //Usando livro.NomeCategoria para exibir nome na Grid
                dgvLivros.Rows.Add(img, livro.Titulo, livro.Autor, livro.AnoPublicacao?.ToString() ?? "N/A", livro.NomeCategoria);
            }
        }

        private void DgvLivros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvLivros.Columns["colExcluir"].Index)
            {
                if (MessageBox.Show("Deseja excluir este livro do banco de dados?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var livroParaExcluir = livros[e.RowIndex];

                    //Exclui do bd
                    ExcluirLivroDoBanco(livroParaExcluir.IdLivro);

                    //Exclui da lista local e atualiza a tela
                    livros.RemoveAt(e.RowIndex);
                    RenderizarTabela();
                }
            }
        }

        private void DgvLivros_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivros.SelectedRows.Count == 0)
            {
                picCapa.Image = null;
                return;
            }

            var idx = dgvLivros.SelectedRows[0].Index;
            if (idx < 0 || idx >= livros.Count) return;
            var livro = livros[idx];
            if (!string.IsNullOrEmpty(livro.CapaUrl))
            {
                try
                {
                    using (var stream = httpClient.GetStreamAsync(livro.CapaUrl).GetAwaiter().GetResult())
                    {
                        picCapa.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    picCapa.Image = null;
                }
            }
            else
            {
                picCapa.Image = null;
            }
        }

        public class Livro
        {
            public int IdLivro { get; set; } 
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int? AnoPublicacao { get; set; }
            public string CapaUrl { get; set; }
            public string Descricao { get; set; }
            public int IdCategoria { get; set; } 
            public string NomeCategoria { get; set; } 
        }
    }
}
