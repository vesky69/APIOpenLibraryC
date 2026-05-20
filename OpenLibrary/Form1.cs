using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenLibrary
{
    public partial class Form1 : Form
    {
        private readonly string dataFilePath;
        private List<Livro> livros = new List<Livro>();
        private readonly HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenLibrary");
            Directory.CreateDirectory(appData);
            dataFilePath = Path.Combine(appData, "meusLivros.json");

            btnSearch.Click += async (s, e) => await BuscarLivroAsync();
            this.Load += Form1_Load;
            dgvLivros.CellClick += DgvLivros_CellClick;
            dgvLivros.SelectionChanged += DgvLivros_SelectionChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Categorias padrão — ComboBox é editável para permitir novas categorias
            var categoriasPadrao = new[] { "Ficção", "Não-Ficção", "Fantasia", "Ciência", "Romance", "Biografia" };
            cboCategoria.Items.AddRange(categoriasPadrao);
            cboCategoria.Text = "Ficção";

            LoadBooksFromFile();
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

            lblStatus.Text = "Buscando...";
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

                    var novo = new Livro
                    {
                        Id = Guid.NewGuid().ToString(),
                        Titulo = livroJson.Value<string>("title") ?? "Desconhecido",
                        Autor = (livroJson["author_name"] != null && livroJson["author_name"].HasValues) ? livroJson["author_name"].First.ToString() : "Desconhecido",
                        AnoPublicacao = livroJson.Value<int?>("first_publish_year"),
                        CapaUrl = coverUrl,
                        Descricao = string.Empty,
                        Categoria = string.IsNullOrWhiteSpace(cboCategoria.Text) ? "Sem Categoria" : cboCategoria.Text.Trim()
                    };

                    livros.Add(novo);
                    SaveBooksToFile();
                    RenderizarTabela();
                    txtSearch.Clear();
                    lblStatus.Text = "✓ Livro adicionado com sucesso!";
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
                Console.Error.WriteLine("Erro na requisição: " + ex);
                lblStatus.Text = "✗ Erro ao buscar o livro. Tente novamente.";
                lblStatus.ForeColor = Color.White;
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
            }
        }

        private void LoadBooksFromFile()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    var json = File.ReadAllText(dataFilePath);
                    livros = JsonConvert.DeserializeObject<List<Livro>>(json) ?? new List<Livro>();
                }
            }
            catch
            {
                livros = new List<Livro>();
            }
        }

        private void SaveBooksToFile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(livros, Formatting.Indented);
                File.WriteAllText(dataFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

                dgvLivros.Rows.Add(img, livro.Titulo, livro.Autor, livro.AnoPublicacao?.ToString() ?? "N/A", livro.Categoria);
            }
        }

        private void DgvLivros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Se clicou na coluna de excluir (última coluna)
            if (e.ColumnIndex == dgvLivros.Columns["colExcluir"].Index)
            {
                if (MessageBox.Show("Deseja excluir este livro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    livros.RemoveAt(e.RowIndex);
                    SaveBooksToFile();
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
    }

    public class Livro
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int? AnoPublicacao { get; set; }
        public string CapaUrl { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
    }
}