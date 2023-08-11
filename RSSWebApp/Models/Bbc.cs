using System;
using CodeHollow.FeedReader;

namespace RSSWebApp.Models
{
	public class Bbc : Ajans
	{
        public string Title { get; set; }
		
        public override List<News> GetNews()
        {
            var feed = FeedReader.Read("https://feeds.bbci.co.uk/turkce/rss.xml");


            List<News> newList = new List<News>();

            foreach (var item in feed.Items)
            {
                News newNews = new News();

                newNews.Title = item.Title;
                newNews.Link = item.Link;
                newNews.Description = item.Description;
                //System.DateTime.Parse(item.PublishingDateString);
                newNews.PubDate = System.DateTime.Parse(item.PublishingDateString);
                newList.Add(newNews);
            }


            return newList;
        }
    }
}

