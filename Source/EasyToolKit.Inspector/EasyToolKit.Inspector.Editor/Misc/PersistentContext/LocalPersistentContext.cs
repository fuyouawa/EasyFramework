using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Helper class that provides a local copy of a <see cref="GlobalPersistentContext{T}"/>.
    /// When the local value is changed, it also changed the global value, but the global value does not change the local value.
    /// </summary>
    /// <typeparam name="T">The type of the context value.</typeparam>
    public sealed class LocalPersistentContext<T>
    {
        private GlobalPersistentContext<T> context;
        private T localValue;

        /// <summary>
        /// The value of the context.
        /// Changing this value, also changes the global context value, but the global value does not change the local value.
        /// </summary>
        public T Value
        {
            get
            {
                return this.localValue;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this.localValue, value))
                {
                    this.context.Value = value;
                    this.localValue = value;
                }
            }
        }

        private LocalPersistentContext(GlobalPersistentContext<T> global)
        {
            if (global == null) { throw new ArgumentNullException("global"); }

            this.context = global;
            this.localValue = this.context.Value;
        }

        /// <summary>
        /// Creates a local context object for the provided global context.
        /// </summary>
        /// <param name="global">The global context object.</param>
        public static LocalPersistentContext<T> Create(GlobalPersistentContext<T> global)
        {
            return new LocalPersistentContext<T>(global);
        }

        /// <summary>
        /// Updates the local value to the current global value.
        /// </summary>
        public void UpdateLocalValue()
        {
            this.localValue = this.context.Value;
        }
    }
}
