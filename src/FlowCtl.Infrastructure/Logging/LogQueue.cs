using System.Collections.Concurrent;

namespace FlowCtl.Infrastructure.Logging;

public class LogQueue: ConcurrentQueue<LogMessage>
{

}
