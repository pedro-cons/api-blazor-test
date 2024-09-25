using Domain.Enumerators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Ad
{
    public int AdId { get; set; }
    [StringLength(500)]
    public required string AdDescription { get; set; }
    public DateTime AdCreationDate { get; set; }
    public EAdStatus AdStatus { get; set; }
    public decimal? AdBalance { get; set; }
    [StringLength(500)]
    public string? AdExternalId { get; set; }
    public int AdTotalLeads { get; set; }
}