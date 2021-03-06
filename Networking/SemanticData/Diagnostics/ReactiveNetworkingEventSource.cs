﻿
using System.Diagnostics.Tracing;

namespace UAOOI.Networking.SemanticData.Diagnostics
{
  [EventSource(Name = "UAOOI-Networking-SemanticData-Diagnostics", Guid = "C8666C20-6FEF-4DD0-BB66-5807BA629DA8")]
  public class ReactiveNetworkingEventSource : EventSource
  {
    ///// <summary>
    ///// Class Keywords - defines the local keywords (flags) that apply to events.
    ///// </summary>
    //internal class Keywords
    //{
    //  //public const EventKeywords Package = (EventKeywords)1;
    //  public const EventKeywords Diagnostic2 = (EventKeywords)2;
    //  //public const EventKeywords Performance = (EventKeywords)3;
    //}
    /// <summary>
    /// Class Tasks.
    /// </summary>
    public class Tasks
    {
      public const EventTask Consumer = (EventTask)1;
      public const EventTask Producer = (EventTask)2;
      public const EventTask Stack = (EventTask)3;
      public const EventTask Infrastructure = (EventTask)4;
      public const EventTask CodeBehavior = (EventTask)5;
    }
    /// <summary>
    /// Gets the log - implements singleton of the <see cref="ReactiveNetworkingEventSource"/>.
    /// </summary>
    /// <value>The log.</value>
    internal static ReactiveNetworkingEventSource Log { get; } = new ReactiveNetworkingEventSource();

    [Event(1, Message = "At {0}.{1} encountered application failure: {2}", Opcode = EventOpcode.Info, Task = Tasks.CodeBehavior, Level = EventLevel.Error/*, Keywords = Keywords.Diagnostic*/)]
    public void Failure(string className, string methodName, string problem)
    {
      WriteEvent(1, className, methodName, problem);
    }
    //[Event(2, Message = "Starting up.", Keywords = Keywords.Performance, Level = EventLevel.Informational)]
    //internal void Startup()
    //{
    //  this.WriteEvent(2);
    //}
    //[Event(3, Message = "Entering method {0}.{1}", Opcode = EventOpcode.Start, Task = EventTask.None, Keywords = Keywords.Performance, Level = EventLevel.Informational)]
    //internal void EnteringMethod(string className, string methodName)
    //{
    //  if (this.IsEnabled()) this.WriteEvent(3, className, methodName);
    //}
    [Event(4, Message = "Unexpected end of message while reading {0} element.", Channel = EventChannel.Operational, Opcode = EventOpcode.Info, Task = Tasks.Consumer, Level = EventLevel.Warning)]//)]Keywords = (EventKeywords)1500,
    internal void MessageInconsistency(int position)
    {
      this.WriteEvent(4, position);
    }
    private ReactiveNetworkingEventSource() { }
  }
}
