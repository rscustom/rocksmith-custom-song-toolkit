using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RocksmithToolkitLib.Extensions
{
    public delegate void PropChangingEventHandler(object sender, PropChangingEventArgs e);

    public class PropChangingEventArgs : EventArgs
    {
        public PropChangingEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            cancel = false;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// <remarks>Gets the old value of the property whose value is changing.</remarks>      
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// Get or sets the new value of the property whose value is changing. 
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// if set to true the property will not change.
        /// </summary>
        public bool cancel { get; set; }

        /// <summary>
        /// Gets the name of the property whose value is changing.     
        /// </summary>
        public string PropertyName { get; private set; }
    }

    public class NotifyPropChangedBase : INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        /// <summary>
        /// Notify listeners that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void DoPropChange(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                DoPropChange(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class NotifyPropChangingBase : NotifyPropChangedBase
    {
        protected virtual void OnPropertyChanging(PropChangingEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }

        protected override void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                var ff = new PropChangingEventArgs(propertyName, field, newValue);
                OnPropertyChanging(ff);
                if (!ff.cancel)
                {
                    newValue = (T) ff.NewValue;
                    if (!EqualityComparer<T>.Default.Equals(field, newValue))
                    {
                        field = newValue;
                        DoPropChange(propertyName);
                    }
                }
            }
        }

        public event PropChangingEventHandler PropertyChanging;
    }
}