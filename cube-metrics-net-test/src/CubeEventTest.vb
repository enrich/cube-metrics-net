Imports cube_metrics_net
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class CubeEventTest
    <TestMethod()>
    Public Sub makeJson()
        Dim c As New CubeEvent("footype",
                               DateTime.Parse("2012-09-24T11:45:34"),
                               Nothing,
                               New CubeData From {{"duration_ms", 241}})

        Dim vv As String = CubeEvent.AsJSON(c)
        Dim expected As String =
                <a><![CDATA[{"type":"footype","time":"2012-09-24T11:45:34","data":{"duration_ms":241}}]]></a>.Value
        Assert.AreEqual(expected, vv)

        Dim cs As New List(Of CubeEvent)({c})

        Dim vvs As String = CubeEvent.AsJSON(cs)
        Dim expecteds As String = "[" & expected & "]"
        Assert.AreEqual(expecteds, vvs)
    End Sub

    <TestMethod()>
    Public Sub makeJsonWithId()
        Dim c As New CubeEvent("footype",
                               DateTime.Parse("2012-09-24T11:45:34"),
                               "abcd",
                               New CubeData From {{"duration_ms", 241}})

        Dim vv As String = CubeEvent.AsJSON(c)
        Dim expected As String =
                <a><![CDATA[{"type":"footype","time":"2012-09-24T11:45:34","id":"abcd","data":{"duration_ms":241}}]]></a>.Value
        Assert.AreEqual(expected, vv)

        Dim cs As New List(Of CubeEvent)({c})

        Dim vvs As String = CubeEvent.AsJSON(cs)
        Dim expecteds As String = "[" & expected & "]"
        Assert.AreEqual(expecteds, vvs)
    End Sub

    <TestMethod()>
    Public Sub makeJsonWithBadType()
        ' space is not allowed
        Try
            Dim c As New CubeEvent("foo type",
                                   DateTime.Parse("2012-09-24T11:45:34"),
                                   "abcd",
                                   New CubeData From {{"duration_ms", 241}})
            Assert.IsNotNull(c)
            Assert.Fail()
        Catch ex As Exception
            ' expected
        End Try
    End Sub

End Class
