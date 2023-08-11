using System;
using CodeHollow.FeedReader;

namespace RSSWebApp.Models
{
	public class Haberturk : Ajans
	{
        public override List<News> GetNews()
        {
            var feed = FeedReader.Read("https://www.haberturk.com/rss/manset.xml");


            List<News> newList = new List<News>();

            foreach (var item in feed.Items)
            {
                News newNews = new News();

                newNews.Title = item.Title;
                newNews.Link = item.Link;
                newNews.Description = item.Description.Remove(0, item.Description.IndexOf("</a>") + 4);
                //System.DateTime.Parse(item.PublishingDateString);
                newNews.PubDate = System.DateTime.Parse(item.PublishingDateString);
                newList.Add(newNews);
            }

            return newList;
        }
    }
}

