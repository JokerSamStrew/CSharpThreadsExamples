using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/*
 * Данный проект демонстрирует работу с потоками на языке C#
 * Источники:
 * - Герберд Шилдт C# 4.0 Полное руководство
 * 
 */

// Создание потока исполнения
public class MyThread
{
    public int Count;
    string thrdName;

    public MyThread(string name)
    {
        this.Count = 0;
        this.thrdName = name;
    }

    // Точка входа в поток
    public void Run()
    {
        Console.WriteLine("Thread \"" + this.thrdName + "\" start running");
        do
        {
            // Останавливает поток из которого он был вызван
            Thread.Sleep(500);
            Console.WriteLine(
                "Thread \"" + this.thrdName 
                + "\". Counter has changed: " + this.Count
                );
            this.Count++;
        } while (this.Count < 10);
        Console.WriteLine("Thread \"" + this.thrdName + "\" end  running");
    }
  
}

public class MyThreadImproved
{
    public int Count;
    public Thread Thread { get; private set; }


    public MyThreadImproved(string name)
    {
        this.Count = 0;
        this.Thread = new Thread(this.Run);
        this.Thread.Name = name;
        this.Thread.Start();
    }

    public MyThreadImproved(string name, int num)
    {
        this.Count = 0;
        this.Thread = new Thread(this.RunWithArg);
        this.Thread.Name = name;

        this.Thread.Start(num);
    }

    // Точка входа в поток
    public void Run()
    {
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" start running");
        do
        {
            // Останавливает поток из которого он был вызван
            Thread.Sleep(500);
            Console.WriteLine(
                "Thread \"" + this.Thread.Name
                + "\". Counter has changed: " + this.Count
                );
            this.Count++;
        } while (this.Count < 10);
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" end  running");
    }

    public void RunWithArg(object num)
    {
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" start running");
        do
        {
            // Останавливает поток из которого он был вызван
            Thread.Sleep(500);
            Console.WriteLine(
                "Thread \"" + this.Thread.Name
                + "\". Counter has changed: " + this.Count
                );
            this.Count++;
        } while (this.Count < (int) num);
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" end  running");
    }

}

class MyThreadPriority
{
    public int Count;
    public Thread Thread;

    static bool stop = false;
    static string currentName;

    // !!! не запускает поток Thread
    public MyThreadPriority(string name)
    {
        this.Count = 0;
        this.Thread = new Thread(this.Run);
        this.Thread.Name = name;
        MyThreadPriority.currentName = name;

    }

    void Run()
    {
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" start running");
        do {
            Count++;
            if (MyThreadPriority.currentName != this.Thread.Name){
                MyThreadPriority.currentName = this.Thread.Name;
                Console.WriteLine("Thread \"" + MyThreadPriority.currentName + "\" in the stream");
            }
        }while(!MyThreadPriority.stop && this.Count < 1000000);
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" end  running");
    }
}

class SumArray
{
    int sum;
    // закрытый объект, доступный для последующих блокировок 
    object lockOn = new object();

    //public int SumIt(int[] nums)
    //{
    //    // Заблокировать весь метод
    //    lock (lockOn)
    //    {
    //        this.sum = 0;
    //        for (int i = 0; i < nums.Length; i++)
    //        {
    //            this.sum += nums[i];
    //            Console.WriteLine(
    //                "For \"" + Thread.CurrentThread.Name
    //                + "\" current sum is " + this.sum
    //                );
    //            Thread.Sleep(10); // Разрешить переключение задач
    //        }
    //    }

    //    return sum;
    //}

    public int SumIt(int[] nums)
    {
        // Заблокировать весь метод
        this.sum = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            this.sum += nums[i];
            Console.WriteLine(
                "For \"" + Thread.CurrentThread.Name
                + "\" current sum is " + this.sum
                );
            Thread.Sleep(10); // Разрешить переключение задач
        }
        

        return sum;
    }
}

class MyThreadLock
{
    public Thread Thread;
    int[] a;
    int answer;

    static SumArray sa = new SumArray();

    public MyThreadLock(string name, int[] nums)
    {
        this.a = nums;
        this.Thread = new Thread(this.Run);
        this.Thread.Name = name;
        this.Thread.Start();
    }

    void Run()
    {
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" start running");
        lock(sa) this.answer = sa.SumIt(this.a);
        Console.WriteLine(
            "Sum for \"" + this.Thread.Name 
            + "\" equal " + this.answer
            );
        Console.WriteLine("Thread \"" + this.Thread.Name + "\" end  running");
    }
}

class TickTock
{
    object lockOn = new object();
    public void Tick(bool running){
        lock (lockOn){
            if (!running){
                Console.Write("Tick ");
                // Уведомить любые ожидающие потоки
                Monitor.Pulse(lockOn); 
                return;
            }

            
            // разрешить выполнение метода Tock()
            Monitor.Pulse(lockOn);
            // ожидать завершение выполнения метода Tock()
            Monitor.Wait(lockOn); 
        }
    }

    public void Tock(bool running)
    {
        lock (lockOn)
        {
            if (!running)
            {
                Console.Write("Tock ");
                // Уведомить любые ожидающие потоки
                Monitor.Pulse(lockOn);
                return;
            }

            
            // разрешить выполнение метода Tock()
            Monitor.Pulse(lockOn);
            // ожидать завершение выполнения метода Tock()
            Monitor.Wait(lockOn);
        }
    }
}

