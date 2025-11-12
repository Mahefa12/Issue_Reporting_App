using IssuesReportingApp.DataStructures;
using IssuesReportingApp.Models;
using IssuesReportingApp.Repositories;

namespace IssuesReportingApp.Services;

public class ServiceRequestIndex
{
    private readonly IServiceRequestRepository _repo;
    private readonly AVLTree<string, ServiceRequest> _byIdentifier = new();
    private readonly CustomPriorityQueue<ServiceRequest> _byPriority = new();
    private readonly Graph<string> _dependencies = new();
    private readonly BinarySearchTree<DateTime, ServiceRequest> _byCreated = new();
    private readonly RedBlackTree<string, ServiceRequest> _byTitle = new();
    private readonly Dictionary<string, HashSet<string>> _undirectedAdj = new();

    public ServiceRequestIndex(IServiceRequestRepository repo)
    {
        _repo = repo;
        BuildIndex();
    }

    private void BuildIndex()
    {
        foreach (var r in _repo.GetAll())
        {
            _byIdentifier.Insert(r.Identifier, r);
            _byPriority.Enqueue(r);
            _byCreated.Insert(r.CreatedDate, r);
            if (!string.IsNullOrWhiteSpace(r.Title)) _byTitle.Insert(r.Title, r);
            _dependencies.AddVertex(r.Identifier);
            if (!_undirectedAdj.ContainsKey(r.Identifier)) _undirectedAdj[r.Identifier] = new HashSet<string>();
            if (r.Dependencies != null)
            {
                foreach (var dep in r.Dependencies)
                {
                    _dependencies.AddEdge(r.Identifier, dep);
                    if (!_undirectedAdj.ContainsKey(dep)) _undirectedAdj[dep] = new HashSet<string>();
                    _undirectedAdj[r.Identifier].Add(dep);
                    _undirectedAdj[dep].Add(r.Identifier);
                }
            }
        }
    }

    public IEnumerable<ServiceRequest> All() => _repo.GetAll();

    public bool TryGetByIdentifier(string id, out ServiceRequest? request)
    {
        if (_byIdentifier.TryGet(id, out var r))
        {
            request = r;
            return true;
        }
        request = _repo.GetByIdentifier(id);
        return request != null;
    }

    public List<string> DependencyPath(string fromId, string toId)
    {
        return _dependencies.BfsPath(fromId, toId);
    }

    public List<ServiceRequest> NextToProcess(int count = 5)
    {
        var list = new List<ServiceRequest>();
        for (int i = 0; i < count && _byPriority.TryPeek(out var _); i++)
        {
            if (_byPriority.TryDequeue(out var item)) list.Add(item);
        }
        // Reinsert to keep heap intact
        foreach (var r in list) _byPriority.Enqueue(r);
        return list;
    }

    public IEnumerable<ServiceRequest> RangeByCreatedDate(DateTime start, DateTime end)
    {
        foreach (var (_, value) in _byCreated.Range(start, end))
        {
            yield return value;
        }
    }

    public IEnumerable<ServiceRequest> Alphabetical(int count = 10)
    {
        int i = 0;
        foreach (var (_, value) in _byTitle.InOrder())
        {
            if (i++ >= count) yield break;
            yield return value;
        }
    }

    public List<(string u, string v)> DependencyMstForComponent(string rootId)
    {
        // Build the component reachable from rootId
        var visited = new HashSet<string>();
        var q = new Queue<string>();
        q.Enqueue(rootId);
        visited.Add(rootId);
        while (q.Count > 0)
        {
            var u = q.Dequeue();
            if (_undirectedAdj.TryGetValue(u, out var neigh))
            {
                foreach (var v in neigh)
                {
                    if (visited.Add(v)) q.Enqueue(v);
                }
            }
        }
        // Compute MST over the component (uniform weights)
        List<(string u, string v)> edges = MinimumSpanningTree.Compute(visited, v => _undirectedAdj.TryGetValue(v, out var ns) ? ns : Array.Empty<string>());
        return edges;
    }
}