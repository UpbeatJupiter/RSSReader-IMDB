using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class Movie
    {
        /// <summary>
        /// Film Id
        /// </summary>
        [Key]
        [Column("Id")]
        public int MovieId { get; set; }

        /// <summary>
        /// Film Adı
        /// </summary>
        [Column("Name")]
        public string MovieName { get; set; }

        /// <summary>
        /// Film Yılı
        /// </summary>
        [Column("Year")]
        [MaxLength(50)]
        public string MovieYear { get; set; }

        /// <summary>
        /// Film Puanı
        /// </summary>
        [Column("Rate")]
        [MaxLength(50)]
        public string MovieRate { get; set; }

        /// <summary>
        /// Film IMDB Linki
        /// </summary>
        [Column("Link")]
        public string MovieLink { get; set; }

        public IList<MovieCastRole> MovieCastRoles { get; set; }
        public IList<MovieCategory> MovieCategories { get; set; }

    }
}

