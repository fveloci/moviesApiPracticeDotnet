using APIPeliculas.DTOs;
using APIPeliculas.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace APIPeliculas.Utils
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory) {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(x => x.Photo, options => options.Ignore());
            CreateMap<CinemaCreationDTO, Cinema>()
                .ForMember(x => x.Location, x => x.MapFrom(dto => 
                geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude)))).ReverseMap();
            CreateMap<Cinema, CinemaDTO>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(field => field.Location.Y))
                .ForMember(x => x.Longitude, dto => dto.MapFrom(field => field.Location.X));
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MovieGenres, options => options.MapFrom(MapMovieGenres))
                .ForMember(x => x.MovieCinemas, options => options.MapFrom(MapMovieCinemas))
                .ForMember(x => x.MovieActors, options => options.MapFrom(MapMovieActor));

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.OnCinema, options => options.MapFrom(src => src.OnCinema == "true"))
                .ForMember(x => x.Genres, options => options.MapFrom(MapMovieGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMovieActor))
                .ForMember(x => x.Cinemas, options => options.MapFrom(MapMovieCinemas));

            CreateMap<IdentityUser, UserDTO>();

        }

        private List<GenreDTO> MapMovieGenres(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<GenreDTO>();

            if (movie.MovieGenres != null)
            {
                foreach(var item in movie.MovieGenres)
                {
                    result.Add(new GenreDTO() { Id = item.GenreId, Name = item.Genre.Name });
                }
            }

            return result;
        }

        private List<MovieActorDTO> MapMovieActor(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<MovieActorDTO>();

            if (movie.MovieActors != null)
            {
                foreach (var item in movie.MovieActors)
                {
                    result.Add(new MovieActorDTO() { Id = item.ActorId, Name = item.Actor.Name, Character = item.Character, Order = item.Order, Photo = item.Actor.Photo });
                }
            }

            return result;
        }

        private List<CinemaDTO> MapMovieCinemas(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<CinemaDTO>();

            if (movie.MovieCinemas != null)
            {
                foreach (var item in movie.MovieCinemas)
                {
                    result.Add(new CinemaDTO() { 
                        Id = item.CinemaId, 
                        Name = item.Cinema.Name,
                        Latitude = item.Cinema.Location.Y,
                        Longitude = item.Cinema.Location.X,
                    });
                }
            }

            return result;
        }

        private List<MovieGenre> MapMovieGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieGenre>();

            if (movieCreationDTO.GenreIds == null) return result;

            foreach (var id in movieCreationDTO.GenreIds)
            {
                result.Add(new MovieGenre() { GenreId = id });
            }
            return result;
        }

        private List<MovieCinema> MapMovieCinemas(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieCinema>();

            if (movieCreationDTO.CinemasIds == null) return result;

            foreach (var id in movieCreationDTO.CinemasIds)
            {
                result.Add(new MovieCinema() { CinemaId = id });
            }
            return result;
        }

        private List<MovieActor> MapMovieActor(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieActor>();

            if (movieCreationDTO.Actors == null) return result;

            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MovieActor() { ActorId = actor.Id, Character = actor.Character });
            }
            return result;
        }
    }
}
