﻿'Open VOGEL (www.openvogel.com)
'Open source software for aerodynamics
'Copyright (C) 2016 Guillermo Hazebrouck (guillermo.hazebrouck@openvogel.com)

'This program Is free software: you can redistribute it And/Or modify
'it under the terms Of the GNU General Public License As published by
'the Free Software Foundation, either version 3 Of the License, Or
'(at your option) any later version.

'This program Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty Of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with this program.  If Not, see < http:  //www.gnu.org/licenses/>.

Imports SharpGL
Imports AeroTools.VisualModel.Interface
Imports AeroTools.CalculationModel.Models.Aero.Components
Imports MathTools.Algebra.EuclideanSpace
Imports MathTools.Algebra.CustomMatrices
Imports System.Xml
Imports AeroTools.VisualModel.Models.Basics
Imports AeroTools.VisualModel.IO
Imports AeroTools.CalculationModel.Models.Structural.Library
Imports AeroTools.CalculationModel.Models.Structural.Library.Elements

Namespace VisualModel.Models.Components

    Public Class WingRegion

#Region " Geometry "

        Public Enum ESpacement As Integer
            Constant = 1
            Cuadratic = 2
            Qubic = 3
            Sinus = 4
        End Enum

        Private _SpanPanels As Integer
        Private _SpanNodes As Integer

        Public Sub New()

            FlapChord = 0.3
            PolarID = Guid.Empty
            Chamber = New ChamberedLine(ChamberType.NACA4)

        End Sub

        ''' <summary>
        ''' Load all geometrical parameters.
        ''' </summary>
        ''' <param name="SpanPanels">Number of spanwise panels</param>
        ''' <param name="TipChord">Tip chord</param>
        ''' <param name="Lenght">Spanwise length</param>
        ''' <param name="SweepBack">Sweep angle</param>
        ''' <param name="Dihedral">Dihedral angle</param>
        ''' <param name="Torsion">Twist</param>
        ''' <param name="TorsionalAxis">Position of the twisting axis</param>
        ''' <remarks></remarks>
        Public Sub LoadGeometry(ByVal SpanPanels As Integer, ByVal TipChord As Double, ByVal Lenght As Double,
                                ByVal SweepBack As Double, ByVal Dihedral As Double, ByVal Torsion As Double, ByVal TorsionalAxis As Double,
                                ByVal MaxChamber As Double, ByVal PosMaxChamber As Double, ByVal SpacingType As Integer)

            If SpanPanels > 0 Then _SpanPanels = SpanPanels
            If TipChord > 0 Then _TipChord = TipChord
            _Sweep = SweepBack
            If Lenght > 0 Then _Length = Lenght
            _Dihedral = Dihedral
            _Twist = Torsion
            _TwistAxis = TorsionalAxis
            _SpacingType = SpacingType

            _Chamber.Dimension(ChamberDim.MaxChamber) = MaxChamber
            _Chamber.Dimension(ChamberDim.PosMaxChamber) = PosMaxChamber

            If SpacingType = ESpacement.Cuadratic Then
                _SpanPanels = Math.Max(_SpanPanels, 2)
            End If

        End Sub

        ''' <summary>
        ''' Gets all the constent from an existing wing region.
        ''' </summary>
        ''' <param name="PanelExt"></param>
        ''' <remarks></remarks>
        Public Sub Assign(ByVal PanelExt As WingRegion)

            _SpanPanels = PanelExt.nSpanPanels
            _TipChord = PanelExt.TipChord
            _Sweep = PanelExt.Sweep
            _Length = PanelExt.Length
            _Dihedral = PanelExt.Dihedral
            _Twist = PanelExt.Twist
            _TwistAxis = PanelExt.TwistAxis
            _FlapChord = PanelExt._FlapChord

            _Chamber.Assign(PanelExt._Chamber)

            _CenterOfShear = PanelExt.CenterOfShear

        End Sub

        ''' <summary>
        ''' Number of panels in spanwise direction.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property nSpanPanels As Integer
            Get
                Return _SpanPanels
            End Get
            Set(ByVal value As Integer)
                _SpanPanels = value
            End Set
        End Property

        ''' <summary>
        ''' Number of nodes in chordwise direction.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property nChordNodes As Integer
            Get
                Return _SpanPanels + 1
            End Get
            Set(ByVal value As Integer)
                _SpanPanels = value - 1
            End Set
        End Property

        ''' <summary>
        ''' Tip chord length.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TipChord As Double

        ''' <summary>
        ''' Spanwise length.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Length As Double

        ''' <summary>
        ''' Sweep angle.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Sweep As Double

        ''' <summary>
        ''' Dihedral angle.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Dihedral As Double

        ''' <summary>
        ''' Twist angle along the span.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Twist As Double

        ''' <summary>
        ''' Chordwise position of the twisting axis as fraction of the local chord.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TwistAxis As Double

        ''' <summary>
        ''' Describes the form of the cord.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Chamber As ChamberedLine

        ''' <summary>
        ''' Lenght of the flap in chordwise direction.
        ''' </summary>
        ''' <returns></returns>
        Public Property FlapChord As Double

        ''' <summary>
        ''' Type of spanwise spacing between the stations.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpacingType As ESpacement

        ''' <summary>
        ''' Index of polar curve to be loaded.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PolarID As Guid

#End Region

#Region " Structure "

        ''' <summary>
        ''' Represents the section at the tip of the panel
        ''' </summary>
        ''' <remarks></remarks>
        Public Property TipSection As Section = New Section

        Private _CenterOfShear As Double = 0.25

        ''' <summary>
        ''' Stablishes the position of the center of shear in relation to the local chord (flap not included)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CenterOfShear
            Set(ByVal value)
                _CenterOfShear = value
            End Set
            Get
                Return _CenterOfShear
            End Get
        End Property

#End Region

#Region " Polars "

        ''' <summary>
        ''' Local polar curve.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PolarFamiliy As PolarFamily

#End Region

