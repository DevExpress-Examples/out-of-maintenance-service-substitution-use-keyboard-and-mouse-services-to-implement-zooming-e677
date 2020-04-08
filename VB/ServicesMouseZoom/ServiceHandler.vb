Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports DevExpress.Services
Imports DevExpress.Portable.Input

Namespace ServicesMouseZoom
	Public Class MyMouseHandlerService
		Inherits MouseHandlerServiceWrapper

		Private provider As IServiceProvider
'INSTANT VB NOTE: The field myzoomEnabled was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private myzoomEnabled_Conflict As Boolean

		Public Sub New(ByVal provider As IServiceProvider, ByVal service As IMouseHandlerService)
			MyBase.New(service)
			Me.provider = provider
		End Sub

		Public Overrides Sub OnMouseWheel(ByVal e As PortableMouseEventArgs)
			Dim curView As DayView = DirectCast(provider, SchedulerControl).DayView
			Dim nSlots As Integer = curView.TimeSlots.Count

			If Me.myzoomEnabled_Conflict Then
				If e.Delta < 0 Then
					For i As Integer = 0 To nSlots - 1
						If curView.TimeSlots(i).Value <= curView.TimeScale Then
							curView.TimeScale = curView.TimeSlots(i).Value
							Exit For
						End If
					Next i

				Else
					For i As Integer = nSlots - 1 To 0 Step -1
						If curView.TimeSlots(i).Value >= curView.TimeScale Then
							curView.TimeScale = curView.TimeSlots(i).Value
							Exit For
						End If
					Next i
				End If
			Else
				MyBase.OnMouseWheel(e)
			End If
		End Sub
		Public Property myZoomEnabled() As Boolean
			Get
				Return Me.myzoomEnabled_Conflict
			End Get
			Set(ByVal value As Boolean)
				Me.myzoomEnabled_Conflict = value
			End Set
		End Property
	End Class

	Public Class MyKeyboardHandlerService
		Inherits KeyboardHandlerServiceWrapper

		Private provider As IServiceProvider

		Public Sub New(ByVal provider As IServiceProvider, ByVal service As IKeyboardHandlerService)
			MyBase.New(service)
			Me.provider = provider
		End Sub
		Public Overrides Sub OnKeyDown(ByVal e As PortableKeyEventArgs)
			If e.Control Then
				Dim mouseHandler As MyMouseHandlerService = DirectCast(provider.GetService(GetType(IMouseHandlerService)), MyMouseHandlerService)
				If mouseHandler IsNot Nothing Then
					mouseHandler.myZoomEnabled = Not mouseHandler.myZoomEnabled
				End If
			End If
			MyBase.OnKeyDown(e)
		End Sub
	End Class

End Namespace
