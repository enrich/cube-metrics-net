''' <summary>
''' send stuff to the cube server
''' </summary>
Public Interface CubeEmitter

    ''' <summary>
    ''' Send the events.
    ''' 
    ''' Returns false if there was an error, i.e. some
    ''' or all the events were not emitted.
    ''' </summary>
    Function emit(ByVal ce As List(Of CubeEvent)) As Boolean
End Interface
