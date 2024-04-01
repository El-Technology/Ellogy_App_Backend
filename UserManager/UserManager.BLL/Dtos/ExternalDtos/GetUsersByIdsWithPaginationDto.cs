using UserManager.Common.Dtos;

namespace UserManager.BLL.Dtos.ExternalDtos;
public class GetUsersByIdsWithPaginationDto
{
    public List<Guid> UserIds { get; set; }
    public PaginationRequestDto PaginationRequest { get; set; }
}
