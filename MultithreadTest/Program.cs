using System;
using System.Threading;
using System.Runtime.CompilerServices;

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
    //public void Tick(bool running){
    //    lock (lockOn){
    //        if (running){
                
    //            // Уведомить любые ожидающие потоки
    //            Monitor.Pulse(lockOn); 
    //            return;
    //        }

    //        Console.Write("Tick ");
    //        // разрешить выполнение метода Tick()
    //        Monitor.Pulse(lockOn);
    //        // ожидать завершение выполнения метода Tick()
    //        Monitor.Wait(lockOn); 
    //    }
    //}

    //public void Tock(bool running)
    //{
    //    lock (lockOn)
    //    {
    //        if (running)
    //        {
    //            // Уведомить любые ожидающие потоки
                
    //            Monitor.Pulse(lockOn);
    //            return;
    //        }

    //        Console.Write("Tock \n");
    //        // разрешить выполнение метода Tock()
    //        Monitor.Pulse(lockOn);
    //        // ожидать завершение выполнения метода Tock()
    //        Monitor.Wait(lockOn);
    //    }
    //}

    [MethodImplAttribute(MethodImplOptions.Synchronized)]
    public void Tick(bool running)
    { 
        if (!running)
        {

            // Уведомить любые ожидающие потоки
            Monitor.Pulse(this);
            return;
        }

        Console.Write("Tick ");
        // разрешить выполнение метода Tick()
        Monitor.Pulse(this);
        // ожидать завершение выполнения метода Tick()
        Monitor.Wait(this);
        
    }

    [MethodImplAttribute(MethodImplOptions.Synchronized)]
    public void Tock(bool running)
    {
        if (!running)
        {
            // Уведомить любые ожидающие потоки

            Monitor.Pulse(this);
            return;
        }

        Console.Write("Tock \n");
        // разрешить выполнение метода Tock()
        Monitor.Pulse(this);
        // ожидать завершение выполнения метода Tock()
        Monitor.Wait(this);
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
                this.tt.Tick(true);
            }
            this.tt.Tick(false);
        } else {
            for (int i = 0; i < 5; i++){
                this.tt.Tock(true);
            }
            this.tt.Tock(false);
        }
    }



}

// В этом классе содержится общий ресурс (Count)
// а также мьютекс (Mtx), управляющей доступом к ней
class SharedRes
{
    public static int Count = 0;
    public static Mutex Mtx = new Mutex();
}

// В этом потоке переменная SharedRes.Count инкрементируется
class IncThread
{
    int num;
    public Thread Thrd;

    public IncThread(string name, int n)
    {
        this.Thrd = new Thread(this.Run);
        this.Thrd.Name = name;
        this.num = n;
        this.Thrd.Start();
    }

    public void Run()
    {
        Console.WriteLine(this.Thrd.Name + " Wait Mutex...");

        // Получить Мьютекс
        //SharedRes.Mtx.WaitOne();

        Console.WriteLine(this.Thrd.Name + " Get Mutex");

        do {
            Thread.Sleep(500);
            SharedRes.Count++;
            Console.WriteLine(
                "In Thread " + this.Thrd.Name
                + ", SharedRes.Count = " + SharedRes.Count
                );
            num--;

        } while(num > 0);

        Console.WriteLine(this.Thrd.Name + " Free Mutex");

        // Освободить Мьютекс
        //SharedRes.Mtx.ReleaseMutex();
    }

}

class DecThread
{
    int num;
    public Thread Thrd;

    public DecThread(string name, int n)
    {
        this.Thrd = new Thread(this.Run);
        this.Thrd.Name = name;
        this.num = n;
        this.Thrd.Start();
    }

    public void Run()
    {
        Console.WriteLine(this.Thrd.Name + " Wait Mutex...");

        // Получить Мьютекс
        //SharedRes.Mtx.WaitOne();

        Console.WriteLine(this.Thrd.Name + " Get Mutex");

        do
        {
            Thread.Sleep(500);
            SharedRes.Count--;
            Console.WriteLine(
                "In Thread " + this.Thrd.Name
                + ", SharedRes.Count = " + SharedRes.Count
                );
            num--;

        } while (num > 0);

        Console.WriteLine(this.Thrd.Name + " Free Mutex");

        // Освободить Мьютекс
        //SharedRes.Mtx.ReleaseMutex();
    }

}

// Этот класс разрешает выполнение только двух своих 
// экземпляров
class SemaphoreThread
{
    public Thread Thrd;

