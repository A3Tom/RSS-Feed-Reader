using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace Rss_Feed_Reader
{

    public partial class Form1 : Form
    {
        #region Declaration of variables

        string feedHeaderTitle, feedHeaderURL, feedHeaderDescription, fileName;
        string rssURL = "http://feeds.bbci.co.uk/news/world/rss.xml"; //RSS Url
        string jsonFilePath = @"C:\Users\Tom\Documents\Rss Feed Files\"; //File path for all saved .json files
        List<Item> sortedWriteList = new List<Item>(); //Contains all articles currently on RSS feed
        List<Item> finalWriteList = new List<Item>(); //Contains only articles matching search criteria
        #endregion

        #region Main code
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PullFromRSS();
            tmrHourTicker.Enabled = true; //Starts Timer
            tmrHourTicker.Interval = 60000; //Sets timer interval to 5 minutes per tick
        }

        /// <summary>
        /// Timer to refresh data
        /// </summary>
        private void tmrHourTicker_Tick(object sender, EventArgs e)
        {
            //Each tick checks if the minute of the hour is under 5 minutes
            //This ensures only one update will be performed per hour meaning no duplicates
            if (DateTime.Now.Minute < 5)
            {
                PullFromRSS();
            }
        }

        /// <summary>
        /// Parses RSS Feed
        /// </summary>
        public void ParseData()
        {
            FeedParser parser = new FeedParser();
            var items = parser.ParseRss(rssURL);

            int i = 0;

            //Sorts list by date descending whilst keeping in text format
            sortedWriteList = items.OrderBy(o => Convert.ToDateTime(o.PublishDate)).ToList();
            sortedWriteList.Reverse();

            //Loops through full article list to find matching criteria for final list
            foreach (Item y in sortedWriteList)
            {
                DateTime sortDate = new DateTime();
                sortDate = Convert.ToDateTime(sortedWriteList.ElementAt(i).PublishDate);
                if (sortDate.Hour == DateTime.Now.Hour)
                {
                    //Adds all matching criteria to final list
                    finalWriteList.Add(new Item()
                    {
                        Title = sortedWriteList.ElementAt(i).Title.ToString(),
                        Description = sortedWriteList.ElementAt(i).Description.ToString(),
                        Link = sortedWriteList.ElementAt(i).Link.ToString(),
                        PublishDate = sortedWriteList.ElementAt(i).PublishDate.ToString()
                    });
                }
                i++;
            }
        }

        /// <summary>
        /// Pulls header data for RSS feed
        /// </summary>
        public void GetHeaders()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(rssURL);
            XmlNodeList xnl = xDoc.SelectNodes("rss/channel");

            foreach (XmlNode node in xnl)
            {
                feedHeaderTitle = node["title"].InnerText;
                feedHeaderURL = node["link"].InnerText;
                feedHeaderDescription = node["description"].InnerText;
            }
        }

        /// <summary>
        /// Writes final list to json file
        /// </summary>
        public void WriteToFile()
        {
            Feed feed = new Feed
            {
                title = feedHeaderTitle,
                link = feedHeaderURL,
                description = feedHeaderDescription,
                items = finalWriteList.ToList()
            };

            string json = JsonConvert.SerializeObject(feed, Newtonsoft.Json.Formatting.Indented);
            lblTitles.Text = json; //Writes to a label on the main form for added visual stimulation.
            System.IO.File.WriteAllText(jsonFilePath + fileName, json);
        }
        #endregion

        #region Additional Classes
        /// <summary>
        /// Creates formatted file name for the json file
        /// </summary>
        public void CreateFileName(string year, string month, string day, string hour)
        {
            year = EnsureDoubleDigit(year);
            month = EnsureDoubleDigit(month);
            day = EnsureDoubleDigit(day);
            hour = EnsureDoubleDigit(hour);

            fileName = year + "-" + month + "-" + day + "-" + hour + ".json";
        }

        /// <summary>
        /// Clears all existing list data
        /// </summary>
        public void ClearPreviousData()
        {
            sortedWriteList.Clear();
            finalWriteList.Clear();
        }

        /// <summary>
        /// ensures uniform length in file names
        /// </summary>
        string EnsureDoubleDigit(string number)
        {
            if (number.Length < 2)
            {
                return "0" + number;
            }
            else
            {
                return number;
            }
        }

        /// <summary>
        /// contains everything needed to run RSS Parser
        /// </summary>
        public void PullFromRSS()
        {
            ClearPreviousData();
            ParseData();
            GetHeaders();
            CreateFileName(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour.ToString());
            WriteToFile();
        }
        #endregion
    }
}
