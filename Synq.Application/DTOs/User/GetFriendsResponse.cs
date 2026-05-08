namespace Synq.Application.DTOs.User;

public class GetFriendsResponse
{
  public String Cursor { get; set; }
  public bool HasMoreAfter { get; set; }
  public List<UserDto> Friends { get; set; }
}
