using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Data.Entity
{
    public class CastRole
    {
        /// <summary>
        /// Cast Role Id
        /// </summary>
        [Column("Id")]
        public int CastRoleId { get; set; }

        /// <summary>
        /// Cast Role Name
        /// </summary>
        [Column("Name")]
        public string CastRoleName { get; set; }

        public IList<MovieCastRole> MovieCastRoles { get; set; }
    }

}

