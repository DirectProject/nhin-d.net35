using System;

using NHINDirect.Config.Store;

namespace NHINDirect.Config.Command
{
    public class CommandsBase
    {
        public readonly static string EntityStatusString = string.Join(" | ", Enum.GetNames(typeof(EntityStatus)));
    }
}