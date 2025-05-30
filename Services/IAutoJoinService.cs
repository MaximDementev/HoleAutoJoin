using System;

namespace HoleAutoJoin.Services
{
    public interface IAutoJoinService : IEventHandler
    {
        bool IsEnabled { get; set; }
    }
}
