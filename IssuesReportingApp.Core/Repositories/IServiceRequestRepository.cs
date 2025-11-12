using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public interface IServiceRequestRepository
{
    IEnumerable<ServiceRequest> GetAll();
    ServiceRequest? GetByIdentifier(string identifier);
    ServiceRequest Add(ServiceRequest request);
    bool Update(ServiceRequest request);
    bool Delete(string identifier);
}