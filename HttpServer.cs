// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)
// ( https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7 )
// modifierad av mig
using System;
using System.Data;
using System.Data.Sql;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

//namespace HttpListenerExample
namespace sqlinl
{
    public class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static ParamData[] nullParamenter = new ParamData[1];
        public static Random rand = new Random();
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";
        public static SqlDatabase offlineBlog001 = new SqlDatabase();
        public static List<string> Filters = new();
        public static List<DataTable> LotsaData = new();
        public static string tagsBlock = "<a href=\"top\">top</a><br> ";
        public static async Task HandleIncomingConnections ()
        {
            //SqlDatabase offlineBlog001 = SqlDatabase.GetSqlDatabase();



            bool runServer = true;
            List<string> myTags = new List<string>();
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {

                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "GET" && (req.Url.AbsolutePath == "/post")))
                {
                    pageData =
                           "<!DOCTYPE>" +
                           "<html>" +
                           "  <head>" +
                           "    <title>HttpListener Example</title>" +
                           "  </head>" +
                           "  <body>" +
                           "    <p>Page Views: {0}</p>" +
                           "    <form method=\"post\" action=\"newpost\">" +
 

                           "    <label for=\"title\">Title:</label><br>" +
                           "    <input type = \"text\" id=\"title\" name=\"title\" value=\"\"><br>" +
                           "     <label for=\"tags\">Tags:</label><br>" +
                           "    <input type = \"text\" id=\"tags\" name=\"tags\" value=\"\"><br><br>" +
                           " <textarea name=\"body\" rows=\"40\" cols=\"72\"></textarea>" +
                           "<br><br>" +

                           "      <input type=\"submit\" value=\"Newpost\" {1}>" +






                           "    </form>" +
                            "  </body>" +
                            "</html>";




                }




