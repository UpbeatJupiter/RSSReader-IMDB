using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace RSSWebApp.Data.Entity
{
    public class Cast
    {
        public Cast()
        {
            Awards = new List<Award>();
            MovieCastRoles = new List<MovieCastRole>();
        }

        /// <summary>
        /// Oyuncu Id
        /// </summary>
        [Column("Id")]
        public int CastId { get; set; }

        /// <summary>
        /// Oyuncu Adı
        /// </summary>
        [Column("Name")]
        public string CastName { get; set; }

        /// <summary>
        /// Oyuncu Detayları
        /// </summary>
        [Column("Details")]
        public string CastDetails { get; set; }

        /// <summary>
        /// Oyuncu Fotografı
        /// </summary>
        [Column("Photo")]
        public string? CastPhoto { get; set; }


        /// <summary>
        /// Cast IMDB Linki
        /// </summary>
        [Column("Link")]
        public string CastLink { get; set; }

        public IList<Award> Awards { get; set; }
        public IList<MovieCastRole> MovieCastRoles { get; set; }
    }

}

