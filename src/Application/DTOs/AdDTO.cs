using Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AdDTO
    {
        public int AdId { get; set; }
        public required string AdDescription { get; set; }
        public DateTime? AdCreationDate { get; set; }
        public EAdStatus AdStatus { get; set; }
        public decimal? AdBalance { get; set; }
        public string? AdExternalId { get; set; }
        public int AdTotalLeads { get; set; }
    }
}
