
Imports cube_metrics_net

''' <summary>
''' for testing
''' </summary>
Public Class CubeFakeUnreliableEmitter
    Implements CubeEmitter
    Public ReadOnly events As New List(Of CubeEvent)
    Public attempts As Integer
    Private happy As Boolean = True
    Private ReadOnly _lck As New Object

    Public Function emit(ByVal ce As List(Of CubeEvent)) As Boolean Implements CubeEmitter.emit
        SyncLock _lck
            attempts += 1
            If Not happy Then Return False
            events.AddRange(ce)
        End SyncLock
        Return True
    End Function

    Public WriteOnly Property HappyProperty() As Boolean
        Set(ByVal value As Boolean)
            SyncLock _lck
                happy = value
            End SyncLock
        End Set
    End Property
End Class
