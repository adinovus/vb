
'Imports System
'Imports System.ComponentModel
'Imports System.Threading
'Imports System.IO.Ports

Public Class Form1

    Dim provider As String
    Dim dataFile As String
    Dim connString As String
   

    Dim baud_rate As String = 9600
    Dim comPorts As Array  'Com ports enumerated into here
    Dim port_num As String
    Dim data_received As String
    Dim first_bit As Char
    Dim gprmc As String = "$GPRMC"
    Dim rfid As String
    Dim locationid As String
    Dim status As String
    Dim balance_db As String
    Dim locationid_db As String
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        comPorts = IO.Ports.SerialPort.GetPortNames()

        For i = 0 To UBound(comPorts)
            ComboBox1.Items.Add(comPorts(i))
        Next

        'Set ComboBox1 text to first available port
        ComboBox1.Text = ComboBox1.Items.Item(0)
        'Set SerialPort1 portname to first available port
        SerialPort1.PortName = ComboBox1.Text

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'if port is open
        AddHandler SerialPort1.DataReceived, AddressOf SerialPort1_DataReceived

        If SerialPort1.IsOpen Then
            SerialPort1.RtsEnable = False
            SerialPort1.DtrEnable = False
            SerialPort1.Close()

            Application.DoEvents()



        End If

        'set port setting
        SerialPort1.PortName = port_num
        SerialPort1.BaudRate = baud_rate
        SerialPort1.DataBits = 8
        SerialPort1.Parity = IO.Ports.Parity.None
        SerialPort1.StopBits = IO.Ports.StopBits.One
        SerialPort1.WriteTimeout = 2000
        'SerialPort1.ReadTimeout = 10000

        Try

            'open port
            SerialPort1.Open()

        Catch ex As Exception

            'give alert any problem
            MsgBox(ex.Message)

        End Try

        If SerialPort1.IsOpen Then
            SerialPort1.RtsEnable = True
            Label1.Text = "connection established"

        End If


    End Sub

    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)

        Invoke(Sub() Label2.Text = "data receiving")
        ' System.Threading.Thread.Sleep(300)
        data_received = (SerialPort1.ReadExisting)

        Invoke(Sub() TextBox1.Text = TextBox1.Text + data_received)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        SerialPort1.WriteLine("hello")
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        port_num = ComboBox1.Text
    End Sub
End Class
