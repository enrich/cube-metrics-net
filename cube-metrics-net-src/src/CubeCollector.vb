
Imports System.Collections.Concurrent
Imports NLog


''' <summary>
''' Collects events (pushed) and observations (pulled) to send to the cube server.
''' Default is to buffer, retry, and post http, every minute
''' </summary>
Public Class CubeCollector

    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    ''' <summary>
    ''' The set of observables the timer should visit
    ''' </summary>
    Private ReadOnly _measures As New ConcurrentBag(Of Measure)

    Private ReadOnly _timer As System.Threading.Timer
    Private ReadOnly _emitter As CubeEmitter
    ''' <summary>
    ''' Written into each event prior to emit
    ''' </summary>
    Private ReadOnly _globalData As CubeData
    ''' <summary>
    ''' data field to fill with the elapsed time
    ''' </summary>
    Private ReadOnly _elapsedTimeField As String

    Public Interface Measure
        Function value() As List(Of CubeEvent)
    End Interface

    Public Sub New(ByVal ce As CubeEmitter, ByVal observationPeriod As TimeSpan, ByVal globalData As CubeData, ByVal field As String)
        _emitter = ce
        _timer = New System.Threading.Timer(Sub() MeasureAll(), Nothing, New TimeSpan(0), observationPeriod)
        _globalData = globalData
        _elapsedTimeField = field
    End Sub

    ''' <summary>
    ''' Time an event, if supplied, and record it
    ''' </summary>
    Public Function Record(Of T)(ByVal type As String,
                                 ByVal d As CubeData,
                                 ByVal action As Func(Of T)) As T
        Dim dd As CubeData = CubeData.Copy(d)
        Dim result As T = TimeAction(dd, action)
        Try
            _emitter.emit(New List(Of CubeEvent)({New CubeEvent(type, Now(), Nothing, dd)}))
        Catch ex As Exception
            _logger.ErrorException("error in emit", ex)
        End Try
        Return result
    End Function

    Public Sub Record(ByVal type As String, ByVal d As CubeData, ByVal action As Action)
        Dim dd As CubeData = CubeData.Copy(d)
        TimeAction(dd, action)
        Try
            _emitter.emit(New List(Of CubeEvent)({New CubeEvent(type, Now(), Nothing, dd)}))
        Catch ex As Exception
            _logger.ErrorException("error in emit", ex)
        End Try
    End Sub

    Public Sub Record(ByVal type As String, ByVal d As CubeData)
        Dim dd As CubeData = CubeData.Copy(d)
        Try
            _emitter.emit(New List(Of CubeEvent)({New CubeEvent(type, Now(), Nothing, dd)}))
        Catch ex As Exception
            _logger.ErrorException("error in emit", ex)
        End Try
    End Sub

    Private Function TimeAction(Of T)(ByVal d As CubeData,
                                      ByVal action As Func(Of T)) As T
        If action Is Nothing Then Return Nothing
        Dim sw As New Stopwatch
        Try
            sw.Start()
            Return action()
        Finally
            sw.Stop()
            d(_elapsedTimeField) = sw.ElapsedMilliseconds
        End Try
    End Function

    Private Sub TimeAction(ByVal d As CubeData, ByVal action As Action)
        If action Is Nothing Then Return
        Dim sw As New Stopwatch
        Try
            sw.Start()
            action()
        Finally
            sw.Stop()
            d(_elapsedTimeField) = sw.ElapsedMilliseconds
        End Try
    End Sub

    Private Sub MeasureAll()
        Try
            _logger.Trace("measuring")
            Dim events As New List(Of CubeEvent)
            For Each ob As Measure In _measures
                events.AddRange(ob.value())
            Next
            For Each e As CubeEvent In events
                For Each kv As KeyValuePair(Of String, Object) In _globalData
                    e.data(kv.Key) = kv.Value
                Next
            Next
            _emitter.emit(events)
        Catch ex As Exception
            _logger.ErrorException("caught exception", ex)
        End Try
    End Sub

    ''' <summary>
    ''' I will poll the supplied measure periodically.
    ''' </summary>
    Public Sub RegisterMeasure(ByVal obs As Measure)
        If obs IsNot Nothing Then _measures.Add(obs)
    End Sub
End Class
