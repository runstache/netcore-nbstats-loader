using NbaStats.Loader.DataObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace NbaStats.Loader.Processors
{
    public interface IProcessor<TEntity>
    {
        /// <summary>
        /// Processes the Provided Entry into the Stats Database.
        /// </summary>
        /// <param name="entry">Entry</param>
        void Process(TEntity entry);
    }
}