class TickTackThread
{
    public Thread Thread;
    TickTock tt;

    public TickTackThread(string name, TickTock tickTock)
    {
        this.Thread = new Thread(this.Run);
        this.Thread.Name = name;
        this.tt = tickTock;

        this.Thread.Start();
    }

    void Run()
    {
        if(this.Thread.Name == "Tick"){
            for(int i = 0; i < 5; i++){
                this.tt.Tick(false);
            }
        } else {
            for (int i = 0; i < 5; i++){
                this.tt.Tock(false);
            }
        }
    }



}


namespace MultithreadTest
{


    class Program
    {
        public static void MyThreadTest(MyThread myThread)
        {
            Console.WriteLine("\t<<< Class \"MyThread\" Test >>>");

            // Создаем поток используя аргумент
            Thread new_thread = new Thread(myThread.Run);

            // Начать выполнение потока
            new_thread.Start();
            do
            {
                Console.Write(".");
                Thread.Sleep(100);
            } while (myThread.Count != 10);


            Console.WriteLine("\t\\\\  Class \"MyThread\" Test //\n");
        }

        public static void MyThreadImprovedTest(MyThread myThread)
        {
            Console.WriteLine("\t<<< Class \"MyThreadImproved\" Test >>>");

            do
            {
                Console.Write(".");
                Thread.Sleep(100);
            } while (myThread.Count != 10);


            Console.WriteLine("\t\\\\  Class \"MyThreadImproved\" Test //\n");
        }

        public static void MoreThreadsTest()
        {
            Console.WriteLine("\t<<< Class \"MoreThreadTest\" Test >>>");

            MyThreadImproved mt1 = new MyThreadImproved("Thread #1");
            MyThreadImproved mt2 = new MyThreadImproved("Thread #2");
            MyThreadImproved mt3 = new MyThreadImproved("Thread #3");

            do
            {
                Console.Write(".");
                Thread.Sleep(100);
            } while (
                    mt1.Thread.IsAlive
                    && mt2.Thread.IsAlive
                    && mt3.Thread.IsAlive
               );

            Console.WriteLine("\t\\\\  Class \"MoreThreadTest\" Test //\n");
        }

        public static void MoreThreadsJoinTest()
        {
            Console.WriteLine("\t<<< Class \"MoreThreadsJoinTest\" Test >>>");

            MyThreadImproved mt1 = new MyThreadImproved("Thread #1");
            MyThreadImproved mt2 = new MyThreadImproved("Thread #2");
            MyThreadImproved mt3 = new MyThreadImproved("Thread #3");

            mt1.Thread.Join();
            Console.WriteLine(mt1.Thread.Name + " joined");
            Console.WriteLine(mt2.Thread.Name + " joined");
            Console.WriteLine(mt3.Thread.Name + " joined");

            Console.WriteLine("\t\\\\  Class \"MoreThreadsJoinTest\" Test //\n");
        }

        public static void MoreThreadsArgTest()
        {
            Console.WriteLine("\t<<< Class \"MoreThreadsArgTest\" Test >>>");

            MyThreadImproved mt = new MyThreadImproved("Thread #1", 5);
            MyThreadImproved mt2 = new MyThreadImproved("Thread #2", 10);

            do
            {
                Thread.Sleep(100);
            } while (mt.Thread.IsAlive || mt2.Thread.IsAlive);

            Console.WriteLine("\t\\\\  Class \"MoreThreadsArgTest\" Test //\n");
        }

        public static void MyThreadsPriorityTest()
        {
            MyThreadPriority high_prioty_thread
                = new MyThreadPriority("HIGH PRIORITY Thread");
            MyThreadPriority low_prioty_thread
                = new MyThreadPriority("LOW PRIORITY Thread");

            // Изменяем приоритеты потоков
            high_prioty_thread.Thread.Priority = ThreadPriority.AboveNormal;
            low_prioty_thread.Thread.Priority = ThreadPriority.BelowNormal;

            // Запускаем потоки
            high_prioty_thread.Thread.Start();
            low_prioty_thread.Thread.Start();

            high_prioty_thread.Thread.Join();
            low_prioty_thread.Thread.Join();
            Console.WriteLine(
                "Thread \"" + high_prioty_thread.Thread.Name 
                + "\" Counter = " + high_prioty_thread.Count
                );
            Console.WriteLine(
                "Thread \"" + low_prioty_thread.Thread.Name 
                + "\" Counter = " + low_prioty_thread.Count
                );
        }

        public static void MyThreadsLockTest()
        {
            int[] a = {1, 2, 3, 4, 5, 6, 7};
            MyThreadLock mt1 = new MyThreadLock("Thread #1", a);
            MyThreadLock mt2 = new MyThreadLock("Thread #2", a);

            mt1.Thread.Join();
            mt2.Thread.Join();
        }

        public static void TickingClockTest()
        {
            TickTock tt = new TickTock();
            TickTackThread mt1 = new TickTackThread("Tick", tt);
            TickTackThread mt2 = new TickTackThread("Tock", tt);

            mt1.Thread.Join();
            mt2.Thread.Join();

            Console.WriteLine("Clock is Finished");
        }

        static void Main(string[] args)
        {
            //MyThreadImprovedTest(new MyThread("Test_Thread"));
            //MoreThreadsTest();
            //MoreThreadsJoinTest();
            //MoreThreadsArgTest();
            //MyThreadsPriorityTest();
            //MyThreadsLockTest();
            TickingClockTest();
        }
    }
}
