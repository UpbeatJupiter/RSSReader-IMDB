function imdbSearch(name) {
    $("#resultDiv").load("/Imdb/GetMovies",
        {
            movieName: name
        },
        function (response) {

        });
}

function castSearch(name) {
    
    $("#resultDiv").load("/Imdb/GetCasts",
        {
            castName: name
        },
        function (response) {

        });
}
