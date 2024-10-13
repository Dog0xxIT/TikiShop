namespace TikiShop.Core.Dto
{
    public class TokensDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public TokensDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    };
}
