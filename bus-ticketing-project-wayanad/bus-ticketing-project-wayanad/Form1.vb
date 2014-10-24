Imports System.Data.OleDb

Public Class Form1


    Dim provider As String
    Dim dataFile As String
    Dim connString As String
    Public myConnection As OleDbConnection = New OleDbConnection
    Public dr As OleDbDataReader

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
    'Dim i As Integer = 0


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'if port is open
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

        Try

            'open port
            SerialPort1.Open()

        Catch ex As Exception

            'give alert any problem
            MsgBox(ex.Message)

        End Try

        If SerialPort1.IsOpen Then
            SerialPort1.RtsEnable = True
            Label4.Text = "connection established"

        End If


    End Sub

    Sub GetSerialPortNames()
        ' Show all available COM ports. 
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
    End Sub


    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        baud_rate = ComboBox2.Text
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'database
        provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source ="
        dataFile = "F:\database\bus-ticketing-db.mdb" ' Change it to your Access Database location
        connString = provider & dataFile
        myConnection.ConnectionString = connString
        'database


        comPorts = IO.Ports.SerialPort.GetPortNames()

        For i = 0 To UBound(comPorts)
            ComboBox1.Items.Add(comPorts(i))
        Next

        'Set ComboBox1 text to first available port
        ComboBox1.Text = ComboBox1.Items.Item(0)
        'Set SerialPort1 portname to first available port
        SerialPort1.PortName = ComboBox1.Text

    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        port_num = ComboBox1.Text
    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

        'define a string


        'get data from serial port
        'data_received = SerialPort1.ReadLine
        'first_bit = data_received(0)

        'rfid = data_received.Substring(1, 3)
        'locationid = data_received.Substring(5, 2)


        'If first_bit = "$" Then
        '    Invoke(Sub() Label7.Text = "passenger entered")
        '    myConnection.Open()

        '    Invoke(Sub() card_id.Text = "")

        '    Dim str As String
        '    str = "SELECT * FROM passenger-info WHERE (rfid = '" & rfid & "')"
        '    Dim cmd As OleDbCommand = New OleDbCommand(str, myConnection)
        '    dr = cmd.ExecuteReader
        '    While dr.Read()
        '        Invoke(Sub() card_id.Text = dr("").ToString)
        '        Invoke(Sub() locationtext.Text = dr("last_name").ToString)
        '    End While
        '    myConnection.Close()
        'End If

        'give an alert when data received

        Dim data_buffer As String

        data_received = SerialPort1.ReadExisting
        Invoke(Sub() TextBox5.Text = TextBox5.Text + data_received)
        Dim data_buffer2 As String

        Dim i As Integer = 0
        Invoke(Sub() data_buffer = TextBox5.Text)
        i = Len(data_buffer)
        Invoke(Sub() Label10.Text = i)
        data_buffer2 = data_buffer.Substring(i - 6, 6)

        Invoke(Sub() Label11.Text = data_buffer2)

        Dim rfid_db
        first_bit = data_buffer(0)
        rfid = data_buffer2.Substring(1, 3)
        'Invoke(Sub() TextBox5.Text = rfid)
        locationid = data_buffer2.Substring(4, 2)
        If first_bit = "$" Then
            Invoke(Sub() Label7.Text = "passenger entered")
            myConnection.Open()
            Dim str As String
            str = "SELECT * FROM passenger_info WHERE (rfid = '" & rfid & "')"
            Dim cmd As OleDbCommand = New OleDbCommand(str, myConnection)
            dr = cmd.ExecuteReader
            While dr.Read()
                rfid_db = dr("rfid").ToString
                status = dr("status").ToString
                balance_db = dr("balance").ToString
                locationid_db = dr("in_location").ToString
            End While
            If rfid_db Is Nothing Then
                Invoke(Sub() Label9.Text = "card not recognized")
                SerialPort1.WriteLine("card not recognized")
            Else

                'checking passenger is already in bus or not

                If status = "y" Then
                    Invoke(Sub() Label8.Text = "passenger leaving the bus")

                    Invoke(Sub() TextBox4.Text = "n")
                Else
                    Invoke(Sub() Label8.Text = "passenger entering the bus")
                    If balance_db < 10 Then
                        Invoke(Sub() Label9.Text = "not enough balance")
                    Else
                        Invoke(Sub() Label9.Text = "passenger allowed to travel")
                        Invoke(Sub() card_id.Text = balance_db)
                        Invoke(Sub() locationtext.Text = locationid_db)
                        Invoke(Sub() TextBox4.Text = "y")
                    End If


                End If

            End If

        End If
        myConnection.Close()



    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        Dim data_send As String
        data_send = TextBox1.Text
        SerialPort1.WriteLine(data_send)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        myConnection.Open()
        rfid = TextBox1.Text.Substring(1, 3)
        locationid = TextBox1.Text.Substring(4, 2)

        Dim str As String
        str = "SELECT * FROM passenger_info WHERE (rfid = '" & rfid & "')"
        Dim cmd As OleDbCommand = New OleDbCommand(str, myConnection)
        dr = cmd.ExecuteReader
        While dr.Read()
            status = dr("status").ToString
            balance_db = dr("balance").ToString
            locationid_db = dr("in_location").ToString


            If status = "y" Then
                Label8.Text = "passenger leaving the bus"

                TextBox4.Text = "n"
            Else
                Label8.Text = "passenger entering the bus"
                If balance_db < 10 Then
                    Label9.Text = "not enough balance"
                Else
                    Label9.Text = "passenger allowed to travel"
                    card_id.Text = dr("balance").ToString
                    locationtext.Text = dr("in_location").ToString
                    TextBox4.Text = "y"
                End If


            End If
        End While
        myConnection.Close()
       
    End Sub

    Private Sub card_id_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles card_id.TextChanged

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        myConnection.Open()

        rfid = TextBox1.Text.Substring(1, 3)
        locationid = TextBox1.Text.Substring(4, 2)
        Dim str2 As String
        str2 = "UPDATE passenger_info SET "

        str2 = str2 + " in_location = '" & locationid & "'"
        ' str2 = str2 + " balance = '" & TextBox3.Text & "'"
        'str = str + " status = '" & TextBox2.Text & "'"
        str2 = str2 + " WHERE "
        str2 = str2 + " rfid = '" & rfid & "'"

        Dim cmd2 As OleDbCommand = New OleDbCommand(str2, myConnection)
        'cmd.Parameters.Add(New OleDbParameter("rfid", CType(TextBox1.Text, String)))
        ' cmd.Parameters.Add(New OleDbParameter("balance", CType(TextBox2.Text, String)))
        'cmd.Parameters.Add(New OleDbParameter("in_location", CType(TextBox3.Text, String)))
        Try
            cmd2.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        myConnection.Close()
    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged
        'myConnection.Open()
        Dim str2 As String
        If TextBox4.Text = "y" Then


            str2 = "UPDATE passenger_info SET "

            str2 = str2 + " status = '" & TextBox4.Text & "',"
            str2 = str2 + " in_location = '" & locationid & "'"
            'str = str + " status = '" & TextBox2.Text & "'"
            str2 = str2 + " WHERE "
            str2 = str2 + " rfid = '" & rfid & "'"
        Else

            Dim travel_distance As Integer
            Dim travel_fare As Integer
            Dim account_balance As Integer
            travel_distance = locationid_db - locationid
            travel_distance = Math.Abs(travel_distance)
            travel_fare = travel_distance * 10
            account_balance = balance_db - travel_fare
            card_id.Text = account_balance
            locationtext.Text = travel_distance


            str2 = "UPDATE passenger_info SET "

            str2 = str2 + " status = '" & TextBox4.Text & "',"
            str2 = str2 + " balance = '" & account_balance & "'"
            'str = str + " status = '" & TextBox2.Text & "'"
            str2 = str2 + " WHERE "
            str2 = str2 + " rfid = '" & rfid & "'"
        End If


        Dim cmd2 As OleDbCommand = New OleDbCommand(str2, myConnection)
        'cmd.Parameters.Add(New OleDbParameter("rfid", CType(TextBox1.Text, String)))
        ' cmd.Parameters.Add(New OleDbParameter("balance", CType(TextBox2.Text, String)))
        'cmd.Parameters.Add(New OleDbParameter("in_location", CType(TextBox3.Text, String)))
        Try
            cmd2.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        ' myConnection.Close()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Form2.Show()
    End Sub

    Private Sub GroupBox3_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox3.Enter

    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Form3.Show()
    End Sub


    Private Sub RichTextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RichTextBox1.TextChanged
        'Dim data_buffer2 As String

        'Dim i As Integer = 0
        'data_buffer2 = RichTextBox1.Text
        'i = Len(data_buffer2)
        'Label10.Text = i
        'Label11.Text = data_buffer2.Substring(i - 5, 5)
        ''RichTextBox1.Text = ""
        ''i = i + 4

    End Sub
End Class
