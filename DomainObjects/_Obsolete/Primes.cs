using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Internal
{
    class Primes
    {
        static Lazy<Primes> defaultInstance = new Lazy<Primes>(() => new Primes());

        static Primes Default => defaultInstance.Value;

        readonly List<int> primes;
        readonly Random random = new Random();

        public Primes(int maxValue = int.MaxValue)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            primes = GeneratePrimes(maxValue);
            sw.Stop();
        }

        public int GetRandom()
        {
            return primes[random.Next(primes.Count)];
        }

        private static List<int> GeneratePrimes(int n)
        {
            var r = from i in Enumerable.Range(2, n - 1).AsParallel()
                    where Enumerable.Range(2, (int)Math.Sqrt(i)).All(j => i % j != 0)
                    select i;
            return r.ToList();
        }
    }
}
