Imports System.Collections.Concurrent
Imports NLog

''' <summary>
''' This emitter uses an unbounded buffer.  Default is to flush every minute.
''' </summary>
Public Class CubeBufferingEmitter
    Implements CubeEmitter
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()
    Private ReadOnly _dest As CubeEmitter
    Private ReadOnly _timer As System.Threading.Timer
    Private ReadOnly _queue As New ConcurrentQueue(Of List(Of CubeEvent))

    Public Sub New(ByVal dest As CubeEmitter)
        Me.New(dest, TimeSpan.FromMinutes(1))
    End Sub

    Public Sub New(ByVal dest As CubeEmitter, ByVal emitPeriod As TimeSpan)
        _dest = dest
        _timer = New System.Threading.Timer(Sub() emitBatch(), Nothing, New TimeSpan(0), emitPeriod)
    End Sub

    Public Function emit(ByVal ce As List(Of CubeEvent)) As Boolean Implements CubeEmitter.emit
        _queue.Enqueue(ce)
        Return True
    End Function

    Private Sub emitBatch()
        Try
            Dim all As New List(Of CubeEvent)
            Dim ce As List(Of CubeEvent) = Nothing
            ' this can consume stuff that has been added since we started;
            ' if we fall behind, then another timer thread will come in
            ' to help, until we have enough threads to keep pace.
            While _queue.TryDequeue(ce)
                all.AddRange(ce)
            End While
            Try
                If Not _dest.emit(all) Then
                    _logger.Error("Failure to emit: dropping records")
                End If
            Catch ex As Exception
                _logger.ErrorException("Exception in emit: dropping records", ex)
            End Try
        Catch ex As Exception
            _logger.ErrorException("Caught strange exception in emit", ex)
        End Try
    End Sub
End Class
