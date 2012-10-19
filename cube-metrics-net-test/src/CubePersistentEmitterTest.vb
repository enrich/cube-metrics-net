Imports cube_metrics_net
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Threading

<TestClass()>
Public Class CubePersistentEmitterTest
    <TestMethod()>
    Public Sub noRetry()
        Dim cfue As New CubeFakeUnreliableEmitter()
        Dim cpe As New CubePersistentEmitter(cfue, 0, TimeSpan.FromMilliseconds(100))
        Assert.IsTrue(cpe.emit(New List(Of CubeEvent)({New CubeEvent("foo", Now(), Nothing, Nothing)})))
        ' it did not need to retry
        Assert.AreEqual(1, cfue.events.Count)
        Assert.AreEqual(1, cfue.attempts)
    End Sub

    <TestMethod()>
    Public Sub noID()
        Dim cfue As New CubeFakeUnreliableEmitter()
        cfue.HappyProperty = False
        Dim cpe As New CubePersistentEmitter(cfue, 2, TimeSpan.FromMilliseconds(100))
        Assert.IsTrue(cpe.emit(New List(Of CubeEvent)({New CubeEvent("foo", Now(), Nothing, Nothing)})))

        ' first attempt failed
        Assert.AreEqual(1, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

        cfue.HappyProperty = True
        ' after waiting long enough we discover that the item was never
        ' queued for retry, because it has no id field
        Thread.Sleep(400)
        Assert.AreEqual(1, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

    End Sub

    <TestMethod()>
    Public Sub retryWithSuccess()
        Dim cfue As New CubeFakeUnreliableEmitter()
        cfue.HappyProperty = False
        Dim cpe As New CubePersistentEmitter(cfue, 2, TimeSpan.FromMilliseconds(200))
        Assert.IsTrue(cpe.emit(New List(Of CubeEvent)({New CubeEvent("foo", Now(), "1", Nothing)})))

        ' first attempt failed
        Assert.AreEqual(1, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

        cfue.HappyProperty = True
        ' wait awhile, and we find that the second attempt succeeds.
        Thread.Sleep(400)
        Assert.AreEqual(2, cfue.attempts)
        Assert.AreEqual(1, cfue.events.Count)

    End Sub

    <TestMethod()>
    Public Sub retryWithGiveUp()
        Dim cfue As New CubeFakeUnreliableEmitter()
        cfue.HappyProperty = False
        Dim cpe As New CubePersistentEmitter(cfue, 2, TimeSpan.FromMilliseconds(200))
        Assert.IsTrue(cpe.emit(New List(Of CubeEvent)({New CubeEvent("foo", Now(), "1", Nothing)})))

        ' first attempt failed
        Assert.AreEqual(1, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

        ' wait for all the other attempts to occur, then give up
        Thread.Sleep(400)
        Assert.AreEqual(3, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

        ' now even if the destination recovers, we never get the events
        cfue.HappyProperty = True
        Thread.Sleep(400)
        Assert.AreEqual(3, cfue.attempts)
        Assert.AreEqual(0, cfue.events.Count)

    End Sub

End Class
