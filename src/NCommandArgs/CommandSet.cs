using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCommandArgs
{
    public class CommandSet : KeyedCollection<string, Command>
    {
        private CommandContext _context;
        public CommandSet(string[] args)
        {
            _context = new CommandContext(this, args);
        }
        //private readonly Regex _valueCommand = new Regex("^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");
        //private const int CommandWidth = 29;

        protected override string GetKeyForItem(Command item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Names != null && (uint)item.Names.Length > 0U)
                return item.Names[0];
            throw new InvalidOperationException("Command has no names!");
        }

        protected override void InsertItem(int index, Command item)
        {
            base.InsertItem(index, item);
            AddImpl(item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Command command = this.Items[index];
            for (int index1 = 1; index1 < command.Names.Length; ++index1)
                Dictionary.Remove(command.Names[index1]);
        }

        protected override void SetItem(int index, Command item)
        {
            base.SetItem(index, item);
            RemoveItem(index);
            AddImpl(item);
        }

        private void AddImpl(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            var stringList = new List<string>(command.Names.Length);
            try
            {
                for (int index = 1; index < command.Names.Length; ++index)
                {
                    Dictionary.Add(command.Names[index], command);
                    stringList.Add(command.Names[index]);
                }
            }
            catch (Exception)
            {
                foreach (string key in stringList)
                    Dictionary.Remove(key);
                throw;
            }
        }

        public new CommandSet Add(Command command)
        {
            base.Add(command);
            return this;
        }

        public CommandSet Add<T>(string key, Action action)
        {
            return Add(key, null, action);
        }

        public CommandSet Add<T>(string key, Action<T> action)
        {
            return Add(key, null, action);
        }

        public CommandSet Add<T>(string key, string description, Action<T> action)
        {
            return Add(new ActionCommand<T>(key, description, action, _context));
        }

        public CommandSet Add(string key, string description, Action action)
        {
            return Add(new ActionCommand(key, description, action, _context));
        }

        public void WriteCommandDescriptions(TextWriter o)
        {
            var longestCommandLength = this.OrderByDescending(k => k.Key.Length).FirstOrDefault()?.Key.Length ?? 0;

            foreach (var p in this)
            {
                o.WriteLine($"  {p.Key}{p.Key.FillColumnSpace(longestCommandLength)}{p.Description}");
            }
        }
        public void Execute()
        {
            foreach (var command in _context.CommandSet)
            {
                if(command.IsPassed)
                    command.Invoke();
            }
        }
    }
}