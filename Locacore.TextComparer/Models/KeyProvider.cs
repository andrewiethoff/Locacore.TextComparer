using System;

namespace Locacore.TextComparer
{
    public class KeyProvider
    {
        public string Key { get; private set; }

        public KeyProvider()
        {
            this.Key = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}