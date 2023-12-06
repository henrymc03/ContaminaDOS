//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Reflection.Metadata;
//using Videoteca_API.Data;
//using Videoteca_API.Models;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Videoteca_API.Controllers
//{

//    [Route("api/[controller]")]
//    [ApiController]
//    public class MovieServiceController : Controller
//    {

//        private VideotecaContext db = new VideotecaContext();

//        // POSTMovie api/<MovieServiceController>
//        [HttpPost]
//        public ActionResult PostMovie([FromBody] MoviesAndSeries movisData)
//        {

//            try
//            {

//                db.MoviesAndSeries.Add(movisData);
//                db.SaveChanges();


//                var movies = new List<MoviesAndSeries>();

//                movies = db.MoviesAndSeries.FromSqlRaw(@"exec dbo.GetMovieDataForTitle @title", new SqlParameter("@title", movisData.title)).ToList();

//                var movie = movies.FirstOrDefault();

//                var idMovie = movie.id;


//                return Ok(db.MoviesAndSeries);


//            }
//            catch (Exception ex)
//            {
//                return BadRequest("Error registering the movie: " + ex);
//            }
//        }

//        // PUT api/<MovieServiceController>/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, MoviesAndSeries movisData)
//        {


//            try
//            {
//                var MovieEdit = new List<MoviesAndSeries>();
//                var parameter = new List<SqlParameter>();
//                parameter.Add(new SqlParameter("@Id_Movie", movisData.id));
//                parameter.Add(new SqlParameter("@title", movisData.title));
//                parameter.Add(new SqlParameter("@synopsis", movisData.synopsis));
//                parameter.Add(new SqlParameter("@releaseYear", movisData.release_year));
//                parameter.Add(new SqlParameter("@duration", movisData.duration));
//                parameter.Add(new SqlParameter("@classification", movisData.classification));
//                parameter.Add(new SqlParameter("@director", movisData.director));
//                parameter.Add(new SqlParameter("@movie_url", movisData.movie_url));


//                var result = Task.Run(() => db.Database
//                .ExecuteSqlRaw(@"exec dbo.EditMoviesAndSerie @Id_Movie, @title, @synopsis, @releaseYear, @duration,
//                @classification, @director, @movie_url",
//                parameter.ToArray()));

//                result.Wait();

//                db.SaveChanges();

//                return Ok(movisData);

//            }
//            catch (Exception ex)
//            {
//                return BadRequest("Update Not Success");
//            }


//        }


//        // POST: MovieController/Delete/5
//        [HttpDelete("{id_delete}")]
//        public ActionResult Delete([FromRoute] int id_delete)
//        {
//            try
//            {

//                var movie = db.MoviesAndSeries.Where(c => c.id == id_delete).FirstOrDefault();

//                if (movie == null)
//                {
//                    return BadRequest("The movie was not found or");
//                }

//                var parameter = new List<SqlParameter>();
//                parameter.Add(new SqlParameter("@Id_Movie", id_delete));

//                var result = Task.Run(() => db.Database
//                .ExecuteSqlRaw(@"exec dbo.DeleteMoviesAndSerie @Id_Movie", parameter.ToArray()));
//                result.Wait();

//                db.SaveChanges();

//                return Ok(movie + "Delete Movie Or Serie Success");

//            }
//            catch (Exception ex)
//            {

//                return BadRequest("Error: " + ex);

//            }



//        }
//    }

    //[HttpPost]
    //public ActionResult PostEpisode([FromBody] Episode episode)
    //{

    //    try
    //    {

    //        db.Episodes.Add(episode);
    //        db.SaveChanges();

    //        var result = Task.Run(() => db.Database
    //        .ExecuteSqlRaw(@"exec dbo.updateNumEpisodes @id_serie", new SqlParameter("@id_serie", episode.movies_series_id)));

    //        result.Wait();


    //        return Ok(db.MoviesAndSeries);


    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest("Error registering the episode: " + ex);
    //    }
    //}

    //[HttpGet]
    //public ActionResult<List<Episode>> Get()
    //{
    //    try
    //    {
    //        return db.Episodes.ToList();
    //    }

    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }

    //}


    //    }
//}