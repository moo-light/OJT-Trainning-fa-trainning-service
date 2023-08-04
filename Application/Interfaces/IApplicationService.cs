using Application.Commons;
using Application.Models.ApplicationModels;
using Application.ViewModels.ApplicationViewModels;
using Domain.Entities;
using Domain.Enums.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationService
    {
        public Task<bool> CreateApplication(ApplicationDTO applicationDTO);
        Task<Pagination<Applications>> GetAllApplication(Guid classId, ApplicationFilterDTO filter,int pageindex = 0 ,int pagesize = 10);
        public Task<bool> UpdateStatus(Guid id, bool status);
    }
}
