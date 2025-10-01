using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace Export.Fetcher
{
    public interface BaseFetcher
    {
        DTO FetchRelated(Entity entity);
    }
}
