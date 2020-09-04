using System.Collections.Generic;

namespace NCommandArgs
{
    public class CommandContext
    {
        public CommandContext(CommandSet set, List<string> args)
        {
            Args = args;
            CommandSet = set;
        }

        public CommandSet CommandSet { get; }
        public List<string> Args { get; }
    }
}