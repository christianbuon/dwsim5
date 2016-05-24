﻿Imports CapeOpen
Imports DWSIM.Thermodynamics.PropertyPackages
Imports Cudafy

<System.Serializable()>
<ComClass(CAPEOPENManager.ClassId, CAPEOPENManager.InterfaceId, CAPEOPENManager.EventsId)>
Public Class CAPEOPENManager

    Implements ICapeIdentification, ICapeThermoPropertyPackageManager, ICapeUtilities
    Implements IDisposable

    Public Const ClassId As String = "7f5822f2-098d-46dd-9b89-0189d666edb1"
    Public Const InterfaceId As String = "54dd580a-9931-48f9-b139-e6279a4bfc06"
    Public Const EventsId As String = "cc6f7907-aad1-41a5-adab-24825cd73c05"

    Private _name, _description As String
    Private _params As ParameterCollection

    Sub New()
        _name = "DWSIM Property Package Manager"
        _description = "Exposes DWSIM Property Packages to clients using CAPE-OPEN Thermodynamic Interface Definitions"
    End Sub

    Public Function GetPropertyPackage(ByVal PackageName As String) As Object Implements ICapeThermoPropertyPackageManager.GetPropertyPackage
        Dim pp As PropertyPackage = Nothing
        Select Case PackageName
            Case "FPROPS"
                pp = New FPROPSPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescFPP")
            Case "CoolProp"
                pp = New CoolPropPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescCPPP")
            Case "PC-SAFT"
                pp = New PCSAFTPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescPCSAFTPP")
            Case "Peng-Robinson (PR)"
                pp = New PengRobinsonPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescPengRobinsonPP")
            Case "Peng-Robinson-Stryjek-Vera 2 (PRSV2-M)", "Peng-Robinson-Stryjek-Vera 2 (PRSV2)"
                pp = New PRSV2PropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescPRSV2PP")
            Case "Peng-Robinson-Stryjek-Vera 2 (PRSV2-VL)"
                pp = New PRSV2VLPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescPRSV2VLPP")
            Case "Soave-Redlich-Kwong (SRK)"
                pp = New SRKPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescSoaveRedlichKwongSRK")
            Case "Peng-Robinson / Lee-Kesler (PR/LK)"
                pp = New PengRobinsonLKPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescPRLK")
            Case "UNIFAC"
                pp = New UNIFACPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescUPP")
            Case "UNIFAC-LL"
                pp = New UNIFACLLPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescUPP")
            Case "NRTL"
                pp = New NRTLPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescNRTLPP")
            Case "UNIQUAC"
                pp = New UNIQUACPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescUNIQUACPP")
            Case "Modified UNIFAC (Dortmund)"
                pp = New MODFACPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescMUPP")
            Case "Modified UNIFAC (NIST)"
                pp = New NISTMFACPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescNUPP")
            Case "Chao-Seader"
                pp = New ChaoSeaderPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescCSLKPP")
            Case "Grayson-Streed"
                pp = New GraysonStreedPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescGSLKPP")
            Case "Lee-Kesler-Plöcker"
                pp = New LKPPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescLKPPP")
            Case "Raoult's Law"
                pp = New RaoultPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescRPP")
            Case "IAPWS-IF97 Steam Tables"
                pp = New SteamTablesPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescSteamTablesPP")
            Case "IAPWS-08 Seawater"
                pp = New SeawaterPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescSEAPP")
            Case "Sour Water"
                pp = New SourWaterPropertyPackage(True)
                pp.ComponentDescription = Calculator.GetLocalString("DescSourWaterPP")
            Case Else
                Throw New CapeBadArgumentException("Property Package not found.")
        End Select
        If Not pp Is Nothing Then pp.ComponentName = PackageName
        Return pp
    End Function

    Public Function GetPropertyPackageList() As Object Implements ICapeThermoPropertyPackageManager.GetPropertyPackageList
        Return New String() {"FPROPS", "CoolProp", "PC-SAFT", "Peng-Robinson (PR)", "Peng-Robinson-Stryjek-Vera 2 (PRSV2-M)", "Peng-Robinson-Stryjek-Vera 2 (PRSV2-VL)", "Soave-Redlich-Kwong (SRK)", "Peng-Robinson / Lee-Kesler (PR/LK)", _
                             "UNIFAC", "UNIFAC-LL", "Modified UNIFAC (Dortmund)", "Modified UNIFAC (NIST)", "NRTL", "UNIQUAC", _
                            "Chao-Seader", "Grayson-Streed", "Lee-Kesler-Plöcker", "Raoult's Law", "IAPWS-IF97 Steam Tables", "IAPWS-08 Seawater", "Sour Water"}
    End Function

    Public Property ComponentDescription() As String Implements ICapeIdentification.ComponentDescription
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Property ComponentName() As String Implements ICapeIdentification.ComponentName
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Sub Edit() Implements ICapeUtilities.Edit
        Throw New CapeNoImplException("Edit() not implemented.")
    End Sub

    Public Sub Initialize() Implements ICapeUtilities.Initialize

        Application.EnableVisualStyles()

        My.Application.ChangeCulture("en")
        My.Application.ChangeUICulture("en")

        _params = New ParameterCollection()

        'set CUDA params

        CudafyModes.Compiler = eGPUCompiler.All
        CudafyModes.Target = GlobalSettings.Settings.CudafyTarget

        'handler for unhandled exceptions

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler Application.ThreadException, AddressOf UnhandledException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledException2

        Dim exlist As List(Of Exception) = Thermodynamics.NativeLibraries.Files.InitLibraries()

        'For Each ex In exlist
        'Throw New CapeFailedInitialisationException(ex.Message.ToString, ex)
        'Next

    End Sub

    Private Sub UnhandledException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        Try
            Dim frmEx As New FormUnhandledException
            frmEx.TextBox1.Text = e.Exception.ToString
            frmEx.ex = e.Exception
            frmEx.ShowDialog()
        Finally

        End Try
    End Sub

    Private Sub UnhandledException2(ByVal sender As Object, ByVal e As System.UnhandledExceptionEventArgs)
        Try
            Dim frmEx As New FormUnhandledException
            frmEx.TextBox1.Text = e.ExceptionObject.ToString
            frmEx.ex = e.ExceptionObject
            frmEx.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub
    Public ReadOnly Property parameters() As Object Implements ICapeUtilities.parameters
        Get
            Return _params
        End Get
    End Property

    <Runtime.InteropServices.ComVisible(False)> Public WriteOnly Property simulationContext() As Object Implements ICapeUtilities.simulationContext
        Set(ByVal value As Object)
            'do nothing
        End Set
    End Property

    Public Sub Terminate() Implements ICapeUtilities.Terminate

        Dim exlist As List(Of Exception) = Thermodynamics.NativeLibraries.Files.RemoveLibraries()

        Me.Dispose()

    End Sub

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    <System.Runtime.InteropServices.ComRegisterFunction()> _
    Private Shared Sub RegisterFunction(ByVal t As Type)

        Dim keyname As String = String.Concat("CLSID\\{", t.GUID.ToString, "}\\Implemented Categories")
        Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(keyname, True)
        If key Is Nothing Then
            key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(keyname)
        End If
        key.CreateSubKey("{CF51E383-0110-4ed8-ACB7-B50CFDE6908E}") ' CAPE-OPEN 1.1 PPM
        'key.CreateSubKey("{678c09a3-7d66-11d2-a67d-00105a42887f}") ' CAPE-OPEN 1.0 TS
        key.CreateSubKey("{678C09A1-7D66-11D2-A67D-00105A42887F}") ' CAPE-OPEN Object 
        keyname = String.Concat("CLSID\\{", t.GUID.ToString, "}\\CapeDescription")
        key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(keyname)
        key.SetValue("Name", "DWSIM Property Package Manager")
        key.SetValue("Description", "DWSIM CAPE-OPEN Property Package Manager")
        key.SetValue("CapeVersion", "1.1")
        key.SetValue("ComponentVersion", My.Application.Info.Version.ToString)
        key.SetValue("VendorURL", "http://dwsim.inforside.com.br")
        key.SetValue("HelpURL", "http://dwsim.inforside.com.br")
        key.SetValue("About", "DWSIM is open-source software, released under the GPL v3 license. (c) 2011-2016 Daniel Medeiros.")
        key.Close()

    End Sub

    <System.Runtime.InteropServices.ComUnregisterFunction()> _
    Private Shared Sub UnregisterFunction(ByVal t As Type)
        Try

            Dim keyname As String = String.Concat("CLSID\\{", t.GUID.ToString, "}")
            Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(keyname, True)
            Dim keyNames() As String = key.GetSubKeyNames
            For Each kn As String In keyNames
                key.DeleteSubKeyTree(kn)
            Next
            Dim valueNames() As String = key.GetValueNames
            For Each valueName As String In valueNames
                key.DeleteValue(valueName)
            Next

        Catch ex As Exception

        End Try
    End Sub

End Class
