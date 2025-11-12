# Issues Reporting App
## Demo Video
- Watch the project demo: https://youtu.be/VHEDLQg4TLU
## Prerequisites
- `.NET SDK 9.0` or later (`dotnet --version` should be `9.x`).
- macOS, Linux, or Windows for the web app.

## Build
- Build the entire solution:
  - From repository root: `dotnet build`
  - Or build a specific project:
    - Web: `dotnet build IssuesReportingWeb/IssuesReportingWeb.csproj`
    - Core library: `dotnet build IssuesReportingApp.Core/IssuesReportingApp.Core.csproj`

## Run (Web App)
- From repository root with helper script:
  - `./run-web.sh`
  - `./run-web.sh --http-only` — serve only `http://localhost:5000`
  - `./run-web.sh --trust-https` — trust dev cert; serve `http://localhost:5000` and `https://localhost:5001`
- Or from the web project directory:
  - `cd IssuesReportingWeb`
  - `dotnet run`
- Open the app:
  - `http://localhost:5000/` (Home)
  - `http://localhost:5000/ServiceRequests` (Service Request Status)

## Using the Programme
- Navigate to `Service Requests` from the Home page.
- Search:
  - Enter a title or identifier (e.g., `ISSUE-1` or 'Streetlight Repair') in the search box.
  - Use suggestions from the dropdown or press `Search`.
  - Click `Track` on a row to focus on a specific request.
- Date range filter:
  - Set `Created from` and `Created to` (YYYY-MM-DD) to filter requests by creation date.
- Request card (when tracking):
  - Shows `Title`, `Identifier`, `Status`, `Category`, and `Priority`.
  - Dependencies: A path preview using BFS when direct dependencies exist.
  - Minimal dependency backbone (MST): Displays the spanning tree edges of the component containing the tracked request.
- Lists:
  - `All Requests` — table with core details and actions.
  - `Next to Process` — upcoming items chosen by priority queue.
  - `Alphabetical by Title` — an in-order listing backed by a balanced tree wrapper.
- Actions:
  - `Track` — focuses the page on that request and shows dependency insights.
  - `Copy ID` — copies the identifier to clipboard.

UI notes:
- Default MVC route (`/ServiceRequests`) renders the MVC view, which shows `All Requests`. The `Next to Process` and `Alphabetical by Title` lists are provided in the Razor Pages variant (`Pages/ServiceRequests/Index.cshtml`) and are not mapped by default in the current routing configuration.
- In the MVC view, the table shows the `Created` timestamp. In the Razor Pages view, the table shows `Updated` when available, otherwise `Created`.

## Service Request Status — Data Structures and Efficiency

This feature is powered by several data structures to keep interactions fast and informative even as data grows. Below are the structures, their roles, and examples.

### AVL Tree — Identifier Lookup
- Role: Fast lookup of a service request by `Identifier`.
- Where: `IssuesReportingApp.Core/Services/ServiceRequestIndex.cs` (`_byIdentifier`).
- Efficiency: `O(log n)` inserts and searches; keeps the tree height minimal.
- Example:
  - API: `_index.TryGetByIdentifier("ISSUE-3", out var r)`.
  - Use-case: Enter `ISSUE-3` in the search box; the tracked card resolves quickly.

### Binary Search Tree — Date Range Filtering
- Role: Efficient filtering by `CreatedDate` for a bounded interval.
- Where: `ServiceRequestIndex._byCreated` and `RangeByCreatedDate(start, end)`.
- Efficiency: In-order traversal with early break; effectively `O(n)` to reach the low bound plus `O(k)` to enumerate `k` items in range.
- Example:
  - UI: Set `Created from` = `2025-01-01`, `Created to` = `2025-12-31`.
  - Server: `_index.RangeByCreatedDate(start, end)` produces only items in-window.
  - Benefit: Avoids scanning all requests; scales with result size.

### Red-Black Tree (Wrapper) — Alphabetical Listing
- Role: Maintain a sorted index by `Title` for in-order listings.
- Where: `ServiceRequestIndex._byTitle` and `Alphabetical(count)`.
- Efficiency: Balanced tree characteristics with `O(log n)` insert; `O(k)` in-order output.
- Implementation note: Provided via a wrapper backed by `SortedDictionary` for deterministic ordering and simplicity.
- Example:
  - UI: `Alphabetical by Title` shows `Title (Identifier)` chips in order.
  - Server: `foreach (var (_, r) in _byTitle.InOrder())` yields sorted titles.

### Heap (Priority Queue) — Next to Process
- Role: Select top-priority requests rapidly for operational attention.
- Where: `ServiceRequestIndex._byPriority` and `NextToProcess(count)`.
- Efficiency: `O(log n)` per enqueue/dequeue; stable `O(k log n)` for `k` selections.
- Example:
  - UI: `Next to Process` shows a small set of identifiers with priorities.
  - Server: Re-enqueues items after peeking to keep the queue intact.

### Graph + BFS — Dependency Path
- Role: Model request dependencies and find a path between requests.
- Where: `ServiceRequestIndex._dependencies` and `DependencyPath(fromId, toId)`.
- Efficiency: BFS is `O(V + E)`; for sparse graphs, typically linear in edges.
- Example:
  - UI: Tracked request card shows a sequence of identifiers as a dependency path.
  - Server: `_dependencies.BfsPath(fromId, toId)` builds a shortest path in hops.

### Minimum Spanning Tree — Minimal Dependency Backbone
- Role: Produce a minimal set of edges that connects all requests within the tracked component.
- Where: `ServiceRequestIndex._undirectedAdj` and `DependencyMstForComponent(rootId)`.
- Efficiency: Built with BFS over uniform weights; `O(V + E)` for component traversal.
- Example:
  - UI: `Minimal dependency backbone (MST)` shows `u → v` edges.
  - Server: `MinimumSpanningTree.Compute(vertices, neighbors)` returns the spanning edges.
  - Benefit: Provides a compact view of essential links to understand the component structure.

### Trie — Search Suggestions
- Role: Offer suggestions while typing titles or identifiers.
- Where: On the Service Requests page, suggestions come from an HTML `datalist` built from loaded items. The trie (`IssuesReportingApp.Core/DataStructures/CustomTrie.cs`) is used by the `RecommendationEngine` to power event-related suggestions.
- Efficiency: Prefix queries in `O(length(prefix) + k)` where `k` is suggestions returned.
- Example:
  - UI: Type `Street`, see suggested titles and identifiers to auto-complete.

Clarification:
- The Service Requests search input does not consult the trie; its suggestions are sourced from the currently loaded items via the HTML `datalist`. The trie is leveraged by the `RecommendationEngine` for event-related suggestions elsewhere in the app.

## Project Structure
- `IssuesReportingApp.Core/Models` — Domain models like `ServiceRequest` and `Issue`.
- `IssuesReportingApp.Core/Repositories` — In-memory repositories and interfaces.
- `IssuesReportingApp.Core/DataStructures` — Algorithms and structures used by the app.
- `IssuesReportingApp.Core/Services/ServiceRequestIndex.cs` — Central index integrating structures.
- `IssuesReportingWeb/Controllers` and `IssuesReportingWeb/Views` — MVC controllers and views for the web app UI. A `Pages` folder is present, but most UI routes use MVC views.
