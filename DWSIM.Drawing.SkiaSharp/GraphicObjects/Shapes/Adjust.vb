﻿Imports DWSIM.Drawing.SkiaSharp.GraphicObjects
Imports DWSIM.Interfaces.Enums.GraphicObjects

Namespace GraphicObjects.Shapes

    Public Class AdjustGraphic

        Inherits ShapeGraphic

        Protected m_mvPT, m_cvPT, m_rvPT As GraphicObject

        Public Property ConnectedToMv() As GraphicObject
            Get
                Return m_mvPT
            End Get
            Set(ByVal value As GraphicObject)
                m_mvPT = value
            End Set
        End Property

        Public Property ConnectedToRv() As GraphicObject
            Get
                Return m_rvPT
            End Get
            Set(ByVal value As GraphicObject)
                m_rvPT = value
            End Set
        End Property

        Public Property ConnectedToCv() As GraphicObject
            Get
                Return m_cvPT
            End Get
            Set(ByVal value As GraphicObject)
                m_cvPT = value
            End Set
        End Property

#Region "Constructors"

        Public Sub New()
            Me.ObjectType = DWSIM.Interfaces.Enums.GraphicObjects.ObjectType.OT_Adjust
            Me.Description = "Adjust Logical Op"
        End Sub

        Public Sub New(ByVal graphicPosition As SKPoint)
            Me.New()
            Me.SetPosition(graphicPosition)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer)
            Me.New(New SKPoint(posX, posY))
        End Sub

        Public Sub New(ByVal graphicPosition As SKPoint, ByVal graphicSize As SKSize)
            Me.New(graphicPosition)
            Me.SetSize(graphicSize)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal graphicSize As SKSize)
            Me.New(New SKPoint(posX, posY), graphicSize)
        End Sub

        Public Sub New(ByVal posX As Integer, ByVal posY As Integer, ByVal width As Integer, ByVal height As Integer)
            Me.New(New SKPoint(posX, posY), New SKSize(width, height))
        End Sub

#End Region

        Public Overrides Sub CreateConnectors(InCount As Integer, OutCount As Integer)

            Me.EnergyConnector.Active = False

        End Sub

        Public Overrides Sub Draw(ByVal g As Object)

            Dim canvas As SKCanvas = DirectCast(g, SKCanvas)

            CreateConnectors(0, 0)

            UpdateStatus()

            MyBase.Draw(g)

            Dim aPen As New SKPaint()
            With aPen
                .Color = SKColors.Red
                .StrokeWidth = LineWidth
                .IsStroke = True
                .IsAntialias = GlobalSettings.Settings.DrawingAntiAlias
                .PathEffect = SKPathEffect.CreateDash(New Single() {10.0F, 5.0F, 2.0F, 5.0F}, 2.0F)
            End With

            If Not Me.ConnectedToMv Is Nothing Then
                canvas.DrawPoints(SKPointMode.Polygon, New SKPoint() {New SKPoint(Me.X + Me.Width / 2, Me.Y + Me.Height / 2), New SKPoint(Me.m_mvPT.X, Me.Y + Me.Height / 2), Me.m_mvPT.GetPosition}, aPen)
            End If
            If Not Me.ConnectedToCv Is Nothing Then
                canvas.DrawPoints(SKPointMode.Polygon, New SKPoint() {New SKPoint(Me.X + Me.Width / 2, Me.Y + Me.Height / 2), New SKPoint(Me.m_cvPT.X, Me.Y + Me.Height / 2), Me.m_cvPT.GetPosition}, aPen)
            End If
            If Not Me.ConnectedToRv Is Nothing Then
                canvas.DrawPoints(SKPointMode.Polygon, New SKPoint() {New SKPoint(Me.X + Me.Width / 2, Me.Y + Me.Height / 2), New SKPoint(Me.m_rvPT.X, Me.Y + Me.Height / 2), Me.m_rvPT.GetPosition}, aPen)
            End If

            Dim myPen As New SKPaint()
            With myPen
                .Color = SKColors.LightSalmon
                .StrokeWidth = LineWidth
                .IsStroke = False
                .IsAntialias = GlobalSettings.Settings.DrawingAntiAlias
            End With

            canvas.DrawOval(New SKRect(X, Y, X + Width, Y + Height), myPen)

            Dim myPen2 As New SKPaint()
            With myPen2
                .Color = SKColors.Red
                .StrokeWidth = LineWidth
                .IsStroke = True
                .IsAntialias = GlobalSettings.Settings.DrawingAntiAlias
            End With

            canvas.DrawOval(New SKRect(X, Y, X + Width, Y + Height), myPen2)

            Dim tpaint As New SKPaint()

            With tpaint
                .TextSize = 18.0#
                .IsAntialias = GlobalSettings.Settings.DrawingAntiAlias
                .Color = SKColors.Red
                .IsStroke = False
                .Typeface = DefaultTypeFace
            End With

            Dim trect As New SKRect(0, 0, 2, 2)
            tpaint.GetTextPath("A", 0, 0).GetBounds(trect)

            Dim ax, ay As Integer
            ax = Me.X + (Me.Width - (trect.Right - trect.Left)) / 2
            ay = Me.Y + (Me.Height - (trect.Top - trect.Bottom)) / 2

            canvas.DrawText("A", ax, ay, tpaint)


        End Sub

    End Class

End Namespace