using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class Category
    {
        /// <summary>
        /// Kategori Id
        /// </summary>
        [Column("Id")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Kategori Adı
        /// </summary>
        [Column("Name")]
        public string CategoryName { get; set; }

        public IList<MovieCategory> MovieCategories { get; set; }
    }

}

