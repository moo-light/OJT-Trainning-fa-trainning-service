using Application.ViewModels.ApplicationViewModels;
using Domain.Enums.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.ApplicationModels
{
    /// <summary>
    /// This Models will help ViewAllApplication with many type of filter
    /// </summary>
    public class ApplicationFilterDTO
    {
        private string? _search = "";

        /// <summary>
        /// filter with user id if provided. default Guid Empty
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// filter with date acording to ByDateType. default = Datetime.Minvalue
        /// </summary>
        public DateTime FromDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// filter with date acording to ByDateType. default = Datetime.Minvalue
        /// </summary>
        public DateTime ToDate { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// filter with application that is approved or not. default null
        /// </summary>
        public bool? Approved { get; set; } = default;
        /// <summary>
        /// filter with searchString default = null
        /// </summary>
        public string Search { get => _search; set => _search = value.Trim(); }
        
        /// <summary>
        /// Filter by Request Date or Created Date default CreationDate
        /// </summary>
        public string ByDateType { get; set; } = nameof(ApplicationFilterByEnum.CreationDate);
    }
}
