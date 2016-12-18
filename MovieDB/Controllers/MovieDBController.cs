using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MovieDB
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class MovieDBController : ApiController
	{
		private static string api_key = "da323c2cab7bae5c2246f0d04951e28f";
		static HttpClient client = new HttpClient();

		public MovieDBResponse Get()
		{
			var query_params = Request.GetQueryNameValuePairs();
			var actor_param = query_params.FirstOrDefault(param => String.Equals(param.Key, "actor", StringComparison.CurrentCulture));
			if (actor_param.Equals(default(KeyValuePair<string, string>)) == false)
			{
				var actor_response = GetData<MovieDBResponse>(buildUriForActorInfo(actor_param.Value)).Result;
				var actor_id = 31;
				if ( actor_response != null && actor_response.results.Count > 0)
				{
					actor_id = actor_response.results.ElementAt(0).id;
				}
				var cast = GetData<Cast>(buildUriForMovieCredits(actor_id)).Result;
				var movieDBResponse = new MovieDBResponse();
				movieDBResponse.page = 1;
				movieDBResponse.total_pages = 1;
				movieDBResponse.total_results = 0;
				var results = new List<MovieDBResult>();
				if (cast == null)
				{
					movieDBResponse.results = results;
					return movieDBResponse;
				}
				cast.cast.ForEach(castInfo =>
					{
						var movie_result = GetData<MovieDBResult>(buildUriForMovie(castInfo.id)).Result;
						if (movie_result != null)
						{
							results.Add(movie_result);
							movieDBResponse.total_results++;
						}
					});
				movieDBResponse.results = results;
				return movieDBResponse;
			}
			else
			{
				return GetData<MovieDBResponse>(buildUriForPopularMovies()).Result;
			}
		}

		static UriBuilder baseUri()
		{
			var builder = new UriBuilder("https://api.themoviedb.org");
			builder.Port = -1;
			var query = HttpUtility.ParseQueryString(builder.Query);
			query["api_key"] = api_key;
			builder.Query = query.ToString();
			return builder;
		}

		static string buildUriForPopularMovies()
		{
			var builder = baseUri();
			builder.Path = "/3/movie/popular";
			return builder.ToString();
		}

		static string buildUriForActorInfo(string actor)
		{
			var builder = baseUri();
			var query = HttpUtility.ParseQueryString(builder.Query);
			builder.Path = "/3/search/person";
			query["query"] = actor;
			builder.Query = query.ToString();
			return builder.ToString();
		}

		static string buildUriForMovieCredits(int actorId)
		{
			var builder = baseUri();
			builder.Path = "/3/person/" + actorId + "/movie_credits";
			return builder.ToString();
		}

		static string buildUriForMovie(int movieId)
		{
			var builder = baseUri();
			builder.Path = "/3/movie/" + movieId;
			return builder.ToString();
		}

		static async Task<T> GetData<T>(string uri)
		{
			T data = default(T);
			HttpResponseMessage response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadAsAsync<T>();
			}
			return data;
		}
	}
}
