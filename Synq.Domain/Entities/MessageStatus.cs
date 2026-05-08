using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Domain.Entities;
public class MessageStatus
{
    public Guid MessageId{ get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; }

    public Message Message { get; set; }
    public User User { get; set; }
}
