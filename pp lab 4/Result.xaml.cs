using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace pp_lab_4
{
    /// <summary>
    /// Логика взаимодействия для Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        SqlCommand sCommand;
        SqlDataAdapter sAdapter;
        DataSet sDs;

        public Result(string table, DataColumn column, string val)
        {
            InitializeComponent();

            try
            {
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;

                Title = $"Результат - таблица:{table}, столбец:{column.ColumnName}, значение:{val}";

                string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""|DataDirectory|\PP Lab 2.mdf"";Integrated Security=True";
                string sql = $"SELECT * FROM {table} WHERE {column.ColumnName} = @val";
                SqlConnection connection = new SqlConnection(cs);
                connection.Open();

                sCommand = new SqlCommand(sql, connection);
                SqlParameter value = new SqlParameter("@val", column.DataType)
                {
                    SqlValue = val
                };
                sCommand.Parameters.Add(value);

                sAdapter = new SqlDataAdapter(sCommand);
                sDs = new DataSet();
                sAdapter.Fill(sDs, table);
                connection.Close();

                dataGridView1.ItemsSource = sDs.Tables[table].DefaultView;
                sDs.Tables[table].DefaultView.AllowDelete = false;
                sDs.Tables[table].DefaultView.AllowEdit = false;
                sDs.Tables[table].DefaultView.AllowNew = false;
                dataGridView1.SelectionMode = DataGridSelectionMode.Extended;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
