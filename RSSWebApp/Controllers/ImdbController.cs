using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using RSSWebApp.Models;
using static Azure.Core.HttpHeader;
using RSSWebApp.Data.Context;
using System.Text.RegularExpressions;
using RSSWebApp.Data.Entity;
using System.IO;
using RSSWebApp.ViewModels;
using RSSWebApp.Models.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RSSWebApp.Controllers
{
    public class ImdbController : Controller
    {
        // GET: /<controller>/
        public IActionResult IndexImdb()
        {
            using (IMDBContext context = new IMDBContext())
            {
                return View(context.Movies.ToList());
            }

        }

        [HttpPost]
        public ActionResult GetMovies(string movieName)
        {
            WebClient wc = new WebClient();
            WebClient wc2 = new WebClient();
            WebClient wc3 = new WebClient();
            WebClient wc4 = new WebClient();

            var dbContext = new RSSWebApp.Data.Context.IMDBContext();

            List<Movie> movieList = new List<Movie>();            //Movie list for Movie table
            List<Cast> directorList = new List<Cast>();           //String list for directors
            List<Cast> writerList = new List<Cast>();             //String list for writers
            List<Cast> starList = new List<Cast>();               //String list for star
            List<MovieCastRole> newMappingList = new List<MovieCastRole>();
            List<MovieCastRole> newCastList = new List<MovieCastRole>();


            List<Movie> allMovieList = dbContext.Movies
                .ToList();
            var allCastList = dbContext.Casts
                .ToList();
            List<CastRole> castRoleList = dbContext.CastRoles
                .ToList();

            int directorRoleId = castRoleList.Where(x => x.CastRoleName == "Director").Select(x => x.CastRoleId).FirstOrDefault();
            int writerRoleId = castRoleList.Where(x => x.CastRoleName == "Writer").Select(x => x.CastRoleId).FirstOrDefault();
            int starRoleId = castRoleList.Where(x => x.CastRoleName == "Star").Select(x => x.CastRoleId).FirstOrDefault();

            // entity does not exists in database
            if (movieList == null || movieList.Count() <= 0)
            {
                string content = wc.DownloadString("https://www.imdb.com/find/?q=" + movieName.Trim() + "&ref_=nv_sr_sm");

                var strResult = content.Replace("\n", "").Replace("\r", "");
                var cleanedMoviePage = content.Replace("\n", "").Replace("\r", "");

                int titleIndex = 0;
                int titleStartIndex = 0;
                int titleEndIndex = 0;
                var title = "";

                int linkIndex = 0;
                int linkStartIndex = 0;
                int linkEndIndex = 0;
                var link = "";


                linkIndex = strResult.IndexOf("find-title-result"); //to find link and title, we'll delete before this ilk linkten öncesini temizleyip texti küçültmek
                strResult = strResult.Substring(linkIndex); //get everything after linkIndex variable

                while (true)
                {
                    var movie = new Movie();
                    titleIndex = strResult.IndexOf("fn_al_tt_"); //index of f
                    linkIndex = strResult.IndexOf("find-title-result");

                    if (titleIndex <= 0)
                    {
                        break;
                    }

                    titleStartIndex = strResult.IndexOf('>', titleIndex) + 1; // index of first occurence of > starting from f's index. 
                    titleEndIndex = strResult.IndexOf('<', titleStartIndex); // index of first occurence of < starting from startIndex's index. 

                    title = strResult.Substring(titleStartIndex, titleEndIndex - titleStartIndex);
                    movie.MovieName = title;


                    //To do: movie nin linkini çekip rate ve yılına ulaşıp setleyeceğim.
                    linkStartIndex = strResult.IndexOf("href=\"/", linkIndex) + 6; //index of first occurence of href=" starting from linkIndex's index 
                    linkEndIndex = strResult.IndexOf('"', linkStartIndex); // index of first occurence of " after start index 

                    link = strResult.Substring(linkStartIndex, linkEndIndex - linkStartIndex); //this gets movies link without /*https://www.imdb.com*/
                    link = "https://www.imdb.com" + link;


                    //First remove first occurence of find-title-result and replace it with empty string. Finally start searching after remained string

                    strResult = strResult.Remove(0, titleEndIndex); //start searching from new string


                    var existMovie = allMovieList.Where(x => x.MovieLink == link).FirstOrDefault();

                    if (existMovie != null && existMovie.MovieId > 0)
                    {
                        movieList.Add(existMovie);
                        continue;
                    }

                    movie.MovieLink = link;

                    //link var mı db de name i getir
                    //yoksa db ye kaydet

                    //go to movie page to get year and rate
                    string contentPage = wc2.DownloadString(link);
                    var movieDetailStrResult = contentPage.Replace("\n", "").Replace("\r", "");


                    //For rate use tt_ov_rdat
                    int yearIndex = 0;
                    int yearStartIndex = 0;
                    int yearEndIndex = 0;
                    var year = "-"; //find year string

                    yearIndex = movieDetailStrResult.IndexOf("tt_ov_rdat");
                    if (yearIndex >= 0)
                    {
                        yearStartIndex = movieDetailStrResult.IndexOf('>', yearIndex) + 1;
                        yearEndIndex = movieDetailStrResult.IndexOf('<', yearIndex);

                        year = movieDetailStrResult.Substring(yearStartIndex, yearEndIndex - yearStartIndex);
                    }

                    movie.MovieYear = year;

                    //For rate use iZlgcd
                    int rateIndex = 0;
                    int rateStartIndex = 0;
                    int rateEndIndex = 0;
                    var rate = "-"; //find rate string

                    rateIndex = movieDetailStrResult.IndexOf("hero-rating-bar__aggregate-rating__score");
                    if (rateIndex >= 0)
                    {
                        rateStartIndex = movieDetailStrResult.IndexOf('>', rateIndex) + 2;       //To find second occurence of > , we get first occurence index
                        rateStartIndex = movieDetailStrResult.IndexOf('>', rateStartIndex) + 1;  //and start searching second > after the first one
                        rateEndIndex = movieDetailStrResult.IndexOf('<', rateStartIndex);

                        rate = movieDetailStrResult.Substring(rateStartIndex, rateEndIndex - rateStartIndex);

                    }
                    movie.MovieRate = rate;

                    movieList.Add(movie);         //Add values of movies to movielist
                    dbContext.Movies.Add(movie);

                    var castSearchStartIndex = movieDetailStrResult.IndexOf("plot", rateEndIndex);
                    movieDetailStrResult = movieDetailStrResult.Remove(0, castSearchStartIndex);

                    var castSearchEndIndex = movieDetailStrResult.IndexOf("title-pc-expandable-panel");
                    movieDetailStrResult = movieDetailStrResult.Remove(castSearchEndIndex);

                    while (movieDetailStrResult.Contains("tt_ov_dr")) //To find directors, writers, stars in a movie
                    {
                        var cast = new Cast();
                        cast.CastPhoto = "";
                        cast.CastDetails = "";

                        //For director use tt_ov_dr
                        int directorIndex = 0;
                        int directorStartIndex = 0;
                        int directorEndIndex = 0;

                        var directorLink = ""; //check if the director only once occure
                        int directorLinkIndex = 0;
                        int directorLinkStartIndex = 0;
                        int directorLinkEndIndex = 0;

                        directorLinkStartIndex = movieDetailStrResult.IndexOf("href=\"/", directorLinkIndex) + 6;
                        directorLinkEndIndex = movieDetailStrResult.IndexOf('"', directorLinkStartIndex); // index of first occurence of " after start index 

                        directorLink = movieDetailStrResult.Substring(directorLinkStartIndex, directorLinkEndIndex - directorLinkStartIndex); //this gets movies link without /*https://www.imdb.com*/
                        directorLink = "https://www.imdb.com" + directorLink;


                        directorIndex = movieDetailStrResult.IndexOf("tt_ov_dr"); //director name

                        directorStartIndex = movieDetailStrResult.IndexOf('>', directorIndex) + 1;
                        directorEndIndex = movieDetailStrResult.IndexOf('<', directorIndex);

                        cast.CastName = movieDetailStrResult.Substring(directorStartIndex, directorEndIndex - directorStartIndex);
                        cast.CastLink = directorLink;

                        if (cast.CastName != null) //cast details
                        {
                            string directorDetailContent = wc3.DownloadString(cast.CastLink);

                            var strCastDetailResult = directorDetailContent.Replace("\n", "").Replace("\r", "");
                            var castPhotoIndex = strCastDetailResult.IndexOf("nm_ov_ph") - 53;

                            if (castPhotoIndex >= 0)
                            {
                                var castPhotoStartIndex = strCastDetailResult.IndexOf("href=\"/", castPhotoIndex) + 6;
                                var castPhotoEndIndex = strCastDetailResult.IndexOf('"', castPhotoStartIndex);

                                cast.CastPhoto = strCastDetailResult.Substring(castPhotoStartIndex, castPhotoEndIndex - castPhotoStartIndex);
                                cast.CastPhoto = "https://www.imdb.com" + cast.CastPhoto;
                            }
                            

                            var castDetailSearchStartIndex = strCastDetailResult.IndexOf("bio-content");

                            if (castDetailSearchStartIndex >= 0)
                            {
                                strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);
                                castDetailSearchStartIndex = strCastDetailResult.IndexOf("ipc-html-content-inner-div") + 28;
                                strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);


                                var castDetailSearchEndIndex = strCastDetailResult.IndexOf("</div>");
                                strCastDetailResult = strCastDetailResult.Remove(castDetailSearchEndIndex);

                                cast.CastDetails = strCastDetailResult;

                                string directorPhotoContent = wc4.DownloadString(cast.CastPhoto);
                                var strCastPhotoResult = directorPhotoContent.Replace("\n", "").Replace("\r", "");
                                strCastPhotoResult = strCastPhotoResult.Replace("\n", "").Replace("\r", "");

                                var castPhotoResultIndex = strCastPhotoResult.IndexOf("media-viewer__touch-handler");

                                if (castPhotoResultIndex >= 0)
                                {
                                    strCastPhotoResult = strCastPhotoResult.Remove(0, castPhotoResultIndex);
                                    castPhotoResultIndex = strCastPhotoResult.IndexOf("img") + 4;
                                    strCastPhotoResult = strCastPhotoResult.Remove(0, castPhotoResultIndex);

                                    castPhotoResultIndex = strCastPhotoResult.IndexOf("img") + 4;
                                    strCastPhotoResult = strCastPhotoResult.Remove(0, castPhotoResultIndex);

                                    castPhotoResultIndex = strCastPhotoResult.IndexOf("img=") + 6;
                                    var castPhotoResultEndIndex = strCastPhotoResult.IndexOf("jpg") + 3;
                                    cast.CastPhoto = strCastPhotoResult.Substring(castPhotoResultIndex, castPhotoResultEndIndex - castPhotoResultIndex);
                                    
                                }

                            }


                        }

                        var existCastDirector = allCastList.Where(x => x.CastLink == directorLink).FirstOrDefault();

                        if (existCastDirector == null)
                        {
                            directorList.Add(cast);

                        }
                        else
                        {
                            existCastDirector.CastName = cast.CastName;
                            existCastDirector.CastDetails = cast.CastDetails;
                            cast = existCastDirector;
                        }

                        MovieCastRole movieCastRole = new MovieCastRole() { Cast = cast, Movie = movie, CastRoleId = directorRoleId };
                        newMappingList.Add(movieCastRole);

                        movieDetailStrResult = movieDetailStrResult.Remove(0, directorEndIndex); //start searching from new string


                    }
                    while (movieDetailStrResult.Contains("tt_ov_wr"))
                    {
                        var cast = new Cast();
                        cast.CastPhoto = "";
                        cast.CastDetails = "";

                        //For writer use tt_ov_wr
                        int writerIndex = 0;
                        int writerStartIndex = 0;
                        int writerEndIndex = 0;

                        var writerLink = ""; //check if the director only once occure
                        int writerLinkIndex = 0;
                        int writerLinkStartIndex = 0;
                        int writerLinkEndIndex = 0;

                        writerLinkStartIndex = movieDetailStrResult.IndexOf("href=\"/", writerLinkIndex) + 6;
                        writerLinkEndIndex = movieDetailStrResult.IndexOf('"', writerLinkStartIndex); // index of first occurence of " after start index 

                        writerLink = movieDetailStrResult.Substring(writerLinkStartIndex, writerLinkEndIndex - writerLinkStartIndex); //this gets movies link without /*https://www.imdb.com*/
                        writerLink = "https://www.imdb.com" + writerLink;



                        writerIndex = movieDetailStrResult.IndexOf("tt_ov_wr");

                        writerStartIndex = movieDetailStrResult.IndexOf('>', writerIndex) + 1;
                        writerEndIndex = movieDetailStrResult.IndexOf('<', writerIndex);

                        cast.CastName = movieDetailStrResult.Substring(writerStartIndex, writerEndIndex - writerStartIndex);
                        cast.CastLink = writerLink;

                        if (cast.CastName != null) //cast details
                        {
                            string writerDetailContent = wc3.DownloadString(cast.CastLink);

                            var strCastDetailResult = writerDetailContent.Replace("\n", "").Replace("\r", "");

                            var castDetailSearchStartIndex = strCastDetailResult.IndexOf("bio-content");
                            if (castDetailSearchStartIndex >= 0)
                            {
                                strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);
                                castDetailSearchStartIndex = strCastDetailResult.IndexOf("ipc-html-content-inner-div") + 28;
                                strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);


                                var castDetailSearchEndIndex = strCastDetailResult.IndexOf("</div>");
                                strCastDetailResult = strCastDetailResult.Remove(castDetailSearchEndIndex);



                                cast.CastDetails = strCastDetailResult;

                            }

                        }

                        var existCastWriter = allCastList.Where(x => x.CastLink == writerLink).FirstOrDefault();


                        if (existCastWriter == null)
                        {
                            writerList.Add(cast);              //add writers into the writer list

                        }
                        else
                        {
                            existCastWriter.CastName = cast.CastName;
                            existCastWriter.CastDetails = cast.CastDetails;
                            cast = existCastWriter;
                        }

                        MovieCastRole movieCastRole = new MovieCastRole() { Cast = cast, Movie = movie, CastRoleId = writerRoleId };
                        newMappingList.Add(movieCastRole);

                        movieDetailStrResult = movieDetailStrResult.Remove(0, writerEndIndex);
                    }

                    while (movieDetailStrResult.Contains("tt_ov_st"))
                    {

                        var cast = new Cast();
                        cast.CastPhoto = "";
                        cast.CastDetails = "";
                        //For star use tt_ov_st
                        int starIndex = 0;
                        int starStartIndex = 0;
                        int starEndIndex = 0;

                        var starLink = ""; //check if the director only once occure
                        int starLinkIndex = 0;
                        int starLinkStartIndex = 0;
                        int starLinkEndIndex = 0;

                        starLinkStartIndex = movieDetailStrResult.IndexOf("href=\"/", starLinkIndex) + 6;
                        starLinkEndIndex = movieDetailStrResult.IndexOf('"', starLinkStartIndex); // index of first occurence of " after start index 

                        starLink = movieDetailStrResult.Substring(starLinkStartIndex, starLinkEndIndex - starLinkStartIndex); //this gets movies link without /*https://www.imdb.com*/
                        starLink = "https://www.imdb.com" + starLink;



                        starIndex = movieDetailStrResult.IndexOf("tt_ov_st");

                        starStartIndex = movieDetailStrResult.IndexOf('>', starIndex) + 1;
                        starEndIndex = movieDetailStrResult.IndexOf('<', starIndex);

                        cast.CastName = movieDetailStrResult.Substring(starStartIndex, starEndIndex - starStartIndex);
                        cast.CastLink = starLink;

                        if (cast.CastName != null) //cast details
                        {
                            string starDetailContent = wc3.DownloadString(cast.CastLink);

                            var strCastDetailResult = starDetailContent.Replace("\n", "").Replace("\r", "");
                            if (strCastDetailResult.Contains("bio-content"))
                            {
                                var castDetailSearchStartIndex = strCastDetailResult.IndexOf("bio-content");
                                if (castDetailSearchStartIndex >= 0)
                                {
                                    strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);
                                    castDetailSearchStartIndex = strCastDetailResult.IndexOf("ipc-html-content-inner-div") + 28;
                                    strCastDetailResult = strCastDetailResult.Remove(0, castDetailSearchStartIndex);


                                    var castDetailSearchEndIndex = strCastDetailResult.IndexOf("</div>");
                                    strCastDetailResult = strCastDetailResult.Remove(castDetailSearchEndIndex);



                                    cast.CastDetails = strCastDetailResult;
                                }
                                
                            }
                            

                        }

                        var existCastStar = allCastList.Where(x => x.CastLink == starLink).FirstOrDefault();

                        if (cast.CastLink.Contains("name"))
                        {
                            if (existCastStar == null)
                            {
                                starList.Add(cast);              //add star into the star list
                            }
                            else
                            {
                                existCastStar.CastName = cast.CastName;
                                existCastStar.CastDetails = cast.CastDetails;
                                cast = existCastStar;
                            }

                            MovieCastRole movieCastRole = new MovieCastRole() { Cast = cast, Movie = movie, CastRoleId = starRoleId };
                            newMappingList.Add(movieCastRole);

                        }

                        movieDetailStrResult = movieDetailStrResult.Remove(0, starEndIndex);
                    }
                }
        
                dbContext.MovieCastRoles.AddRange(newMappingList);
                dbContext.SaveChanges();

            }

            ViewBag.movieList = movieList.ToList();
            return View("Views/IMDB/Movies.cshtml");
        }


        [HttpPost]
        public ActionResult GetCasts(string castName)
        {
            IMDBContext context = new IMDBContext();


            //var castDbList = context.Casts.ToList();

            var moviesOfCast = new CastDetailsDto();


            var castInfo = context.Casts
                .Where(x => x.CastName == castName)
                .Select(x => new CastDto()
                {
                    CastId = x.CastId,
                    CastName = x.CastName,
                    CastDetails = x.CastDetails,
                    CastPhoto = x.CastPhoto

                }).FirstOrDefault();


            

            if (castInfo != null )
            {
                moviesOfCast.CastInfo = castInfo;

                var movieIds = context.MovieCastRoles
                                .Where(x => x.CastId == castInfo.CastId)
                                .Select(x => x.MovieId)
                                .Distinct()
                                .ToList();


                var movies = context.Movies
                    .Where(x => movieIds.Contains(x.MovieId))
                    .Select(x => new MovieDto()
                    {
                        MovieId = x.MovieId,
                        MovieName = x.MovieName,
                        MovieRate = x.MovieRate,
                        MovieYear = x.MovieYear
                    }).ToList();

                moviesOfCast.MovieList = movies;
            }
            return View("Views/IMDB/CastDetails.cshtml", moviesOfCast);



        }


    }
}