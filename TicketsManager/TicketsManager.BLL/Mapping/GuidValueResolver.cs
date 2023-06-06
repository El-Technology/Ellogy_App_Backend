using AutoMapper;

namespace TicketsManager.BLL.Mapping;

public class GuidValueResolver : IValueResolver<object, object, Guid>
{
    public Guid Resolve(object source, object destination, Guid destMember, ResolutionContext context)
    {
        return Guid.NewGuid();
    }
}
