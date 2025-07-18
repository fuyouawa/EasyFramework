using System;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.Inspector.Editor
{	/// <summary>
    /// Context that persists across reloading and restarting Unity.
    /// </summary>
    public abstract class GlobalPersistentContext
    {
        /// <summary>
        /// Time stamp for when the persistent context value was last used.
        /// Used for purging unused context.
        /// </summary>
        [OdinSerialize]
        public long TimeStamp { get; private set; }

        /// <summary>
        /// Instatiates a persistent context.
        /// </summary>
        protected GlobalPersistentContext()
        { }

        /// <summary>
        /// Updates the time stamp to now.
        /// </summary>
        protected void UpdateTimeStamp()
        {
            this.TimeStamp = DateTime.Now.Ticks;
        }
    }

    /// <summary>
    /// Context that persists across reloading and restarting Unity.
    /// </summary>
    /// <typeparam name="T">The type of the context value.</typeparam>
    public sealed class GlobalPersistentContext<T> : GlobalPersistentContext
    {
        [OdinSerialize]
        private T value;

        /// <summary>
        /// The value of the context.
        /// </summary>
        public T Value
        {
            get
            {
                this.UpdateTimeStamp();
                return this.value;
            }
            set
            {
                this.value = value;
                this.UpdateTimeStamp();
            }
        }

        /// <summary>
        /// Creates a new persistent context object.
        /// </summary>
        public static GlobalPersistentContext<T> Create()
        {
            var c = new GlobalPersistentContext<T>();
            c.UpdateTimeStamp();
            return c;
        }

        /// <summary>
        /// Formats a string with the time stamp, and the value.
        /// </summary>
        public override string ToString()
        {
            return new DateTime(this.TimeStamp).ToString("dd/MM/yy HH:mm:ss") + " <" + typeof(T) + "> " + (this.value != null ? this.value.ToString() : "(null)");
        }
    }
}
