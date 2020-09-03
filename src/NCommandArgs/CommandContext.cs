namespace NCommandArgs
{
    public class CommandContext
    {
        public CommandContext(CommandSet set, string[] args)
        {
            Args = args;
            CommandSet = set;
        }

        public CommandSet CommandSet { get; }
        public string[] Args { get; }
    }
}