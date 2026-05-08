namespace Synq.Application.DTOs;
public class GroupDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public int MembersCount { get; set; }
}