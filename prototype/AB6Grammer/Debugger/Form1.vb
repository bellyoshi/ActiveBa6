Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
          If Not bFound Then
            If Process.GetProcessesByName("test").Length = 1 Then
                hProc = OpenProcess(dwAllAccess, True, CType(Process.GetProcessesByName("test")(0).Id, System.UInt32))
                bFound = True
            End If
        Else
            If Process.GetProcessesByName("test").Length <> 1 Then
                bFound = False
                Return
            End If
        End If
        Dim p = Process.GetProcessesByName("test")(0)


        Dim bAddr = & AABBCCDD
        ReadProcessMemory(hProc, bAddr, bBuff, 4, bytesRW)
        MsgBox(BitConverter.ToInt32(bBuff, 0).ToString())
    End Sub
End Class
