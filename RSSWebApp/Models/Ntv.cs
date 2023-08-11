using System;
using CodeHollow.FeedReader;

namespace RSSWebApp.Models
{
	public class Ntv : Ajans
	{
        public override List<News> GetNews()
        {
            var feed = FeedReader.Read("https://www.ntv.com.tr/son-dakika.rss");

            
            List<News> newList = new List<News>();

            foreach (var item in feed.Items)
            {
                News newNews = new News();

                newNews.Title = item.Title;
                newNews.Link = item.Link;
                var ntvDesc = item.Content.Remove(0, item.Content.IndexOf("/><p>") + 5);
                var ntvDescEnd = ntvDesc.Remove(ntvDesc.IndexOf("</p>"));
                if (ntvDesc.IndexOf("</p>") >= 0)
                {
                    newNews.Description = ntvDescEnd;
                }
                //newNews.Description = ntvDesc.Remove(item.Content.IndexOf("</p><"));
                newNews.PubDate = System.DateTime.Parse(item.PublishingDateString);
                newList.Add(newNews);
            }

            return newList;
        }
    }
}

