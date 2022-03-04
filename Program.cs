using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
//using static  System.Data.SqlClient.SqlParameter;
//using Microsoft.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace sqlinl
{
    class Program
    {
        static void Main(string[] args)
        {
             
            Console.WriteLine("Hello SQL!");

            InitDB();
            HttpServer.Run();






        }

        public static void InitDB()
        {

            var jnblogdb01 = new SqlDatabase(); // { DatabaseName = "testDb2" };


            var fiatLux = new ParamData[ 1 ];
            var isThereADB = jnblogdb01.GetDataTable(@"select * from master.dbo.sysdatabases where name ='jnblogdb01';", fiatLux);
            if (isThereADB.Rows.Count == 0)
            {
                fiatLux[0] = new ParamData { Name = "@dbName", Data = "jnblogdb01" };// parametrarna gör ingenting, men måste inkluderas...

                jnblogdb01.ExecuteSQL("CREATE DATABASE jnblogdb01", fiatLux);

                jnblogdb01.DatabaseName = "jnblogdb01";




                jnblogdb01.ExecuteSQLNoParams(@"
                

            create table POSTS 
            ( 
                                               id INT IDENTITY(1,1) PRIMARY KEY,
                                               title VARCHAR(50),
                                                tags VARCHAR(1000),
                                               date DATETIME,
                                               body VARCHAR(max),
 

                )"
                   );

                jnblogdb01.ExecuteSQLNoParams

                    (@"

                        create table TAGS
                                            (
                                                id INT IDENTITY(1,1) PRIMARY KEY,

                                                name VARCHAR(50)

                                            )"
                   );


                jnblogdb01.ExecuteSQLNoParams

                    (@"

                        create table TAGSPOSTS
                                            (                                                
                                                tag INT,
                                                post INT,
                                            )"
                   );

                var nullParamenter = new ParamData[1];

                //DateTime.Now.ToString()
                List<(string, string, string, string)> insertData = new List<(string, string, string, string)> { ("post ett", "citroner kanoner", "blah bla blah blah blah", DateTime.Now.ToString()), ("post två", "citroner", "blah bla blah blah blah", new DateTime(2008, 3, 1, 7, 0, 0).ToString()), ("post tre", "citroner fioler", "blah bla blah blah blah", new DateTime(2017, 1, 1, 7, 0, 0).ToString()), ("post fyra", "basuner violer citroner kanoner", "blah bla blah blah blah", new DateTime(2018, 5, 1, 7, 0, 0).ToString()), ("post fem", "kapuner citroner", "blah", new DateTime(2020, 5, 1, 7, 0, 0).ToString()), ("post sex", "kanoner pultroner", "blah bla blah blah blah", new DateTime(2011, 5, 1, 7, 0, 0).ToString()), ("post sju", "violer miljoner", "blah bla blah blah blah", new DateTime(2018, 5, 1, 7, 0, 0).ToString()) };
                foreach (var item in insertData)
                {
                    string sTitle = item.Item1;
                    string sTags = item.Item2;
                    string sBody = item.Item3;
                    string sDate = item.Item4;

                    //jnblogdb01.ExecuteSQLNoParams(@" insert into  POSTS ( title, tags, body, date) values ('post nr 1','fioler pistoler' " )";

                    jnblogdb01.ExecuteSQLNoParams(@" insert into  POSTS ( title, tags, body, date) values ('" + sTitle + "' ,'" + sTags + "' ,'" + sBody + "' ,'" + sDate + "');");
                    foreach (string word in sTags.Trim().Split(' '))
                    {
                        var tagsDT = jnblogdb01.GetDataTable(@" SELECT name FROM TAGS WHERE name='" + word + "' ", nullParamenter);
                        if (tagsDT.Rows.Count == 0)
                        {
                            jnblogdb01.ExecuteSQLNoParams(@" insert into  TAGS ( name ) values ('" + word + "' );");
                        }
                    }

                    var myID = new ParamData[1];
                    myID[0] = new ParamData { Name = "@table", Data = "POSTS" };

                    foreach (string word in sTags.Trim().Split(' '))
                    {



                        jnblogdb01.ExecuteSQLNoParams(@"





                                      insert into  TAGSPOSTS(tag, post) values((SELECT id FROM TAGS WHERE name = '" + word + "') , (SELECT IDENT_CURRENT('POSTS'))); ");

                    }
                }

            }
            else
            {
                Console.WriteLine("SORRY DB ALREADY EXISTS!");
                jnblogdb01.DatabaseName = "jnblogdb01";
            }
        }

        public static void printDataTable( DataTable dt, int[] cols)
        {
            foreach (DataRow item in dt.Rows)
            {

                foreach (var num in cols)
                {
                    if (num < dt.Columns.Count && num >= 0)
                    {

                        Console.Write(item[num]+" ");
                    }
                }
                Console.WriteLine("");
            }
        }
    }
}
