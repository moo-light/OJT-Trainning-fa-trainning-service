using Application.Interfaces;
using Application.ViewModels.Location;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LocationDTO> AddNewLocation(CreateLocationDTO createLocationDTO)
        {
            var location = _mapper.Map<Location>(createLocationDTO);
            if (_unitOfWork.LocationRepository.GetByNameAsync(createLocationDTO.LocationName).Result != null)
            {
                return null;
            }

            await _unitOfWork.LocationRepository.AddAsync(location);

            return (await _unitOfWork.SaveChangeAsync() > 0) ? _mapper.Map<LocationDTO>(location) : null;
        }
        public async Task<List<LocationDTO>> GetAllLocation()
        {
            var location = await _unitOfWork.LocationRepository.GetAllAsync();
            var result = _mapper.Map<List<LocationDTO>>(location);
            return result;
        }

    }
}
