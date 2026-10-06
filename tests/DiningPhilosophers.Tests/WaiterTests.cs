using wspolbiezne2;
using Xunit;

namespace DiningPhilosophers.Tests;

public sealed class WaiterTests
{
    [Fact]
    public void CancellationAfterGrantingForksDoesNotLeaveThemReserved()
    {
        var waiter = new Waiter(3);
        using var firstToken = new CancellationTokenSource();
        using (var reservation = waiter.RequestToEat(0, firstToken.Token))
        {
            Assert.NotNull(reservation);
            firstToken.Cancel();
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        using var nextReservation = waiter.RequestToEat(1, timeout.Token);
        Assert.NotNull(nextReservation);
    }

    [Fact]
    public void CancelledQueueEntryDoesNotBlockTheNextRequest()
    {
        var waiter = new Waiter(3);
        using var firstReservation = waiter.RequestToEat(0, CancellationToken.None);
        using var cancelledToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));

        using var cancelledReservation = waiter.RequestToEat(1, cancelledToken.Token);
        Assert.Null(cancelledReservation);
        firstReservation!.Dispose();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        using var nextReservation = waiter.RequestToEat(1, timeout.Token);
        Assert.NotNull(nextReservation);
    }
}
