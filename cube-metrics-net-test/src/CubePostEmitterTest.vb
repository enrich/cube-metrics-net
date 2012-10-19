Imports cube_metrics_net
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' I used a real cube host for this test; supply one and reenable the test if you want.
''' </summary>
<Ignore()>
<TestClass()>
Public Class CubePostEmitterTest

    Dim host As String = "your host here"

    <TestMethod()>
    Public Sub sendJson()
        Dim c As New CubeEvent("footype",
                               DateTime.Parse("2012-09-24T11:45:34"),
                               Nothing,
                               New CubeData From {{"duration_ms", 241}})
        Dim cs As New List(Of CubeEvent)({c})
        Dim cpe As New CubePostEmitter(host)
        cpe.emit(cs)
    End Sub

    <TestMethod()>
    Public Sub sendALot()
        Dim cpe As New CubePostEmitter(host)
        Dim r As New Random(CInt(DateTime.Now().TimeOfDay.TotalSeconds))
        For i As Integer = 1 To 100
            Dim cs As New List(Of CubeEvent)
            For j As Integer = 1 To 50

                '' value below is for the dashboard i happen to have.
                cs.Add(New CubeEvent("foo",
                                     DateTime.Now().Subtract(TimeSpan.FromMilliseconds(r.NextDouble() * 100000000)),
                                    Nothing,
                                    New CubeData From {
                                         {"duration_ms", r.NextDouble()},
                                         {"value", r.NextDouble()},
                                         {"client", "test"}
                                     }))


            Next
            cpe.emit(cs)
        Next
    End Sub
End Class
