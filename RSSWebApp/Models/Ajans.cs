using System;
namespace RSSWebApp.Models
{
	public abstract class Ajans
	{
		public string Title { get; set; }
		public string Desc { get; set; }
        public string Link { get; set; }
		public abstract List<News> GetNews();
    }
}