#Region " IO "

        ''' <summary>
        ''' Writes the content to an XML file.
        ''' </summary>
        ''' <param name="writer"></param>
        ''' <remarks></remarks>
        Public Sub WriteToXML(ByRef writer As XmlWriter)

            writer.WriteAttributeString("SpanVortexRings", String.Format("{0}", nSpanPanels))
            writer.WriteAttributeString("ExternalChord", String.Format("{0}", TipChord))
            writer.WriteAttributeString("Length", String.Format("{0}", Length))
            writer.WriteAttributeString("SweepBack", String.Format("{0}", Sweep))
            writer.WriteAttributeString("Diedral", String.Format("{0}", Dihedral))
            writer.WriteAttributeString("Torsion", String.Format("{0}", Twist))
            writer.WriteAttributeString("TorsionAxis", String.Format("{0}", TwistAxis))
            writer.WriteAttributeString("MaxChamber", String.Format("{0}", Chamber.Dimension(ChamberDim.MaxChamber)))
            writer.WriteAttributeString("MaxChamberPosition", String.Format("{0}", Chamber.Dimension(ChamberDim.PosMaxChamber)))
            writer.WriteAttributeString("_Flapped", String.Format("{0}", Chamber.Flapped))
            writer.WriteAttributeString("_TipFlap", String.Format("{0}", Chamber.FlapChord))
            writer.WriteAttributeString("_FlapDeflection", String.Format("{0}", Chamber.FlapDeflection))

            writer.WriteAttributeString("SpacingType", String.Format("{0}", SpacingType))

            writer.WriteAttributeString("Area", String.Format("{0}", TipSection.AE))
            writer.WriteAttributeString("Iu", String.Format("{0}", TipSection.GJ))
            writer.WriteAttributeString("Iv", String.Format("{0}", TipSection.EIy))
            writer.WriteAttributeString("Iw", String.Format("{0}", TipSection.EIz))
            writer.WriteAttributeString("Sv", String.Format("{0}", TipSection.CMy))
            writer.WriteAttributeString("Sw", String.Format("{0}", TipSection.CMz))
            writer.WriteAttributeString("J", String.Format("{0}", TipSection.rIp))
            writer.WriteAttributeString("m", String.Format("{0}", TipSection.m))

            writer.WriteAttributeString("SChord", String.Format("{0}", CenterOfShear))
            writer.WriteAttributeString("PolarID", String.Format("{0}", _PolarID))

        End Sub

        ''' <summary>
        ''' Reads the content from an XML file.
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <remarks></remarks>
        Public Sub ReadFromXML(ByRef reader As XmlReader)

            nSpanPanels = IOXML.ReadInteger(reader, "SpanVortexRings", 4)
            TipChord = IOXML.ReadDouble(reader, "ExternalChord", 1.0)
            Length = IOXML.ReadDouble(reader, "Length", 1.0)
            Sweep = IOXML.ReadDouble(reader, "SweepBack", 0.0)
            Dihedral = IOXML.ReadDouble(reader, "Diedral", 0.0)
            Twist = IOXML.ReadDouble(reader, "Torsion", 0.0)
            TwistAxis = IOXML.ReadDouble(reader, "TorsionAxis", 0.0)
            Chamber.Dimension(ChamberDim.MaxChamber) = IOXML.ReadDouble(reader, "MaxChamber", 0.0)
            Chamber.Dimension(ChamberDim.PosMaxChamber) = IOXML.ReadDouble(reader, "MaxChamberPosition", 0.0)
            SpacingType = IOXML.ReadInteger(reader, "SpacingType", 1)

            Me.Chamber.Flapped = IOXML.ReadBoolean(reader, "_Flapped", False)
            Me.Chamber.FlapChord = IOXML.ReadDouble(reader, "_TipFlap", 0.2)
            Me.Chamber.FlapDeflection = IOXML.ReadDouble(reader, "_FlapDeflection", 0.0#)

            TipSection.AE = IOXML.ReadDouble(reader, "Area", 1000)
            TipSection.GJ = IOXML.ReadDouble(reader, "Iu", 1000)
            TipSection.EIy = IOXML.ReadDouble(reader, "Iv", 1000)
            TipSection.EIz = IOXML.ReadDouble(reader, "Iw", 1000)
            TipSection.rIp = IOXML.ReadDouble(reader, "J", 1000)
            TipSection.CMy = IOXML.ReadDouble(reader, "Sv", 1000)
            TipSection.CMz = IOXML.ReadDouble(reader, "Sw", 1000)
            TipSection.m = IOXML.ReadDouble(reader, "m", 10)

            CenterOfShear = IOXML.ReadDouble(reader, "SChord", 0.0)
            _PolarID = New Guid(IOXML.ReadString(reader, "PolarID", Guid.Empty.ToString))

        End Sub

#End Region

    End Class

    Public Class LiftingSurface

        Inherits Surface

        ''' <summary>
        ''' Permite bloquear la edicion del contenido
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Lock As Boolean = False

        ''' <summary>
        ''' Position of the local origin in global coordinates.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property LocalOrigin As New EVector3

        Private _DirectionPoints As New EBase3

        ''' <summary>
        ''' Indicates if the mesh has to be symmetric about the plane y = 0.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Symmetric As Boolean = False

        ''' <summary>
        ''' Indicates if the wake has to be convected.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConvectWake As Boolean = True

        ''' <summary>
        ''' Indicates if the wake should be convected by the trailing edge only.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TrailingEdgeConvection As Boolean
            Set(ByVal value As Boolean)
                _TrailingEdgeConvection = value
                SetPrimitives()
            End Set
            Get
                Return _TrailingEdgeConvection
            End Get
        End Property

        ''' <summary>
        ''' Indicates at which time-step the wake has to be trimmed.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CuttingStep As Integer = 50

        Private _nChordPanels As Integer = 5
        Private _nSpanPanels As Integer
        Private _nChordNodes As Integer
        Private _nSpanNodes As Integer

        Private _nBoundaryNodes As Integer
        Private _nBoundarySegments As Integer

        Private _nPrimitivePanels As Integer
        Private _nPrimitiveNodes As Integer
        Private _TrailingEdgeConvection As Boolean = True

        ''' <summary>
        ''' Contains the wing regions.
        ''' </summary>
        ''' <returns></returns>
        Public Property WingRegions As New List(Of WingRegion)

        Private Const _nMaximumWingRegions As Integer = 10
        Private _CurrentWingRegion As Integer = 0

        Private _RootChord As Double
        Private _RootFlap As Double
        Private _FlapPanels As Integer

        Private _BoundaryNodes(1) ' Matriz de nodos del contorno
        Private _BoundaryPanels(1) ' Matriz de paneles del contorno
        Private _PrimitiveData(2, 2) As Integer ' Comienzo y fin del borde de conveccion (paneles y nodos)

        Private _PrimitivePanel1 As Integer
        Private _PrimitivePanel2 As Integer

        Public Sub New()

            Mesh = New Mesh()

            Name = "Lifting surface"
            ID = Guid.NewGuid

            NumberOfChordPanels = 5
            RootChord = 1.0#
            RootFlap = 0.2
            FlapPanels = 3
            Symmetric = True
            IncludeInCalculation = True
            InitializeRegions()

            FirstPrimitiveSegment = 1
            LastPrimitiveSegment = 1

            FirstPrimitiveSegment = NumberOfChordPanels + 1
            LastPrimitiveSegment = FirstPrimitiveSegment + WingRegions(0).nSpanPanels - 1

            GenerateMesh()

            VisualProperties = New VisualProperties(ComponentTypes.etLiftingSurface)

        End Sub

#Region " Geometry and mesh properties "

        ''' <summary>
        ''' Number of chordwise panels
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NumberOfChordPanels As Integer
            Get
                Return _nChordPanels
            End Get
            Set(ByVal value As Integer)
                _nChordPanels = value
            End Set
        End Property

        ''' <summary>
        ''' Number of spanwise panels.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NumberOfSpanPanels As Integer
            Get
                Return _nSpanPanels
            End Get
        End Property

        ''' <summary>
        ''' Length of the root chord.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RootChord As Double
            Get
                Return _RootChord
            End Get
            Set(ByVal value As Double)
                _RootChord = value
            End Set
        End Property

        ''' <summary>
        ''' Length of the root flap (as fraction of the root chord).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RootFlap As Double
            Set(ByVal value As Double)
                _RootFlap = value
            End Set
            Get
                Return _RootFlap
            End Get
        End Property

        ''' <summary>
        ''' Number of chordwise flap panels.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FlapPanels As Integer
            Set(ByVal value As Integer)
                If value < NumberOfChordPanels Then
                    _FlapPanels = value
                Else
                    _FlapPanels = NumberOfChordPanels - 1
                End If
            End Set
            Get
                Return _FlapPanels
            End Get
        End Property

        ''' <summary>
        ''' Indicates which regions is currently selected or selects an existing region.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CurrentRegionID As Integer
            Get
                Return _CurrentWingRegion + 1
            End Get
            Set(ByVal value As Integer)
                If value >= 1 And value <= WingRegions.Count Then
                    _CurrentWingRegion = value - 1
                Else
                    'FMacroPanelActual = 0
                End If
            End Set
        End Property

#End Region

#Region " Convective border and primitive segements "

        ''' <summary>
        ''' Number of segments around the perimeter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property nBoundarySegments
            Get
                Return _nBoundarySegments
            End Get
        End Property

        ''' <summary>
        ''' Number of boundary nodes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property nBoundaryNodes
            Get
                Return _nBoundaryNodes
            End Get
        End Property

        ''' <summary>
        ''' Number of primitive segments.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property nPrimitiveSegments
            Get
                Me.SetPrimitives()
                Return Me._nPrimitivePanels
            End Get
        End Property

        ''' <summary>
        ''' Number of primitive nodes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property nPrimitiveNodes
            Get
                Me.SetPrimitives()
                Return Me._nPrimitiveNodes
            End Get
        End Property

        ''' <summary>
        ''' Position of the segment defined as firt primitive.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FirstPrimitiveSegment As Integer
            Get
                Return _PrimitiveData(1, 1)
            End Get
            Set(ByVal value As Integer)
                Me._PrimitiveData(1, 1) = value
                Me.SetPrimitives()
            End Set
        End Property

        ''' <summary>
        ''' Position of the segment defined as last primitive.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastPrimitiveSegment As Integer
            Get
                Return _PrimitiveData(1, 2)
            End Get
            Set(ByVal value As Integer)
                Me._PrimitiveData(1, 2) = value
                Me.SetPrimitives()
            End Set
        End Property

        ''' <summary>
        ''' Position of the node defined as first primitive (depends on the first primitive segment).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FirstPrimitiveNode As Integer
            Get
                Return Me._PrimitiveData(2, 1)
            End Get
        End Property

        ''' <summary>
        ''' Position of the node defined as last primitive (depends on the last primitive segment).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property LastPrimitiveNode As Integer
            Get
                Return Me._PrimitiveData(2, 2)
            End Get
        End Property

        ''' <summary>
        ''' Returns the index of a given primitive node.
        ''' </summary>
        ''' <param name="Node"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPrimitiveNodeIndex(ByVal Node As Integer) As Integer
            Get
                If Node >= 1 And Node <= Me._nBoundaryNodes Then
                    Return Me._BoundaryNodes(Node)
                Else
                    Return 1
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns the location of a given primitive node.
        ''' </summary>
        ''' <param name="Nodo"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPrimitiveNodePosition(ByVal Nodo As Integer) As EVector3
            Get
                If Nodo >= 1 And Nodo <= Me._nBoundaryNodes Then
                    Return Mesh.Nodes(_BoundaryNodes(Nodo)).Position
                Else
                    Return New EVector3
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns the index of a given primitive segment.
        ''' </summary>
        ''' <param name="Segment"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPrimitivePanelIndex(ByVal Segment As Integer) As Integer
            Get
                If Segment >= 1 And Segment <= Me._nBoundarySegments Then
                    Return _BoundaryPanels(Segment)
                Else
                    Return 1
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns the panel associated to a given primitive segment.
        ''' </summary>
        ''' <param name="Segmento"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPrimitivePanel(ByVal Segmento As Integer) As Panel
            Get
                If Segmento >= 1 And Segmento <= Me._nBoundarySegments Then
                    Return Mesh.Panels(Me._BoundaryPanels(Segmento) - 1)
                Else
                    Return Mesh.Panels(0)
                End If
            End Get
        End Property

#End Region

#Region " Wing regions management "

        ''' <summary>
        ''' Loads a simple wing region.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub InitializeRegions()

            Dim NewPanel As New WingRegion
            NewPanel.LoadGeometry(5, 1.0#, 1.0#, 0.0#, 0.0#, 0.0#, 0.5#, 0.0#, 0.25#, 1)
            WingRegions.Add(NewPanel)

        End Sub

        ''' <summary>
        ''' Adds a new region at the end.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub AddRegion()

            Dim NewMacroPanel As New WingRegion
            NewMacroPanel.LoadGeometry(4, 1.0#, 1.0#, 0.0#, 0.0#, 0.0#, 0.5#, 0.0#, 0.25#, 1)
            WingRegions.Add(NewMacroPanel)

        End Sub

        ''' <summary>
        ''' Insets a new wing region at a given position.
        ''' </summary>
        ''' <param name="Posicion"></param>
        ''' <remarks></remarks>
        Public Overloads Sub InsertRegion(ByVal Posicion As Integer)

            If Posicion >= 1 And Posicion <= WingRegions.Count Then

                Dim NewMacroPanel As New WingRegion
                NewMacroPanel.LoadGeometry(4, 1.0#, 1.0#, 0.0#, 0.0#, 0.0#, 0.5#, 0.0#, 0.25#, 1)
                WingRegions.Insert(Posicion - 1, NewMacroPanel)

            End If

        End Sub

        ''' <summary>
        ''' Inserts a new wing region after the current one.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overloads Sub InsertRegion()

            Dim NewMacroPanel As New WingRegion
            NewMacroPanel.LoadGeometry(4, 1.0#, 1.0#, 0.0#, 0.0#, 0.0#, 0.5#, 0.0#, 0.25#, 1)
            WingRegions.Insert(_CurrentWingRegion, NewMacroPanel)

        End Sub

        ''' <summary>
        ''' Removes the current wing region.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RemoveCurrentRegion()

            If WingRegions.Count > 1 Then

                WingRegions.RemoveAt(_CurrentWingRegion)
                _CurrentWingRegion = Math.Max(0, _CurrentWingRegion - 1)

            End If

        End Sub

        ''' <summary>
        ''' Currently active region
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentRegion As WingRegion
            Get
                Return WingRegions.Item(_CurrentWingRegion)
            End Get
        End Property

#End Region

#Region " 3D model and vortices generation "

        Public Function GetVortex(ByVal VortexNumber As Integer) As LatticeSegment
            Return Mesh.Lattice.Item(VortexNumber)
        End Function

        Public Overrides Sub GenerateMesh()

            Dim X As Double
            Dim Chord As Double
            Dim Chordi As Double
            Dim Chordf As Double
            Dim Stramo As Double
            Dim Y_stripe As Double
            Dim Scolumn1 As Double
            Dim Scolumn2 As Double
            Dim Y As Double
            Dim Sigma1 As Double
            Dim Sigma2 As Double
            Dim B1 As Double
            Dim C1 As Double
            Dim B2 As Double
            Dim C2 As Double
            Dim D2 As Double
            Dim E As Double
            Dim Ttwist As Double
            Dim MrotA(3, 3) As Double
            Dim Pij As New EVector3
            Dim Phi As Double
            Dim XBp As Double
            Dim YBp As Double
            Dim ZBp As Double

            ' Determine the number of elements:

            _nChordNodes = _nChordPanels + 1
            _nSpanNodes = 1
            _nSpanPanels = 0

            For i = 0 To WingRegions.Count - 1

                _nSpanNodes = WingRegions.Item(i).nChordNodes + _nSpanNodes
                _nSpanPanels = WingRegions.Item(i).nSpanPanels + _nSpanPanels

            Next

            _nBoundaryNodes = 2 * _nChordNodes + 2 * _nSpanNodes ' Numero de nodos en el contorno
            _nBoundarySegments = 2 * _nChordPanels + 2 * _nSpanPanels ' Numero de paneles en el contorno

            SetPrimitives()

            ' Clear the mesh:

            Mesh.Nodes.Clear()
            Mesh.Panels.Clear()

            ' Load quad panels (base on indices only):

            For p = 1 To _nSpanPanels

                For q = 0 To _nChordPanels - 1

                    Dim Panel As New Panel

                    Panel.N1 = (p - 1) * _nChordNodes + q
                    Panel.N2 = (p - 1) * _nChordNodes + q + 1
                    Panel.N3 = p * _nChordNodes + q + 1
                    Panel.N4 = p * _nChordNodes + q

                    Mesh.Panels.Add(Panel)

                Next

            Next

            ' Build the boundary index matrix

            ReDim _BoundaryNodes(_nBoundaryNodes)
            ReDim _BoundaryPanels(_nBoundarySegments)

            Dim s As Integer

            s = 1
            For q = 1 To _nChordPanels + 1
                _BoundaryNodes(s) = q
                s = s + 1
            Next
            For p = 1 To _nSpanPanels - 1
                _BoundaryNodes(s) = (p + 1) * (_nChordPanels + 1)
                s = s + 1
            Next
            For q = 1 To _nChordPanels + 1
                _BoundaryNodes(s) = (_nChordPanels + 1) * (_nSpanPanels + 1) - (q - 1)
                s = s + 1
            Next
            For p = 1 To _nSpanPanels - 1
                _BoundaryNodes(s) = (_nSpanPanels + 1 - p) * (_nChordPanels + 1) - _nChordPanels
                s = s + 1
            Next

            _BoundaryNodes(_nBoundaryNodes) = 1

            s = 1
            For p = 1 To _nChordPanels
                _BoundaryPanels(s) = p
                s = s + 1
            Next
            For p = 1 To _nSpanPanels
                _BoundaryPanels(s) = _nChordPanels * p
                s = s + 1
            Next
            For p = 1 To _nChordPanels
                _BoundaryPanels(s) = _nChordPanels * _nSpanPanels - (p - 1)
                s = s + 1
            Next
            For p = 1 To _nSpanPanels
                _BoundaryPanels(s) = _nChordPanels * _nSpanPanels - p * _nChordPanels + 1
                s = s + 1
            Next

            ' Build rotation matrix:

            Dim rotationMatrix As New RotationMatrix

            rotationMatrix.Generate(Orientation.ToRadians)

            ' Locate root chord nodes:

            Dim nodeCounter As Integer = 1

            Dim flap As Double = RootFlap

            For i = 1 To _nChordNodes

                If WingRegions(0).Chamber.Flapped Then

                    If i <= _nChordNodes - FlapPanels Then

                        X = (i - 1) / (_nChordNodes - FlapPanels - 1) * (1 - flap)

                    Else

                        X = (1 - flap) + (i - _nChordNodes + FlapPanels) / FlapPanels * flap

                    End If

                Else

                    X = (i - 1) / _nChordPanels

                End If

                Dim pLoc As New EVector2

                Dim deflection As Single = WingRegions(0).Chamber.FlapDeflection

                WingRegions(0).Chamber.FlapDeflection = 0
                WingRegions(0).Chamber.EvaluatePoint(pLoc, X)

                pLoc.X *= _RootChord
                pLoc.Y *= _RootChord

                WingRegions(0).Chamber.FlapDeflection = deflection

                Dim Point As New NodalPoint(pLoc.X, 0, pLoc.Y)

                If Symmetric Then Point.Position.Y = -Point.Position.Y
                Point.Position.Substract(CenterOfRotation)
                Point.Position.Rotate(rotationMatrix)
                Point.Position.Add(CenterOfRotation)
                Point.Position.Add(Position)

                Mesh.Nodes.Add(Point)

                nodeCounter += 1

            Next

            ' Comienza a asignar la geometria a cada macro panel:

            Dim leadingEdge As New EVector3

            For mpIndex = 0 To WingRegions.Count - 1

                'Inicia las variables comunes del panel:

                Dim delta As Double = WingRegions(mpIndex).Sweep / 180.0 * Math.PI
                Dim gamma As Double = WingRegions(mpIndex).Dihedral / 180.0 * Math.PI
                Dim twist As Double = 0.0#

                'Inicia el origen de coordenadas local:

                If mpIndex = 0 Then

                    leadingEdge.X = 0.0#
                    leadingEdge.Y = 0.0#
                    leadingEdge.Z = 0.0#
                    twist = 0.0#

                Else

                    leadingEdge.X = XBp
                    leadingEdge.Y = YBp
                    leadingEdge.Z = ZBp
                    twist = (twist + WingRegions(mpIndex - 1).Twist / 180.0 * Math.PI) * Math.Cos(gamma)  ' Prueba!!!

                End If

                If mpIndex = 0 Then
                    Chordi = _RootChord
                    Chordf = WingRegions(mpIndex).TipChord
                Else
                    Chordi = WingRegions(mpIndex - 1).TipChord
                    Chordf = WingRegions(mpIndex).TipChord
                End If

                Stramo = WingRegions(mpIndex).Length
                Sigma1 = 1 - 1 / WingRegions.Item(mpIndex).nSpanPanels
                Scolumn1 = Stramo - Chordf / _nChordPanels
                Sigma2 = 1 / WingRegions.Item(mpIndex).nSpanPanels
                Scolumn2 = Chordi / _nChordPanels

                C1 = (Scolumn1 - Stramo * Sigma1) / (Sigma1 ^ 2 - Sigma1)
                B1 = Stramo - C1

                E = ((Scolumn1 - Stramo * Sigma1) - (Sigma1 ^ 3 - Sigma1)) / (Sigma1 ^ 2 - Sigma1)
                D2 = (Scolumn2 - Stramo * Sigma2) / (Sigma2 ^ 3 + E * Sigma2 ^ 2 - Sigma2 * (E + 1))
                C2 = E * D2
                B2 = Stramo - C2 - D2

                ' Genera la geometria de cada segemento de cuerda:

                For k = 1 To WingRegions(mpIndex).nSpanPanels

                    ' a) Calculates the distance to the column in spanwise direction

                    Y = k / WingRegions.Item(mpIndex).nSpanPanels

                    Select Case WingRegions.Item(mpIndex).SpacingType

                        Case WingRegion.ESpacement.Constant
                            Y_stripe = Stramo * Y
                        Case WingRegion.ESpacement.Cuadratic
                            Y_stripe = B1 * Y + C1 * Y ^ 2
                        Case WingRegion.ESpacement.Qubic
                            Y_stripe = B2 * Y + C2 * Y ^ 2 + D2 * Y ^ 3

                    End Select

                    'Calculates the local chord and the position of Scolumn

                    Chord = Chordi + (Chordf - Chordi) * Y_stripe / Stramo

                    If mpIndex = 0 Then

                        Ttwist = WingRegions(mpIndex).TwistAxis '* k / NPS(i, j)

                    Else

                        Ttwist = WingRegions(mpIndex - 1).TwistAxis + (WingRegions.Item(mpIndex - 1).TwistAxis - WingRegions.Item(mpIndex - 1).TwistAxis) * Y_stripe / Stramo

                    End If

                    'Calculates the twisting angle

                    Phi = WingRegions(mpIndex).Twist * Y / 180.0 * Math.PI

                    Dim flap_chord_i As Single
                    Dim flap_chord_f As Single = WingRegions(mpIndex).Chamber.FlapChord

                    If mpIndex = 0 Then
                        flap_chord_i = RootFlap
                    Else
                        flap_chord_i = WingRegions(mpIndex - 1).Chamber.FlapChord
                    End If

                    Dim LocalChamber As New ChamberedLine(WingRegions(mpIndex).Chamber)

                    For l = 1 To _nChordNodes ' For each nodal point in the current column...

                        'Calculates the distance from the nodal point to the leading edge

                        flap = flap_chord_i + (flap_chord_f - flap_chord_i) * Y

                        If WingRegions(mpIndex).Chamber.Flapped Then

                            WingRegions(mpIndex).Chamber.FlapChord = flap

                            If l <= _nChordNodes - FlapPanels Then
                                X = (l - 1) / (_nChordNodes - FlapPanels - 1) * (1 - flap)
                            Else
                                X = (1 - flap) + (l - _nChordNodes + FlapPanels) / FlapPanels * flap
                            End If

                        Else

                            X = (l - 1) / _nChordPanels

                        End If

                        'Calculates the chamber

                        Dim pLoc As New EVector2
                        WingRegions(mpIndex).Chamber.EvaluatePoint(pLoc, X)
                        pLoc.X *= Chord
                        pLoc.Y *= Chord

                        'Twisting

                        Pij.X = Math.Cos(Phi) * (pLoc.X - Ttwist * Chord) + Math.Sin(Phi) * pLoc.Y + Y_stripe * Math.Tan(delta) + Ttwist * Chord
                        Pij.Y = Y_stripe
                        Pij.Z = -Math.Sin(Phi) * (pLoc.X - Ttwist * Chord) + Math.Cos(Phi) * pLoc.Y

                        'Overal twisting

                        Dim point As New NodalPoint

                        point.Position.X = Pij.X * Math.Cos(twist) + Pij.Z * Math.Sin(twist) + leadingEdge.X
                        point.Position.Y = Pij.X * Math.Sin(gamma) * Math.Sin(twist) + Pij.Y * Math.Cos(gamma) - Pij.Z * Math.Sin(gamma) * Math.Cos(twist) + leadingEdge.Y
                        point.Position.Z = -Pij.X * Math.Cos(gamma) * Math.Sin(twist) + Pij.Y * Math.Sin(gamma) + Pij.Z * Math.Cos(gamma) * Math.Cos(twist) + leadingEdge.Z

                        If k = WingRegions(mpIndex).nSpanPanels And l = 1 Then
                            XBp = point.Position.X
                            YBp = point.Position.Y
                            ZBp = point.Position.Z
                        End If

                        'Posicionamiento, simetria y orientacion:

                        point.Position.Substract(CenterOfRotation)
                        point.Position.Rotate(rotationMatrix)
                        point.Position.Add(CenterOfRotation)
                        point.Position.Add(Position)

                        Mesh.Nodes.Add(point)

                        nodeCounter += 1

                    Next

                    WingRegions(mpIndex).Chamber.FlapChord = flap_chord_f

                Next

            Next

            ' Assign property to primitive panels

            For i = FirstPrimitiveSegment To LastPrimitiveSegment
                Mesh.Panels(GetPrimitivePanelIndex(i) - 1).IsPrimitive = True
            Next

            Mesh.GenerateLattice()

            ' Local base:

            LocalDirections.U.X = 0.5
            LocalDirections.U.Y = 0.0
            LocalDirections.U.Z = 0.0
            LocalDirections.U.Rotate(rotationMatrix)

            LocalDirections.V.X = 0.0
            LocalDirections.V.Y = 0.5
            LocalDirections.V.Z = 0.0
            LocalDirections.V.Rotate(rotationMatrix)

            LocalDirections.W.X = 0.0
            LocalDirections.W.Y = 0.0
            LocalDirections.W.Z = 0.5
            LocalDirections.W.Rotate(rotationMatrix)

            ' Direction points:

            _DirectionPoints.U.X = 0.5
            _DirectionPoints.U.Y = 0.0
            _DirectionPoints.U.Z = 0.0
            _DirectionPoints.U.Substract(CenterOfRotation)
            _DirectionPoints.U.Rotate(rotationMatrix)
            _DirectionPoints.U.Add(CenterOfRotation)
            _DirectionPoints.U.Add(Position)

            _DirectionPoints.V.X = 0.0
            _DirectionPoints.V.Y = 0.5
            _DirectionPoints.V.Z = 0.0
            _DirectionPoints.V.Substract(CenterOfRotation)
            _DirectionPoints.V.Rotate(rotationMatrix)
            _DirectionPoints.V.Add(CenterOfRotation)
            _DirectionPoints.V.Add(Position)

            _DirectionPoints.W.X = 0.0
            _DirectionPoints.W.Y = 0.0
            _DirectionPoints.W.Z = 0.5
            _DirectionPoints.W.Substract(CenterOfRotation)
            _DirectionPoints.W.Rotate(rotationMatrix)
            _DirectionPoints.W.Add(CenterOfRotation)
            _DirectionPoints.W.Add(Position)

            ' Local origin²

            LocalOrigin.SetToCero()
            LocalOrigin.Substract(CenterOfRotation)
            LocalOrigin.Rotate(rotationMatrix)
            LocalOrigin.Add(CenterOfRotation)
            LocalOrigin.Add(Position)

            GenerateStructure()

            ' Launch base sub to raise update event.

            MyBase.GenerateMesh()

        End Sub

        Public Overrides Sub Refresh3DModel(ByRef gl As OpenGL, Optional ByVal SelectionMode As SelectionModes = SelectionModes.smNoSelection, Optional ByVal ElementIndex As Integer = 0)

            ' Version para OpenGL

            Dim Code As Integer

            ' Panels:

            If VisualProperties.VisualizationMode = VisualizationMode.Lattice Then

                Dim Nodo As EVector3

                gl.InitNames()
                Code = Selection.GetSelectionCode(ComponentTypes.etLiftingSurface, ElementIndex, EntityTypes.etQuadPanel, 0)
                Dim p As Integer = 0

                Dim PColor As New EVector3
                PColor.X = VisualProperties.ColorPrimitives.R / 255
                PColor.Y = VisualProperties.ColorPrimitives.G / 255
                PColor.Z = VisualProperties.ColorPrimitives.B / 255
                Dim SColor As New EVector3
                If Not Selected Then
                    SColor.X = VisualProperties.ColorSurface.R / 255
                    SColor.Y = VisualProperties.ColorSurface.G / 255
                    SColor.Z = VisualProperties.ColorSurface.B / 255
                Else
                    ' default selected color is {255, 194, 14} (orange)
                    SColor.X = 1
                    SColor.Y = 0.76078
                    SColor.Z = 0.0549
                End If

                Dim Transparency As Double
                If VisualProperties.VisualizationMode = VisualizationMode.Lattice Then
                    Transparency = VisualProperties.Transparency
                ElseIf VisualProperties.VisualizationMode = VisualizationMode.Structural Then
                    Transparency = 0.4
                End If

                For Each Panel In Mesh.Panels

                    gl.PushName(Code + p)

                    p += 1

                    gl.Begin(OpenGL.GL_TRIANGLES)

                    If Panel.IsPrimitive And VisualProperties.ShowPrimitives Then
                        gl.Color(PColor.X, PColor.Y, PColor.Z, Transparency)
                    Else
                        gl.Color(SColor.X, SColor.Y, SColor.Z, Transparency)
                    End If

                    ' First triangle:

                    Nodo = Mesh.Nodes(Panel.N1).Position
                    gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                    Nodo = Mesh.Nodes(Panel.N2).Position
                    gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                    Nodo = Mesh.Nodes(Panel.N3).Position
                    gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                    ' Second triangle:

                    If Not Panel.IsTriangular Then

                        Nodo = Mesh.Nodes(Panel.N3).Position
                        gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                        Nodo = Mesh.Nodes(Panel.N4).Position
                        gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                        Nodo = Mesh.Nodes(Panel.N1).Position
                        gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                    End If

                    If Symmetric Then

                        ' First triangle:

                        Nodo = Mesh.Nodes(Panel.N1).Position
                        gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                        Nodo = Mesh.Nodes(Panel.N2).Position
                        gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                        Nodo = Mesh.Nodes(Panel.N3).Position
                        gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                        ' Second triangle:

                        If Not Panel.IsTriangular Then

                            Nodo = Mesh.Nodes(Panel.N3).Position
                            gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                            Nodo = Mesh.Nodes(Panel.N4).Position
                            gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                            Nodo = Mesh.Nodes(Panel.N1).Position
                            gl.Vertex(Nodo.X, -Nodo.Y, Nodo.Z)

                        End If

                    End If

                    gl.End()
                    gl.PopName()

                Next

            End If

            ' Nodes:

            If VisualProperties.VisualizationMode = VisualizationMode.Lattice Then

                gl.PointSize(VisualProperties.SizeNodes)
                gl.Color(0.0F, 0.0F, 0.0F)

                gl.InitNames()
                Code = Selection.GetSelectionCode(ComponentTypes.etLiftingSurface, ElementIndex, EntityTypes.etNode, 0)

                Dim p As Integer = 0
                Dim Nodo As EVector3

                For Each Node In Mesh.Nodes

                    If SelectionMode = SelectionModes.smNodePicking Or Node.Active Then

                        gl.PushName(Code + p)

                        gl.Begin(OpenGL.GL_POINTS)

                        Nodo = Mesh.Nodes(p).Position

                        gl.Vertex(Nodo.X, Nodo.Y, Nodo.Z)

                        gl.End()

                        gl.PopName()

                        p += 1

                    End If

                Next

            End If

            ' Genera el mallado:

            If VisualProperties.VisualizationMode = VisualizationMode.Lattice Or VisualProperties.VisualizationMode = VisualizationMode.Structural Then

                Dim SColor As New EVector3
                SColor.X = 0.75
                SColor.Y = 0.75
                SColor.Z = 0.75
                Dim Thickness As Double = 1.0

                If (VisualProperties.VisualizationMode = VisualizationMode.Lattice) Then
                    SColor.X = VisualProperties.ColorMesh.R / 255
                    SColor.Y = VisualProperties.ColorMesh.G / 255
                    SColor.Z = VisualProperties.ColorMesh.B / 255
                    Thickness = VisualProperties.ThicknessMesh
                End If

                If VisualProperties.ShowMesh Then

                    gl.InitNames()

                    Dim Nodo1 As EVector3
                    Dim Nodo2 As EVector3

                    gl.LineWidth(Thickness)
                    gl.Color(SColor.X, SColor.Y, SColor.Z)

                    Code = Selection.GetSelectionCode(ComponentTypes.etLiftingSurface, ElementIndex, EntityTypes.etVortex, 0)

                    For i = 0 To NumberOfSegments - 1

                        gl.PushName(Code + i)

                        gl.Begin(OpenGL.GL_LINES)
                        Nodo1 = Mesh.Nodes(GetVortex(i).N1).Position
                        Nodo2 = Mesh.Nodes(GetVortex(i).N2).Position

                        gl.Vertex(Nodo1.X, Nodo1.Y, Nodo1.Z)
                        gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)

                        If Symmetric Then

                            gl.Begin(OpenGL.GL_LINES)
                            Nodo1 = Mesh.Nodes(GetVortex(i).N1).Position
                            Nodo2 = Mesh.Nodes(GetVortex(i).N2).Position

                            gl.Vertex(Nodo1.X, -Nodo1.Y, Nodo1.Z)
                            gl.Vertex(Nodo2.X, -Nodo2.Y, Nodo2.Z)

                        End If

                        gl.End()
                        gl.PopName()

                    Next

                End If

            End If

            If VisualProperties.VisualizationMode = VisualizationMode.Structural Then

                gl.Color(0, 0, 0)

                Dim Nodo1 As EVector3
                Dim Nodo2 As EVector3

                Code = Selection.GetSelectionCode(ComponentTypes.etLiftingSurface, ElementIndex, EntityTypes.etStructuralElement, 0)
                Dim Code2 As Integer = Selection.GetSelectionCode(ComponentTypes.etLiftingSurface, ElementIndex, EntityTypes.etStructuralNode, 0)

                For i = 0 To _StructuralPartition.Count - 1

                    Nodo2 = _StructuralPartition(i).p

                    If (i > 0) Then

                        gl.LineWidth(3.0)
                        gl.Color(0.5, 0.5, 0.5, 1.0)
                        gl.PushName(Code + i)
                        gl.Begin(OpenGL.GL_LINES)
                        Nodo1 = _StructuralPartition(i - 1).p
                        gl.Vertex(Nodo1.X, Nodo1.Y, Nodo1.Z)
                        gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)
                        gl.End()
                        gl.PopName()

                        gl.LineWidth(1.0)
                        gl.Enable(OpenGL.GL_LINE_STIPPLE)
                        gl.LineStipple(2, &HC0F)
                        gl.Begin(OpenGL.GL_LINES)
                        gl.Vertex(
                            Nodo1.X + _StructuralPartition(i - 1).Basis.V.X * _StructuralPartition(i - 1).LocalSection.CMy,
                            Nodo1.Y + _StructuralPartition(i - 1).Basis.V.Y * _StructuralPartition(i - 1).LocalSection.CMy,
                            Nodo1.Z + _StructuralPartition(i - 1).Basis.V.Z * _StructuralPartition(i - 1).LocalSection.CMy)
                        gl.Vertex(
                            Nodo2.X + _StructuralPartition(i).Basis.V.X * _StructuralPartition(i).LocalSection.CMy,
                            Nodo2.Y + _StructuralPartition(i).Basis.V.Y * _StructuralPartition(i).LocalSection.CMy,
                            Nodo2.Z + _StructuralPartition(i).Basis.V.Z * _StructuralPartition(i).LocalSection.CMy)
                        gl.End()
                        gl.Disable(OpenGL.GL_LINE_STIPPLE)

                    End If

                    gl.PointSize(4.0)
                    gl.Color(0.0, 0.0, 0.0, 1.0)

                    gl.Begin(OpenGL.GL_POINTS)

                    ' Structural node:

                    gl.PushName(Code2 + i + 1)
                    gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)
                    gl.PopName()

                    ' Mass node:

                    gl.Color(0.0, 0.0, 0.0, 1.0)
                    gl.Vertex(
                        Nodo2.X + _StructuralPartition(i).Basis.V.X * _StructuralPartition(i).LocalSection.CMy,
                        Nodo2.Y + _StructuralPartition(i).Basis.V.Y * _StructuralPartition(i).LocalSection.CMy,
                        Nodo2.Z + _StructuralPartition(i).Basis.V.Z * _StructuralPartition(i).LocalSection.CMy)
                    gl.End()

                    If VisualProperties.ShowLocalCoordinates Then

                        Dim base As EBase3 = _StructuralPartition(i).Basis
                        Dim l As Double

                        If (i = 0) Then
                            l = 0.5 * _StructuralPartition(0).p.DistanceTo(_StructuralPartition(1).p)
                        Else
                            l = 0.5 * _StructuralPartition(i - 1).p.DistanceTo(_StructuralPartition(i).p)
                        End If

                        gl.LineWidth(2.0)
                        gl.Begin(OpenGL.GL_LINES)

                        gl.Color(0.0, 1.0, 0.0, 1.0)
                        gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)
                        gl.Vertex(Nodo2.X + l * base.U.X, Nodo2.Y + l * base.U.Y, Nodo2.Z + l * base.U.Z)

                        gl.Color(1.0, 0.0, 0.0, 1.0)
                        gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)
                        gl.Vertex(Nodo2.X + l * base.V.X, Nodo2.Y + l * base.V.Y, Nodo2.Z + l * base.V.Z)

                        gl.Color(0.0, 0.0, 1.0, 1.0)
                        gl.Vertex(Nodo2.X, Nodo2.Y, Nodo2.Z)
                        gl.Vertex(Nodo2.X + l * base.W.X, Nodo2.Y + l * base.W.Y, Nodo2.Z + l * base.W.Z)

                        gl.End()

                    End If

                Next

            End If

            ' Show local coordinates:

            If VisualProperties.ShowLocalCoordinates Then

                gl.LineWidth(2.0)
                gl.Begin(OpenGL.GL_LINES)

                gl.Color(0.0, 1.0, 0.0)
                gl.Vertex(LocalOrigin.X, LocalOrigin.Y, LocalOrigin.Z)
                gl.Vertex(_DirectionPoints.U.X, _DirectionPoints.U.Y, _DirectionPoints.U.Z)

                gl.Color(1.0, 0.0, 0.0)
                gl.Vertex(LocalOrigin.X, LocalOrigin.Y, LocalOrigin.Z)
                gl.Vertex(_DirectionPoints.V.X, _DirectionPoints.V.Y, _DirectionPoints.V.Z)

                gl.Color(0.0, 0.0, 1.0)
                gl.Vertex(LocalOrigin.X, LocalOrigin.Y, LocalOrigin.Z)
                gl.Vertex(_DirectionPoints.W.X, _DirectionPoints.W.Y, _DirectionPoints.W.Z)

                gl.End()

            End If

        End Sub

        Private Sub SetPrimitives()

            If TrailingEdgeConvection Then
                _PrimitiveData(1, 1) = NumberOfChordPanels + 1
                _PrimitiveData(1, 2) = FirstPrimitiveNode + NumberOfSpanPanels - 1
            End If

            ' Se asegura de que el maximo es mayor que el minimo, y que la cantidad de paneles esta dentro del limite:

            'If FSPRIMA(1, 1) > FNBP Then FSPRIMA(1, 1) = FNBP
            'If FSPRIMA(1, 1) < 1 Then FSPRIMA(1, 1) = 1
            'If FSPRIMA(1, 2) > FSPRIMA(1, 1) Then FSPRIMA(1, 2) = FSPRIMA(1, 1)
            'If FSPRIMA(1, 2) < 1 Then FSPRIMA(1, 2) = 1

            '' Carga el nodo inicial y final del borde de conveccion:

            _PrimitiveData(2, 1) = _PrimitiveData(1, 1)  ' Primer nodo primitivo
            _PrimitiveData(2, 2) = _PrimitiveData(1, 2) + 1 ' Ultimo nodo primitivo

            _nPrimitiveNodes = _PrimitiveData(2, 2) - _PrimitiveData(2, 1) + 1 ' Mumero de nodos primitivos
            _nPrimitivePanels = _PrimitiveData(1, 2) - _PrimitiveData(1, 1) + 1  ' Numero de paneles primitivos

        End Sub

