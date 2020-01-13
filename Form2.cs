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
    public partial class Form2 : Form
    {
        private SQLiteConnection sqlConnection = null;
        public Form2()
        {
            InitializeComponent();
            label4.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sqlConnection != null)
            {
                Hide();
                Form3 form = new Form3(sqlConnection);
                form.Show();
            } else
            {
                MessageBox.Show("Выберите базу данных!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            if (sqlConnection != null)
            {

                int i = 0;

                SQLiteCommand cmd = sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT (USERNAME) , (PASSWORD) FROM [Users] WHERE USERNAME='" + textBox1.Text + "' and PASSWORD='" + textBox2.Text + "'";
                await cmd.ExecuteNonQueryAsync();
                DataTable dt = new DataTable();
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                da.Fill(dt);
                i = Convert.ToInt32(dt.Rows.Count.ToString());
                if (i == 0)
                {
                    label4.Visible = true;
                    label4.Text = "Вы ввели неверный логин или пароль!";
                }
                else
                {
                    Hide();
                    Form1 form = new Form1(sqlConnection);
                    form.Show();
                }
            }
            else
            {
                MessageBox.Show("Выберите базу данных!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private async void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Все файлы (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                string connectionString = @"Data Source="+op.FileName+";Version=3;" + "MultipleActiveResultSets = True";

                sqlConnection = new SQLiteConnection(connectionString);
                await sqlConnection.OpenAsync();
                button3.Text = "Файл выбран";
            }
        }
    }
}