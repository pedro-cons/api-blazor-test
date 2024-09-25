using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
/// <summary>
/// Return standardization for the API client.
/// </summary>
public class ResponseDTO<T> : ResponseDTO
{
    /// <summary>
    /// Requested content.
    /// </summary>
    public T Data { get; set; }
}


/// <summary>
/// Return standardization for the API client.
/// </summary>
public class ResponseDTO
{
    /// <summary>
    /// Create an instance of <see cref="ResponseDTO"/>.
    /// </summary>
    /// <param name="withTracker">Determines whether an identifier will be sent to the client for exception tracking.</param>
    public ResponseDTO(bool withTracker = false)
    {
        if (withTracker)
            this.Id = Guid.NewGuid();
    }

    /// <summary>
    /// Determines whether the answer produced results (correctly) or not.
    /// </summary>
    public bool Sucess
    {
        get
        {
            return (this.Error?.Count ?? 0) == 0;
        }
    }

    /// <summary>
    /// Date and time the response was generated, to assist in tracking exceptions.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Unique identifier of the response, when fatal exception.
    /// </summary>
    public Guid? Id { get; private set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Additional return information.
    /// </summary>
    public List<string>? Information { get; set; } = new List<string>();

    /// <summary>
    /// Alerts/notices about return. Full alerts can be obtained by logging into the service.
    /// </summary>
    public List<string>? Error { get; set; } = new List<string>();

    /// <summary>
    /// Remover listas não utilizadas.
    /// </summary>
    public void Limpar()
    {
        if (this.Information?.Count == 0)
            this.Information = null;
        if (this.Error?.Count == 0)
            this.Error = null;
    }
}

