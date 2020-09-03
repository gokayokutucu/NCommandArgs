using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NCommandArgs
{
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(string message, string commandName)
            : base(message)
        {
            this.CommandName = commandName;
        }

        public CommandException(string message, string commandName, Exception innerException)
            : base(message, innerException)
        {
            this.CommandName = commandName;
        }

        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.CommandName = info.GetString(nameof(CommandName));
        }

        public string CommandName { get; }

        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CommandName", (object)this.CommandName);
        }
    }
}