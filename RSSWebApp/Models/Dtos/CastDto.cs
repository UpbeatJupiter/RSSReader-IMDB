using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSWebApp.Models.Dtos
{
	public class CastDto
	{
		
        public int CastId { get; set; }

        
        public string CastName { get; set; }

        public string? CastDetails { get; set; }

        
        public string? CastPhoto { get; set; }


    }
}

