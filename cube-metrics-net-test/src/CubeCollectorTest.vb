Imports cube_metrics_net
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Threading

<TestClass()>
Public Class CubeCollectorTest

    <TestMethod()>
    Public Sub mistakes()
        Dim cfe As New CubeFakeEmitter()
        Dim cc As CubeCollector = New CubeCollector(cfe, TimeSpan.FromMinutes(10), New CubeData, "value")
        cc.Record(Of String)("foo", New CubeData From {{"foo", "bar"}}, Nothing)
        cc.Record(Of String)("foo", Nothing, Function()
                                                 Thread.Sleep(100)
                                                 Return Nothing
                                             End Function)
        cc.RegisterMeasure(Nothing)
    End Sub


    <TestMethod()>
    Public Sub collectEvents()
        Dim cfe As New CubeFakeEmitter()
        Dim cc As CubeCollector = New CubeCollector(cfe, TimeSpan.FromMinutes(10), New CubeData, "value")
        cc.Record(Of String)("foo", New CubeData From {{"foo", "bar"}}, Function()
                                                                            Thread.Sleep(100)
                                                                            Return Nothing
                                                                        End Function)
        Assert.AreEqual(1, cfe.events.Count)
        ' a little less than 100, there's some jitter.  i saw 91 once.  weird.
        Assert.IsTrue(CInt(cfe.events(0).data("value")) > 95, CStr(cfe.events(0).data("value")))
        Assert.AreEqual("bar", cfe.events(0).data("foo"))
    End Sub

    Private Class MyMeasure
        Implements CubeCollector.Measure
        Public observations As Integer

        Public Function value() As List(Of CubeEvent) Implements CubeCollector.Measure.value
            Interlocked.Increment(observations)
            Return New List(Of CubeEvent)({New CubeEvent("foo", Now(), Nothing, Nothing)})
        End Function
    End Class

    <TestMethod()>
    Public Sub collectMeasures()
        Dim cfe As New CubeFakeEmitter()
        Dim cc As CubeCollector =
            New CubeCollector(cfe, TimeSpan.FromMilliseconds(100), New CubeData, "value")
        Dim mm As New MyMeasure
        cc.RegisterMeasure(mm)
        ' let the observations occur
        Thread.Sleep(1000)

        Assert.IsTrue(mm.observations > 1, "there should be some observations")
        Assert.IsTrue(cfe.events.Count > 1, "there should be some events")
    End Sub

    <TestMethod()>
    Public Sub testStatic()
        ' set up the statics
        Dim cfe As CubeFakeEmitter = New CubeFakeEmitter()

        ' this is how you would get the collector from some random place
        Dim cc As CubeCollector = New CubeCollector(cfe, TimeSpan.FromMilliseconds(100), New CubeData, "value")
        cc.Record("foo", New CubeData From {{"foo", "bar"}}, Sub() Thread.Sleep(100))
        Assert.AreEqual(1, cfe.events.Count)
        ' a little less than 100, there's some jitter.  i saw 91 once.  weird.
        Assert.IsTrue(CInt(cfe.events(0).data("value")) > 95, CStr(cfe.events(0).data("value")))
        Assert.AreEqual("bar", cfe.events(0).data("foo"))

    End Sub
End Class
