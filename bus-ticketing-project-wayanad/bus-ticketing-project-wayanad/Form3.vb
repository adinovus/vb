Imports System.Data.OleDb

Public Class Form3
    Dim provider As String
    Dim dataFile As String
    Dim connString As String
    Public myConnection As OleDbConnection = New OleDbConnection
    Public dr As OleDbDataReader

    Dim rfid As String
    Dim balance_db As String
    Dim locationid_db As String

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        myConnection.Open()

        rfid = TextBox1.Text
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
            Label2.Text = "           invalid card" & vbCrLf & "please enter another card"
        Else
            Label2.Text = "balance on card no " & rfid & " is Rs" & balance_db
        End If
        myConnection.Close()
    End Sub

    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'database
        provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source ="
        dataFile = "F:\database\bus-ticketing-db.mdb" ' Change it to your Access Database location
        connString = provider & dataFile
        myConnection.ConnectionString = connString
        'database

    End Sub
End Class