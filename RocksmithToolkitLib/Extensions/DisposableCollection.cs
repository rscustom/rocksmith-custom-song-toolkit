using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Extensions
{
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
    {
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                foreach (var disposable in this)
                    disposable.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
