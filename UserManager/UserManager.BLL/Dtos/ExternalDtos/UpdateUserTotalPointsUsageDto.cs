namespace UserManager.BLL.Dtos.ExternalDtos;
public class UpdateUserTotalPointsUsageDto
{
    public Guid UserId { get; set; }
    public int UsedTokens { get; set; }
}
