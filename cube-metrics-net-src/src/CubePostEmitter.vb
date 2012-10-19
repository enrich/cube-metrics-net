Imports System.Net
Imports System.IO
Imports System.Text
Imports NLog

''' <summary>
''' emit via HTTP POST.
''' </summary>
Public Class CubePostEmitter
    Implements CubeEmitter
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()
    Private ReadOnly _cubecollectorhost As String

    Public Sub New(ByVal cubecollectorhost As String)
        _cubecollectorhost = cubecollectorhost
    End Sub

    Public Function emit(ByVal ce As List(Of CubeEvent)) As Boolean Implements CubeEmitter.emit
        ' TODO: use websockets, in .net 4.5.
        Dim turi As New Uri(_cubecollectorhost & "/1.0/event/put")
        Dim request As HttpWebRequest = CType(WebRequest.Create(turi), HttpWebRequest)
        request.Method = "POST"
        Dim vvs As String = CubeEvent.AsJSON(ce)

        _logger.Trace("emit request: {0}", vvs)
        Dim cb As Byte() = Encoding.UTF8.GetBytes(vvs)

        _logger.Trace("writing bytes: {0}", cb.Count)

        Try
            Using rs As Stream = request.GetRequestStream()
                rs.Write(cb, 0, cb.Count())
                rs.Flush()
                rs.Close()
                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    If response.StatusCode <> HttpStatusCode.OK Then
                        _logger.Error("emit error uri: {0} response: {1}", turi.ToString(), response.StatusCode)
                        Return False
                    End If
                    Return True
                End Using
            End Using
        Catch ex As WebException
            _logger.TraceException("failure connecting to " & request.RequestUri.ToString(), ex)
            Return False
        End Try
    End Function


End Class
