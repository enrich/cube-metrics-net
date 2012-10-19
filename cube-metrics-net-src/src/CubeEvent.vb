Imports Newtonsoft.Json
Imports System.Text.RegularExpressions

''' <summary>
''' These are the fields defined by Cube.
''' 
''' Note: the type field must be alphanumeric or underscore.
''' </summary>
<JsonObject()>
Public Class CubeEvent

    Public Class TypeException
        Inherits Exception
        Public Sub New(ByVal s As String)
            MyBase.new(s)
        End Sub
    End Class

    Private Shared ReadOnly okType As New Regex("^[a-z_][a-zA-Z0-9_]*$")
    Private Shared ReadOnly settings As New JsonSerializerSettings

    <JsonProperty()>
    Private ReadOnly type As String

    <JsonProperty()>
    Private ReadOnly time As DateTime

    <JsonProperty()>
    Public ReadOnly id As Object

    <JsonProperty()>
    Public ReadOnly data As CubeData

    Public Sub New(ByVal type As String,
                   ByVal time As Date,
                   ByVal id As Object,
                   ByVal data As CubeData)
        If Not okType.IsMatch(type) Then Throw New TypeException("bad type: " & type)
        Me.type = type
        Me.time = time
        Me.id = id
        Me.data = data
    End Sub

    Shared Sub New()
        settings.NullValueHandling = NullValueHandling.Ignore
    End Sub

    Public Shared Function AsJSON(ByVal ce As CubeEvent) As String
        Return JsonConvert.SerializeObject(ce, settings)
    End Function

    Public Shared Function AsJSON(ByVal ces As List(Of CubeEvent)) As String
        Return JsonConvert.SerializeObject(ces, settings)
    End Function
End Class
