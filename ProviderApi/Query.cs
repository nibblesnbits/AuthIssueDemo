using HotChocolate.Authorization;

namespace ProviderApi;

public class Provider
{
    public int Id { get; set; }
}

public class Query
{
    [Authorize("IsPatient")]
    public Provider[] GetProviders()
    {
        return new[] { new Provider() };
    }
}
