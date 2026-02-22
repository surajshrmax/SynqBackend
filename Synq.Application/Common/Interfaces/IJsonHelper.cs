namespace Synq.Application.Common.Interfaces;

public interface IJsonHelper<ObjectType>
{
  public string Encode(ObjectType data);

  public ObjectType? Decode(string data);
}
