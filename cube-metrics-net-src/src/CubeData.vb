
Imports Newtonsoft.Json

<JsonDictionary()>
Public Class CubeData
    Inherits Dictionary(Of String, Object)

    Public Sub New(ByVal dictionary As IDictionary(Of String, Object))
        MyBase.New(dictionary)
    End Sub

    Public Sub New()
    End Sub

    Public Shared Function Copy(ByVal cd As CubeData) As CubeData
        If cd Is Nothing Then Return New CubeData
        Return New CubeData(cd)
    End Function

    ''' <summary>
    ''' write the other's contents into me
    ''' </summary>
    Public Function Merge(ByVal other As IDictionary(Of String, Object)) As CubeData
        For Each kv As KeyValuePair(Of String, Object) In other
            Me(kv.Key) = kv.Value
        Next
        Return Me
    End Function
End Class