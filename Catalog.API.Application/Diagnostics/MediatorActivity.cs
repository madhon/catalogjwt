namespace Catalog.API.Application.Diagnostics;

using System.Diagnostics;

public static class MediatorActivity
{
    public static readonly ActivitySource Source = new("Catalog.API.Mediator");
}