using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector
{
    public class ValueDropdownAttribute : Attribute
    {
        public string OptionsGetter { get; set; }

        public ValueDropdownAttribute(string optionsGetter)
        {
            OptionsGetter = optionsGetter;
        }
    }

    public interface IValueDropdownItem
    {
        string GetText();
        object GetValue();
    }

    public class ValueDropdownItem : IValueDropdownItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public ValueDropdownItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public string GetText()
        {
            return Text;
        }

        public object GetValue()
        {
            return Value;
        }
    }

    public class DelayedValueDropdownItem : IValueDropdownItem
    {
        public string Text { get; set; }
        public Func<object> ValueGetter { get; set; }

        public DelayedValueDropdownItem(string text, Func<object> valueGetter)
        {
            Text = text;
            ValueGetter = valueGetter;
        }

        public string GetText()
        {
            return Text;
        }

        public object GetValue()
        {
            return ValueGetter();
        }
    }

    public class ValueDropdownItem<T> : IValueDropdownItem
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public ValueDropdownItem(string text, T value)
        {
            Text = text;
            Value = value;
        }

        public string GetText()
        {
            return Text;
        }

        public object GetValue()
        {
            return Value;
        }
    }

    public class DelayedValueDropdownItem<T> : IValueDropdownItem
    {
        public string Text { get; set; }
        public Func<T> ValueGetter { get; set; }

        public DelayedValueDropdownItem(string text, Func<T> valueGetter)
        {
            Text = text;
            ValueGetter = valueGetter;
        }

        public string GetText()
        {
            return Text;
        }

        public object GetValue()
        {
            return ValueGetter();
        }
    }

    public class ValueDropdownList : List<IValueDropdownItem>
    {
        public ValueDropdownList()
        {
        }

        public ValueDropdownList(IEnumerable<IValueDropdownItem> items) : base(items)
        {
        }

        public void Add(string text, object value)
        {
            Add(new ValueDropdownItem(text, value));
        }

        public void AddDelayed(string text, Func<object> valueGetter)
        {
            Add(new DelayedValueDropdownItem(text, valueGetter));
        }

        public void Add(object value)
        {
            Add(new ValueDropdownItem(value.ToString(), value));
        }
    }

    public class ValueDropdownList<T> : List<IValueDropdownItem>
    {
        public ValueDropdownList()
        {
        }

        public ValueDropdownList(IEnumerable<IValueDropdownItem> items) : base(items)
        {
        }

        public void Add(string text, T value)
        {
            Add(new ValueDropdownItem<T>(text, value));
        }

        public void AddDelayed(string text, Func<T> valueGetter)
        {
            Add(new DelayedValueDropdownItem<T>(text, valueGetter));
        }

        public void Add(T value)
        {
            Add(new ValueDropdownItem<T>(value.ToString(), value));
        }
    }
}