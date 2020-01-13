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
using System.IO;
using System.Data.OleDb;
using System.Data.SQLite;

namespace APP
{
    public partial class Form1 : Form
    {
        bool general, table2, table3;
        private SQLiteConnection sqlConnection = null;
        public Form1(SQLiteConnection connection)
        {
            sqlConnection = connection;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Columns.Add("ID", 30);
            listView1.Columns.Add("ФИО", 200);
            listView1.Columns.Add("Коэффициент", 120);
            listView1.Columns.Add("Среднее", 90);

             LoadTeachersAsync();
            if (listView1.Items.Count > 0)
            {
                 avg();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sqlConnection.Close();
            Application.Exit();
        }


        private void LoadTeachersAsync()
        {
            SQLiteDataReader sqlReader = null;
            SQLiteCommand getTeachersCommand = new SQLiteCommand("SELECT * FROM [General] ORDER BY [Коэффициент] DESC", sqlConnection);

            try
            {
                sqlReader = getTeachersCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        Convert.ToString(sqlReader["ID"]),
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

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
             LoadTeachersAsync();
            if (listView1.Items.Count > 0)
                 avg();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Вставить insert = new Вставить(sqlConnection, general, table2, table3);

            insert.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Update update = new Update(sqlConnection, Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text), general, table2, table3);
                update.Show();
            }
            else
            {
                MessageBox.Show("Выделите строку перед данной операцией!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DialogResult res = MessageBox.Show("Вы действительно хотите удалить эту запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                switch (res)
                {

                    case DialogResult.OK:
                        SQLiteCommand deleteTeacherCommand = new SQLiteCommand("DELETE FROM [General] WHERE [ID]=@ID", sqlConnection);
                        deleteTeacherCommand.Parameters.AddWithValue("ID", Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                        try
                        {
                            await deleteTeacherCommand.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        listView1.Items.Clear();
                        LoadTeachersAsync();
                        if (listView1.Items.Count > 0)
                        {
                            avg();
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("Выделите строку перед данной операцией!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Application.Exit();
            sqlConnection.Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("База данных учителей\nBy Vladimir Sechko, 2019", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void avg()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT AVG (Коэффициент) AS 'Среднее' FROM [General] ", sqlConnection);
            SQLiteDataReader sqlReader =  command.ExecuteReader();

            while (sqlReader.Read())
            {

                listView1.Items[0].SubItems.Add(Convert.ToString(sqlReader["Среднее"]));
            }
            sqlReader.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SQLiteDataReader sqlReader = null;
            SQLiteCommand search = new SQLiteCommand("SELECT * FROM [General] WHERE (ФИО) LIKE '%" + textBox1.Text+ "%'", sqlConnection);

            try
            {
                listView1.Items.Clear();
                sqlReader = search.ExecuteReader();

                while (await sqlReader.ReadAsync())
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        Convert.ToString(sqlReader["ID"]),
                        Convert.ToString(sqlReader["ФИО"]),
                        Convert.ToString(sqlReader["Коэффициент"])
                    });

                    listView1.Items.Add(item);
                    avg();
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

        private void импортИзExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //OpenFileDialog op = new OpenFileDialog();
            //op.Filter = "Книги Excel|*.xls; *.xlsx";
            //if (op.ShowDialog() == DialogResult.OK)
            //{
            //    string ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + op.FileName + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=0\"";
            //    OleDbConnection Conn = new OleDbConnection(ConStr); //Get data from Excel Sheet to DataTable
            //    Conn.Open();
            //    OleDbCommand oconn = new OleDbCommand("SELECT (ФИО), (Коэффициент) FROM [Лист1$]", Conn);
            //    OleDbDataAdapter adp = new OleDbDataAdapter(oconn);
            //    DataTable dt = new DataTable();
            //    adp.Fill(dt);
            //    Conn.Close();
            //    //Load Data from DataTable to SQL Server Table.
            //    using (SqlBulkCopy BC = new SqlBulkCopy(sqlConnection))
            //    {
            //        BC.DestinationTableName = "dbo.Teachers";
            //        foreach (var column in dt.Columns)
            //            BC.ColumnMappings.Add(column.ToString(), column.ToString());
            //        BC.WriteToServer(dt);
            //    }
            //    await LoadTeachersAsync();
            //    await avg();
            //}
        }

        private async void toolStripButton4_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы действительно хотите очистить таблицу?", "Форматирование", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (res == DialogResult.OK)
            {
                SQLiteCommand deleteTeachersCommand = new SQLiteCommand("DELETE FROM [General]", sqlConnection);
                try
                {
                    await deleteTeachersCommand.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                listView1.Items.Clear();
                LoadTeachersAsync();
            }
        }

        private void экспортВExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();

            xla.Visible = true;

            Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);

            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;

            int i = 1;

            int j = 1;

            foreach (ListViewItem comp in listView1.Items)

            {

                ws.Cells[i, j] = comp.Text.ToString();

                //MessageBox.Show(comp.Text.ToString());

                foreach (ListViewItem.ListViewSubItem drv in comp.SubItems)

                {

                    ws.Cells[i, j] = drv.Text.ToString();

                    j++;

                }

                j = 1;

                i++;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            general = true;
            table2 = false;
            table3 = false;
            listView1.Clear();
            listView1.Columns.Add("ID", 30);
            listView1.Columns.Add("ФИО", 200);
            listView1.Columns.Add("Коэффициент", 120);
            listView1.Columns.Add("Среднее", 90);
            
            LoadTeachersAsync();
            if (listView1.Items.Count > 0)
                avg();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            table2 = true;
            general = false; 
            table3 = false;
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("ID", 30);
            listView1.Columns.Add("ФИО", 100);

            SQLiteDataReader sqlReader = null;
            SQLiteCommand getLolCommand = new SQLiteCommand("SELECT * FROM [FIO]", sqlConnection);

            try
            {
                sqlReader = getLolCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        Convert.ToString(sqlReader["ID"]),
                        Convert.ToString(sqlReader["ФИО"])
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
        
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            table3 = true;
            general = false;
            table2 = false;
        }
    }
}