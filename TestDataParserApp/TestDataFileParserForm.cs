using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
namespace WindowsFormsApp222
{
    public partial class TestDataFileParserForm : Form
    {
        private string ConnectionString;
        public TestDataFileParserForm()
        {
            InitializeComponent();
            this.ConnectionString = ConfigurationManager.ConnectionStrings["TestDataParserConnectionString"].ConnectionString;
        }

        private void Add_Data_Line(TestDataFileParser parser)
        {
            SqlConnection sqlConnectionString = new SqlConnection(this.ConnectionString);
            string sqlString = "insert into ParserTable(Clinic_Number, Barcode, Patient_Id, Patient_Name, Dob, Gender, Collection_Date, Collection_Time, Test_Code, Test_Name, Result, Unit, Refrange_Low, Refrange_High, Note, Non_Spec_Ref) VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14,@param15,@param16)";
            using (SqlCommand cmd = new SqlCommand(sqlString, sqlConnectionString))
            {
                cmd.Parameters.AddWithValue("@param1", parser.Clinic_Number);
                cmd.Parameters.AddWithValue("@param2", parser.BarCode);
                cmd.Parameters.AddWithValue("@param3", parser.PatientId);
                cmd.Parameters.AddWithValue("@param4", parser.PatientName);
                cmd.Parameters.AddWithValue("@param5", parser.Dob);
                cmd.Parameters.AddWithValue("@param6", parser.Gender);
                cmd.Parameters.AddWithValue("@param7", parser.CollectionDate);
                cmd.Parameters.AddWithValue("@param8", parser.CollectionTime);
                cmd.Parameters.AddWithValue("@param9", parser.TestCode);
                cmd.Parameters.AddWithValue("@param10", parser.TestName);
                cmd.Parameters.AddWithValue("@param11", parser.Result);
                cmd.Parameters.AddWithValue("@param12", parser.Unit);
                cmd.Parameters.AddWithValue("@param13", parser.RefrangeLow);
                cmd.Parameters.AddWithValue("@param14", parser.RefrangeHigh);
                cmd.Parameters.AddWithValue("@param15", parser.Note);
                cmd.Parameters.AddWithValue("@param16", parser.NonSpecRef);
                cmd.CommandType = CommandType.Text;
                try
                {
                    sqlConnectionString.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "Error Message");
                }
            }
        }
        private void Read_File(string fileName)
        {
            String currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo currentDirectoryInfo = new DirectoryInfo(currentDirectory);
            String filePath = currentDirectoryInfo.Parent.Parent.FullName + "\\" + fileName;
            TestDataFileParser parser = new TestDataFileParser();
            using (var fileStream = File.OpenRead(filePath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                String line;
                int lineNumber = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineNumber++;
                    if(lineNumber > 1)
                    {
                        string[] valuesArray = line.Split('|');
                        parser.Clinic_Number = valuesArray[0];
                        parser.BarCode = valuesArray[1];
                        parser.PatientId = valuesArray[2];
                        parser.PatientName = valuesArray[3];
                        parser.Dob = valuesArray[4];
                        parser.Gender= valuesArray[5];
                        parser.CollectionDate = valuesArray[6];
                        parser.CollectionTime = valuesArray[7];
                        parser.TestCode = valuesArray[8];
                        parser.TestName = valuesArray[9];
                        parser.Result = valuesArray[10];
                        parser.Unit = valuesArray[11];
                        parser.RefrangeLow = valuesArray[12];
                        parser.RefrangeHigh = valuesArray[13];
                        parser.Note = valuesArray[14];
                        parser.NonSpecRef = valuesArray[15];
                        this.Add_Data_Line(parser);
                    }

                }
            }
        }

        private void Load_Data()
        {
            SqlConnection sqlConnectionString = new SqlConnection(this.ConnectionString);
            SqlDataAdapter ad = new SqlDataAdapter();
            string sqlCommand = "select * from ParserTable";
            DataSet ds = new DataSet();
            SqlCommand command;
            DataView dv;

            try
            {
                sqlConnectionString.Open();
                command = new SqlCommand(sqlCommand, sqlConnectionString);
                ad.SelectCommand = command;
                ad.Fill(ds);
                sqlConnectionString.Close();
                dv = ds.Tables[0].DefaultView;
                dataGridView1.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            this.Read_File("TestData.txt");
            this.Load_Data();

        }
    }
}
