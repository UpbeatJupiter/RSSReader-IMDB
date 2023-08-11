using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class Award
    {
        /// <summary>
        /// Ödül Id
        /// </summary>
        [Column("Id")]
        public int AwardId { get; set; }

        /// <summary>
        /// Ödül Adı
        /// </summary>
        [Column("Name")]
        public string AwardName { get; set; }

        /// <summary>
        /// Ödül Yılı
        /// </summary>
        [Column("Year")]
        public string AwardYear { get; set; }

        public Cast Cast { get; set; }
    }

}

