using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class MovieCastRole
    {
        /// <summary>
        /// Film Id
        /// </summary>
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        /// <summary>
        /// Oyuncu Id
        /// </summary>
        public int CastId { get; set; }

        [ForeignKey("CastId")]
        public Cast Cast { get; set; }

        /// <summary>
        /// Oyuncu Rolü Id
        /// </summary>
        public int CastRoleId { get; set; }

        [ForeignKey("CastRoleId")]
        public CastRole CastRole { get; set; }
    }

}