    // Здесь создается семафор дающий
    // дающий два разрешения из двух первоначально имеющихся
    static Semaphore sem = new Semaphore(2, 2);

    public SemaphoreThread(string name)
    {
        this.Thrd = new Thread(this.Run);
        this.Thrd.Name = name;
        this.Thrd.Start();
    }

    void Run()
    {
        Console.WriteLine(this.Thrd.Name + " wait permittion...");
        SemaphoreThread.sem.WaitOne();

        Console.WriteLine(this.Thrd.Name + " get permittion");

        for(char ch = 'A'; ch < 'D'; ch++)
        {
            Console.WriteLine(this.Thrd.Name + " : " + ch);
        }

        // Освободить семафор
        SemaphoreThread.sem.Release();
    }
}

class EventThread
{
    public Thread Thrd;
    ManualResetEvent mre;

    public EventThread(string name, ManualResetEvent evt)
    {
        this.Thrd = new Thread(this.Run);
        this.Thrd.Name = name;
        this.mre = evt;
        this.Thrd.Start();
    }

    void Run()
    {
        Console.WriteLine("Inside of " + this.Thrd.Name);

        for(int i = 0; i < 5; i++)
        {
            Console.WriteLine(this.Thrd.Name);
            Thread.Sleep(500);
        }

        Console.WriteLine(this.Thrd.Name + " finished!!!");

        // Уведомить о событии
        this.mre.Set();
    }
}

class AbortThread
{
    public Thread Thrd;

    public AbortThread(string name) {
        this.Thrd = new Thread(this.Run);
        this.Thrd.Name = name;
        this.Thrd.Start();
    }

    void Run()
    {
        try
        {
            Console.WriteLine(this.Thrd + " started");

            for (int i = 1; i <= 1000; i++)
            {
                Console.Write(i + " ");
                if (i % 10 == 0)
                {
                    Console.WriteLine();
                    Thread.Sleep(250);
                }


            }

            Console.WriteLine(this.Thrd.Name + " finished.");
        }catch (ThreadAbortException exc){
            Console.WriteLine("Thread had been interrupted. Error code " + exc.ExceptionState);
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

        public static void MutexTest()
        {
            // Сконструировать два потока
            IncThread mt1 = new IncThread("IncThread", 5);

            // Разрешить инкрементируемому потоку начаться
            Thread.Sleep(1);

            DecThread mt2 = new DecThread("DecThread", 5);

            mt1.Thrd.Join();
            mt2.Thrd.Join();
        }

        public static void SemaphoreTest()
        {
            SemaphoreThread mt1 = new SemaphoreThread("Thread #1");
            SemaphoreThread mt2 = new SemaphoreThread("Thread #2");
            SemaphoreThread mt3 = new SemaphoreThread("Thread #3");

            mt1.Thrd.Join();
            mt2.Thrd.Join();
            mt3.Thrd.Join();
        }

        public static void ManualEventTest()
        {
            ManualResetEvent evtObj = new ManualResetEvent(false);
            EventThread mt1 = new EventThread("EventThread #1", evtObj);
            Console.WriteLine("Main thread waiting for event");

            // Ожидать уведомление о событии
            evtObj.WaitOne();

            Console.WriteLine("Main thread get signal from " + mt1.Thrd.Name);

            // Установить событийный объект в исходное состояние
            evtObj.Reset();

            mt1 = new EventThread("EventThread #2", evtObj);

            // Ожидать уведомление о событии.
            evtObj.WaitOne();
            

            Console.WriteLine("Main thread get signal from " + mt1.Thrd.Name);
        }

        static void AbortTest()
        {
            AbortThread mt1 = new AbortThread("Abort Thred #1");

            //  Разрешить порожденному потоку начать свое выполнение
            Thread.Sleep(500);

            Console.WriteLine("Thread Interruption");
            mt1.Thrd.Abort(100);

            mt1.Thrd.Join(); // Ожидать прерывание потока

            Console.WriteLine("Main Thread Interrupted");
        }

        static void Main(string[] args)
        {
            //MyThreadImprovedTest(new MyThread("Test_Thread"));
            //MoreThreadsTest();
            //MoreThreadsJoinTest();
            //MoreThreadsArgTest();
            //MyThreadsPriorityTest();
            //MyThreadsLockTest();
            //TickingClockTest();
            //MutexTest();
            //SemaphoreTest();
            //ManualEventTest();
            AbortTest();
        }
    }
}
