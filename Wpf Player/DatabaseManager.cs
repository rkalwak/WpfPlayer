using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data;

namespace Wpf_Player
{
    class DatabaseManager
    {

        private string connectionString;
        private SqlConnection sqlConnection;
        private DataSet dataSet;
        public DatabaseManager() { }
        public DatabaseManager(string server,string database)
        {
            //@"Server=SQLSERVER;Database=musicclient;Integrated Security=SSPI"
            connectionString = String.Format(@"Server={0};Database={1};Integrated Security=SSPI",server,database);
            sqlConnection = new SqlConnection(connectionString);
            dataSet = new DataSet("music");
            openConnection();
        }


        public bool openConnection()
        {
            sqlConnection.Open();
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                return true;
            else
                return false;
        }
        public bool closeConnection()
        {
            sqlConnection.Close();
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                return true;
            else
                return false;
        }
        public DataTable getQueryFromDB(string query)
        {
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            dataSet.Clear();
            da.Fill(dataSet, "music");
            DataTable dt = new DataTable();
            da.Fill(dt);
            //return  dataSet.Tables["music"]; 
            return dt;
        }
        public int getCountFromDB(string query)
        {
            //zapytanie musi zwracac tylko liczbe np; select count(*) from music;
            int result = 0;
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                result=(int)cmd.ExecuteScalar();
                
            }
            else
                result = 0;
            return result;
            

            
        }
        public void insertDataIntoDB(string query)
        {
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            try
            {

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
        }

    }
}
