cube-metrics-net
================

Vb.net collector/emitter for [mbostock's cube metrics.](http://square.github.com/cube/)

It's as simple as possible:

The collector can receive events, or measure the duration of actions,
and it can also probe observable measures periodically.

There are a few emitters to choose from; the default stack of decorators
includes buffering and retrying.


Installation
============

Fork and add as a submodule.  Add the cube-metrics-net-src project to your solution.


Usage
=====

Measure the duration of actions:

``` vb.net
Dim cubehost As String = "http://your.cube.server:1080"
Dim cc As New CubeCollector.Factory(cubehost).getCubeCollector(),
Dim dd As New CubeData From {{"key", "value"}}
cc.Record(type, dd, Sub() SomeAction())
``` 

Measure things periodically:

``` vb.net
Class MyMeasure
    Implements CubeCollector.Measure

    Public Function value() As List(Of CubeEvent) Implements CubeCollector.Measure.value
        dim ev as New CubeEvent("foo", Now(), Nothing, Nothing)
        Return New List(Of CubeEvent)({ev})
    End Function
End Class

Dim obs as new MyMeasure()
cc.RegisterMeasure(obs)
``` 


The measure specifies the time, in order to report things that happened in the past.

There's no collector injection mechanism, or singleton; use whichever method you prefer.


Feedback
========

If you use this code, please let me know!  If anything was confusing or broken, please let me know!


Contributing
============

Feedback, suggestions, code reviews, pull requests, are all welcome.


License
=======

[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0)



Dependencies
============

Includes [Json.NET](http://james.newtonking.com/projects/json-net.aspx) ([MIT](http://json.codeplex.com/license) license) and [Nlog](http://nlog-project.org/) ([BSD](http://www.opensource.org/licenses/bsd-license.php) license)
