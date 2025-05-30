using HoleAutoJoin.Commands;

namespace HoleAutoJoin.Core
{
    public interface ICommandFactory
    {
        JoinCommand CreateJoinCommand();
        SettingsCommand CreateSettingsCommand();
    }
}
