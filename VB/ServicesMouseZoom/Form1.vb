Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing
Imports DevExpress.Utils
Imports System.IO
Imports DevExpress.Utils.Controls
Imports DevExpress.Services
Imports DevExpress.XtraEditors

Namespace ServicesMouseZoom
	Partial Public Class Form1
		Inherits XtraForm

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

			Dim oldMouseHandler As IMouseHandlerService = CType(schedulerControl1.GetService(GetType(IMouseHandlerService)), IMouseHandlerService)
			If oldMouseHandler IsNot Nothing Then
				Dim newMouseHandler As New MyMouseHandlerService(schedulerControl1, oldMouseHandler)
				schedulerControl1.RemoveService(GetType(IMouseHandlerService))
				schedulerControl1.AddService(GetType(IMouseHandlerService), newMouseHandler)
			End If

			Dim oldKeyboardHandler As IKeyboardHandlerService = CType(schedulerControl1.GetService(GetType(IKeyboardHandlerService)), IKeyboardHandlerService)
			If oldKeyboardHandler IsNot Nothing Then
				Dim newKeyboardHandler As New MyKeyboardHandlerService(schedulerControl1, oldKeyboardHandler)
				schedulerControl1.RemoveService(GetType(IKeyboardHandlerService))
				schedulerControl1.AddService(GetType(IKeyboardHandlerService), newKeyboardHandler)
			End If

			CustomizeScheduler()
		End Sub

		Private Sub schedulerControl1_InitAppointmentImages(ByVal sender As Object, ByVal e As AppointmentImagesEventArgs) Handles schedulerControl1.InitAppointmentImages
			Dim info As New AppointmentImageInfo()
			info.Image = Me.sportsImages.Images(e.Appointment.LabelId)
			e.ImageInfoList.Add(info)

		End Sub

		#Region "Initial Settings"

		Public Const sportEventsResourceName As String = "ServicesMouseZoom.Data.sportevents.xml"
		Private sportsImages As ImageCollection
		Private channelsImages As ImageCollection

		Public Sub CustomizeScheduler()
			schedulerControl1.ActiveViewType = SchedulerViewType.Day
			schedulerControl1.WeekView.Enabled = False
			schedulerControl1.WorkWeekView.Enabled = False
			schedulerControl1.MonthView.Enabled = False
			schedulerControl1.TimelineView.Enabled = False
			schedulerControl1.GroupType = SchedulerGroupType.Resource
			schedulerControl1.DayView.ResourcesPerPage = 3
			schedulerControl1.ActiveViewType = SchedulerViewType.Day
			schedulerControl1.Views.DayView.DayCount = 1
			schedulerControl1.Views.MonthView.ResourcesPerPage = 2
			schedulerControl1.Views.MonthView.WeekCount = 2
			schedulerControl1.Views.TimelineView.ResourcesPerPage = 2
			schedulerControl1.Views.WeekView.ResourcesPerPage = 2
			schedulerControl1.Views.WorkWeekView.ResourcesPerPage = 2
			AddSportChannels()
			FillData()
			schedulerControl1.Start = New DateTime(2008, 03, 12)
			schedulerControl1.DayView.ShowWorkTimeOnly = True
		End Sub


		Private Sub FillData()
			Me.schedulerStorage1.EnableReminders = False
			Me.schedulerStorage1.Appointments.Mappings.End = "Finish"
			Me.schedulerStorage1.Appointments.Mappings.Label = "SportID"
			Me.schedulerStorage1.Appointments.Mappings.ResourceId = "ResourceID"
			Me.schedulerStorage1.Appointments.Mappings.Start = "Start"
			Me.schedulerStorage1.Appointments.Mappings.Subject = "Caption"
			Me.schedulerStorage1.Appointments.Mappings.AllDay = "AllDay"

			schedulerStorage1.Appointments.DataSource = GetSportEventsData()
		End Sub

		Private Shared Function GetResourceStream(ByVal resourceName As String) As Stream
			Return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
		End Function

		Public Shared Function GetSportEventsData() As DataTable
			Dim sportEventDS As New DataSet()
			Using stream As Stream = GetResourceStream(sportEventsResourceName)
				sportEventDS.ReadXml(stream)
				stream.Close()
			End Using
			Return sportEventDS.Tables(0)
		End Function

		Private Sub AddSportChannels()
			Me.sportsImages = ImageHelper.CreateImageCollectionFromResources("ServicesMouseZoom.Images.sports.png", System.Reflection.Assembly.GetExecutingAssembly(), New Size(16, 16))
			Me.channelsImages = ImageHelper.CreateImageCollectionFromResources("ServicesMouseZoom.Images.channels.png", System.Reflection.Assembly.GetExecutingAssembly(), New Size(60, 40))

			schedulerStorage1.Resources.BeginUpdate()
			AddResource(0, "Channel 1")
			AddResource(1, "Channel 2")
			AddResource(2, "Channel 3")
			AddResource(3, "Channel 4")
			AddResource(4, "Channel 5")
			AddResource(5, "Channel 6")
			AddResource(6, "Channel 7")
			AddResource(7, "Channel 8")
			schedulerStorage1.Resources.EndUpdate()
		End Sub
		Private Sub AddResource(ByVal index As Integer, ByVal caption As String)
			Dim r As New Resource(index.ToString(), caption)
			r.Image = Me.channelsImages.Images(index)
			r.Color = schedulerControl1.ResourceColorSchemas.GetSchema(index).CellLight
			schedulerStorage1.Resources.Add(r)
		End Sub
		#End Region
	End Class

End Namespace
