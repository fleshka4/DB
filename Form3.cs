using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace APP
{
    public partial class Form3 : Form
    {
        private SQLiteConnection sqlConnection = null;
        public Form3(SQLiteConnection connection)
        {
            sqlConnection = connection;
            InitializeComponent();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("База данных учителей\nBy Vladimir Sechko, 2019", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlConnection.Close();
            Application.Exit();
        }

        private async void toolStripButton5_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            await LoadTeachersAsync();
            if (listView1.Items.Count > 0)
                await avg();
        }

        private async Task avg()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT AVG (Коэффициент) AS 'Среднее' FROM [Teachers] ", sqlConnection);
            SQLiteDataReader sqlReader = command.ExecuteReader();

            while (await sqlReader.ReadAsync())
            {

                listView1.Items[0].SubItems.Add(Convert.ToString(sqlReader["Среднее"]));
            }
            sqlReader.Close();
        }

        private async Task LoadTeachersAsync()
        {
            SQLiteDataReader sqlReader = null;
            SQLiteCommand getTeachersCommand = new SQLiteCommand("SELECT * FROM [Teachers] ORDER BY [Коэффициент] DESC", sqlConnection);

            try
            {
                sqlReader =  getTeachersCommand.ExecuteReader();

                while (await sqlReader.ReadAsync())
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        Convert.ToString(sqlReader["Id"]),
                        Convert.ToString(sqlReader["ФИО"]),
                        Convert.ToString(sqlReader["Коэффициент"])
                    });

                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null && !sqlReader.IsClosed)
                    sqlReader.Close();
            }
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Columns.Add("ID", 1);
            listView1.Columns.Add("ФИО", 200);
            listView1.Columns.Add("Коэффициент", 120);
            listView1.Columns.Add("Среднее", 90);

            await LoadTeachersAsync();
            if (listView1.Items.Count > 0)
            {
                await avg();
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            sqlConnection.Close();
            Application.Exit();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SQLiteDataReader sqlReader = null;
            SQLiteCommand search = new SQLiteCommand("SELECT * FROM [Teachers] WHERE [ФИО] LIKE N'%" + textBox1.Text + "%'", sqlConnection);

            try
            {
                listView1.Items.Clear();
                sqlReader = search.ExecuteReader();

                while (await sqlReader.ReadAsync())
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        Convert.ToString(sqlReader["Id"]),
                        Convert.ToString(sqlReader["ФИО"]),
                        Convert.ToString(sqlReader["Коэффициент"])
                    });

                    listView1.Items.Add(item);
                    await avg();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null && !sqlReader.IsClosed)
                    sqlReader.Close();
            }
        }
    }
}