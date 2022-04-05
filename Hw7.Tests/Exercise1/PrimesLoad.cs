using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Hw7.Tests.Exercise1
{
    /// <summary>
    /// Test load.
    /// </summary>
    internal class PrimesLoad
    {
        private readonly ConcurrentBag<int> _cache = new();

        /// <summary>
        /// Returns primes cache with all found prime numbers.
        /// </summary>
        public IEnumerable<int> GetPrimesCache()
        {
            return _cache;
        }

        /// <summary>
        /// Find primes in range
        /// </summary>
        /// <param name="range">Range <see cref="Range"/> to find primes.</param>
        public void FindPrimes(object? range)
        {
            if (range is not Range rng)
                throw new ArgumentException("Invalid range", nameof(range));

            for (var i = rng.Start.Value; i < rng.End.Value; i++)
            {
                if (IsPrime(i))
                {
                    AppendPrimesCache(i);
                }
            }
        }

        /// <summary>
        /// Add prime number to cache in thread safe manner.
        /// </summary>
        private void AppendPrimesCache(int i)
        {
            _cache.Add(i);
        }

        /// <summary>
        /// Checks if given number is prime.
        /// </summary>
        public static bool IsPrime(int n)
        {
            if (n <= 1)
                return false;

            for (var i = 2; i < n; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
    }
}
