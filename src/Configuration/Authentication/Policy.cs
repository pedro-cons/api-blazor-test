using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Domain.Helpers;

namespace Configuration.Authentication;
/// <summary>
/// Access policies available.
/// </summary>
public partial class Policy
{
    public static IEnumerable<PolicyClaimValue> Compilar(IConfiguration settings)
    {
        var section = settings.GetSection("Policies").AsDictionary();
        foreach (var item in section)
        {
            string policy = item.Key;
            foreach (var sub in (Dictionary<string, object>)item.Value)
            {
                foreach (var c in (Dictionary<string, object>)sub.Value)
                {
                    yield return new PolicyClaimValue()
                    {
                        Policy = policy,
                        ClaimType = c.Key,
                        ClaimValue = c.Value.ToString(),
                    };
                }
            }
        }
    }

    public struct PolicyClaimValue
    {
        public string Policy;
        public string ClaimType;
        public string ClaimValue;
    }
}

public partial class Policy
{
    public const string Search = "Search";
}