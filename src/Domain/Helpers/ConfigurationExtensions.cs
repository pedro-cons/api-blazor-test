using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers;
/// <summary>
/// Extensions for Microsoft.Extensions.Configuration.IConfiguration.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Convert a configuration to a common dictionary.
    /// </summary>
    /// <param name="configuration">Configuration to be converted.</param>
    /// <returns></returns>
    public static Dictionary<string, object> AsDictionary(this IConfiguration configuration)
    {
        var dic = new Dictionary<string, object>();
        foreach (var el in configuration.GetChildren())
        {
            if (el.Value != null)
                dic[el.Key] = el.Value;
            else
                dic[el.Key] = AsDictionary(el.GetChildren());
        }
        return dic;
    }

    /// <summary>
    /// Convert a configuration session collation to a common dictionary.
    /// </summary>
    /// <param name="sectionItems">Grouping of session items to be converted.</param>
    /// <returns></returns>
    public static Dictionary<string, object> AsDictionary(this IEnumerable<IConfigurationSection> sectionItems)
    {
        var dic = new Dictionary<string, object>();
        foreach (var el in sectionItems)
        {
            if (el.Value != null)
                dic[el.Key] = el.Value;
            else
                dic[el.Key] = AsDictionary(el.GetChildren());
        }
        return dic;
    }

    /// <summary>
    /// Convert a configuration session to a common dictionary.
    /// </summary>
    /// <param name="section">Configuration session to be converted.</param>
    /// <returns></returns>
    public static Dictionary<string, object> AsDictionary(this IConfigurationSection section)
    {
        var dic = new Dictionary<string, object>();
        foreach (var el in section.GetChildren())
        {
            if (el.Value != null)
                dic[el.Key] = el.Value;
            else
                dic[el.Key] = AsDictionary(el.GetChildren());
        }
        return dic;
    }

}