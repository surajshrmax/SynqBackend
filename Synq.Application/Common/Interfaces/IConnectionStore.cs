namespace Synq.Application.Common.Interfaces;

public interface IConnectionStore
{
  void Add(string key, string value);
  void Remove(string key);
  bool TryGet(string key, out string value);
}
