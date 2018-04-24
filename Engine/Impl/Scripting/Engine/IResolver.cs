using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public interface IResolver
    {
        /**
   * Allows checking whether there is currently an object bound to the key.
   *
   * @param key the key to check
   * @return true if there is currently an object bound to the key. False otherwise.
   */
        bool ContainsKey(Object key);

        /**
         * Returns the object currently bound to the key or false if no object is currently bound
         * to the key
         *
         * @param key the key of the object to retrieve.
         * @return the object currently bound to the key or 'null' if no object is currently bound to the key.
         */
        Object Get(Object key);

        /**
         * Returns the set of key that can be resolved using this resolver.
         * @return the set of keys that can be resolved by this resolver.
         */
        ICollection<String> KeySet();
    }
}
