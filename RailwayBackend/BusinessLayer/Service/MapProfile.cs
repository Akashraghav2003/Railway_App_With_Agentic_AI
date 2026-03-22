using AutoMapper;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<UserDTO, User>().ReverseMap();

            // Train ↔ TrainResponseDTO
            CreateMap<Train, TrainResponseDTO>()
                .ForMember(dest => dest.TrainClasses, opt => opt.MapFrom(src => src.TrainClasses))
                .ReverseMap();

            // TrainClass ↔ TrainClassResponseDTO
            CreateMap<TrainClass, TrainClassResponseDTO>()
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassId))
                .ReverseMap();



            // Train ↔ TrainDTO
            CreateMap<Train, TrainDTO>()

                .ForMember(dest => dest.TrainClass, opt => opt.MapFrom(src => src.TrainClasses))
                .ReverseMap();

            // TrainClass ↔ TrainClassDTO
            CreateMap<TrainClass, TrainClassDTO>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassName))
                .ReverseMap();

            // Reservation and ReservationDTO
            CreateMap<Reservation, ReservationDTO>()
                .ForMember(dest => dest.Passenger, opt => opt.MapFrom(src => src.Passenger))
                .ReverseMap();

            // Passenger and PassengerDTO
            CreateMap<Passenger, PassengerDTO>().ReverseMap();

            CreateMap<Cancellation, CancellationDTO>().ReverseMap();

            // Reservation ↔ ReservationResponse
            CreateMap<Reservation, ReservationResponse>()
                .ForMember(dest => dest.Passengers, opt => opt.MapFrom(src => src.Passenger))
                .ForMember(dest => dest.TrainName, opt => opt.MapFrom(src => src.Train != null ? src.Train.TrainName : string.Empty))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Train != null ? src.Train.SourceStation : string.Empty))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Train != null ? src.Train.DestinationStation : string.Empty))
                .ReverseMap();
            CreateMap<Passenger, PassengerResponse>().ReverseMap();


        }
    }
}