using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlinl
{
    public class SqlDatabase
    {
        // -----------------------------------------------------------------------------------------------
        //  SqlDatabase.cs by Marcus Medina, Copyright (C) 2021, Codic Education AB.
        //  Published under GNU General Public License v3 (GPL-3)
        // -----------------------------------------------------------------------------------------------
        public string ConnectionString { get; set; } = @"Server={1};Database={0};Trusted_Connection=True;";
        public string DatabaseName { get; set; } //  = "testDb";
        public string Server { get; set; } = @"(localdb)\mssqllocaldb";

        public void ExecuteSQL(string sql, ParamData[] parameters)
        {
            // Sätt ihop connectionstring
            var connString = string.Format(ConnectionString, DatabaseName, Server);
            // Förbered SQLConnection
            using var connection = new SqlConnection(connString);
            // Öppna koppling till databasen
            connection.Open();
            // Förbered query
            using var command = new SqlCommand(sql, connection);
            // använd parametrar
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param != null && param.Name != null && param.Data != null)
                        command.Parameters.AddWithValue(param.Name, param.Data);
                }
            }
            // Kör query
            command.ExecuteNonQuery();
        }

        public void ExecuteSQLNoParams(string sql) // ej som overload, kanske säkrare?
        {
            // Sätt ihop connectionstring
            var connString = string.Format(ConnectionString, DatabaseName, Server);
            // Förbered SQLConnection
            using var connection = new SqlConnection(connString);
            // Öppna koppling till databasen
            connection.Open();
            // Förbered query
            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        public DataTable GetDataTable(string sql, ParamData[] parameters)
        {
            var dt = new DataTable();
            // Sätt ihop connectionstring
            var connString = string.Format(ConnectionString, DatabaseName, Server);
            // Förbered SQLConnection
            using var connection = new SqlConnection(connString);
            // Öppna koppling till databasen
            connection.Open();
            // Förbered query
            using var command = new SqlCommand(sql, connection);
            // använd parametrar
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param != null && param.Name != null && param.Data != null)
                        command.Parameters.AddWithValue(param.Name, param.Data);
                }
            }
            // Kör query
            using var adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);

            return dt;
        }

        public int GetANumber(string sql, ParamData[] parameters)
        {
            int returnMe = 0;

            // Sätt ihop connectionstring
            var connString = string.Format(ConnectionString, DatabaseName, Server);
            // Förbered SQLConnection
            using var connection = new SqlConnection(connString);
            // Öppna koppling till databasen
            connection.Open();
            // Förbered query
            using var command = new SqlCommand(sql, connection);
            // använd parametrar
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param != null && param.Name != null)
                        command.Parameters.AddWithValue(param.Name, param.Data);
                }
            }
            // Kör 
            returnMe = (Int32)command.ExecuteScalar();


            return returnMe;
        }

        public decimal GetANumber2(string sql, ParamData[] parameters)
        {
            int returnMe = 0;

            // Sätt ihop connectionstring
            var connString = string.Format(ConnectionString, DatabaseName, Server);
            // Förbered SQLConnection
            using var connection = new SqlConnection(connString);
            // Öppna koppling till databasen
            connection.Open();
            // Förbered query
            using var command = new SqlCommand(sql, connection);
            // använd parametrar
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param != null && param.Name != null)
                        command.Parameters.AddWithValue(param.Name, param.Data);
                }
            }
            // Kör 
            returnMe = (Int32)command.ExecuteScalar();


            return returnMe;
        }



        public static string getString(DataTable dt, int[] cols)
        {
            string posts = "";
            foreach (DataRow item in dt.Rows)
            {

                foreach (var num in cols)
                {
                    if (num < dt.Columns.Count && num >= 0)
                    {

                        //Console.Write(item[num] + " ");
                        posts = posts + "<br>" + item[num] + "<br>";
                    }
                }

            }
            return posts;
        }

        public static string getStringTags(DataTable dt, int[] cols)
        {
            List<string> WTF = new();
            string posts = "<a href=\"top\">top</a><br>";
            WTF.Add(posts);
            foreach (DataRow item in dt.Rows)
            {
                string link = "<a href=\"" + item[0] + "\"> " + item[0] + "</a> <br>";
                WTF.Add(link);
                Console.WriteLine("link: "+link);
                //posts = posts +  link;
                //foreach (var num in cols)
                //{
                //    if (num <= dt.Columns.Count && num >= 0)
                //    {

                //        //Console.Write(item[num] + " "+num+ "  ");
                //        //posts = posts + "<a href=\">" + item[num] + "> " + item[num] + "</a> <br>";
                //        //posts =
                //    }
                //}

            }
            string returnMe= String.Join(" ", WTF);
            Console.WriteLine("returnme: "+returnMe);
            return returnMe;
        }

        public static void printDataTable(DataTable dt, int[] cols)
        {
            foreach (DataRow item in dt.Rows)
            {

                foreach (var num in cols)
                {
                    if (num < dt.Columns.Count && num >= 0)
                    {

                        Console.Write(item[num] + " ");
                    }
                }
                Console.WriteLine("");
            }
        }




    }

    public class ParamData
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }



}
