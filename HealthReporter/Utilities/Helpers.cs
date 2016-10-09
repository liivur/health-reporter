using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthReporter.Models;

namespace HealthReporter.Utilities
{
    public static class Helpers<T>
    {
        // finds index by id in an IList. Return -1 if id not found
        public static int GetIndexById(IList<T> list, byte[] id)
        {
            if (id == null)
            {
                return -1;
            }

            var length = list.Count();
            for (int i = 0; i < length; i++)
            {
                var elemId = ((IHasPrimaryKey)list[i]).GetPrimaryKey();
                if (elemId.SequenceEqual(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public interface IHasPrimaryKey
    {
        byte[] GetPrimaryKey();
    }
}
