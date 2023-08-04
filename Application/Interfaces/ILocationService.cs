using Application.ViewModels.Location;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILocationService
    {
        public Task<LocationDTO> AddNewLocation(CreateLocationDTO createLocationDTO);
        public Task<List<LocationDTO>> GetAllLocation();
    }
}
