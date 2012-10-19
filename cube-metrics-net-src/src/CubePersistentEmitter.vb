Imports System.Collections.Concurrent
Imports NLog
Imports System.Threading

''' <summary>
''' Retries failures, for events with 'id' field.  Default is to try once a minute, up to three times per batch
''' </summary>
    Public Class CubePersistentEmitter
    Implements CubeEmitter
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Private Class RetryBatch
        Public ReadOnly events As List(Of CubeEvent)
        Public ReadOnly retries As Integer

        Public Sub New(ByVal e As List(Of CubeEvent), ByVal i As Integer)
            events = e
            retries = i
        End Sub
    End Class

    Private ReadOnly _dest As CubeEmitter
    Private ReadOnly _timer As Timer
    Private ReadOnly _retryQueue As New ConcurrentQueue(Of RetryBatch)
    Private ReadOnly _maxRetries As Integer

    Public Sub New(ByVal dest As CubeEmitter)
        Me.New(dest, 3, TimeSpan.FromMinutes(1))
    End Sub

    Public Sub New(ByVal dest As CubeEmitter,
                   ByVal maxRetries As Integer,
                   ByVal retryPeriod As TimeSpan)
        _dest = dest
        _maxRetries = maxRetries
        _timer = New System.Threading.Timer(Sub() retry(), Nothing, New TimeSpan(0), retryPeriod)
    End Sub

    Public Function emit(ce As List(Of CubeEvent)) As Boolean Implements CubeEmitter.emit
        _logger.Trace("try emitting: " & ce.Count)
        Try
            If _dest.emit(ce) Then Return True
        Catch ex As Exception
            ' catch EVERYTHING!  maybe not the best idea.  :-)
            _logger.TraceException("caught exception", ex)
        End Try

        _logger.Trace("failed, requeue records with 'id' fields")
        ' non-id'ed ones might create duplicates, so omit them.
        Dim retryEvents As New List(Of CubeEvent)
        For Each c As CubeEvent In ce
            If c.id IsNot Nothing Then retryEvents.Add(c)
        Next
        If retryEvents.Count > 0 Then
            _logger.Trace("requeuing: " & retryEvents.Count)
            _retryQueue.Enqueue(New RetryBatch(retryEvents, 1))
        End If
        Return True
    End Function

    Private Sub retry()
        Try
            Dim rb As RetryBatch = Nothing
            While _retryQueue.TryDequeue(rb)
                If Not _dest.emit(rb.events) Then
                    If rb.retries < _maxRetries Then
                        _retryQueue.Enqueue(New RetryBatch(rb.events, rb.retries + 1))
                    Else
                        _logger.Trace("giving up on retry for batch of size: " & rb.events.Count)
                    End If
                End If
            End While
        Catch ex As Exception
            _logger.TraceException("caught exception", ex)
        End Try
    End Sub
End Class