#End Region

#Region " Structure "

        ''' <summary>
        ''' Determines whether or not the structure will be included on the calculation.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property IncludeStructure As Boolean = True

        ''' <summary>
        ''' Wing root section.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property RootSection As Section = New Section

        ''' <summary>
        ''' Contains all nodes and segments representing the structure.
        ''' </summary>
        ''' <remarks></remarks>
        Private _StructuralPartition As New List(Of StructuralPartition)

        ''' <summary>
        ''' Represents the structural nodal points
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property StructuralPartition As List(Of StructuralPartition)
            Get
                Return _StructuralPartition
            End Get
        End Property

        ''' <summary>
        ''' Generates the nodal partition and the section partition.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub GenerateStructure()

            _StructuralPartition.Clear()

            Dim lePoint As EVector3
            Dim tePoint As EVector3

            Dim sp As Integer = 0 ' Starting position

            Dim RootSection As Section = Me.RootSection
            Dim RootChord As Double = _RootChord

            ' Build partition

            Dim i0 As Integer = 0

            For Each Panel As WingRegion In WingRegions

                ' Create the partitions:

                For i = i0 To Panel.nSpanPanels

                    lePoint = Mesh.Nodes(sp * _nChordNodes).Position

                    If Panel.Chamber.Flapped Then
                        tePoint = Mesh.Nodes((sp + 1) * _nChordNodes - 1 - FlapPanels).Position
                    Else
                        tePoint = Mesh.Nodes((sp + 1) * _nChordNodes - 1).Position
                    End If

                    Dim pStructural As New EVector3

                    pStructural.X = lePoint.X + Panel.CenterOfShear * (tePoint.X - lePoint.X)
                    pStructural.Y = lePoint.Y + Panel.CenterOfShear * (tePoint.Y - lePoint.Y)
                    pStructural.Z = lePoint.Z + Panel.CenterOfShear * (tePoint.Z - lePoint.Z)

                    Dim LocalPartition = New StructuralPartition

                    LocalPartition.p.X = pStructural.X
                    LocalPartition.p.Y = pStructural.Y
                    LocalPartition.p.Z = pStructural.Z

                    Dim coord As Double = i / (Panel.nSpanPanels - i0)

                    LocalPartition.LocalSection.AE = RootSection.AE + coord * (Panel.TipSection.AE - RootSection.AE)
                    LocalPartition.LocalSection.GJ = RootSection.GJ + coord * (Panel.TipSection.GJ - RootSection.GJ)
                    LocalPartition.LocalSection.EIy = RootSection.EIy + coord * (Panel.TipSection.EIy - RootSection.EIy)
                    LocalPartition.LocalSection.EIz = RootSection.EIz + coord * (Panel.TipSection.EIz - RootSection.EIz)
                    LocalPartition.LocalSection.rIp = RootSection.rIp + coord * (Panel.TipSection.rIp - RootSection.rIp)
                    LocalPartition.LocalSection.m = RootSection.m + coord * (Panel.TipSection.m - RootSection.m)
                    LocalPartition.LocalSection.CMy = RootSection.CMy + coord * (Panel.TipSection.CMy - RootSection.CMy)
                    LocalPartition.LocalSection.CMz = RootSection.CMz + coord * (Panel.TipSection.CMz - RootSection.CMz)
                    LocalPartition.LocalChord = RootChord + coord * (Panel.TipChord - RootSection.m)

                    If (sp > 0) Then

                        Dim oldP As EVector3 = StructuralPartition(StructuralPartition.Count - 1).p

                        LocalPartition.Basis.U.X = pStructural.X - oldP.X
                        LocalPartition.Basis.U.Y = pStructural.Y - oldP.Y
                        LocalPartition.Basis.U.Z = pStructural.Z - oldP.Z
                        LocalPartition.Basis.U.Normalize()

                        If (sp = 1) Then

                            StructuralPartition(0).Basis.U.X = pStructural.X - oldP.X
                            StructuralPartition(0).Basis.U.Y = pStructural.Y - oldP.Y
                            StructuralPartition(0).Basis.U.Z = pStructural.Z - oldP.Z
                            StructuralPartition(0).Basis.U.Normalize()

                            StructuralPartition(0).Basis.W.FromVectorProduct(StructuralPartition(0).Basis.V, StructuralPartition(0).Basis.U)
                            StructuralPartition(0).Basis.V.FromVectorProduct(StructuralPartition(0).Basis.U, StructuralPartition(0).Basis.W)

                        End If

                    End If

                    LocalPartition.Basis.V.X = tePoint.X - lePoint.X
                    LocalPartition.Basis.V.Y = tePoint.Y - lePoint.Y
                    LocalPartition.Basis.V.Z = tePoint.Z - lePoint.Z
                    LocalPartition.Basis.V.Normalize()

                    If (sp > 0) Then
                        LocalPartition.Basis.W.FromVectorProduct(LocalPartition.Basis.V, LocalPartition.Basis.U)
                        LocalPartition.Basis.V.FromVectorProduct(LocalPartition.Basis.U, LocalPartition.Basis.W)
                    End If

                    _StructuralPartition.Add(LocalPartition)

                    sp += 1

                Next

                RootSection = Panel.TipSection
                RootChord = Panel.TipChord
                i0 = 1

            Next

        End Sub

