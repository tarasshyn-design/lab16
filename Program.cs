using System;
using System.Collections.Generic;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Dictionary<string, List<(string, int)>> graph = new()
{
    ["Склад"] = [("Школа", 7), ("Ринок", 2), ("Лікарня", 12)],
    ["Школа"] = [("Склад", 7), ("Ринок", 3), ("Бібліотека", 4)],
    ["Ринок"] = [("Склад", 2), ("Школа", 3), ("Лікарня", 9)],
    ["Лікарня"] = [("Склад", 12), ("Ринок", 9), ("Вокзал", 6), ("Парк", 5)],
    ["Бібліотека"] = [("Школа", 4), ("Вокзал", 1)],
    ["Вокзал"] = [("Бібліотека", 1), ("Лікарня", 6), ("Парк", 3)],
    ["Парк"] = [("Вокзал", 3), ("Лікарня", 5)],
    ["Хутір"] = []
};

var (dist, prev, taken, processed) = Dijkstra(graph, "Склад");
Console.WriteLine($"Вийнято з черги: {taken}; оброблено вершин: {processed}");

foreach (string target in new[] { "Парк", "Лікарня", "Хутір" })
{
    List<string> route = BuildRoute(prev, "Склад", target);
    if (route.Count == 0)
        Console.WriteLine($"{target}: маршруту немає");
    else
        Console.WriteLine($"{target}: {string.Join(" -> ", route)} ({dist[target]} хв)");
}

(Dictionary<string, int> dist, Dictionary<string, string?> prev, int taken, int processed) Dijkstra(
    Dictionary<string, List<(string, int)>> g,
    string start)
{
    Dictionary<string, int> dist = [];
    Dictionary<string, string?> prev = [];
    HashSet<string> done = [];

    int taken = 0;
    int processed = 0;

    foreach (string v in g.Keys)
    {
        dist[v] = int.MaxValue;
        prev[v] = null;
    }

    dist[start] = 0;
    PriorityQueue<(string vertex, int cost), int> queue = new();
    queue.Enqueue((start, 0), 0);

    while (queue.Count > 0)
    {
        var (v, _) = queue.Dequeue();
        taken++;

        if (done.Contains(v))
            continue;

        done.Add(v);
        processed++;

        if (!g.TryGetValue(v, out var neighbors) || dist[v] == int.MaxValue)
            continue;

        foreach (var (nb, w) in neighbors)
        {
            int candidate = dist[v] + w;
            if (candidate < dist[nb])
            {
                dist[nb] = candidate;
                prev[nb] = v;
                queue.Enqueue((nb, candidate), candidate);
            }
        }
    }

    return (dist, prev, taken, processed);
}

List<string> BuildRoute(Dictionary<string, string?> prev, string start, string target)
{
    if (!prev.ContainsKey(target))
        return [];

    if (start == target)
        return [start];

    List<string> route = [];
    string? current = target;

    while (current is not null)
    {
        route.Add(current);
        if (current == start)
            break;

        current = prev.GetValueOrDefault(current);
    }

    if (route.Count == 0 || route[^1] != start)
        return [];

    route.Reverse();
    return route;
}
