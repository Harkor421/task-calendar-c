using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Guineo
{
    public partial class Form1 : Form
    {
        OleDbDataAdapter ds;
        private BindingSource bindingSource = null;
        private OleDbCommandBuilder oleCommandBuilder = null;
        DataTable dataTable = new DataTable();
        public string selecteddate;

        public Form1()
        {
            InitializeComponent();
            panel1.Hide();
            panel2.Hide();
            
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            panel1.Hide();
            panel2.Show();
            label5.Text = monthCalendar1.SelectionRange.Start.ToShortDateString();
            refreshgrid();
            string selecteddate = monthCalendar1.SelectionRange.Start.ToShortDateString();
            label8.Text = selecteddate; 
            Console.WriteLine(selecteddate);
        }

        private void searchdb(string search)
        {
            try
            {
                dataGridView1.DataSource = null;
                dataTable.Clear(); 
                OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Internal.accdb");
                con.Open();
                ds = new OleDbDataAdapter("SELECT Task_ID, task_title, details, fecha FROM Tasks WHERE task_title LIKE '%" + search + "%' OR details LIKE '%" + search + "%'", con); //Query to display the wanted columns in the DB
                oleCommandBuilder = new OleDbCommandBuilder(ds);
                ds.Fill(dataTable);
                bindingSource = new BindingSource { DataSource = dataTable };
                dataGridView1.DataSource = bindingSource;
                con.Close();
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns[0].HeaderText = "ID"; 
                dataGridView1.Columns[1].HeaderText = "Title";
                dataGridView1.Columns[2].HeaderText = "Details";
                dataGridView1.Columns[3].HeaderText = "Date";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void refreshgrid()
        {
            try
            {
                dataGridView1.DataSource = null;
                dataTable.Clear(); //Clears the datatable for refreshing any changes
                OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Internal.accdb");
                con.Open();
                ds = new OleDbDataAdapter("SELECT Task_ID, task_title, details FROM Tasks WHERE fecha LIKE '%" + monthCalendar1.SelectionRange.Start.ToShortDateString() + "%'", con); //Query to display the wanted columns in the DB
                oleCommandBuilder = new OleDbCommandBuilder(ds);
                ds.Fill(dataTable);
                bindingSource = new BindingSource { DataSource = dataTable };// Binds any changes done in the datagrid with the database
                dataGridView1.DataSource = bindingSource;
                con.Close();
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns[0].HeaderText = "ID"; //Setting the names for the columns in the DataGrid
                dataGridView1.Columns[1].HeaderText = "Title";
                dataGridView1.Columns[2].HeaderText = "Details";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            textBox2.Text = "";
            textBox3.Text = "";
        }
        
        //search button
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            panel2.Show();
            searchdb(textBox1.Text);

        }

        //Create task
        private void button2_Click(object sender, EventArgs e)
        {
          panel2.Hide();
          panel1.Show();
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
         

            if (textBox2.Text != "" && textBox3.Text != "")
            {
                try
                {
                    OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Internal.accdb");
                    con.Open();

                    Console.WriteLine("INSERT INTO Tasks(task_title, details, fecha) VALUES(@param1, @param2, @param3)");


                    OleDbCommand cmd = new OleDbCommand("INSERT INTO Tasks (task_title, details, fecha) VALUES(@param1,@param2,@param3)", con);
                    cmd.Parameters.AddWithValue("@param1", textBox2.Text);
                    cmd.Parameters.AddWithValue("@param2", textBox3.Text);
                    cmd.Parameters.AddWithValue("@param3", monthCalendar1.SelectionRange.Start.ToShortDateString());
                    cmd.ExecuteNonQuery();
                    con.Close();
                    textBox2.Clear();
                    textBox3.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {


            try
            {
                DialogResult dialogResult = MessageBox.Show("You are about to delete a task permanently from the Database, are you sure?", "WARNING", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {

                    //Gets the index from the selected row in the datagrid.
                    int selectedIndex = dataGridView1.CurrentCell.RowIndex;

                    //Turns it into a UserID, so the information can be deleted in the database.
                    int rowID = Convert.ToInt32(dataGridView1.CurrentCell.RowIndex);
                    try
                    {

                        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Internal.accdb");
                        con.Open();
                        OleDbCommand cmd = new OleDbCommand("DELETE FROM Tasks WHERE fecha LIKE '%" + monthCalendar1.SelectionRange.Start.ToShortDateString() + "%'", con);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Deletion attempted");
                        con.Close();
                        MessageBox.Show("deleted");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //Deletion of the row in the dataGridView
                    dataGridView1.Rows.RemoveAt(selectedIndex);

                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }
    }
}
