using System;
using System.Threading;
using System.Collections.Generic;
using static Laba_4._2_2023.EventBus;

class Program
{
    static void Main(string[] args)
    {
        var publisher = new Publisher();

        var subscriber1 = new Subscriber(publisher, 1);
        var subscriber2 = new Subscriber(publisher, 2);
        var subscriber3 = new Subscriber(publisher, 3);

        subscriber1.Subscribe("event1", () => Console.WriteLine("Event1 handler 1 called"));
        subscriber2.Subscribe("event1", () => Console.WriteLine("Event1 handler 2 called"));
        subscriber2.Subscribe("event2", () => Console.WriteLine("Event2 handler 1 called"));
        subscriber3.Subscribe("event1", () => Console.WriteLine("Event1 handler 3 called"));
        subscriber3.Subscribe("event2", () => Console.WriteLine("Event2 handler 2 called"));

        publisher.RaiseEvent("event1");
        publisher.RaiseEvent("event2");

        subscriber1.Unsubscribe("event1", () => Console.WriteLine("Event1 handler 1 called"));

        publisher.RaiseEvent("event1");
        publisher.RaiseEvent("event2");
    }
}