function imdbSearch(name) {
    debugger
    $("#resultDiv").load("/Imdb/GetMovies",
        {
            movieName: name
        },
        function (response) {

        });
}

function castSearch(name) {
    debugger
    $("#resultDiv").load("/Imdb/GetCasts",
        {
            castName: name
        },
        function (response) {

        });
}
