using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class AuthorizationDTO
{
    /// <summary>
    /// JWT token containing user authentication and authorization data.
    /// </summary>
    public string TokenAcess { get; set; }

    /// <summary>
    /// End validity of the user session.
    /// </summary>
    public DateTime ValidTo{ get; set; }
}