                else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/newpost"))
                {
                    Console.WriteLine("Shutdown requested");
                    //string test = req.InputStream.;

                    if (!req.HasEntityBody)
                    {
                        Console.WriteLine("No client data was sent with the request.");
                        return;
                    }
                    System.IO.Stream body = req.InputStream;
                    System.Text.Encoding encoding = req.ContentEncoding;
                    System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
                    if (req.ContentType != null)
                    {
                        Console.WriteLine("Client data content type {0}", req.ContentType);
                    }
                    Console.WriteLine("Client data content length {0}", req.ContentLength64);

                    Console.WriteLine("Start of client data:");
                    //Convert the data to a string and display it on the console.
                    string s = reader.ReadToEnd();
                    string sTitle = "";
                    //List<string> sTags = new();
                    string sTags = "";
                    string sBody = "";
                    //public int IndexOf(string value, int startIndex, int count);
                    int titleIndex = s.IndexOf("title=", 0, s.Length);
                    titleIndex = 0;
                    int titleIndexEnd = s.IndexOf("title=", 0, s.Length) + 6;
                    titleIndexEnd = 6;
                    int tagsIndex = s.IndexOf("&tags=", 0, s.Length);
                    int tagsIndexEnd = s.IndexOf("&tags=", 0, s.Length) + 6;
                    int bodyIndex = s.IndexOf("&body=", 0, s.Length);
                    int bodyIndexEnd = s.IndexOf("&body=", 0, s.Length) + 6;

                    Console.WriteLine(s);
                    Console.WriteLine("End of client data:");
                    sTitle = s.Substring(titleIndexEnd, (tagsIndex - titleIndexEnd)).Replace('+', ' ').Replace("&tags=", " "); //VARFÖR I HELVETE FUNKAR INTE DETTA?!?!?!?!


                    sTags = s.Substring(tagsIndexEnd, (bodyIndex - tagsIndexEnd)).Replace('+', ' ');
                    sBody = s.Substring(bodyIndexEnd, (s.Length - bodyIndexEnd)).Replace('+', ' ');
 
                    // eftersom privat offlineblog kör oparametriserat
                    offlineBlog001.DatabaseName = "jnblogdb01";
 
                    offlineBlog001.ExecuteSQLNoParams(@" insert into  POSTS ( title, tags, body, date) values ('" + sTitle + "' ,'" + sTags + "' ,'" + sBody + "' ,'"+ DateTime.Now.ToString() +"');");
                    foreach (string word in sTags.Trim().Split(' '))
                    {
                        var tagsDT = offlineBlog001.GetDataTable(@" SELECT name FROM TAGS WHERE name='"+word +"' ", nullParamenter);
                        if (tagsDT.Rows.Count == 0)
                        {
                            offlineBlog001.ExecuteSQLNoParams(@" insert into  TAGS ( name ) values ('" + word + "' );");
                        }
                    }

                    var myID = new ParamData[1];
                    myID[0] = new ParamData { Name = "@table", Data = "POSTS" };




                    foreach (string word in sTags.Trim().Split(' '))
                    {



                        offlineBlog001.ExecuteSQLNoParams(@"





                        insert into  TAGSPOSTS(tag, post) values((SELECT id FROM TAGS WHERE name = '" + word + "') , (SELECT IDENT_CURRENT('POSTS'))); ");

                    }


                    string tagsBlock = "<a href=\"top\">top</a><br> ";
                    Console.WriteLine("tagsblock"+tagsBlock);
 


                    var myTagsDT = offlineBlog001.GetDataTable(@" SELECT name FROM TAGS ", nullParamenter);
                     string tagsBlockPartDeux = SqlDatabase.getStringTags(myTagsDT, new int[] { 0 });
                     tagsBlock = tagsBlock + tagsBlockPartDeux;

                    string titlePlusDateBody = "  ";
 
 
                    var postsDT = offlineBlog001.GetDataTable(@" SELECT title, tags, date, body FROM POSTS ORDER BY date DESC", nullParamenter);

                    titlePlusDateBody = SqlDatabase.getString(postsDT, new int[] { 0, 1, 2, 3 });
                    pageData =( 
                           @"<!DOCTYPE>" +
                           "<html>" +
                           "  <head>" +
                           "    <title>My offlineBlog</title>" +
                           "  </head>" +
                           "  <body>"+
                            "<h2>Offlineblog</h2>" +
                           " <div style=\"width: 100 %; \">  " +
                                     "<div style=\"width: 30 %; float: left; background: green;\">  " +
                                    "</div>" +
                                    " <div style=\"width: 50 %; float: left; background: white;>\"            " +
                                            "      " + titlePlusDateBody +
                                    "</div>" +
                                    " <div style=\"width: 20 %; float: left; background: LightGray;>\"          " +
                                             //"   " + tagsQlock +
                                             " " +
                                    //"<form action=\"post\" id=\"npost\"> < button form =\"npost\">Ny Post</button>" +
                                    "</div>" +
                           "</div>" +
                           //"<a href=\"top\">top</a><br>" +
                            tagsBlock +
                           " </body> </html>");



                    body.Close();
                    reader.Close();
 
                }



                else if ((req.HttpMethod == "GET" && (req.Url.AbsolutePath == "/top")))
                {
                    Filters.Clear();
                    LotsaData.Clear();
                    offlineBlog001.DatabaseName = "jnblogdb01";

                    var myTagsDT = offlineBlog001.GetDataTable(@" SELECT name FROM TAGS ", nullParamenter);
                    string tagsBlockPartDeux = SqlDatabase.getStringTags(myTagsDT, new int[] { 0 });
                    tagsBlock = tagsBlock + tagsBlockPartDeux;

                    var postsDT = offlineBlog001.GetDataTable(@" SELECT title, date, body FROM POSTS ORDER BY date DESC", nullParamenter);
                    string titlePlusDateBody = " ";
                    titlePlusDateBody = SqlDatabase.getString(postsDT, new int[] { 1, 2, 3, 4 });

 
                    pageData = (
                                    @"<!DOCTYPE>" +
                                    "<html>" +
                                    "  <head>" +
                                    "    <title>My offlineBlog</title>" +
                                    "  </head>" +
                                    "  <body>" +
                                    "<h2>Offlineblog</h2>" +
                                    " <div style=\"width: 100 %; \">  " +
                                    "<div style=\"width: 30 %; float: left; background: green;\">  " +
                                    "</div>" +
                                    " <div style=\"width: 50 %; float: left; background: white;\"            " +
                                    "      " + titlePlusDateBody +
                                    "</div>" +
                                    " <div style=\"width: 20 %; float: left; background: LightGray;\"          " +
                                     //"   " + tagsBlock +
                                    "<form action=\"post\" id=\"npost\"> < button form =\"npost\">Ny Post</button>" +
                                     "</div>" +
                                    "</div>" +
                                "   " + tagsBlock +
                                     "" +

                                    " </body> </html>");



                }

 


                else if ((req.HttpMethod == "GET" && (req.Url.AbsolutePath != "/top"))) /////////////////////////HIT
                {
                    offlineBlog001.DatabaseName = "jnblogdb01";
                    string tag = req.Url.AbsolutePath;
                    tag = tag.Trim('/' );
                    Filters.Add(tag);
 
                    Console.WriteLine("url: "+tag);
                    ParamData[] taggar = new ParamData[1];
                    taggar[0] = new ParamData { Name = "@tag", Data = tag };
 
                    for (int i = 0; i < Filters.Count; i++)
                    {
                        ParamData[] taggar2 = new ParamData[1];
                        taggar2[0] = new ParamData { Name = "@tag", Data = Filters[i] };
                         
                        DataTable Temporary = offlineBlog001.GetDataTable(@"

                                                                select   POSTS.id, tags, title, date, body
                                                                from TAGSPOSTS 
                                                                join POSTS on POSTS.id = TAGSPOSTS.post
                                                                join TAGS on TAGS.id = TAGSPOSTS.tag
                                                                where name = @tag







                                                                ", taggar2);

                        LotsaData.Add(Temporary);
 
                    }


                    DataTable result = LotsaData[LotsaData.Count-1];
                    for (int j = Filters.Count; j > 0; j--)
                    {
           
                            result = (from t1 in result.AsEnumerable()
                                      join t2 in LotsaData[j - 1].AsEnumerable() on t1.Field<int>("id") equals t2.Field<int>("id")
                                      select t1).CopyToDataTable();
                            Console.Write ("result itererera: ");
                            SqlDatabase.printDataTable(result, new int[] { 0, 1, 2 });
 


                    }

 







                    //                                            ", taggar);
                    var myTagsDT = offlineBlog001.GetDataTable(@" SELECT name FROM TAGS ", nullParamenter);
                    string tagsBlockPartDeux = SqlDatabase.getStringTags(myTagsDT, new int[] { 0 });
                    tagsBlock = tagsBlock + tagsBlockPartDeux;
                    //SqlDatabase.printDataTable(postsDT2, new int[] { 0, 1, 2 });
                    Console.WriteLine("RESULT");
                    SqlDatabase.printDataTable(result, new int[] { 0, 1, 2 });
                    string titlePlusDateBody = " ";
                    //titlePlusDateBody = SqlDatabase.getString(postsDT2, new int[] { 0, 1, 2 });
                    titlePlusDateBody = SqlDatabase.getString(result, new int[] { 1, 2, 3, 4 });
                    //pageData =
                    //       "<!DOCTYPE>" +
                    //       "<html>" +
                    //       "  <head>" +
                    //       "    <title>HttpListener Example</title>" +
                    //       "  </head>" +
                    //       "  <body>" + titlePlusDateBody +
                    //       " </body> </html>";

                    pageData = (
                   @"<!DOCTYPE>" +
                   "<html>" +
                   "  <head>" +
               "    <title>My offlineBlog</title>" +
                    "  </head>" +
                     "  <body>" +
                     "<h2>Offlineblog</h2>" +
                      " <div style=\"width: 100 %; \">  " +
                       "<div style=\"width: 30 %; float: left; background: green;\">  " +
                        "</div>" +
                         " <div style=\"width: 50 %; float: left; background: white;\"            " +
                        "      " + titlePlusDateBody +
                      "</div>" +
                        " <div style=\"width: 20 %; float: left; background: LightGray;\"          " +
                        //"   " + tagsBlock +
                        "<form action=\"post\" id=\"npost\"> < button form =\"npost\">Ny Post</button>" +
                        "</div>" +
                      "</div>" +
                     "   " + tagsBlock +
                     "" +

                     " </body> </html>");



                }


                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                //byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                byte[] data = Encoding.UTF8.GetBytes(pageData);
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();


            }










        }

        public static bool IsFileLocked (FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public static void writeFile (string addMe)
        {
            //File.CreateText("C:\\Users\\ADMIN\\Blog" + addMe);
            //string path = "C:\\Users\\ADMIN\\Blog" + addMe;
            //string textFileName = addMe.Replace("html", "txt");
            //string newText = System.IO.File.ReadAllText("C:\\Users\\ADMIN\\Blog" + textFileName);
            //string text = System.IO.File.ReadAllText("C:\\Users\\ADMIN\\Blog" + addMe);
            //File.WriteAllText(path, text.Replace("replaceme", newText));
            //text = System.IO.File.ReadAllText("C:\\Users\\ADMIN\\Blog" + addMe);
            //pageData = text;

        }
        //public static void Run(string[] args)
        public static void Run ()
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests

            Task listenTask = HandleIncomingConnections();


            listenTask.GetAwaiter().GetResult();






            // Close the listener
            listener.Close();
        }
    }
}