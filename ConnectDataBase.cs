using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopServer2k18
{
    class ConnectDataBase
    {
        string connetionString;

        SqlConnection con;
        SqlCommand command;
        SqlDataReader dataReader;

        public ConnectDataBase()
        {
            connetionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\All_VS\Refactoring\TopServer2k18\PerecvetDataBase.mdf;Integrated Security=True;Connect Timeout=30";
            con = new SqlConnection(connetionString);
        }

        public void Insert(bool _access, string _name = "no name")
        {

            string sql = @"INSERT INTO controller1 (access, name) VALUES('" + _access + "','" + _name + "');";

            try
            {
                con.Open();
                command = new SqlCommand(sql, con);
                command.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FATAL SYSTEM ERROR! PLS KILL YOURSELF!\n OR " + ex.Message);
                con.Close();
            }
        }

        public string Select()
        {

            string sql = @"SELECT * FROM controller1";

            try
            {
                con.Open();
                command = new SqlCommand(sql, con);

                dataReader = command.ExecuteReader();
                List<Entry> dataJson = new List<Entry>();
                while (dataReader.Read())
                {
                    // Int32 name = (Int32)dataReader.GetValue(0); // Пока игнорируем :)
                    DateTime datetime = (DateTime)dataReader.GetValue(1);
                    bool access = (bool)dataReader.GetValue(2);
                    string name = (string)dataReader.GetValue(3);

                    dataJson.Add(new Entry(datetime, access, name));

                    
                }
                con.Close();
                return JsonConvert.SerializeObject(dataJson, Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FATAL SYSTEM ERROR PLS KILL YOURSELF!\nPath:" + connetionString + " OR\n" + ex.ToString());
                con.Close();
            }
            con.Close();
            return "";
        }

    }
}
