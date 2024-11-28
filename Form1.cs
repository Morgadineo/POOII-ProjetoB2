/** 
 * Arquivo principal do projeto.
 * -----------------------------
 * Feito por: Arthur Morgado Teixeira
 * Turma:     CC4M
 * E-mail:    arthurmorgado7751@gmail.com
 * Telefone:  27 99613-0202
 * -----------------------------
 * Neste arquivo está a implementação das funcionalidades
 * de cada botão utilizando na interface.
 */

/** 
 * IMPORTS
 */
using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Necessário para realizar a conexão com o banco de dados MySql.

namespace Products_CRUD
{
    /** 
    * Classe principal Form1.
    * -----------------------
    * Responsável por implementar as funcionalidades.
    * Esta classe possui o padrão Singleton implementado,
    * para que haja apenas uma instância de conexão com o
    * banco de dados.
    */
    public partial class Form1 : Form
    {
        // Singleton para conexão com o banco de dados.
        private static MySqlConnection _connection;
        /** 
        * Informações da String de conexão.
        * ---------------------------------
        * O banco de dados "biblioteca" foi criado localmente
        * no computador utilizado.
        * Como foi realizado nos computadores
        * da própria universidade, as informações de usuário e senha
        * devem ser alteradas e o banco de dados deve ser criado
        * localmente. Dentro da pasta do projeto, estão os códigos
        * necessários para a criação do banco de dados e a população
        * do mesmo.
        */
        private static readonly string connectionString = "server=localhost;" +
                                                          "user=root;password=123456;" +
                                                          "database=biblioteca;sslmode=none;";

        public Form1()
        {
            InitializeComponent();  // Inicializa os componentes presentes em "Form1.Designer.cs".
            LoadBooks();            // Carregar os livros ao inicializar.
        }

