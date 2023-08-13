using HotChocolate.Authorization;

namespace PatientApi;

public class Patient {
    public int Id { get; set; }
}
public class Provider {
    public int Id { get; set; }
}

public class Query {
    [Authorize]
    public Patient[] GetPatients() {
        return new[] { new Patient() };
    }
    public Provider[] GetProviders() {
        return new[] { new Provider() };
    }
}
