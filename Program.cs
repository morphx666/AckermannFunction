using System.Diagnostics;
using System.Reflection;

namespace AckermannFunction {
    public static class Program {
        public static void Main(string[] args) {
            const int MB = 1014 * 1024;
            ulong v1 = 0;
            ulong v2 = 0;
            int m = (int)(16 * MB);
            
            Stopwatch sw = new();

            if(args.Length < 2 || args.Length > 3) {
                ShowUsage();
            } else {
                if(!ulong.TryParse(args[0], out v1)) ShowUsage();
                if(!ulong.TryParse(args[1], out v2)) ShowUsage();
                if(args.Length == 3) {
                    if(args[2].StartsWith("/s:")) {
                        string[] tokens = args[2].Split(':');
                        if(!int.TryParse(tokens[1], out m)) ShowUsage();
                        if(m <= 0) {
                            ShowUsage();
                        } else {
                            m = (int)(m * MB);
                        }
                    }
                }
            }

            sw.Start();
            Console.WriteLine($"Ackermann({v1}, {v2}) = {Ack(v1, v2, m):N0}");
            Console.WriteLine($"{sw.Elapsed.Hours:00}:{sw.Elapsed.Minutes:00}:{sw.Elapsed.Seconds:00}:{sw.Elapsed.Milliseconds:000}");
            sw.Stop();
        }

        private static ulong Ack(ulong m, ulong n, int stackSize) {
            ulong result = 0;
            Thread t = new(() => result = Ack(m ,n), stackSize) { Priority = ThreadPriority.Highest };
            t.Start();
            t.Join();
            return result;
        }

        private static ulong Ack(ulong m, ulong n) {
            if(m == 0) {
                return n + 1;
            } else if(n == 0) {
                return Ack(m - 1, 1);
            } else {
                return Ack(m - 1, Ack(m, n - 1));
            }
        }

        private static void ShowUsage(string errMsg = "") {
            Action<string, ConsoleColor> Wlwc = (string text, ConsoleColor fc) => {
                ConsoleColor dfc = Console.ForegroundColor;
                Console.ForegroundColor = fc;
                Console.WriteLine(text);
                Console.ForegroundColor = dfc;
            };

            Wlwc($"{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Name} {AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}", ConsoleColor.White);
            Console.WriteLine();
            if(errMsg != "") {
                Wlwc(errMsg, ConsoleColor.Red);
                Console.WriteLine();
            }
            Console.WriteLine("m:  Positive Number");
            Console.WriteLine("n:  Positive Number");
            Console.WriteLine("/s: Optional positive number that defines the Stack Size in MegaBytes.");
            Console.WriteLine("    If not used, the default Stack Size will be 16MB");
            Console.WriteLine();

            Environment.Exit(1);
        }
    }
}