        // Método para garantir que a conexão seja única (Singleton).
        private static MySqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new MySqlConnection(connectionString);
                }
                return _connection;
            }
        }

        // Função para carregar todos os livros com os nomes dos autores na grid.
        private void LoadBooks()
        {
            string query = @"
                SELECT Livros.idLivro, Livros.Titulo, Livros.AnoPublicacao, Livros.Genero, 
                    GROUP_CONCAT(Autor.Nome SEPARATOR ', ') AS Autores
                FROM Livros
                LEFT JOIN Livros_has_Autor ON Livros.idLivro = Livros_has_Autor.Livros_idLivro
                LEFT JOIN Autor ON Livros_has_Autor.Autor_idAutor = Autor.idAutor
                GROUP BY Livros.idLivro";

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, Connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.DataSource = dataTable;

                // Configuração das colunas
                dataGrid.Columns["Titulo"].HeaderText = "Título";
                dataGrid.Columns["AnoPublicacao"].HeaderText = "Ano de Publicação";
                dataGrid.Columns["Genero"].HeaderText = "Gênero";
                dataGrid.Columns["Autores"].HeaderText = "Autores";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar livros: " + ex.Message);
            }
        }

        /** 
        * Função btnRead_Click.
        * ---------------------
        * Função chamada quando o botão click é pressionado.
        * A ideia do botão "Read" é servir como um filtro para 
        * as informações preenchidas no formulário. Exibindo na 
        * gridView as informações coletadas.
        */
        private void btnRead_Click(object sender, EventArgs e)
        {
            string titulo = txtTitle.Text;
            string ano = txtYear.Text;
            string genero = txtGenre.Text;
            string autor = txtAuthor.Text;

            string query = "SELECT Livros.idLivro, Livros.Titulo, Livros.AnoPublicacao, Livros.Genero, " +
                           "GROUP_CONCAT(Autor.Nome SEPARATOR ', ') AS Autores " +
                           "FROM Livros " +
                           "LEFT JOIN Livros_has_Autor ON Livros.idLivro = Livros_has_Autor.Livros_idLivro " +
                           "LEFT JOIN Autor ON Livros_has_Autor.Autor_idAutor = Autor.idAutor " +
                           "WHERE 1=1 "; // Iniciar a consulta

            // Filtros baseados nos campos de texto preenchidos.
            if (!string.IsNullOrEmpty(titulo))
            {
                query += "AND Livros.Titulo LIKE @Titulo ";
            }
            if (!string.IsNullOrEmpty(ano))
            {
                query += "AND Livros.AnoPublicacao LIKE @AnoPublicacao ";
            }
            if (!string.IsNullOrEmpty(genero))
            {
                query += "AND Livros.Genero LIKE @Genero ";
            }
            if (!string.IsNullOrEmpty(autor))
            {
                query += "AND Autor.Nome LIKE @Autor ";
            }

            query += "GROUP BY Livros.idLivro";

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, Connection);

                // Adicionar parâmetros para evitar SQL Injection
                dataAdapter.SelectCommand.Parameters.AddWithValue("@Titulo", "%" + titulo + "%");
                dataAdapter.SelectCommand.Parameters.AddWithValue("@AnoPublicacao", "%" + ano + "%");
                dataAdapter.SelectCommand.Parameters.AddWithValue("@Genero", "%" + genero + "%");
                dataAdapter.SelectCommand.Parameters.AddWithValue("@Autor", "%" + autor + "%");

                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // Função Create: Inserir um novo livro no banco.
        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Connection.Open();

                // Inserir um livro na tabela Livros
                string queryLivro = "INSERT INTO Livros (Titulo, AnoPublicacao, Genero) VALUES (@Titulo, @AnoPublicacao, @Genero)";
                MySqlCommand cmdLivro = new MySqlCommand(queryLivro, Connection);
                cmdLivro.Parameters.AddWithValue("@Titulo", txtTitle.Text);
                cmdLivro.Parameters.AddWithValue("@AnoPublicacao", txtYear.Text);
                cmdLivro.Parameters.AddWithValue("@Genero", txtGenre.Text);
                cmdLivro.ExecuteNonQuery(); // Executar a inserção do livro

                // Verificar se o campo de autor não está vazio
                string autorNome = txtAuthor.Text;
                int autorId = -1; // Inicializa com um valor inválido

                if (!string.IsNullOrEmpty(autorNome))
                {
                    // Verificar se o autor já existe na tabela Autor
                    string queryAutor = "SELECT idAutor FROM Autor WHERE Nome = @NomeAutor";
                    MySqlCommand cmdAutor = new MySqlCommand(queryAutor, Connection);
                    cmdAutor.Parameters.AddWithValue("@NomeAutor", autorNome);
                    object autorIdObj = cmdAutor.ExecuteScalar();

                    if (autorIdObj == DBNull.Value) // Autor não existe
                    {
                        // Inserir o autor na tabela Autor
                        string insertAutorQuery = "INSERT INTO Autor (Nome) VALUES (@NomeAutor)";
                        MySqlCommand insertAutorCmd = new MySqlCommand(insertAutorQuery, Connection);
                        insertAutorCmd.Parameters.AddWithValue("@NomeAutor", autorNome);
                        insertAutorCmd.ExecuteNonQuery(); // Inserir o autor

                        // Pegar o ID do autor recém-inserido
                        autorId = (int)insertAutorCmd.LastInsertedId;
                    }
                    else // Autor existe
                    {
                        autorId = Convert.ToInt32(autorIdObj); // Usar o ID do autor existente
                    }

                    // Pegar o id do livro recém-criado
                    string queryGetLivroId = "SELECT LAST_INSERT_ID()";
                    MySqlCommand cmdGetLivroId = new MySqlCommand(queryGetLivroId, Connection);
                    int livroId = Convert.ToInt32(cmdGetLivroId.ExecuteScalar());

                    // Agora associar o autor ao livro na tabela Livros_has_Autor
                    string insertLivroAutorQuery = "INSERT INTO Livros_has_Autor (Livros_idLivro, Autor_idAutor) " +
                                                   "VALUES (@Livros_idLivro, @Autor_idAutor)";
                    MySqlCommand insertLivroAutorCmd = new MySqlCommand(insertLivroAutorQuery, Connection);
                    insertLivroAutorCmd.Parameters.AddWithValue("@Livros_idLivro", livroId);
                    insertLivroAutorCmd.Parameters.AddWithValue("@Autor_idAutor", autorId);
                    insertLivroAutorCmd.ExecuteNonQuery(); // Associar o livro ao autor
                }

                MessageBox.Show("Livro adicionado com sucesso.");
                LoadBooks(); // Recarregar a grid após a inserção
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        // Função Update: Atualizar as informações de um livro.
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGrid.SelectedRows.Count > 0)
                {
                    int selectedRowIndex = dataGrid.SelectedRows[0].Index;
                    int livroId = Convert.ToInt32(dataGrid.Rows[selectedRowIndex].Cells["idLivro"].Value);

                    Connection.Open();
                    string query = "UPDATE Livros SET Titulo = @Titulo, AnoPublicacao = @AnoPublicacao, Genero = @Genero WHERE idLivro = @idLivro";
                    MySqlCommand cmd = new MySqlCommand(query, Connection);
                    cmd.Parameters.AddWithValue("@Titulo", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@AnoPublicacao", txtYear.Text);
                    cmd.Parameters.AddWithValue("@Genero", txtGenre.Text);
                    cmd.Parameters.AddWithValue("@idLivro", livroId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Livro atualizado com sucesso.");
                    LoadBooks(); // Recarregar a grid após a atualização
                }
                else
                {
                    MessageBox.Show("Por favor, selecione um livro para atualizar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        // Função Delete: Excluir um livro.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGrid.SelectedRows.Count > 0)
                {
                    int selectedRowIndex = dataGrid.SelectedRows[0].Index;
                    int livroId = Convert.ToInt32(dataGrid.Rows[selectedRowIndex].Cells["idLivro"].Value);

                    Connection.Open();
                    string query = "DELETE FROM Livros WHERE idLivro = @idLivro";
                    MySqlCommand cmd = new MySqlCommand(query, Connection);
                    cmd.Parameters.AddWithValue("@idLivro", livroId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Livro excluído com sucesso.");
                    LoadBooks(); // Recarregar a grid após a exclusão
                }
                else
                {
                    MessageBox.Show("Por favor, selecione um livro para excluir.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        // Função para limpar os campos de texto.
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTitle.Clear();
            txtYear.Clear();
            txtGenre.Clear();
            txtAuthor.Clear();
        }

        // Função para preencher os campos de texto ao clicar na grid.
        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verificar se a linha selecionada é válida.
            {
                DataGridViewRow selectedRow = dataGrid.Rows[e.RowIndex];

                // Preencher os campos com os dados da linha selecionada.
                txtTitle.Text = selectedRow.Cells["Titulo"].Value.ToString();
                txtYear.Text = selectedRow.Cells["AnoPublicacao"].Value.ToString();
                txtGenre.Text = selectedRow.Cells["Genero"].Value.ToString();
                txtAuthor.Text = selectedRow.Cells["Autores"].Value.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtYear_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