#End Region

#Region " UVLM method "

        ''' <summary>
        ''' Calculates the normal vectors and the control points.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CalculateControlPointsAndNormals()

            Try

                Dim Nodo1 As New EVector3
                Dim Nodo2 As New EVector3
                Dim Nodo3 As New EVector3
                Dim Nodo4 As New EVector3

                Dim Vector1 As EVector3
                Dim Vector2 As EVector3
                Dim Vector3 As EVector3
                Dim Vector4 As EVector3

                Dim Diagonal1 As New EVector3
                Dim Diagonal2 As New EVector3

                For i = 0 To NumberOfPanels - 1

                    Nodo1 = Mesh.Nodes(Mesh.Panels(i).N1).Position
                    Nodo2 = Mesh.Nodes(Mesh.Panels(i).N2).Position
                    Nodo3 = Mesh.Nodes(Mesh.Panels(i).N3).Position
                    Nodo4 = Mesh.Nodes(Mesh.Panels(i).N4).Position

                    Vector1 = Nodo1.GetVectorToPoint(Nodo2)
                    Vector2 = Nodo2.GetVectorToPoint(Nodo3)
                    Vector3 = Nodo3.GetVectorToPoint(Nodo4)
                    Vector4 = Nodo4.GetVectorToPoint(Nodo1)

                    Mesh.Panels(i).ControlPoint.X = 0.25 * (Nodo1.X + Nodo2.X + Nodo3.X + Nodo4.X)
                    Mesh.Panels(i).ControlPoint.Y = 0.25 * (Nodo1.Y + Nodo2.Y + Nodo3.Y + Nodo4.Y)
                    Mesh.Panels(i).ControlPoint.Z = 0.25 * (Nodo1.Z + Nodo2.Z + Nodo3.Z + Nodo4.Z)

                    Diagonal1.X = Nodo2.X - Nodo4.X
                    Diagonal1.Y = Nodo2.Y - Nodo4.Y
                    Diagonal1.Z = Nodo2.Z - Nodo4.Z

                    Diagonal2.X = Nodo3.X - Nodo1.X
                    Diagonal2.Y = Nodo3.Y - Nodo1.Y
                    Diagonal2.Z = Nodo3.Z - Nodo1.Z

                    Mesh.Panels(i).NormalVector = Algebra.VectorProduct(Diagonal1, Diagonal2).NormalizedDirection
                    Mesh.Panels(i).Area = 0.5 * Algebra.VectorProduct(Vector1, Vector2).EuclideanNorm + 0.5 * Algebra.VectorProduct(Vector3, Vector4).EuclideanNorm

                Next

            Catch ex As Exception

                MsgBox("Unable to calculate local data on lifting surface:" & vbNewLine & ex.Message, MsgBoxStyle.OkOnly, "Error while calculating local data")

            End Try

        End Sub

