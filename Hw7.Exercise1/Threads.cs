namespace Hw7.Exercise1
{
    /// <summary>
    /// Thread tools.
    /// </summary>
    public static class Threads
    {
        /// <summary>
        /// Creates and starts threads with entry <paramref name="entryPoint"/> point 
        /// for each argument from <paramref name="args"/> collection.
        /// </summary>
        /// <param name="entryPoint">Thread entry point.</param>
        /// <param name="args">Entry point argument.</param>
        /// <returns>
        /// Returns threads collection. 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when one of the method arguments is <c>null</c>.
        /// </exception>
        public static Thread[] StartAll(ParameterizedThreadStart entryPoint, IEnumerable<object> args)
        {
            if (entryPoint is null)
                throw new ArgumentNullException(nameof(entryPoint));
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            var threads = new Thread[args.ToArray().Length];
            var arg = args.ToArray();
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(entryPoint);
                threads[i].Start(arg[i]);
            }

            return threads;

        }
        /// <summary>
        /// Blocks current thread until all <paramref name="threads"/> will be done.
        /// </summary>
        /// <param name="threads">Threads to await.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when <paramref name="threads"/> is <c>null</c>.
        /// </exception>
        public static void WaitAll(IEnumerable<Thread> threads)
        {
            if (threads is null)
                throw new ArgumentNullException(nameof(threads));

            foreach (var thread in threads)
                thread.Join();
        }
    }
}
