using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace lab2SGBD
{
    public partial class Form1 : Form
    {
        static string con = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
        SqlConnection cs = new SqlConnection(con);
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();
        DataSet ds2 = new DataSet();
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string parentTableName = ConfigurationManager.AppSettings["ParentTableName"];
            da.SelectCommand = new SqlCommand("Select * from " + parentTableName, cs);
            ds.Clear();
            da.Fill(ds);
            dataGridViewParent.DataSource = ds.Tables[0];
        }
        private void GenerateTextBoxes()
        {
            string childColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
            List<string> childColumnNamesList = new List<string>(childColumnNames.Split(','));
            int l = 1;
            foreach (var column in childColumnNamesList)
            {
                TextBox textBox = new TextBox();
                Label label = new Label();
                label.Text = column;
                label.Top = l * 40;
                textBox.Top = l * 40;
                textBox.Left = 100;
                textBox.Name = "textBox" + column;
                panelTextBoxes.Controls.Add(label);
                panelTextBoxes.Controls.Add(textBox);
                l++;
            }
        }
        private void buttonDisplay_Click(object sender, EventArgs e)
        {
            string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
            string parentId = ConfigurationManager.AppSettings["ParentId"];
            da.SelectCommand = new SqlCommand("Select * from " + childTableName +" where " + parentId + "=@Id", cs);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value =
                Convert.ToInt32(dataGridViewParent.CurrentCell.Value);
            ds2.Clear();
            da.Fill(ds2);
            dataGridViewChild.DataSource = ds2.Tables[0];
            GenerateTextBoxes();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
            string childColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
            string columnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"];
            SqlCommand cmd =
                new SqlCommand("Insert into " + childTableName + " (" + childColumnNames + ") " + " values (" + columnNamesInsertParameters + ")", cs);
            List<string> childColumnNamesList = new List<string>(childColumnNames.Split(','));
            foreach (var column in childColumnNamesList)
            {
                TextBox textBox = (TextBox) panelTextBoxes.Controls["textBox" + column];
                cmd.Parameters.AddWithValue("@" + column, textBox.Text);
            }
            cs.Open();
            cmd.ExecuteNonQuery();
            ds2.Clear();
            da.Fill(ds2);
            cs.Close();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
            string childColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
            string columnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"];
            List<string> childColumnNamesList = new List<string>(childColumnNames.Split(','));
            List<string> columnNamesInsertParametersList = new List<string>(columnNamesInsertParameters.Split(','));
            string toSet = "";
            for (int i = 0; i < childColumnNamesList.Count - 1; i++)
                toSet += childColumnNamesList[i] + "=" + columnNamesInsertParametersList[i]+", ";
            toSet += childColumnNamesList[childColumnNamesList.Count - 1] + "=" +
                     columnNamesInsertParametersList[columnNamesInsertParametersList.Count - 1];
            SqlCommand cmd = new SqlCommand("Update " + childTableName + " set " + toSet + " where " + childColumnNamesList[0]+"="+columnNamesInsertParametersList[0], cs);
            foreach (var column in childColumnNamesList)
            {
                TextBox textBox = (TextBox) panelTextBoxes.Controls["textBox" + column];
                cmd.Parameters.AddWithValue("@" + column, textBox.Text);
            }
            cs.Open();
            cmd.ExecuteNonQuery();
            ds2.Clear();
            da.Fill(ds2);
            cs.Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
            string childColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
            string columnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"];
            List<string> columnNamesInsertParametersList = new List<string>(columnNamesInsertParameters.Split(','));
            List<string> childColumnNamesList = new List<string>(childColumnNames.Split(','));
            SqlCommand cmd = new SqlCommand("Delete from " + childTableName + " where " + childColumnNamesList[0] + "="+ columnNamesInsertParametersList[0] , cs);
            TextBox textBox = (TextBox) panelTextBoxes.Controls["textBox" + childColumnNamesList[0]];
            cmd.Parameters.AddWithValue(columnNamesInsertParametersList[0], textBox.Text);
            cs.Open();
            cmd.ExecuteNonQuery();
            ds2.Clear();
            da.Fill(ds2);
            cs.Close();
        }
    }
}