using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Configuration.Authentication;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Controllers;

/// <summary>
/// Controller responsible for authentication
/// </summary>
[AllowAnonymous]
[Route("Api/[controller]")]
public class AuthenticationController : BaseController
{
    #region Properties

    private readonly JwtIssuerOptions JwtOptions;
    private readonly JsonSerializerOptions SerializerSettings;
    private Tokens Token;

    #endregion

    #region Constructor

    /// <summary>
    /// Create an instance of the AutenticacaoController class.
    /// </summary>
    /// <param name="_jwtOptions"></param>
    /// <param name="_tokensSnapshot"></param>
    public AuthenticationController(IOptions<JwtIssuerOptions> _jwtOptions, IOptionsSnapshot<Tokens> _tokensSnapshot)
    {
        JwtOptions = _jwtOptions.Value;
        ThrowIfInvalidOptions(JwtOptions);
        SerializerSettings = new JsonSerializerOptions
        {
            WriteIndented = false
        };

        Token = _tokensSnapshot.Value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Authenticates the user and provides the authorization token for other operations.
    /// </summary>
    /// <param name="guid">Application access code.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ResponseDTO<AuthorizationDTO>> Authentication(Guid guid)
    {
        var response = new ResponseDTO<AuthorizationDTO>();

        //client authentication
        if (!Token.Sistemas.ContainsValue(guid))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Error.Add("Unauthorized.");
            return response;
        }

        //client permissions
        string sistema = string.Empty;
        var identity = GetClaimsIdentity(guid, out sistema);
        if (identity is null)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Error.Add("Functionality not permitted.");
            return response;
        }

        var claims = new[]
        {
                        new Claim(JwtRegisteredClaimNames.Sub, guid.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, await this.JwtOptions.JtiGenerator()),
                        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(this.JwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                        identity.Result.FindFirst(Operation.List)
                };

        var jwt = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claims,
                notBefore: JwtOptions.NotBefore,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: JwtOptions.SigningCredentials
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        response.Data = new AuthorizationDTO()
        {
            TokenAcess = encodedJwt,
            ValidTo = jwt.ValidTo,
        };

        return response;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Convert a date to Date in Unix format, used in jwt.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


    /// <summary>
    /// Display exceptions for validating mandatory JWT options.
    /// </summary>
    /// <param name="options">Options JWT.</param>
    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        if (options.SigningCredentials == null)
            throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

        if (options.JtiGenerator == null)
            throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
    }

    /// <summary>
    /// Obtain access rights for a given application.
    /// </summary>
    /// <param name="guid">System access code.</param>
    /// <summary>
    private Task<ClaimsIdentity> GetClaimsIdentity(Guid guid, out string sistemaLicenca)
    {
        var sistema = Token.Sistemas.FirstOrDefault(x => x.Value == guid);

        if (!sistema.Equals(default(KeyValuePair<string, Guid>)))
        {
            sistemaLicenca = sistema.Key;
            return Task.FromResult(new ClaimsIdentity(new GenericIdentity(guid.ToString(), "Token"),
                    new[]
                    {
                        new Claim(Operation.List, "Allowed")
                    }));

        }

        sistemaLicenca = string.Empty;
        return Task.FromResult<ClaimsIdentity>(null);
    }

    #endregion
}
