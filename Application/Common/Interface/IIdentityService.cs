﻿using ESMART.Application.Common.Models;

namespace ESMART.Domain.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> CreatUserAsync(string userName, string password);
    }
}
