using RabbitmqExample.Common;

namespace RabbitmqExample.Publishers.Models;

public class Publisher : PublisherBase
{
    #region Constructors

    public Publisher(List<string> queueNames)
        : base(queueNames) { }

    #endregion // Constructors
}
