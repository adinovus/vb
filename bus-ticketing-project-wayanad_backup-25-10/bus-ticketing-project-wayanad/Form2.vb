Imports System.Data.OleDb
Public Class Form2
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
    Dim recharge_amount As Integer
    Dim status As String
    Dim balance_db As String
    Dim locationid_db As String

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        myConnection.Open()
        If TextBox1.Text = "" Then
            MsgBox("enter card id")
        ElseIf TextBox2.Text = "" Then
            MsgBox("enter amount")
        Else
            rfid = TextBox1.Text
            recharge_amount = TextBox2.Text
            Dim str As String
            str = "SELECT * FROM passenger_info WHERE (rfid = '" & rfid & "')"
            Dim cmd As OleDbCommand = New OleDbCommand(str, myConnection)
            dr = cmd.ExecuteReader
            Dim rfid_db
            While dr.Read()
                balance_db = dr("balance").ToString
                rfid_db = dr("rfid").ToString
            End While
            If rfid_db Is Nothing Then
                Label3.Text = "           invalid card" & vbCrLf & "please enter another card"
            Else
                recharge_amount = recharge_amount + balance_db

                Dim str2 As String
                str2 = "UPDATE passenger_info SET "

                str2 = str2 + " balance = '" & recharge_amount & "'"
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
                While dr.Read()
                    balance_db = dr("balance").ToString
                End While
                Label3.Text = "recharge successfull" & vbCrLf & "Current balance is " & balance_db
            End If
            TextBox1.Text = ""
            TextBox2.Text = ""
        End If

        myConnection.Close()
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'database
        provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source ="
        dataFile = "F:\database\bus-ticketing-db.mdb" ' Change it to your Access Database location
        connString = provider & dataFile
        myConnection.ConnectionString = connString
        'database


    End Sub
End Class