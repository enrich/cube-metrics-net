Imports cube_metrics_net
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Threading

<TestClass()>
Public Class CubeBufferingEmitterTest
    <TestMethod()>
    Public Sub buffer()
        Dim cfe As New CubeFakeEmitter()
        Dim cbe As New CubeBufferingEmitter(cfe, TimeSpan.FromMilliseconds(100))
        Assert.IsTrue(cbe.emit(New List(Of CubeEvent)({New CubeEvent("foo", Now(), Nothing, Nothing)})))
        Thread.Sleep(200)
        Assert.AreEqual(1, cfe.events.Count)
    End Sub
End Class
