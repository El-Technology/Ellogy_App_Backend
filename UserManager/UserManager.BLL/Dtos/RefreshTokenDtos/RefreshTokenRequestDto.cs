namespace UserManager.BLL.Dtos.RefreshTokenDtos;

public class RefreshTokenRequestDto
{
	public string Jwt { get; set; }
	public string RefreshToken { get; set; }
}
