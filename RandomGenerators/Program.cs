using Random.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumbersTask
{
    public class ThreadPoolExample
    {
        static void Main()
        {
            while (true) // Continue the game untill the user does want to anymore...
            {
                int threads = 3;
                int threadLoad = 10;
                ulong initialValue = 7;
                int Method = 1;

                Console.WriteLine("Usage: Threads <num> Load <num> Seed <num> Method <num>");
                Console.WriteLine("Method: 0-Tree,1-Leapfrog,2-Modified Leapfrog. Argument+1 - speedtest.");
                String[] args = Console.ReadLine().Split(' ');

                bool test_threads = int.TryParse(args[0], out threads);
                bool test_load = int.TryParse(args[1], out threadLoad);
                bool test_seed = ulong.TryParse(args[2], out initialValue);
                bool test_Method = int.TryParse(args[3], out Method);

                if (test_threads == false || test_load == false || test_seed == false || test_Method == false)
                {
                    System.Console.WriteLine("Please enter a numeric argument.");
                    System.Console.WriteLine("Usage: Threads <num> Load <num> Seed <num> Method <num>");
                }

                // One event is used for each Fibonacci object

                //LCGLeapfrog[] LCGLeapfrogArray = new LCGLeapfrog[threads];
                TimeSpan? start = DateTime.Now.TimeOfDay;
                TimeSpan? end = null;
                if (Method == 1)
                {
                    ManualResetEvent[] doneEvents = new ManualResetEvent[threads];

                    LCGLeapfrog[] lcgArray = new LCGLeapfrog[threads];

                    //Pradinė seka iš n-gijų sk. narių
                    LCGLeapfrog initialSeq = new LCGLeapfrog(initialValue, threads);
                    initialSeq.Calculateinitial(threads);


                    for (int i = 0; i < threads; i++)
                    {
                        doneEvents[i] = new ManualResetEvent(false);

                        //Inicializuojam su pradinės sekos nariais
                        //laipsnį n parenkam > gijų sk., kad sekos nesidublikuotu
                        ulong seed = initialSeq.Result[i];
                        LCGLeapfrog f = new LCGLeapfrog(seed, threadLoad, threads, doneEvents[i]);

                        lcgArray[i] = f;
                        ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                    }
                    WaitHandle.WaitAll(doneEvents);
                    Console.WriteLine("All calculations are complete.");

                    // Display the results...
                    for (int i = 0; i < threads; i++)
                    {
                        LCGLeapfrog f = lcgArray[i];
                        for (int j = 0; j < f.Result.Length; j++)
                        {
                            Console.WriteLine("Thread({0}) = {1}", i, f.Result[j]);
                        }
                    }
                }
                else if (Method == 0)
                {
                    ManualResetEvent[] doneEvents = new ManualResetEvent[threads];
                    LCG[] lcgArray = new LCG[threads];

                    //Sugeneruojam pradinę seką su n-gijų skaičius narių

                    LCG2 initialSeq = new LCG2(initialValue, threads);
                    initialSeq.Calculateinitial(threads);

                    //Kiekvienai gijai, generuojam naują seką, 
                    //pradines reikšmes parinkdami iš pradinės sekos

                    for (int i = 0; i < threads; i++)
                    {
                        doneEvents[i] = new ManualResetEvent(false);

                        //Pradinė reikšmė
                        ulong seed = initialSeq.Result[i];

                        //Generuojam seką
                        LCG f = new LCG(seed, threadLoad, doneEvents[i]);
                        lcgArray[i] = f;

                        ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                    }

                    // Wait for all threads in pool to calculation...
                    WaitHandle.WaitAll(doneEvents);
                    Console.WriteLine("All calculations are complete.");

                    // Display the results...
                    for (int i = 0; i < threads; i++)
                    {
                        LCG f = lcgArray[i];
                        for (int j = 0; j < f.Result.Length; j++)
                        {
                            Console.WriteLine("Thread({0}) = {1}", i, f.Result[j]);
                        }
                    }
                }
                else if (Method == 2)
                {
                    ManualResetEvent[] doneEvents = new ManualResetEvent[threads];
                    LCG[] lcgArray = new LCG[threads];

                    //Pradinė seka su tarpais n-narių kiekis sekoje
                    LCGLeapfrog initialSeq = new LCGLeapfrog(initialValue, threads, threadLoad);
                    initialSeq.Calculateinitial(threads);

                    
                    for (int i = 0; i < threads; i++)
                    {
                        doneEvents[i] = new ManualResetEvent(false);

                        //Pradinės reikšmės iš išretintos sekos
                        //Toliau kiekviena gija generuoja nuosekliai
                        ulong seed = initialSeq.Result[i];
                        LCG f = new LCG(seed, threadLoad, doneEvents[i]);
                        lcgArray[i] = f;

                        ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                    }

                    WaitHandle.WaitAll(doneEvents);
                    Console.WriteLine("All calculations are complete.");

                    // Display the results...
                    for (int i = 0; i < threads; i++)
                    {
                        LCG f = lcgArray[i];
                        for (int j = 0; j < f.Result.Length; j++)
                        {
                            Console.WriteLine("Thread({0}) = {1}", i, f.Result[j]);
                        }
                    }
                }
                else if (Method == 11)
                {
                    TimeSpan? previousTime = null;
                    for (int k = 1; k < threads; k++)
                    {
                        start = DateTime.Now.TimeOfDay;
                        ManualResetEvent[] doneEvents = new ManualResetEvent[k];
                        LCGLeapfrog[] lcgArray = new LCGLeapfrog[k];
                        LCGLeapfrog initialSeq = null;
                        initialSeq = new LCGLeapfrog(initialValue, k);
                        initialSeq.Calculateinitial(k);
                        Console.WriteLine("launching {0} tasks...", k);
                        for (int i = 0; i < k; i++)
                        {
                            doneEvents[i] = new ManualResetEvent(false);
                            ulong seed = 0;
                            seed = initialSeq.Result[i];
                            LCGLeapfrog f = new LCGLeapfrog(seed, threadLoad / k, k, doneEvents[i]);
                            lcgArray[i] = f;
                            ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                        }
                        WaitHandle.WaitAll(doneEvents);
                        end = DateTime.Now.TimeOfDay;
                        if (previousTime != null)
                        {
                            Console.WriteLine("Speedup: {0}%", ((previousTime.Value.TotalMilliseconds / (end - start).Value.TotalMilliseconds) - 1) * 100);
                        }
                        previousTime = end - start;
                        Console.WriteLine("Time taken: {0}", previousTime);

                    }
                }
                else if (Method == 21)
                {
                    TimeSpan? previousTime = null;
                    for (int k = 1; k < threads; k++)
                    {
                        ManualResetEvent[] doneEvents = new ManualResetEvent[k];
                        LCG[] lcgArray = new LCG[k];
                        LCGLeapfrog initialSeq = null;
                        initialSeq = new LCGLeapfrog(initialValue, k, threadLoad / k);
                        initialSeq.Calculateinitial(k);
                        Console.WriteLine("launching {0} tasks...", k);
                        for (int i = 0; i < k; i++)
                        {
                            doneEvents[i] = new ManualResetEvent(false);
                            ulong seed = 0;
                            seed = initialSeq.Result[i];
                            LCG f = new LCG(seed, threadLoad / k, doneEvents[i]);
                            lcgArray[i] = f;
                            ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                        }
                        WaitHandle.WaitAll(doneEvents);
                        Console.WriteLine("All calculations are complete.");

                        end = DateTime.Now.TimeOfDay;
                        if (previousTime != null)
                        {
                            Console.WriteLine("Speedup: {0}%", ((previousTime.Value.TotalMilliseconds / (end - start).Value.TotalMilliseconds) - 1) * 100);
                        }
                        previousTime = end - start;
                        Console.WriteLine("Time taken: {0}", previousTime);
                    }

                }

                end = DateTime.Now.TimeOfDay;
                Console.WriteLine("Time taken: {0}", end - start);
                while (true) // Continue asking until a correct answer is given.
                {
                    Console.Write("Do you want to play again [Y/N]?");
                    string answer = Console.ReadLine().ToUpper();
                    if (answer == "Y")
                        break; // Exit the inner while-loop and continue in the outer while loop.
                    if (answer == "N")
                        return; // Exit the Main-method.
                }
                //Console.ReadKey();
            }
        }
    }
}