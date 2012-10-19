
Imports cube_metrics_net

''' <summary>
''' for testing
''' </summary>
Public Class CubeFakeEmitter
    Implements CubeEmitter
    Public ReadOnly events As New List(Of CubeEvent)
    Private ReadOnly _lck As New Object

    Public Function emit(ByVal ce As List(Of CubeEvent)) As Boolean Implements CubeEmitter.emit
        SyncLock _lck
            events.AddRange(ce)
        End SyncLock
        Return True
    End Function
End Class
