using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class MovieCategory
    {
        /// <summary>
        /// Film Id
        /// </summary>
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        /// <summary>
        /// Kategori Id
        /// </summary>
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

