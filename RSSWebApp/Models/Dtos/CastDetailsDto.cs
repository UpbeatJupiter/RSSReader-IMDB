using System;
using RSSWebApp.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Models.Dtos
{
	public class CastDetailsDto
	{

        public CastDto CastInfo { get; set; }
        public List<MovieDto> MovieList { get; set; }
    }
}

