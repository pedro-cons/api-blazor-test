using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class TokenDTO
{
    public DataToken data { get; set; }
    public bool sucess { get; set; }
    public DateTime Time { get; set; }
    public int? id { get; set; }
    public List<object> information { get; set; }
    public List<string> error { get; set; }

    public bool IsExpired() =>
        DateTime.UtcNow >= data.validoTo;
}