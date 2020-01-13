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
    public partial class Update : Form
    {
        private SQLiteConnection sqlConnection = null;

        private int id;
        bool general, table2, table3;
        public Update(SQLiteConnection connection, int id, bool general, bool table2, bool table3)
        {
            InitializeComponent();

            sqlConnection = connection;
            this.id = id;
            this.general = general;
            this.table2 = table2;
            this.table3 = table3;
        }



        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0 && textBox2.TextLength > 0)
            {
                SQLiteCommand updateTeachercommand = new SQLiteCommand("UPDATE [General] SET [ФИО]=@ФИО, [Коэффициент]=@Коэффициент WHERE [ID]=@ID", sqlConnection);
                updateTeachercommand.Parameters.AddWithValue("ID", id);
                updateTeachercommand.Parameters.AddWithValue("ФИО", textBox1.Text);
                updateTeachercommand.Parameters.AddWithValue("Коэффициент", Convert.ToDouble(textBox2.Text));

                try
                {
                    await updateTeachercommand.ExecuteNonQueryAsync();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                label3.Visible = true;
                label3.Text = "Поля должны быть заполнены!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void Update_Load_1(object sender, EventArgs e)
        {
            SQLiteCommand getTeacherInfoCommand = new SQLiteCommand("SELECT [ФИО], [Коэффициент] FROM [General] WHERE [ID]=@ID", sqlConnection);
            getTeacherInfoCommand.Parameters.AddWithValue("ID", id);
            SQLiteDataReader sqlReader = null;

            try
            {
                sqlReader = getTeacherInfoCommand.ExecuteReader();
                while (await sqlReader.ReadAsync())
                {
                    textBox1.Text = Convert.ToString(sqlReader["ФИО"]);
                    textBox2.Text = Convert.ToString(sqlReader["Коэффициент"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null && !sqlReader.IsClosed)
                {
                    sqlReader.Close();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (general)
            {
                label1.Text = "ФИО";
            } else if (table2)
            {
                label1.Text = "ID";
            }
        }
    }
}