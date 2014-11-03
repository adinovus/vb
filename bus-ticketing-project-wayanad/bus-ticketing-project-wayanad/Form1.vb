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
    Dim rfid As String
    Dim locationid As String
    Dim status As String
    Dim balance_db As String
    Dim locationid_db As String


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
            Button1.Enabled = False

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


    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        port_num = ComboBox1.Text
    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

       
        Dim data_buffer As String

        data_received = SerialPort1.ReadExisting
        Invoke(Sub() TextBox5.Text = TextBox5.Text + data_received)
        Dim data_buffer2 As String

        Dim i As Integer = 0
        Invoke(Sub() data_buffer = TextBox5.Text)
        i = Len(data_buffer)
        'Invoke(Sub() Label10.Text = i)
        data_buffer2 = data_buffer.Substring(i - 6, 6)

        'Invoke(Sub() Label11.Text = data_buffer2)

        Dim rfid_db
        first_bit = data_buffer2(0)
        rfid = data_buffer2.Substring(1, 3)
        'Invoke(Sub() TextBox5.Text = rfid)
        locationid = data_buffer2.Substring(4, 2)
        If first_bit = "$" Then
            Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "****card swiped****" + vbCrLf)
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
                Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "card not recognized" + vbCrLf)
                SerialPort1.WriteLine("recognition fail                &")
            Else

                'Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + rfid_db + vbCrLf)
                'checking passenger is already in bus or not

                If status = "y" Then
                    Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "passenger with card number " + rfid_db + " leaving the bus" + vbCrLf)

                    Invoke(Sub() TextBox4.Text = "n")
                Else
                    Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "passenger with card number " + rfid_db + " entering the bus" + vbCrLf)
                    If balance_db < 10 Then
                        Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "not enough balance" + vbCrLf)
                        SerialPort1.WriteLine("  no balance     access denied  &")
                    Else
                        Invoke(Sub() RichTextBox1.Text = RichTextBox1.Text + "passenger allowed to travel" + vbCrLf)
                        SerialPort1.WriteLine("Please Step In                  &")
                        Invoke(Sub() card_id.Text = balance_db)
                        Invoke(Sub() locationtext.Text = locationid_db)
                        Invoke(Sub() TextBox4.Text = "y")
                    End If


                End If

            End If

        End If
        myConnection.Close()



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
            RichTextBox1.Text = RichTextBox1.Text + "passenger entering location is " + locationid + vbCrLf

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

            RichTextBox1.Text = RichTextBox1.Text + "distance travelled is " + CStr(travel_distance) + vbCrLf
            RichTextBox1.Text = RichTextBox1.Text + "travel fare is " + CStr(travel_fare) + vbCrLf
            RichTextBox1.Text = RichTextBox1.Text + "balance remaining - " + CStr(account_balance) + vbCrLf
            SerialPort1.WriteLine("Distance" & CStr(travel_distance) & ",Fare" & CStr(travel_fare) & ",Balance " & CStr(account_balance) & "     &")


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

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Form3.Show()
    End Sub

End Class
