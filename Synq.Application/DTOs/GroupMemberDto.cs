using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.DTOs;
public class GroupMemberDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Role { get; set; }
    public string ImageUrl { get; set; }
    public DateTime JoinedDate { get; set; }
}
