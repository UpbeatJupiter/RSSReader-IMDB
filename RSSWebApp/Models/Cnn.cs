using System;
using System.Xml;
using CodeHollow.FeedReader;

namespace RSSWebApp.Models
{
	public class Cnn : Ajans
	{
        public override List<News> GetNews()
        {
            var feed = FeedReader.Read("https://www.cnnturk.com/feed/rss/turkiye/news");

            //var atom = (CodeHollow.FeedReader.Feeds.AtomFeed)feed.SpecificFeed;

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