#End Region

#Region " Other methods "

        '''' <summary>
        '''' Generates a general surface from this wing.
        '''' </summary>
        '''' <param name="Superficie"></param>
        '''' <remarks></remarks>
        'Public Sub GenerateGeneralSurface(ByRef Superficie As ResultContainer)

        '    For i = 0 To NumberOfNodes - 1
        '        Superficie.AddNodalPoint(Mesh.Nodes(i).Position)
        '    Next

        '    For i = 0 To NumberOfPanels - 1
        '        Superficie.AddPanel(Mesh.Panels(i))
        '    Next

        'End Sub

        ''' <summary>
        ''' Generates a copy of the instance.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Clone() As Surface

            Dim Surface As New LiftingSurface

            Surface.RootChord = RootChord
            Surface.NumberOfChordPanels = NumberOfChordPanels
            Surface.Symmetric = True
            Surface.FirstPrimitiveSegment = FirstPrimitiveSegment
            Surface.LastPrimitiveSegment = LastPrimitiveSegment
            Surface.ConvectWake = ConvectWake
            Surface.RootSection.Assign(RootSection)

            ' Add panels:

            Dim p As Integer = 0

            For Each Panel In WingRegions

                If p > 0 Then
                    Surface.AddRegion()
                End If

                Surface.WingRegions(p).Assign(Panel)
                Surface.WingRegions(p).TipSection.Assign(WingRegions(p).TipSection)

                p += 1

            Next

            Surface.GenerateMesh()

            Return Surface

        End Function

#End Region

#Region " Geometrical opperations "

        ''' <summary>
        ''' Aligns the surface by means of four reference points (it is not working well).
        ''' </summary>
        ''' <param name="Point1"></param>
        ''' <param name="Point2"></param>
        ''' <param name="Point3"></param>
        ''' <param name="Point4"></param>
        ''' <remarks></remarks>
        Public Overrides Sub Align(ByVal Point1 As EVector3, ByVal Point2 As EVector3, ByVal Point3 As EVector3, ByVal Point4 As EVector3)

            ' Rotate arround P1 in order to align segments:

            Me.CenterOfRotation.X = Point2.X
            Me.CenterOfRotation.Y = Point2.Y
            Me.CenterOfRotation.Z = Point2.Z

            Dim V1 As EVector3 = Point1 - Point3
            Dim V2 As EVector3 = Point2 - Point4

            Dim V1h As New EVector2

            V1h.X = V1.X
            V1h.Y = V1.Y
            V1h.Normalize()

            Dim V2h As New EVector2
            V2h.X = V2.X
            V2h.Y = V2.Y
            V2h.Normalize()

            Dim V2ho As EVector2 = V2h.OrthogonalVector(V1h)
            Dim V1ho As New EVector2
            V1ho.Y = V1h.X
            V1ho.X = -V1h.Y

            Dim Sign As Integer = Math.Sign(V2ho.X * V1ho.X + V2ho.Y * V1ho.Y)

            Orientation.Psi += Sign * Math.Acos(V1h.X * V2h.X + V1h.Y * V2h.Y) * 180 / Math.PI

            'Dim Vertical As New EVector3
            'Vertical.Z = V1.Z - V2.Z
            'Me.OrientacionGlobal.Tita = Math.Acos(V2.ProductoInterno(V2 + Vertical)) * 180 / Math.PI

            ' Translate P1 to P2:

            Position.X = Point2.X
            Position.Y = Point2.Y
            Position.Z = Point2.Z

            GenerateMesh()

        End Sub

#End Region

#Region " IO "

        ''' <summary>
        ''' Reads the wing from an XML file.
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <remarks></remarks>
        Public Overrides Sub ReadFromXML(ByRef reader As XmlReader)

            Dim count As Integer = 1

            While reader.Read

                If Not reader.NodeType = XmlNodeType.Element Then Continue While

                Select Case reader.Name

                    Case "SurfaceProperties"

                        RootChord = IOXML.ReadDouble(reader, "RootChord", 0.0)
                        RootFlap = IOXML.ReadDouble(reader, "RootFlap", 0.2)
                        FlapPanels = IOXML.ReadInteger(reader, "FlapPanels", 3)
                        IncludeInCalculation = IOXML.ReadBoolean(reader, "Include", True)

                        Position.X = IOXML.ReadDouble(reader, "PGX", 0.0)
                        Position.Y = IOXML.ReadDouble(reader, "PGY", 0.0)
                        Position.Z = IOXML.ReadDouble(reader, "PGZ", 0.0)

                        Orientation.Psi = IOXML.ReadDouble(reader, "OGPsi", 0.0)
                        Orientation.Tita = IOXML.ReadDouble(reader, "OGTita", 0.0)
                        Orientation.Fi = IOXML.ReadDouble(reader, "OGFi", 0.0)
                        Orientation.Secuence = IOXML.ReadInteger(reader, "OGSecuence", CInt(EulerAngles.RotationSecuence.ZYX))

                        CenterOfRotation.X = IOXML.ReadDouble(reader, "PCRX", 0.0)
                        CenterOfRotation.Y = IOXML.ReadDouble(reader, "PCRY", 0.0)
                        CenterOfRotation.Z = IOXML.ReadDouble(reader, "PCRZ", 0.0)

                        TrailingEdgeConvection = IOXML.ReadBoolean(reader, "TrailingConvection", False)
                        FirstPrimitiveSegment = IOXML.ReadInteger(reader, "PRIM1", 1)
                        LastPrimitiveSegment = IOXML.ReadInteger(reader, "PRIM2", 2)
                        ConvectWake = IOXML.ReadBoolean(reader, "ConvectWake", True)
                        CuttingStep = IOXML.ReadInteger(reader, "CuttingStep", 1)
                        Symmetric = IOXML.ReadBoolean(reader, "Symmetric", True)

                        RootSection.AE = IOXML.ReadDouble(reader, "RootA", 0.0)
                        RootSection.GJ = IOXML.ReadDouble(reader, "RootIu", 0.0)
                        RootSection.EIy = IOXML.ReadDouble(reader, "RootIv", 0.0)
                        RootSection.EIz = IOXML.ReadDouble(reader, "RootIw", 0.0)
                        RootSection.CMy = IOXML.ReadDouble(reader, "RootSv", 0.0)
                        RootSection.CMz = IOXML.ReadDouble(reader, "RootSw", 0.0)
                        RootSection.rIp = IOXML.ReadDouble(reader, "RootJ", 0.0)
                        RootSection.m = IOXML.ReadDouble(reader, "Rootm", 0.0)

                        NumberOfChordPanels = IOXML.ReadInteger(reader, "NumberChordRings", 6)

                    Case "Identity"

                        Name = reader.GetAttribute("Name")
                        ID = New Guid(IOXML.ReadString(reader, "ID", Guid.NewGuid.ToString))

                    Case "MacroPanel", String.Format("MacroPanel{0}", count)

                        If count > 1 Then AddRegion()
                        CurrentRegionID = count
                        CurrentRegion.ReadFromXML(reader)
                        count += 1

                    Case "VisualProperties"

                        VisualProperties.ReadFromXML(reader.ReadSubtree)

                End Select

            End While

            Me.GenerateMesh()

        End Sub

        ''' <summary>
        ''' Writes the wing to an XML file.
        ''' </summary>
        ''' <param name="writer"></param>
        ''' <remarks></remarks>
        Public Overrides Sub WriteToXML(ByRef writer As XmlWriter)

            ' Identity:

            writer.WriteStartElement("Identity")
            writer.WriteAttributeString("Name", Name)
            writer.WriteAttributeString("ID", ID.ToString)
            writer.WriteEndElement()

            ' Surface properties:

            writer.WriteStartElement("SurfaceProperties")

            writer.WriteAttributeString("RootChord", String.Format("{0}", RootChord))
            writer.WriteAttributeString("RootFlap", String.Format("{0}", RootFlap))
            writer.WriteAttributeString("FlapPanels", String.Format("{0}", FlapPanels))
            writer.WriteAttributeString("Include", String.Format("{0}", IncludeInCalculation))

            writer.WriteAttributeString("PGX", String.Format("{0}", Position.X))
            writer.WriteAttributeString("PGY", String.Format("{0}", Position.Y))
            writer.WriteAttributeString("PGZ", String.Format("{0}", Position.Z))

            writer.WriteAttributeString("OGPsi", String.Format("{0}", Orientation.Psi))
            writer.WriteAttributeString("OGTita", String.Format("{0}", Orientation.Tita))
            writer.WriteAttributeString("OGFi", String.Format("{0}", Orientation.Fi))
            writer.WriteAttributeString("OGSecuence", String.Format("{0}", CInt(Orientation.Secuence)))

            writer.WriteAttributeString("PCRX", String.Format("{0}", CenterOfRotation.X))
            writer.WriteAttributeString("PCRY", String.Format("{0}", CenterOfRotation.Y))
            writer.WriteAttributeString("PCRZ", String.Format("{0}", CenterOfRotation.Z))

            writer.WriteAttributeString("PRIM1", String.Format("{0}", FirstPrimitiveSegment))
            writer.WriteAttributeString("PRIM2", String.Format("{0}", LastPrimitiveSegment))
            writer.WriteAttributeString("ConvectWake", String.Format("{0}", CInt(ConvectWake)))
            writer.WriteAttributeString("CuttingStep", String.Format("{0}", CuttingStep))
            writer.WriteAttributeString("Symmetric", String.Format("{0}", CInt(Symmetric)))
            writer.WriteAttributeString("TrailingConvection", String.Format("{0}", CInt(TrailingEdgeConvection)))

            writer.WriteAttributeString("RootA", String.Format("{0}", RootSection.AE))
            writer.WriteAttributeString("RootIu", String.Format("{0}", RootSection.GJ))
            writer.WriteAttributeString("RootIv", String.Format("{0}", RootSection.EIy))
            writer.WriteAttributeString("RootIw", String.Format("{0}", RootSection.EIz))
            writer.WriteAttributeString("RootSv", String.Format("{0}", RootSection.CMy))
            writer.WriteAttributeString("RootSw", String.Format("{0}", RootSection.CMz))
            writer.WriteAttributeString("RootJ", String.Format("{0}", RootSection.rIp))
            writer.WriteAttributeString("Rootm", String.Format("{0}", RootSection.m))

            writer.WriteAttributeString("NumberChordRings", String.Format("{0}", NumberOfChordPanels))
            writer.WriteAttributeString("NumberMacroPanels", String.Format("{0}", WingRegions.Count))

            writer.WriteEndElement()

            ' Macro panels:

            For i = 0 To WingRegions.Count - 1

                CurrentRegionID = i

                writer.WriteStartElement("MacroPanel")
                WingRegions(i).WriteToXML(writer)
                writer.WriteEndElement()

            Next

            ' Visual properties:

            writer.WriteStartElement("VisualProperties")
            VisualProperties.WriteToXML(writer)
            writer.WriteEndElement()

        End Sub

        ''' <summary>
        ''' Returns a string with information about this wing.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetriveStringData() As String

            Dim Data As String = ""

            Data += "Surface data:" & vbNewLine
            Data += String.Format("Total number of nodal points: {0}", Mesh.Nodes.Count) & vbNewLine
            Data += String.Format("Total number of vortex rings: {0}", Mesh.Panels.Count) & vbNewLine
            Data += String.Format("Total number of vortex segments: {0}", Mesh.Lattice.Count) & vbNewLine

            Me.CalculateControlPointsAndNormals()

            ' Calculate surface area:

            Dim TotalArea As Double = 0

            For Each Ring In Mesh.Panels

                TotalArea += Ring.Area

            Next

            Data += String.Format("Total area: {0:F6}m²", TotalArea) & vbNewLine

            ' Calculate surface length:

            Dim TotalWingspan As Double
            Dim ProjectedWingspan As Double

            For Each Panel In WingRegions

                TotalWingspan += Panel.Length
                ProjectedWingspan += Panel.Length * Math.Abs(Math.Cos(Math.PI * Panel.Dihedral / 180))

            Next

            Data += String.Format("Wingspan: {0:F6}m", TotalWingspan) & vbNewLine
            Data += String.Format("Projected wingspan: {0:F6}m", ProjectedWingspan) & vbNewLine

            ' Aspect ratio:

            Data += String.Format("Aparent aspect ratio: {0:F6}", TotalWingspan ^ 2 / TotalArea) & vbNewLine
            Data += String.Format("Projected aspect ratio: {0:F6}", ProjectedWingspan ^ 2 / TotalArea) & vbNewLine

            Return Data

        End Function

#End Region

    End Class

End Namespace