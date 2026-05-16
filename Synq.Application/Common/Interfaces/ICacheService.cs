using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Application.Common.Interfaces;

public interface ICacheService
{
    public Task SetValueAsync(string key, string value);

    public Task<string?> GetValueAsync(string key);
}
