namespace unidoc;
using System;
using System.Net.Http;
using System.IO;
using System.Data;
using Newtonsoft.Json; // Requires Newtonsoft.Json NuGet package
 using HtmlAgilityPack;
using System.Runtime.CompilerServices;

class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Uni u = new Uni();
        u.printUniDoc("https://docs.google.com/document/d/e/2PACX-1vRPzbNQcx5UriHSbZ-9vmsTow_R6RRe7eyAU60xIF9Dlz-vaHiHNO2TKgDi7jy4ZpTpNqM7EvEcfr_p/pub");

        Console.WriteLine("Done") ;
    }
}
        

public class Uni
{
    public void printUniDoc(string uri)
    {
        getwebdata(uri);
        Console.WriteLine("This is uni doc ");
        
    }


    public void getwebdata(string url)
    {
        int Xlimit = 350;
        int YLimit = 350;

        using (var client = new HttpClient())
        {
            // Create a synchronous request message
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            // Send the request synchronously
            using (var response = client.Send(request))
            {
                response.EnsureSuccessStatusCode(); // Throws exception for bad status codes

                // Read the response content synchronously as a string
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string responseBody = reader.ReadToEnd();
                    // Console.WriteLine(responseBody);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(responseBody);

                    HtmlNode tableNode = doc.DocumentNode.SelectSingleNode("//table");
                    // var headers = tableNode.SelectNodes(".//th").Select(th => th.InnerText.Trim()).ToList();

                    List<List<string>> tableData = new List<List<string>>();

                    foreach (HtmlNode row in tableNode.SelectNodes(".//tr"))
                    {
                        List<string> rowData = new List<string>();
                        foreach (HtmlNode cell in row.SelectNodes(".//td|th")) // Include both td and th for data
                        {
                            rowData.Add(cell.InnerText.Trim());
                        }
                        tableData.Add(rowData);
                    }

                    int rowcnt = 0;
                    // string[,] thepic = new string[340,25];
                    string[,] thepic = new string[Xlimit,YLimit];
                    for (int i = 0; i < Xlimit; i++)
                    {
                        for (int j = 0; j < YLimit; j++)
                        {
                            thepic[i, j] = " "; // Assign space character
                        }
                    }


                    foreach(List<string> a in tableData)
                    {
                        if (rowcnt >0)
                        {
                            int rowline = 0;
                            int xcor = 0;
                            int ycore = 0;
                            string thechar = " ";

                            foreach(string b in a)
                            {
                                switch(rowline)
                                {
                                    case 0:
                                        rowline++;
                                        xcor = Convert.ToInt32(b);
                                        break;
                                    case 1:
                                        rowline++;
                                        thechar = "x";
                                        break;
                                    case 2:
                                        ycore = Convert.ToInt32(b);
                                        rowline++;

                                        thepic[xcor,ycore] = thechar;
                                        rowline = 0;
                                        break;

                                }

                                /*Console.WriteLine("=====");
                                Console.WriteLine(b);
                                Console.WriteLine("=====");*/
                            }
                        }
                        rowcnt++;
                    }

                    for (int k=0;k<Xlimit;k++)
                    {
                        for (int j=0;j<120;j++)
                        {
                            if (thepic[k,j].Trim().Length>0)
                            {
                                Console.Write(thepic[k,j]);
                            }
                        }
                        Console.WriteLine("");
                    }

                    Console.WriteLine("Rowcnt "+rowcnt.ToString());

                }
            }
        }

    }

    
}