using System.Diagnostics;
using System.Net;
using HtmlAgilityPack;

var day = DateTime.Now.Day;
var month = DateTime.Now.Month;
var year = DateTime.Now.Year;

if (args.Length != 0)
{
    // args = 02 07 2006
    day = int.Parse(args[0]); // 02
    month = int.Parse(args[1]); // 07
    year = int.Parse(args[2]); // 2006
}

Console.WriteLine("Current date: " + day + "." + month + "." + year);
if (args.Length == 0)
{
    Console.WriteLine("Do you want to use another date? (y/n)");
    bool useOtherDate = Console.ReadLine() == "y";

    if (useOtherDate)
    {
        Console.WriteLine("Min Date: 01.01.2015 | Seperate by '.'");
        Console.Write("DD.MM.YYYY: ");
        string[] date = Console.ReadLine().Trim().Split('.');
        day = int.Parse(date[0]);
        month = int.Parse(date[1]);
        year = int.Parse(date[2]);
    }
}

var monthString = "";
switch (month)
{
    case 1:
        monthString = "January";
        break;
    case 2:
        monthString = "February";
        break;
    case 3:
        monthString = "March";
        break;
    case 4:
        monthString = "April";
        break;
    case 5:
        monthString = "May";
        break;
    case 6:
        monthString = "June";
        break;
    case 7:
        monthString = "July";
        break;
    case 8:
        monthString = "August";
        break;
    case 9:
        monthString = "September";
        break;
    case 10:
        monthString = "October";
        break;
    case 11:
        monthString = "November";
        break;
    case 12:
        monthString = "December";
        break;
}

var dayString = day.ToString();
if (day < 10)
{
    dayString = "0" + day;
}

var web = new HtmlWeb();
var doc = web.Load("https://apod.nasa.gov/apod/archivepix.html");
var nodes = doc.DocumentNode.SelectNodes($"/html/body/b").Descendants();

string searchPattern = $"\n{year} {monthString} {dayString}:";
foreach (var htmlNode in nodes)
{
    if (htmlNode.Name == "br") continue;
    
    if (htmlNode.Name == "#text" && htmlNode.InnerText.Trim() == searchPattern.Trim())
    {
        var urlHtml = htmlNode.NextSibling.Attributes.FirstOrDefault(x => x.Name == "href").Value;
        DownloadImage("https://apod.nasa.gov/apod/" + urlHtml);
        break;
    }
}

void DownloadImage(string url)
{
    var doc = web.Load(url);

    var nodes = doc.DocumentNode.SelectNodes("/html/body/center[1]/p[2]/a/img");
    var imageDescription = doc.DocumentNode.SelectNodes("/html/body/p[1]").FirstOrDefault().InnerText.Trim();

    var imageShortCut = nodes.FirstOrDefault().Attributes.FirstOrDefault(x => x.Name == "src").Value;
    var imageUrl = "https://apod.nasa.gov/apod/" + imageShortCut;

    var imageName = $"{year}-{month}-{day}.jpg";
    if (!File.Exists(imageName))
    {
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(new Uri(imageUrl), imageName);
        }
    }

    string argument = "/open, \"" + imageName + "\"";

    Process.Start("explorer.exe", argument);
    
    Console.WriteLine("\n" + imageDescription);
}

// I know there is a better way to do this, but I'm too lazy to fix it.