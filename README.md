#######################################

A proxy for The Movie Database's (TMDb's) API. It only implements two endpoints:

1) GET '/api/movieDB' - the equivalent of the '/movie/popular' endpoint
2) GET '/api/movieDB?actor' - returns the filmography of the given actor. It combines three endpoints: a) '/search/person?actor' to get the actor id, b) '/person/{actor_id}/movie_credits' to retrieve all movies, and c) '/movie/{movie_id}' to retrieve info on each movie