using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace NCommandArgs
{
    public abstract class Command
    {
        private static readonly char[] NameTerminator = new char[2] { '=', ':' };
        protected CommandContext _context;
        protected string _key;


        protected Command(string key, string description, CommandContext context)
        {
            switch (key)
            {
                case "":
                    throw new ArgumentException("Cannot be the empty string.", nameof(key));
                case null:
                    throw new ArgumentNullException(nameof(key));
                default:
                    _key = key;
                    _context = context;
                    Names = key.Split('|');
                    Description = description;
                    IsPassed = context.Args.Count(x => Names.Contains(x)) > 0;
                    //this.CommandValueType = this.ParsePrototype();
                    if (Array.IndexOf<string>(this.Names, "<>") < 0 || this.Names.Length != 1 && this.Names.Length <= 1)
                        break;
                    throw new ArgumentException("The default option handler '<>' cannot require values.", nameof(key));
            }
        }

        public string Key => _key;

        public Lazy<IEnumerable<string>> Values
        {
            get
            {
                var otherKeys = _context.CommandSet.KeyGroups
                    .Except(new List<string[]>{ Names })
                    .SelectMany(x => x);

                var values = _context.Args
                    .SkipWhile(a => !Names.Contains(a))
                    .Skip(1) //Skip for command
                    .TakeWhile(k => !otherKeys.Contains(k));

                return new Lazy<IEnumerable<string>>(values);
            }
        }

        public string Description { get; }

        internal string[] Names { get; }

        public bool IsPassed { get; }

        public string[] GetNames()
        {
            return (string[])Names.Clone();
        }

        public abstract void Invoke();

        public override string ToString()
        {
            return Key;
        }
    }

    public sealed class ActionCommand : Command
    {
        private readonly Action _action;

        public ActionCommand(string key, string description, Action action, CommandContext context)
            : base(key, description, context)
        {

            _key = key;
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _context = context;
        }

        public override void Invoke()
        {
            _action.Invoke();
        }
    }

    public sealed class ActionCommand<T> : Command
    {
        private readonly Action<T> _action;

        public ActionCommand(string key, string description, Action<T> action, CommandContext context)
            : base(key, description, context)
        {

            _key = key;
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _context = context;
        }

        private T GetParamValue(string key)
        {
            var names = key.Split('|');
            if (_context.Args.Count(a => names.Contains(a)) < 1)
                throw new IndexOutOfRangeException($"{key} cannot found");

            if (Values.Value == null || !Values.Value.Any())
                throw new ArgumentNullException();

            try
            {
                var values = (T)Values.Value;

                if (Values.IsValueCreated && Values.Value.Count() > 1)
                    return values;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (!TypeDescriptor.GetConverter(typeof(T)).IsValid(Values.Value.FirstOrDefault()!))
                throw new ArgumentException();

            return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(Values.Value.FirstOrDefault());
        }

        public override void Invoke()
        {
            _action.Invoke(GetParamValue(_key));
        }
    }
}