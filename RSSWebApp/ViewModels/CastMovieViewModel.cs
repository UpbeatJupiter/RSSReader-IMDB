using System;
using RSSWebApp.Data.Entity;
namespace RSSWebApp.ViewModels
{
    public class CastMovieViewModel
    {
        public List<Cast> CastsVM { get; set; } //Dto olsun ekranada dtolar ile gelsin
        public List<MovieCastRole> MovieCastRolesVM { get; set; }
        public List<Movie> MoviesVM { get; set; }

    }
}

