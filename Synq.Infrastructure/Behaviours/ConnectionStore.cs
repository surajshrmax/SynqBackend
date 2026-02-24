using System.Collections.Concurrent;
using Synq.Application.Common.Interfaces;

namespace Synq.Infrastructure.Behaviours;

public class ConnectionStore : IConnectionStore
{
  private readonly ConcurrentDictionary<string, string> _connections = new();

  public void Add(string key, string value) => _connections[key] = value;

  public void Remove(string key) => _connections.TryRemove(key, out _);

  public bool TryGet(string key, out string value) => _connections.TryGetValue(key, out value);

}
