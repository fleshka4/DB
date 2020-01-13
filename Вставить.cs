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
    public partial class Вставить : Form
    {
        private SQLiteConnection sqlConnection = null;
        bool general, table2, table3;

        public Вставить(SQLiteConnection connection, bool general, bool table2, bool table3)
        {
            InitializeComponent();
            sqlConnection = connection;
            this.general = general;
            this.table2 = table2;
            this.table3 = table3;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0 && textBox2.TextLength > 0)
            {
                SQLiteCommand insertTeacherCommand = new SQLiteCommand("INSERT INTO [FIO] (ФИО)VALUES(@ФИО)", sqlConnection);
                SQLiteCommand insertTeacherCommand2 = new SQLiteCommand("INSERT INTO [point] (Коэффициент)VALUES(@Коэффициент)", sqlConnection);
                insertTeacherCommand.Parameters.AddWithValue("ФИО", textBox1.Text);
                insertTeacherCommand2.Parameters.AddWithValue("Коэффициент", Convert.ToDouble(textBox2.Text));
                try
                {
                    await insertTeacherCommand.ExecuteNonQueryAsync();
                    await insertTeacherCommand2.ExecuteNonQueryAsync();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else
            {
                label3.Visible = true;
                label3.Text = "Поля должны быть заполнены!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Вставить_Load(object sender, EventArgs e)
        {
           
                if (general)
                {
                    label1.Text = "ФИО";
                }
                else if (table2)
                {
                    label1.Text = "ID";
                }
            
        }


    }
}