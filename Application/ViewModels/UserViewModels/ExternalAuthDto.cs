using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels;

public class ExternalAuthDto
{
    public string Provider { get; set; } = "GOOGLE";
    public string? IdToken { get; set; }
}
