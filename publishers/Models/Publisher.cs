using RabbitmqExample.Common;

namespace RabbitmqExample.Publishers.Models;

public class Publisher : PublisherBase
{
    #region Constructors

    public Publisher()
        : base() { }

    #endregion // Constructors

    #region Properties

    protected override string HostName => "localhost";

    #endregion // Properties
}
