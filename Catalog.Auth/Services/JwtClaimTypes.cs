namespace Catalog.Auth.Services
{
    public static class JwtClaimTypes
    {
        /// <summary>The party to which the ID Token was issued. If present, it MUST contain the OAuth 2.0 Client ID of this party. This Claim is only needed when the ID Token has a single audience value and that audience is different than the authorized party. It MAY be included even when the authorized party is the same as the sole audience. The azp value is a case sensitive string containing a StringOrURI value.</summary>
        public const string AuthorizedParty = "azp";

        /// <summary>JWT ID. A unique identifier for the token, which can be used to prevent reuse of the token. These tokens MUST only be used once, unless conditions for reuse were negotiated between the parties; any such negotiation is beyond the scope of this specification.</summary>
        public const string JwtId = "jti";

        /// <summary>
        /// The type of grant requested, eg client credentials or password
        /// </summary>
        public const string GrantType = "gty";
    }
}
