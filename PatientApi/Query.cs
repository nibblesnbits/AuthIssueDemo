using HotChocolate.Authorization;

namespace PatientApi;

public class Patient {
    public int Id { get; set; }
}

public class Query {
    [Authorize("IsProvider")]
    public Patient[] GetPatients() {
        return new[] { new Patient() };
    }
}
